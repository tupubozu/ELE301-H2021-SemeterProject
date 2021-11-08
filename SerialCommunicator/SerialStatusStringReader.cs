using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace ELE301.SemesterProject.SerialCommunication
{
	public partial class SerialStatusStringReader : IDisposable
	{
		DataParserStateMachine fsm;
		SerialPort port;
		public event EventHandler<SerialStatusUpdateEventArgs> StatusRecieved;
		Task updater;
		CancellationTokenSource updateCanceler;

		public SerialStatusStringReader(SerialPort serialPort)
		{
			port = serialPort;
			fsm = new DataParserStateMachine();
			updateCanceler = new CancellationTokenSource();
			updater = Task.Run( async () =>
			{
				for (; !updateCanceler.Token.IsCancellationRequested; )
					await this.Update();
			},updateCanceler.Token);
		}

		~SerialStatusStringReader()
		{
			Dispose();
		}

		async Task Update()
		{
			try
			{
				if (port.BytesToRead != 0)
					lock (port)
					{
						while (port.BytesToRead != 0)
						{
							if (fsm.Update((char)port.ReadChar(), out SerialStatusUpdateEventArgs e))
							{
								StatusRecieved?.Invoke(this, e);
							}
						}
					}
				else await Task.Delay(5);
			}
			catch(Exception ex)
			{
				Log.Error(ex, "Serial operations failed");
			}
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
