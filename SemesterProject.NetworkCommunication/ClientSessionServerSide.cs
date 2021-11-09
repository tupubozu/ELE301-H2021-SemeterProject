﻿using System;
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

		Thread worker;
		
		Aes _crypto;

		Queue<Dictionary<int, int>> allowedKeyTableQueue;

		public ClientSessionServerSide(Socket client, Aes aes)
		{
			_client = client;
			_clientStream = new NetworkStream(_client);
			allowedKeyTableQueue = new Queue<Dictionary<int, int>>();
			_crypto = aes;

			worker = new Thread( () =>
			{
				try
				{
					for (; ; )
						this.update();
				}
				catch (ThreadAbortException ex)
				{
					Log.Information(ex, "Worker thread aborted");
				}
				catch (Exception ex)
                {
					Log.Error(ex, "Unknown error");
                }
			});
			worker.Start();
		}
		~ClientSessionServerSide()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			try
			{
				worker?.Abort();
				worker?.Join();
				_clientStream.Dispose();
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
