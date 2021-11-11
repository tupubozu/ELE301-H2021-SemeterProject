using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Serilog;
using SemesterProject.Common.Values;

namespace SemesterProject.NetworkCommunication
{
	public class SocketServer: IDisposable
	{
		private Socket listener;
		private List<SocketServerSession> sessions;

		private CancellationTokenSource cancellation;
		private Task worker;

		private Aes crypto;

		private UdpClient broadcast;
		private IPEndPoint broadcastEndPoint;
		private CancellationTokenSource broadcastCanceller;
		private Task broadcaster;

		private DateTime lastSessionCheck;
		private TimeSpan sessionCheckInterval;

		public SocketServer(Aes aes)
		{
			Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, CommonValues.TcpServerPort);
			listener.Bind(serverEndPoint);
			
			Init(listener, aes);
		}

		public SocketServer(Socket listener, Aes aes)
		{
			Init(listener, aes);
		}
		~SocketServer()
		{
			Log.Debug(this.ToString(), "Destuctor");
			this.Dispose();
		}
		public void Dispose()
		{
			Log.Debug(messageTemplate: "Dispose {0}", this);
			try
			{
				Log.Debug("Stopping broadcaster: {0}", this.GetType().Name);
				broadcastCanceller?.Cancel();
				if (!(broadcaster?.IsCompleted ?? false))
					broadcaster?.Wait();
				Log.Debug("Stopped broadcaster: {0}", this.GetType().Name);
				broadcaster?.Dispose();
				broadcastCanceller?.Dispose();

				Log.Debug("Stopping worker: {0}", this.GetType().Name);
				cancellation?.Cancel();
				if (!(worker?.IsCompleted ?? false))
					worker?.Wait();
				Log.Debug("Stopped worker: {0}", this.GetType().Name);
				worker?.Dispose();
				cancellation?.Dispose();

				foreach (var session in sessions) session?.Dispose();
				listener?.Dispose();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Unkown error");
			}
		}

		public override string ToString()
		{
			return $"{base.GetType().Name}({listener})";
		}

		private void Init(Socket listener, Aes aes)
		{
			lastSessionCheck = DateTime.Now;
			sessionCheckInterval = TimeSpan.FromSeconds(30);
			Log.Information("Starting server on {0}", listener.LocalEndPoint);
			crypto = aes;
			this.listener = listener;
			this.listener.Blocking = false;
			this.listener.Listen(1000);
			sessions = new List<SocketServerSession>();
			cancellation = new CancellationTokenSource();

			InitWorker();

			Log.Information("Started server on {0}", listener.LocalEndPoint);

			InitBroadcast();
		}

        #region Broadcaster
        private void InitBroadcast()
		{
			broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, CommonValues.UdpBroadcastPort);
			broadcastCanceller = new CancellationTokenSource();

			broadcast = new UdpClient(CommonValues.UdpBroadcastHostPort);
			broadcast.Connect(broadcastEndPoint);

			Log.Information("Starting broadcaster on {0}", broadcast.Client.LocalEndPoint);
			broadcaster = Task.Run(async () =>
			{
				try
				{
					for (; ; )
					{
						broadcastCanceller.Token.ThrowIfCancellationRequested();
						await BroadcastUpdate();
					}
				}
				catch (OperationCanceledException ex)
				{
					Log.Debug(ex, "Broadcast listener stopped");
				}
				catch (Exception ex)
				{
					Log.Error(ex, "Unknown error");
				}

			}, broadcastCanceller.Token);
			Log.Information("Started broadcaster on {0}", broadcast.Client.LocalEndPoint);
		}

		private async Task BroadcastUpdate()
		{
			Task timeDelay = Task.Delay(5000);
			byte[] buffer = { (CommonValues.BroadcastValidatorValue & (0xff << 8)) >> 8, CommonValues.BroadcastValidatorValue & 0xff };
			broadcast.Send(buffer, 2);
			Log.Debug("Broadcast sendt");
			await timeDelay;

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
						cancellation.Token.ThrowIfCancellationRequested();
						await this.UpdateWorker();
					}
				}
				catch (OperationCanceledException ex)
				{
					Log.Debug(ex, "Worker thread aborted");
				}
				catch (Exception ex)
				{
					Log.Error(ex, "Unknown error");
				}
			}, cancellation.Token);
		}

		private async Task UpdateWorker()
		{
			try
			{
				var client = listener.Accept();
				Log.Information($"New connection from {client.RemoteEndPoint}");
				sessions.Add(new SocketServerSession(client, crypto));
			}
			catch (SocketException ex)
			{
				Task task = Task.Delay(100);
				Log.Debug(ex, "Network error");
				await task;
			}

			DateTime currentTime = DateTime.Now;
			if (sessions.Count != 0 && currentTime - lastSessionCheck >= sessionCheckInterval)
			{
				lastSessionCheck = currentTime;
				List<SocketServerSession> rmList = new List<SocketServerSession>();
				Log.Debug("Checking for expired/inactive sessions");
				foreach (var session in sessions)
				{
					if (session.IsCompleted)
					{
						rmList.Add(session);
					}
				}
				Log.Debug("Found expired/inactive sessions: {0}", rmList.Count);

				Log.Debug("Removing expired/inactive sessions");
				foreach (var session in rmList)
				{
					try
					{
						session.Dispose();
					}
					catch (Exception){ }
					sessions.Remove(session);
				}
				Log.Debug("Removed expired/inactive sessions");
			}
		}
        #endregion
    }
}
