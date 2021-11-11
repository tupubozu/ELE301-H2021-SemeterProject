using System;
using SemesterProject.SerialCommunication;
using SemesterProject.NetworkCommunication;
using SemesterProject.Common.Core;
using Serilog;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace SemesterProject.Kortleser.CLI
{
	class Program
	{
		static SocketClientSession clientSession;
		static SerialCommunicator dataReader;
		static AuthStateController controller;
		static async Task Main(string[] args)
		{
			ProgramCore.SetupLoging();

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

					Log.Information("Continuing with port: {0}", ports[portIndex]);
				}
				catch (Exception ex)
				{
					portIndex = 0;
					Log.Error(ex, "Continuing with port: {0}", ports[portIndex]);
				}
			}

			using var comPort = new SerialPort(ports[portIndex], 9600, Parity.None, 8, StopBits.One);
			comPort.Encoding = System.Text.Encoding.ASCII;
			try
			{
				comPort.Open();
			}
			catch (Exception ex)
			{
				Log.Information("Failed to open serial port...");
				Log.Fatal(ex, "Failed to open serial port...");
				return;
			}

			dataReader = new SerialCommunicator(comPort);
			dataReader.StatusRecieved += DataReader_StatusRecieved;

			Aes aes = AesSecret.GetAes();

			clientSession = new SocketClientSession(aes);
            clientSession.UpdateNodeClock += ClientSession_UpdateNodeClock;
            clientSession.UpdateAccessTable += ClientSession_UpdateAccessTable;

			controller = new AuthStateController();
            controller.RequestAccessTable += Controller_RequestAccessTable;
            controller.Closed += Controller_Closed;
            controller.KeypadPress += Controller_KeypadPress;
            controller.AuthSuccess += Controller_AuthSuccess;
            controller.AuthFailure += Controller_AuthFailure;
            controller.AuthTimeout += Controller_AuthTimeout;
            controller.Breached += Controller_Breached;

			Console.CancelKeyPress += ProgramCore.Console_CancelKeyPress;

			await ProgramCore.CheckStopFlag();

			dataReader.Dispose();
			clientSession.Dispose();
			Log.Information("End of program");

#if DEBUG
			Console.ReadKey(true);
#endif
		}

        private static void Controller_Breached(object sender, AuthControllerEventArgs e)
        {
			var message = new NetworkMessage()
			{
				Type = NetworkMessage.MessageType.Breach,
				MessageTimestamp = DateTime.Now,
				NodeNumber = e.StatusData.NodeNumber,
				UnitTimestamp = e.StatusData.Timestamp,
				MessageObject = null
			};

			clientSession.EnqueueNetworkData(message);
		}

        private static void Controller_AuthTimeout(object sender, AuthControllerEventArgs e)
        {
			var message = new NetworkMessage()
			{
				Type = NetworkMessage.MessageType.AuthTimeout,
				MessageTimestamp = DateTime.Now,
				NodeNumber = e.StatusData.NodeNumber,
				UnitTimestamp = e.StatusData.Timestamp,
				MessageObject = null
			};

			clientSession.EnqueueNetworkData(message);
		}

        private static void Controller_AuthFailure(object sender, AuthControllerEventArgs e)
        {
			var message = new NetworkMessage()
			{
				Type = NetworkMessage.MessageType.AuthFailure,
				MessageTimestamp = DateTime.Now,
				NodeNumber = e.StatusData.NodeNumber,
				UnitTimestamp = e.StatusData.Timestamp,
				MessageObject = e.Permission
			};

			clientSession.EnqueueNetworkData(message);
		}

        private static void Controller_AuthSuccess(object sender, AuthControllerEventArgs e)
        {
			var message = new NetworkMessage()
			{
				Type = NetworkMessage.MessageType.AuthSuccess,
				MessageTimestamp = DateTime.Now,
				NodeNumber = e.StatusData.NodeNumber,
				UnitTimestamp = e.StatusData.Timestamp,
				MessageObject = e.Permission
			};

			clientSession.EnqueueNetworkData(message);
		}

        private static void Controller_KeypadPress(object sender, AuthControllerEventArgs e)
        {
			var message = new NetworkMessage()
			{
				Type = NetworkMessage.MessageType.KeypadPress,
				MessageTimestamp = DateTime.Now,
				NodeNumber = e.StatusData.NodeNumber,
				UnitTimestamp = e.StatusData.Timestamp,
				MessageObject = null
			};

			clientSession.EnqueueNetworkData(message);
		}

        private static void Controller_Closed(object sender, AuthControllerEventArgs e)
        {
			var message = new NetworkMessage()
			{
				Type = NetworkMessage.MessageType.Closed,
				MessageTimestamp = DateTime.Now,
				NodeNumber = e.StatusData.NodeNumber,
				UnitTimestamp = e.StatusData.Timestamp,
				MessageObject = null
			};

			clientSession.EnqueueNetworkData(message);
		}

        private static void Controller_RequestAccessTable(object sender, AuthControllerEventArgs e)
        {
			var message = new NetworkMessage()
			{
				Type = NetworkMessage.MessageType.RequestAccessTable,
				MessageTimestamp = DateTime.Now,
				NodeNumber = e.StatusData.NodeNumber,
				UnitTimestamp = e.StatusData.Timestamp,
				MessageObject = null
			};

			clientSession.EnqueueNetworkData(message);
        }

        private static void ClientSession_UpdateAccessTable(object sender, NetworkMessageUpdateEventArgs e)
        {
			if (e.MessageData.MessageObject is SortedSet<UserPermission>)
				controller.AuthTable = e.MessageData.MessageObject as SortedSet<UserPermission>;
        }

        private static void ClientSession_UpdateNodeClock(object sender, NetworkMessageUpdateEventArgs e)
        {
            var commands = SerialCommand.SetDateTime(e.MessageData.UnitTimestamp);
			foreach (var command in commands) dataReader.EnqueueCommand(command);
        }

		private static void DataReader_StatusRecieved(object sender, SerialStatusData e)
		{
			Log.Information("Serial status data recieved from node {0}", e.NodeNumber);
			Console.WriteLine(e);
			NetworkMessage data = new NetworkMessage()
			{
				MessageTimestamp = DateTime.Now,
				UnitTimestamp = e.Timestamp,
				NodeNumber = e.NodeNumber,
				Type = NetworkMessage.MessageType.Other,
				MessageObject = null
			};
		
			clientSession?.EnqueueNetworkData(data);
		}
	}
}
