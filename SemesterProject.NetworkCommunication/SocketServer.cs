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
		List<ClientSessionServerSide> sessions;

		Thread worker;
		
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
			sessions = new List<ClientSessionServerSide>();

			worker = new Thread(() =>
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
			Log.Information("Server thread started");
		}


		~SocketServer()
		{
			Log.Debug(this.ToString(), "Destuctor");
			this.Dispose();
		}

		public void Dispose()
		{
			Log.Debug(this.ToString(), "Dispose");
			try
			{
				worker?.Abort();
				worker?.Join();

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
            return base.ToString();
        }

        void update()
		{
			try
			{
				var client = _listener.Accept();
				Log.Information($"New connection from {client.RemoteEndPoint}");
				sessions.Add(new ClientSessionServerSide(client, _crypto));
			}
			catch (SocketException ex)
			{
				Log.Error(ex, "Network error");
			}
		}
	}
}
