using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;

namespace SemesterProject.SerialCommunication
{
	public partial class SerialCommunicator : IDisposable
	{
		Queue<SerialCommand> commandQueue;
		DataParserStateMachine fsm;
		SerialPort port;
		public event EventHandler<SerialStatusUpdateEventArgs> StatusRecieved;
		Task updater;
		CancellationTokenSource updateCanceler;

		public SerialCommunicator(SerialPort serialPort)
		{
			commandQueue = new Queue<SerialCommand>();
			port = serialPort;
			fsm = new DataParserStateMachine();
			updateCanceler = new CancellationTokenSource();
			updater = Task.Run( async () =>
			{
				for (; !updateCanceler.Token.IsCancellationRequested; )
					await this.Update();
			},updateCanceler.Token);
		}

		~SerialCommunicator()
		{
			Dispose();
		}

		public void EnqueueCommand(SerialCommand command)
		{
			commandQueue.Enqueue(command);
		}

		async Task Update()
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

				if (fsm.CurrentState == DataParserStateMachine.StateFlag.Closed && commandQueue.Count > 0)
				{
					port.WriteLine(commandQueue.Dequeue().ToString());
				}
				else await Task.Delay(5);
			}
			catch (Exception ex)
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
