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
		
		static bool programStopFlag = false;
		static object syncObject = new object();
		static async Task Main(string[] args)
		{
			ProgramCore.SetupLoging();

			Log.Information("Starting program");
			Console.WriteLine("SENTRAL v0.0.1\n\tSemestergruppe 13");

			using Aes aes = Aes.Create();
			aes.Mode = CipherMode.CFB;
			aes.Padding = PaddingMode.ISO10126;
			aes.KeySize = 128;
			aes.BlockSize = 128;

			AesSecret aesSecret = AesSecret.GetSecret(aes);

			Console.CancelKeyPress += ProgramCore.Console_CancelKeyPress;

			Log.Information("Creating server object");
			using SocketServer socketServer = new(aes);

			await ProgramCore.checkStopFlag();

			Log.Information("End of program");

#if DEBUG
			Console.ReadKey(true);
#endif
		}
	}
}
