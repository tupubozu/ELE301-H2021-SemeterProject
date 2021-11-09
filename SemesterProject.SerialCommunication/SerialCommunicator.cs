﻿using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using SemesterProject.Common.Core;

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

			Log.Information("Starting monitoring on {0}", port.PortName);
			updater = Task.Run(async () =>
			{
				try
				{
					for (; ; )
					{
						updateCanceler.Token.ThrowIfCancellationRequested();
						await this.Update();
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

			}, updateCanceler.Token);
			Log.Information("Started monitoring on {0}", port.PortName);
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
				if (port.BytesToRead != 0)
				{
					Log.Debug("Handling serial input bytes: {0}", port.BytesToRead);
					for (; ; )
					{
						if (port.BytesToRead == 0) break;
						else if (fsm.Update((char)port.ReadChar(), out SerialStatusUpdateEventArgs e))
						{
							StatusRecieved?.Invoke(this, e);
						}
					}
				}
				else if (fsm.CurrentState == DataParserStateMachine.StateFlag.Closed && commandQueue.Count > 0)
				{
					Log.Debug("Handling serial command: {0}", commandQueue.Peek());
					port.WriteLine(commandQueue.Dequeue().ToString());
				}
				else
				{
					Log.Debug("Pausing serial operations");
					await Task.Delay(5);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Serial operations failed");
			}
		}

		public async void Dispose()
		{
			Log.Debug("Dispose {0}", this.GetType().Name);
			try
			{
				Log.Debug("Stopping worker: {0}", this.GetType().Name);
				updateCanceler?.Cancel();
				if (!updater.IsCompleted)
					await updater;
				Log.Debug("Stopped worker: {0}", this.GetType().Name);

				updater?.Dispose();
				updateCanceler?.Dispose();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Unknown error");
			}
		}
	}
}
