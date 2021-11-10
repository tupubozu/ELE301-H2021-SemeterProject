﻿using System;
using SemesterProject.SerialCommunication;
using SemesterProject.NetworkCommunication;
using SemesterProject.Common.Core;
using Serilog;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace SemesterProject.Kortleser.CLI
{
	class Program
	{
		static SocketClientSession clientSession;
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

			using var dataReader = new SerialCommunicator(comPort);
			dataReader.StatusRecieved += DataReader_StatusRecieved;

			Aes aes = AesSecret.GetAes();

			clientSession = new SocketClientSession(aes);

			Console.CancelKeyPress += ProgramCore.Console_CancelKeyPress;

			await ProgramCore.CheckStopFlag();
			Log.Information("End of program");

#if DEBUG
			Console.ReadKey(true);
#endif
		}

		private static void DataReader_StatusRecieved(object sender, SerialStatusUpdateEventArgs e)
		{
			Log.Information("Serial status data recieved from node {0}",e.StatusData.NodeNumber);
			Console.WriteLine(e.StatusData);
			NetworkMessage data = new NetworkMessage()
			{
				MessageTimestamp = DateTime.Now,
				UnitTimestamp = e.StatusData.Timestamp,
				UnitNumber = e.StatusData.NodeNumber,
				Type = NetworkMessage.MessageType.Other,
				MessageObject = null
			};

            try
            {
				clientSession?.EnqueueNetworkData(data);
			}
			catch (Exception) { }
		}
	}
}
