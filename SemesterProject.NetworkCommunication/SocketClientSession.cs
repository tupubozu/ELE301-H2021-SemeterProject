using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using SemesterProject.Common.Core;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace SemesterProject.NetworkCommunication
{
	class SocketClientSession: IDisposable
	{
		const int bufferSize = 1024;
		byte[] _rawBuffer;
		byte[] _cryptoBuffer;
		Socket _client;

		Task _worker;
		CancellationTokenSource _canceler;

		Aes _crypto;

		public SocketClientSession(Socket client, Aes aes)
		{
			_rawBuffer = new byte[bufferSize];
			_cryptoBuffer = new byte[bufferSize];
			_client = client;
			_crypto = aes;

			_canceler = new CancellationTokenSource();
			_worker = Task.Run(async () =>
			{
				for (; !_canceler.Token.IsCancellationRequested;)
					await this.update();
			}, _canceler.Token);
		}
		~SocketClientSession()
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
			}
			catch (Exception)
			{ }
		}

		async Task update()
		{
			if (_client.Available != 0)
			{
				SerialStatusData data;
				int rawBuffer_cnt = _client.Receive(_rawBuffer);
				using (var rawMem = new MemoryStream(_rawBuffer))
				{
					using (var decryptor = _crypto.CreateDecryptor())
					{
						using (var cryptoStr = new CryptoStream(rawMem, decryptor, CryptoStreamMode.Read))
						{
							int cryptoBuffer_cnt = cryptoStr.Read(_cryptoBuffer, 0, rawBuffer_cnt);
							BinaryFormatter binaryFormatter = new BinaryFormatter();
							data = binaryFormatter?.Deserialize(new MemoryStream(_cryptoBuffer, 0, cryptoBuffer_cnt, false, false)) as SerialStatusData;
						}
					}
				}

			}
		}
	}
}
