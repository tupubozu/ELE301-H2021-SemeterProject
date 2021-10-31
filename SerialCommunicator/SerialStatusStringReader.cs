using System;
using System.IO.Ports;
using Serilog;

namespace SerialCommunicator
{
	public class SerialStatusStringReader
	{
		SerialStatusStringParserFsm fsm;
		SerialPort port;
		public event EventHandler<SerialStatusUpdateEventArgs> StatusRecieved;

		public SerialStatusStringReader(SerialPort serialPort)
		{
			port = serialPort;
		}

		public void Update()
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
	}
}
