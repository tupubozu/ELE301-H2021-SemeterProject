using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;
using SemesterProject.Common.Core;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net;
using Serilog;

namespace SemesterProject.NetworkCommunication
{
	public class SocketServerSession: IDisposable
	{
        #region Events
        public static event EventHandler<NetworkMessage> MessageRecieved;
		public static event EventHandler<NetworkMessage> UpdateAccessTable;
		public static event EventHandler<NetworkMessage> Breach;
		public static event EventHandler<NetworkMessage> AuthSuccess;
		public static event EventHandler<NetworkMessage> AuthFailure;
		public static event EventHandler<NetworkMessage> AuthTimeout;
		public static event EventHandler<NetworkMessage> OtherMessage;
		public static event EventHandler<NetworkMessage> KeypadPress;
        #endregion

        public bool IsCompleted { get => worker.IsCompleted; }

		private Socket client;
		private NetworkStream clientStream;
		private CryptoStream cryptoWriter;
		private CryptoStream cryptoReader;

		private CancellationTokenSource canceller;
		private Task worker;

		private Aes crypto;

		private Queue<NetworkMessage> messageQueue;

		public SocketServerSession(Socket client, Aes aes)
		{
			client.Blocking = true;
			this.client = client;
			crypto = aes;
			clientStream = new NetworkStream(this.client, FileAccess.ReadWrite);
			cryptoReader = new CryptoStream(clientStream, crypto.CreateDecryptor(), CryptoStreamMode.Read);
			cryptoWriter = new CryptoStream(clientStream, crypto.CreateEncryptor(), CryptoStreamMode.Write);
			messageQueue = new Queue<NetworkMessage>();


			canceller = new CancellationTokenSource();

			InitWorker();
		}
		~SocketServerSession()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			Log.Debug("Dispose {0}", this.GetType().Name);
			try
			{
				Log.Debug("Stopping worker: {0}", this.GetType().Name);
				canceller?.Cancel();
				if (!worker?.IsCompleted ?? false)
					worker?.Wait();
				Log.Debug("Stopped worker: {0}", this.GetType().Name);

				worker?.Dispose();
				canceller?.Dispose();

				clientStream?.Dispose();
				client?.Dispose();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Unkown error");
			}
		}
        public override string ToString()
        {
			return client.RemoteEndPoint.ToString();
        }
        public void EnqueueNetworkData(NetworkMessage data)
		{
			messageQueue.Enqueue(data);
		}

        #region Worker
        private void InitWorker()
        {
			worker = Task.Run(async () =>
			{
				try
				{
					for (; ; )
					{
						canceller.Token.ThrowIfCancellationRequested();
						await this.UpdateWorker();
					}
				}
				catch (OperationCanceledException ex)
				{
					Log.Information(ex, "Worker thread aborted");
				}
				catch (IOException ex)
				{
					Log.Error(ex, "Stream error");
					canceller.Cancel();
				}
				catch (Exception ex)
				{
					Log.Error(ex, "Unknown error");
				}
			}, canceller.Token);
		}

		private async Task UpdateWorker()
		{
			try
			{
				bool Active = false;

				if (client.Available != 0 && !(cryptoReader is null))
				{
					Active = true;

					NetworkMessage data = null;
                    try
                    {
						BinaryFormatter binaryFormatter = new BinaryFormatter();
						data = binaryFormatter?.Deserialize(cryptoReader) as NetworkMessage;
					}
					catch (SerializationException ex)
					{
						Log.Error(ex, "Serialization failed");
						Log.Information("Network data unreadable. Try checking preshared keys on host {0}", (client.RemoteEndPoint as IPEndPoint)?.Address);
						throw;
					}
					
					if (!(data is null))
					{
						MessageRecieved?.Invoke(this, data);
						switch (data.Type)
						{
							case NetworkMessage.MessageType.RequestAccessTable:
								//SortedSet<UserPermission>
								UpdateAccessTable?.Invoke(this, data);
								break;
							case NetworkMessage.MessageType.Breach:
								Breach?.Invoke(this, data);
								break;
							case NetworkMessage.MessageType.KeypadPress:
								KeypadPress?.Invoke(this, data);
								break;
							case NetworkMessage.MessageType.AuthSuccess:
								AuthSuccess?.Invoke(this, data);
								break;
							case NetworkMessage.MessageType.AuthFailure:
								AuthFailure?.Invoke(this, data);
								break;
							case NetworkMessage.MessageType.AuthTimeout:
								AuthTimeout?.Invoke(this, data);
								break;
							case NetworkMessage.MessageType.Other:
								OtherMessage?.Invoke(this, data);
								break;
							default:
								break;
						}
					}
				}

				if (messageQueue.Count != 0 && !(cryptoWriter is null))
				{
					Active = true;
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					binaryFormatter.Serialize(cryptoWriter, messageQueue.Dequeue());
				}

				if (!Active) await Task.Delay(100);
			}
			catch (ArgumentException ex)
			{
				Log.Error(ex, "Network communiction failed: {0}", client);
			}
			catch (SerializationException ex)
			{
				Log.Error(ex, "Network communiction failed: {0}", client);
				throw;
			}
			catch (SecurityException ex)
			{
				Log.Error(ex, "Network communiction failed: {0}", client);
			}
		}
        #endregion
    }
}
