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
		Socket _listener;
		List<SocketClientSession> sessions;

		Task _worker;
		CancellationTokenSource _canceler;

		Aes _crypto;

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
			Log.Information($"Starting server on {listener.LocalEndPoint}");
			_crypto = aes;
			_listener = listener;
			_listener.Listen(1000);
			sessions = new List<SocketClientSession>();

			_canceler = new CancellationTokenSource();
			_worker = Task.Run(() =>
			{
				for (; !_canceler.Token.IsCancellationRequested;)
					this.update();
			}, _canceler.Token);
		}


		~SocketServer()
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

				foreach (var session in sessions) session?.Dispose();
				_canceler?.Dispose();
				_listener?.Dispose();
			}
			catch (Exception)
			{ }
		}

		void update()
		{
			var client = _listener.Accept();
			Log.Information($"New connection from {client.RemoteEndPoint}");
			sessions.Add(new SocketClientSession(client,_crypto));
		}
	}
}
