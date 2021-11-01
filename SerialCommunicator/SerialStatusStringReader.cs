using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace SerialCommunicator
{
	public class SerialStatusStringReader : IDisposable
	{
		SerialStatusStringParserFsm fsm;
		SerialPort port;
		public event EventHandler<SerialStatusUpdateEventArgs> StatusRecieved;
		Task updater;
		CancellationTokenSource updateCanceler;
		public SerialStatusStringReader(SerialPort serialPort)
		{
			port = serialPort;
			fsm = new SerialStatusStringParserFsm();
			updateCanceler = new CancellationTokenSource();
			updater = Task.Run(() =>
			{
				for (; !updateCanceler.Token.IsCancellationRequested; )
					this.Update();
			},updateCanceler.Token);
		}

		~SerialStatusStringReader()
		{
			Dispose();
		}

		void Update()
		{
			try
			{
				while (port.BytesToRead != 0)
				{
					if (fsm.Update((char)port.ReadChar(), out SerialStatusUpdateEventArgs e))
					{
						StatusRecieved?.Invoke(this, e);
					}
				}
			}
			catch(Exception ex)
			{
				Log.Error(ex, "Serial operations failed");
			}
		}

		protected virtual void OnStatusRecieved(SerialStatusUpdateEventArgs e)
		{
			EventHandler<SerialStatusUpdateEventArgs> handler = StatusRecieved;
			handler?.Invoke(this, e);
		}

		public void Dispose()
		{
			try
			{
				updateCanceler?.Cancel();
				updater?.Wait(100);
				updater?.Dispose();
				updateCanceler?.Dispose();
			}
			catch (Exception)
			{ }
		}
	}
}
