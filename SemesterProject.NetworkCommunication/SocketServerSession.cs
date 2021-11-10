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
	class SocketServerSession: IDisposable
	{
		Socket client;
		NetworkStream clientStream;
		CryptoStream cryptoWriter;
		CryptoStream cryptoReader;

		CancellationTokenSource canceller;
		Task worker;

		
		Aes crypto;

		public bool IsCompleted { get => worker.IsCompleted; }

		Queue<NetworkMessage> messageQueue;

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

			worker = Task.Run( () =>
			{
				try
				{
					for (; ; )
					{
						canceller.Token.ThrowIfCancellationRequested();
						this.update();
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
				if (!(worker is null) || !worker.IsCompleted)
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
		public void EnqueueNetworkData(NetworkMessage data)
		{
			messageQueue.Enqueue(data);
		}

		

		void update()
		{
			try
			{
				if (client.Available != 0 && !(cryptoReader is null))
				{
					NetworkMessage data = null;
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					data = binaryFormatter?.Deserialize(cryptoReader) as NetworkMessage;
					if (!(data is null))
                    {
						Log.Information("Network message recieved from node: {0}", data.UnitNumber);
                    }
				}
				if (messageQueue.Count != 0 && !(cryptoWriter is null))
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					binaryFormatter.Serialize(cryptoWriter, messageQueue.Dequeue());
				}
			}
			catch (ArgumentException ex)
			{
				Log.Error(ex, "Network communiction failed: {0}", client);
			}
			catch (SerializationException ex)
			{
				Log.Error(ex, "Network communiction failed: {0}", client);
			}
			catch (SecurityException ex)
			{
				Log.Error(ex, "Network communiction failed: {0}", client);
			}

		}
	}
}
