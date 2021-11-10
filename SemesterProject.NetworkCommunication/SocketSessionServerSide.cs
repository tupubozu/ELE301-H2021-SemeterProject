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
	class SocketSessionServerSide: IDisposable
	{
		Socket client;
		NetworkStream clientStream;

		CancellationTokenSource canceller;
		Task worker;

		
		Aes crypto;

		public bool IsCompleted { get => worker.IsCompleted; }

		Queue<Dictionary<int, int>> allowedKeyTableQueue;

		public SocketSessionServerSide(Socket client, Aes aes)
		{
			this.client = client;
            clientStream = new NetworkStream(this.client);
			allowedKeyTableQueue = new Queue<Dictionary<int, int>>();
			crypto = aes;

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
				catch (Exception ex)
				{
					Log.Error(ex, "Unknown error");
				}
			}, canceller.Token);
		}
		~SocketSessionServerSide()
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
				if (!worker.IsCompleted)
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
		public void EnqueueAllowedTable(Dictionary<int, int> table)
		{
			allowedKeyTableQueue.Enqueue(table);
		}

		

		void update()
		{
			try
			{
				if (client.Available != 0)
				{
					NetworkMessage data;

					using (var decryptor = crypto.CreateDecryptor())
					using (var cryptoStr = new CryptoStream(clientStream, decryptor, CryptoStreamMode.Read))
					{
						BinaryFormatter binaryFormatter = new BinaryFormatter();
						data = binaryFormatter?.Deserialize(cryptoStr) as NetworkMessage;
					}

				}

				if (allowedKeyTableQueue.Count != 0)
				{
					using (var encryptor = crypto.CreateEncryptor())
					using (var cryptoStr = new CryptoStream(clientStream, encryptor, CryptoStreamMode.Read))
					{
						BinaryFormatter binaryFormatter = new BinaryFormatter();
						binaryFormatter.Serialize(cryptoStr, allowedKeyTableQueue.Dequeue());
					}
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
