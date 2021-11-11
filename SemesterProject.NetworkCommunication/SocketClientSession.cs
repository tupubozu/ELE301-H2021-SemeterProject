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
		private Queue<NetworkMessage> messageQueue = new Queue<NetworkMessage>();

		private Aes crypto;
		private Socket server;
		private NetworkStream serverStream;
		private CryptoStream cryptoWriter;
		private CryptoStream cryptoReader;

		private UdpClient broadcastInterceptor;
		private CancellationTokenSource broadcastCanceller;
		private Task broadcastListener;

		private CancellationTokenSource workerCanceller;
		private Task worker;

		#region Events
		public event EventHandler<NetworkMessageUpdateEventArgs> MessageRecieved;
		public event EventHandler<NetworkMessageUpdateEventArgs> UpdateNodeClock;
		public event EventHandler<NetworkMessageUpdateEventArgs> UpdateAccessTable;
		#endregion
		public bool IsCompleted { get => worker.IsCompleted; }

		public SocketClientSession(Aes aes)
		{
			crypto = aes;
			server = null;

			Init();
			InitBroadcast();

		}
		public SocketClientSession(IPEndPoint serverEnd, Aes aes)
		{
			broadcastInterceptor = null;
			crypto = aes;
			server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			server.Connect(serverEnd);
			serverStream = new NetworkStream(server);
			cryptoReader = new CryptoStream(serverStream, crypto.CreateDecryptor(), CryptoStreamMode.Read);
			cryptoWriter = new CryptoStream(serverStream, crypto.CreateEncryptor(), CryptoStreamMode.Write);
			Init();
			InitWorker();
		}

		private void Init()
		{
			broadcastCanceller = new CancellationTokenSource();
			workerCanceller = new CancellationTokenSource();
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
				if (!(broadcastListener?.IsCompleted ?? false))
					broadcastListener?.Wait();
				Log.Debug("Stopped broadcast listener: {0}", this.GetType().Name);
				broadcastListener?.Dispose();
				broadcastCanceller?.Dispose();

				Log.Debug("Stopping worker: {0}", this.GetType().Name);
				workerCanceller?.Cancel();
				if (!(worker?.IsCompleted ?? false))
					worker?.Wait();
				Log.Debug("Stopped worker: {0}", this.GetType().Name);
				worker?.Dispose();
				workerCanceller?.Dispose();

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

		#region BroadcastListener
		private void InitBroadcast()
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

			Log.Information("Starting broadcast listener on {0}", broadcastInterceptor?.Client?.LocalEndPoint);
			InitBroadcastListenerWorker();
			Log.Information("Started broadcast listener on {0}", broadcastInterceptor?.Client?.LocalEndPoint);
		}
		private void InitBroadcastListenerWorker()
		{
			broadcastListener = Task.Run(async () =>
			{
				try
				{
					for (; ; )
					{
						broadcastCanceller.Token.ThrowIfCancellationRequested();
						await BroadcastUpdate();
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
				finally
				{
					Log.Information("Broadcast listener stopped");
				}

			}, broadcastCanceller.Token);
		}
		private async Task BroadcastUpdate()
		{
			try
			{
				if (server is null)
				{
					if (broadcastInterceptor?.Available != 0)
					{
						IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 0);

						byte[] temp = broadcastInterceptor.Receive(ref serverEndPoint);
						ushort ctrl;
						if (temp.Length == 2)
						{
							ctrl = (ushort)((temp[0] << 8) | temp[1]);
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
							InitWorker();
						}
					}
					else await Task.Delay(100);
				}
				else
				{
					broadcastInterceptor?.Dispose();
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
		#endregion

		#region Worker
		private void InitWorker()
		{
			worker = Task.Run(async () =>
			{
				try
				{
					for (; ; )
					{
						if (server is null) break;
						workerCanceller.Token.ThrowIfCancellationRequested();
						await this.UpdateWorker();
					}
				}
				catch (OperationCanceledException ex)
				{
					Log.Debug(ex, "Worker thread aborted");
				}
				catch (IOException ex)
				{
					Log.Error(ex, "Stream error");

					server.Dispose();
					server = null;

					InitBroadcast();
				}
				catch (Exception ex)
				{
					Log.Error(ex, "Unknown error");
				}
				finally
				{
					Log.Information("Worker stopped");
				}
			}, workerCanceller.Token);
		}

		private async Task UpdateWorker()
		{
			try
			{
				bool Active = false;

				if (server.Available != 0 && !(cryptoReader is null))
				{
					Active = true;
					NetworkMessage data = null;
					try
					{
						//SerialStatusData data;
						BinaryFormatter binaryFormatter = new BinaryFormatter();
						data = binaryFormatter.Deserialize(cryptoReader) as NetworkMessage;

					}
					catch(SerializationException ex)
					{
						Log.Error(ex, "Serialization failed");
						Log.Information("Network data unreadable. Try checking preshared keys on host {0}",(server.RemoteEndPoint as IPEndPoint)?.Address);
					}

					if (!(data is null))
					{
						Log.Information("Network message received from node: ", data.NodeNumber);
						NetworkMessageUpdateEventArgs e = new NetworkMessageUpdateEventArgs()
						{
							MessageData = data
						};
						MessageRecieved?.Invoke(this, e);
						switch (data.Type)
						{
							case NetworkMessage.MessageType.UpdateAccessTable:
								UpdateAccessTable?.Invoke(this, e);
								break;
							case NetworkMessage.MessageType.UpdateUnitTime:
								UpdateNodeClock?.Invoke(this, e);
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
		#endregion
	}
}
