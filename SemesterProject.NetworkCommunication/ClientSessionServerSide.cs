using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using SemesterProject.Common.Core;
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

		Task _worker;
		CancellationTokenSource _canceler;

		Aes _crypto;

		Queue<Dictionary<int, int>> allowedKeyTableQueue;

		public ClientSessionServerSide(Socket client, Aes aes)
		{
			_client = client;
			_clientStream = new NetworkStream(_client);
			allowedKeyTableQueue = new Queue<Dictionary<int, int>>();
			_crypto = aes;

			_canceler = new CancellationTokenSource();
			_worker = Task.Run(async () =>
			{
				for (; !_canceler.Token.IsCancellationRequested;)
					await this.update();
			}, _canceler.Token);
		}
		~ClientSessionServerSide()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			try
			{
				_canceler?.Cancel();
				_worker?.Wait(100);
				_worker?.Dispose();
				_canceler?.Dispose();
				_crypto.Dispose();
				_clientStream.Dispose();
			}
			catch (Exception)
			{ }
		}

		async Task update()
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
			catch (Exception ex)
			{
				Log.Error(ex, "Network communiction failed: {0}", _client);
			}
			
		}
	}
}
