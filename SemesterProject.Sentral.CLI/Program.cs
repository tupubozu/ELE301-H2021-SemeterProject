using System;
using SemesterProject.Common.Core;
using SemesterProject.NetworkCommunication;
using System.Security.Cryptography;
using Serilog;
using System.Threading.Tasks;
using System.Threading;

namespace SemesterProject.Sentral.CLI
{
    class Program
	{
		class SyncObject
		{
			public bool programStopFlag = false;
		}
		static SyncObject syncObject = new SyncObject();
		static async Task Main(string[] args)
		{
			SetupLoging();
			Aes aes;

			//if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			//	aes = new AesCng();
			//else if (Environment.OSVersion.Platform == PlatformID.Unix)
				aes = Aes.Create();
			//else
			//	return;

			aes.Mode = CipherMode.CFB;
			aes.Padding = PaddingMode.ISO10126;
			aes.KeySize = 128;
			aes.BlockSize = 128;

			AesSecret aesSecret = AesSecret.GetSecret(aes);
			
			var sync = new SyncObject();
			ConsoleCancelEventHandler func = (object sender, ConsoleCancelEventArgs e) =>
			{
				Log.Information("Keyboard interrupt: Setting stop-flag to true...");
				
				lock (sync)
					sync.programStopFlag = true;

				return;
			};

			//Console.CancelKeyPress += Console_CancelKeyPress;
			Console.CancelKeyPress += func;

			try
			{
				Log.Information("Creating server object");
				using SocketServer socketServer = new(aes);

				for (; ;)
				{
					lock (sync)
						if (sync.programStopFlag) break;
					
					await Task.Delay(10);
				}
			}
			catch (Exception ex) 
			{
				Log.Information(ex, "Exception");
				Log.Error(ex, "Exception");
			}

			Log.Information("End of program");

#if DEBUG
			Console.ReadKey(true);
#endif
		}
		static void SetupLoging()
		{
			Log.Logger = new LoggerConfiguration()
#if DEBUG
				.WriteTo.Console(Serilog.Events.LogEventLevel.Verbose)
#else
				.WriteTo.Console(Serilog.Events.LogEventLevel.Information)
#endif
				.Enrich.FromLogContext()
				.MinimumLevel.Debug()
				.CreateLogger();
		}
		private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			Log.Information("Keyboard interrupt: Setting stop-flag to true...");
			Task.Run(() =>
			{
				lock (syncObject)
					syncObject.programStopFlag = true;

			});
		}
	}
}
