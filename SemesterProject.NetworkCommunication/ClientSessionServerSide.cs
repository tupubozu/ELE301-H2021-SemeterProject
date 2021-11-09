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
using Serilog;

namespace SemesterProject.NetworkCommunication
{
	class ClientSessionServerSide: IDisposable
	{
		const int bufferSize = 1024;
		Socket _client;
		NetworkStream _clientStream;

		CancellationTokenSource cancellation;
		Task worker;
		
		Aes _crypto;

		public bool IsCompleted { get => worker.IsCompleted; }

		Queue<Dictionary<int, int>> allowedKeyTableQueue;

		public ClientSessionServerSide(Socket client, Aes aes)
		{
			_client = client;
			_clientStream = new NetworkStream(_client);
			allowedKeyTableQueue = new Queue<Dictionary<int, int>>();
			_crypto = aes;

			cancellation = new CancellationTokenSource();

			worker = Task.Run( () =>
			{
				try
				{
					for (; ;)
						cancellation.Token.ThrowIfCancellationRequested();
						this.update();
				}
				catch (OperationCanceledException ex)
				{
					Log.Information(ex, "Worker thread aborted");
				}
				catch (Exception ex)
				{
					Log.Error(ex, "Unknown error");
				}
			}, cancellation.Token);
		}
		~ClientSessionServerSide()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			Log.Debug(messageTemplate: "Dispose {0}", this);
			try
			{
				cancellation?.Cancel();
				if (!worker.IsCompleted)
					worker?.Wait();

				worker?.Dispose();
				cancellation?.Dispose();

				_clientStream?.Dispose();
				_client?.Dispose();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Unkown error");
			}
		}

		void update()
		{
			try
			{
				if (_client.Available != 0)
				{
					SerialStatusData data;

					using (var decryptor = _crypto.CreateDecryptor())
					using (var cryptoStr = new CryptoStream(_clientStream, decryptor, CryptoStreamMode.Read))
					{
						BinaryFormatter binaryFormatter = new BinaryFormatter();
						data = binaryFormatter?.Deserialize(cryptoStr) as SerialStatusData;
					}

				}

				if (allowedKeyTableQueue.Count != 0)
				{
					using (var encryptor = _crypto.CreateEncryptor())
					using (var cryptoStr = new CryptoStream(_clientStream, encryptor, CryptoStreamMode.Read))
					{
						BinaryFormatter binaryFormatter = new BinaryFormatter();
						binaryFormatter.Serialize(cryptoStr, allowedKeyTableQueue.Dequeue());
					}
				}
			}
			catch (ArgumentException ex)
			{
				Log.Error(ex, "Network communiction failed: {0}", _client);
			}
			catch (SerializationException ex)
			{
				Log.Error(ex, "Network communiction failed: {0}", _client);
			}
			catch (SecurityException ex)
			{
				Log.Error(ex, "Network communiction failed: {0}", _client);
			}

		}
	}
}
