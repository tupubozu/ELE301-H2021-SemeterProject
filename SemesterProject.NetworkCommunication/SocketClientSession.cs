using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using System.IO;
using SemesterProject.Common.Core;
using SemesterProject.Common.Values;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Security;

namespace SemesterProject.NetworkCommunication
{
	public class SocketClientSession : IDisposable
	{
		Queue<NetworkMessage> messageQueue = new Queue<NetworkMessage>();
		public Dictionary<int, int> AllowedKeyTable = new Dictionary<int, int>();

		Aes crypto;
		Socket server;
		NetworkStream serverStream;
		CryptoStream cryptoWriter;
		CryptoStream cryptoReader;

		UdpClient broadcastInterceptor;
		CancellationTokenSource broadcastCanceller;
		Task broadcastListener;

		CancellationTokenSource canceller;
		Task worker;

		public bool IsCompleted { get => worker.IsCompleted; }


		public SocketClientSession(Aes aes)
        {
            init();

            crypto = aes;

            server = null;

            initBroadcast();

        }

        void initBroadcast()
        {
            try
            {
                broadcastInterceptor = new UdpClient(CommonValues.UdpBroadcastPort);
            }
            catch (SocketException ex)
            {
                Log.Debug(ex, "Failed to bind UdpClient to port {0}", CommonValues.UdpBroadcastPort);
                broadcastInterceptor = null;
            }

            Log.Information("Starting broadcast listener on {0}", broadcastInterceptor.Client.LocalEndPoint);
            broadcastListener = Task.Run(() =>
            {
                try
                {
                    for (; ; )
                    {
                        broadcastCanceller.Token.ThrowIfCancellationRequested();
                        broadcastUpdate();
                        if (broadcastInterceptor is null) break;
                    }
                }
                catch (OperationCanceledException ex)
                {
                    Log.Debug(ex, "Broadcast listener stopped externally");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unknown error");
                }
				Log.Debug("Broadcast listener stopped");

			}, broadcastCanceller.Token);
            Log.Information("Started broadcast listener on {0}", broadcastInterceptor.Client.LocalEndPoint);
        }

        public SocketClientSession(IPEndPoint serverEnd,Aes aes)
		{
			init();
			broadcastInterceptor = null;
			crypto = aes;
			server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			server.Connect(serverEnd);
			serverStream = new NetworkStream(server);
			cryptoReader = new CryptoStream(serverStream, crypto.CreateDecryptor(), CryptoStreamMode.Read);
			cryptoWriter = new CryptoStream(serverStream, crypto.CreateEncryptor(), CryptoStreamMode.Write);
			worker = createWorker();
		}
		~SocketClientSession()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			Log.Debug("Dispose {0}", this.GetType().Name);
			try
			{
				Log.Debug("Stopping broadcast listener: {0}", this.GetType().Name);
				broadcastCanceller?.Cancel();
				if (!broadcastListener.IsCompleted)
					broadcastListener?.Wait();
				Log.Debug("Stopped broadcast listener: {0}", this.GetType().Name);
				broadcastListener?.Dispose();
				broadcastCanceller?.Dispose();

				Log.Debug("Stopping worker: {0}", this.GetType().Name);
				canceller?.Cancel();
				if (!worker.IsCompleted)
					worker?.Wait();
				Log.Debug("Stopped worker: {0}", this.GetType().Name);
				worker?.Dispose();
				canceller?.Dispose();

			}
			catch (Exception ex)
			{
				Log.Error(ex, "Unkown error");
			}
		}

		void init()
		{
			broadcastCanceller = new CancellationTokenSource();
			canceller = new CancellationTokenSource();
		}

		void broadcastUpdate()
		{
			try
			{
				if (server is null)
				{
					if (broadcastInterceptor.Available != 0)
					{
						IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 0);
						
						byte[] temp = broadcastInterceptor.Receive(ref serverEndPoint);
						ushort ctrl;	
						if (temp.Length == 2)
                        {
							ctrl = (ushort)((temp[0]<<8) | temp[1]);
						}
						else
                        {
							ctrl = BitConverter.ToUInt16(temp, 0);
						}

						if (ctrl == CommonValues.BroadcastValidatorValue)
						{
							Log.Information("Found server on {0}", serverEndPoint.Address);
							serverEndPoint.Port = CommonValues.TcpServerPort;

							Log.Information("Connecting to server on {0}", serverEndPoint);
							server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
							server.Connect(serverEndPoint);
							Log.Information("Connected to server on {0}", serverEndPoint);
							serverStream = new NetworkStream(server, FileAccess.ReadWrite);
							cryptoReader = new CryptoStream(serverStream, crypto.CreateDecryptor(), CryptoStreamMode.Read);
							cryptoWriter = new CryptoStream(serverStream, crypto.CreateEncryptor(), CryptoStreamMode.Write);
							worker = createWorker();
						}
					}
				}
				else
				{
					broadcastInterceptor.Dispose();
					broadcastInterceptor = null;
				}
			}
			catch (SocketException ex)
			{
				Log.Debug(ex, "Network error");
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Unknown error");
			}
		}
		Task createWorker()
		{
			Task job = Task.Run(() =>
			{
				try
				{
					for (; ; )
					{
						canceller.Token.ThrowIfCancellationRequested();
						this.updateWorker();
					}
				}
				catch (OperationCanceledException ex)
				{
					Log.Information(ex, "Worker thread aborted");
				}
				catch (IOException ex)
                {
					Log.Error(ex, "Stream error");
                    
					server.Dispose();
					
					server = null;
					initBroadcast();
				}
				catch (Exception ex)
				{
					Log.Error(ex, "Unknown error");
				}
			}, canceller.Token);
			return job;
		}

		public void EnqueueNetworkData(NetworkMessage data)
		{
			messageQueue.Enqueue(data);
		}

		void updateWorker()
		{
			try
			{
				if (server.Available != 0 && !(cryptoReader is null))
				{
					NetworkMessage data = null;
					//SerialStatusData data;
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					data = binaryFormatter.Deserialize(cryptoReader) as NetworkMessage;

					if (!(data is null))
					{
						Log.Information("Network message received from node: ", data.UnitNumber);
						switch (data.Type)
						{
							case NetworkMessage.MessageType.UpdateAccessTable:
								AllowedKeyTable = data.MessageObject as Dictionary<int, int> ?? AllowedKeyTable;
								break;
							case NetworkMessage.MessageType.UpdateUnitTime:
								break;
							default:
								break;
						}
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
				Log.Error(ex, "Network communiction failed: {0}", server);
			}
			catch (SerializationException ex)
			{
				Log.Error(ex, "Network communiction failed: {0}", server);
			}
			catch (SecurityException ex)
			{
				Log.Error(ex, "Network communiction failed: {0}", server);
			}

		}
	}
}
