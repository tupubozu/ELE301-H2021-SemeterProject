using System;
using System.Collections.Generic;
using System.Text;
using Serilog;
using System.Threading.Tasks;
using System.Threading;

namespace SemesterProject.Common.Core
{
	public static class ProgramCore
	{
		public static CancellationTokenSource ProgramCancel = new CancellationTokenSource();

		private static bool programStopFlag = false;
		private static object syncObject =  new object();
		public static void SetupLoging()
		{
			Log.Logger = new LoggerConfiguration()
#if DEBUG
				.WriteTo.Console(Serilog.Events.LogEventLevel.Verbose)
#else
				.WriteTo.Console(Serilog.Events.LogEventLevel.Information)
				.WriteTo.Console(Serilog.Events.LogEventLevel.Warning)
				.WriteTo.Console(Serilog.Events.LogEventLevel.Error)
#endif
				.Enrich.FromLogContext()
				.MinimumLevel.Debug()
				.CreateLogger();
		}
		public static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			Console.Out.WriteLine("KeyboardInterrupt");
			Log.Information("Keyboard interrupt: Setting stop flag to true...");
			lock (syncObject)
				programStopFlag = true;
			Log.Debug("Keyboard interrupt: Stop flag is set to true");

			e.Cancel = true;
		}

		public static async Task CheckStopFlag()
		{
			Log.Debug("Awaiting stop flag");
			for (; ; )
			{
				lock (syncObject)
					if (programStopFlag) break;
				await Task.Delay(100);
			}
			ProgramCancel.Cancel();
			Log.Debug("Stop flag recieved");
		}
	}
}
