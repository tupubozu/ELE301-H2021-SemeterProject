using System;
using SemesterProject.SerialCommunication;
using SemesterProject.Common.Core;
using Serilog;
using System.IO.Ports;
using System.Threading.Tasks;

namespace ELE301.SemesterProject.Kortleser.CLI
{
	class Program
	{
		static bool programStopFlag = false;
		static async Task Main(string[] args)
		{
			SetupLoging();

			Log.Information("Starting program");
			Console.WriteLine("KORTLESER v0.0.1\n\tSemestergruppe 13");

			var ports = SerialPort.GetPortNames();
			int portIndex = 0;
			if (ports.Length == 0)
			{
				Log.Information("No port available. Exiting...");
				return;
			}
			else if (ports.Length == 1)
			{
				portIndex = 0;
			}
			else if (ports.Length > 1)
			{
				try
				{
					Console.WriteLine("Available ports: ");
					for (int i = 0; i < ports.Length; i++)
					{
						Console.WriteLine("\t{0,2}. {1}", i + 1, ports[i]);
					}

					Console.Write("Select port (max {0}): ", ports.Length);
					portIndex = Int32.Parse(Console.ReadLine()) - 1;
					if (portIndex > ports.Length - 1 || portIndex < 0)
						throw new Exception("Illegal value");

					Log.Information("Continuing with port: {0}",ports[portIndex]);
				}
				catch (Exception ex)
				{
					portIndex = 0;
					Log.Error(ex, "Continuing with port: {0}", ports[portIndex]);
				}
			}

			using var comPort = new SerialPort(ports[portIndex], 9600, Parity.None, 8, StopBits.One);
			comPort.Encoding = System.Text.Encoding.ASCII;
			if (!comPort.IsOpen)
			{
				try
				{
					comPort.Open();
				}
				catch (Exception ex)
				{
					Log.Fatal(ex, "Failed to open serial port...");
					return;
				}
			}

			using var dataReader = new SerialStatusStringReader(comPort);
			dataReader.StatusRecieved += DataReader_StatusRecieved;

			Console.CancelKeyPress += Console_CancelKeyPress;

			for(;!programStopFlag;)
			{
				await Task.Delay(1);
			}
			Log.Information("End of program");
		}

		private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			Log.Information("Keyboard interrupt: Setting stop-flag to true...");
			programStopFlag = true;
		}

		private static void DataReader_StatusRecieved(object sender, SerialStatusUpdateEventArgs e)
		{
			Log.Information("Serial status data recieved");

			switch (e.statusData.Source)
			{
				case SerialStatusData.DataSource.Physical:
					break;
				case SerialStatusData.DataSource.Simulation:
					break;
				default:
					break;
			}
		}

		static void SetupLoging()
		{
			Log.Logger = new LoggerConfiguration()
				.WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
				.Enrich.FromLogContext()
				.MinimumLevel.Debug()
				.CreateLogger();
		}
	}
}
