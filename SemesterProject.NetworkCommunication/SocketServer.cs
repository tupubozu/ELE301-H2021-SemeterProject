using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Serilog;

namespace SemesterProject.NetworkCommunication
{
	public class SocketServer: IDisposable
	{
		const ushort validator = 1999;

		Socket _listener;
		List<SocketSessionServerSide> sessions;

		CancellationTokenSource cancellation;
		Task worker;
		
		Aes _crypto;

		UdpClient broadcast;
		IPEndPoint broadcastEndPoint;
		CancellationTokenSource broadcastCanceller;
		Task broadcaster;

		public SocketServer(Aes aes)
		{
			Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 1337);
			listener.Bind(serverEndPoint);
			
			init(listener, aes);
		}


		public SocketServer(Socket listener, Aes aes)
		{
			init(listener, aes);
		}

		private void init(Socket listener, Aes aes)
		{
			Log.Information("Starting server on {0}", listener.LocalEndPoint);
			_crypto = aes;
			_listener = listener;
			_listener.Blocking = false;
			_listener.Listen(1000);
			sessions = new List<SocketSessionServerSide>();
			cancellation = new CancellationTokenSource();

			worker = Task.Run(async () =>
			{
				try
				{
					for (; ; )
					{
						cancellation.Token.ThrowIfCancellationRequested();
						await this.update();
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

			Log.Information("Started server on {0}", listener.LocalEndPoint);

			initBroadcast();
		}

		async Task broadcastUpdate()
		{
			Task timeDelay = Task.Delay(5000);
			byte[] buffer = { (validator & (0xff << 8)) >> 8, validator & 0xff };
			broadcast.Send(buffer,2);
			Log.Debug("Broadcast sendt");
			await timeDelay;

		}

		void initBroadcast()
		{
			broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, 9001);
			broadcastCanceller = new CancellationTokenSource();

			broadcast = new UdpClient(69000);
			broadcast.Connect(broadcastEndPoint);

			Log.Information("Starting broadcaster on {0}", broadcast.Client.LocalEndPoint);
			broadcaster = Task.Run(async () =>
			{
				try
				{
					for (; ; )
					{
						broadcastCanceller.Token.ThrowIfCancellationRequested();
						await broadcastUpdate();
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
				if (!broadcaster.IsCompleted)
					broadcaster?.Wait();
				Log.Debug("Stopped broadcaster: {0}", this.GetType().Name);
				broadcaster?.Dispose();
				broadcastCanceller?.Dispose();

				Log.Debug("Stopping worker: {0}", this.GetType().Name);
				cancellation?.Cancel();
				if (!worker.IsCompleted)
					worker?.Wait();
				Log.Debug("Stopped worker: {0}", this.GetType().Name);
				worker?.Dispose();
				cancellation?.Dispose();

				foreach (var session in sessions) session?.Dispose();
				_listener?.Dispose();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Unkown error");
			}
		}

		public override string ToString()
		{
			return $"{base.GetType().Name}({_listener})";
		}

		async Task update()
		{
			try
			{
				var client = _listener.Accept();
				Log.Information($"New connection from {client.RemoteEndPoint}");
				sessions.Add(new SocketSessionServerSide(client, _crypto));
			}
			catch (SocketException ex)
			{
				Task task = Task.Delay(100);
				Log.Debug(ex, "Network error");
				await task;
			}

			List<SocketSessionServerSide> rmList = new List<SocketSessionServerSide>();
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
				session.Dispose();
				sessions.Remove(session);
            }
			Log.Debug("Removed expired/inactive sessions");

		}
	}
}
