using System;
using SemesterProject.Common.Core;
using SemesterProject.NetworkCommunication;
using System.Security.Cryptography;
using Serilog;
using System.Threading.Tasks;
using System.Threading;
using Npgsql;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace SemesterProject.Sentral.CLI
{
	class Program
	{
		static async Task Main(string[] args)
		{
			ProgramCore.SetupLoging();

			Log.Information("Starting program");
			Console.WriteLine("SENTRAL v0.0.1\n\tSemestergruppe 13");

			using Aes aes = AesSecret.GetAes();

			NpgsqlConnection conn = GetDbConnection();

			Console.CancelKeyPress += ProgramCore.Console_CancelKeyPress;
			{
				Log.Information("Creating server object");
				using SocketServer socketServer = new(aes);

				await ProgramCore.CheckStopFlag();
			}
			Log.Information("End of program");

#if DEBUG
			Console.ReadKey(true);
#endif
		}
		static NpgsqlConnection GetDbConnection()
		{
			string dbSecrets = Path.Combine(Path.GetDirectoryName(PathHelper.GetSecretsPathFromSecretsId("2582243a-5592-4d35-96c1-e622e5f09a1a")), "dbSecrets.xml"); // XML
			//string dbSecrets = PathHelper.GetSecretsPathFromSecretsId("2582243a-5592-4d35-96c1-e622e5f09a1a"); //JSON

			DbSettings settings = null;

			bool forceSecretsCreation = false;
		ForcedSecretsCreation:
			if (!File.Exists(dbSecrets) || forceSecretsCreation)
			{
				if (!forceSecretsCreation) Log.Information("Database secrets not found");

				Log.Information("Generating secrets");
				settings = new();

				try
				{
					Log.Information("Generating serializeable sectrets object");
					//string secretContent = JsonSerializer.Serialize(settings);
					Log.Information("Creating new secrets file");
					Directory.CreateDirectory(Path.GetDirectoryName(dbSecrets));
					using var secretsFile = File.Open(dbSecrets, FileMode.Create, FileAccess.Write);
					var serializer = new XmlSerializer(typeof(DbSettings));
					serializer.Serialize(secretsFile, settings);
					//using var secretsWriter = new StreamWriter(secretsFile, Encoding.UTF8);
					//secretsWriter.WriteLine(secretContent);
					Log.Information("Secrets file created");
				}
				catch(Exception ex)
				{
					Log.Error(ex, "Secrets creation failed");
				}
			}
			else
			{
				Log.Information("Serializing database secrets from secrets file");

				try
				{
					using var secretsFile = File.Open(dbSecrets, FileMode.Open, FileAccess.Read);
					var serializer = new XmlSerializer(typeof(DbSettings));
					settings = serializer.Deserialize(secretsFile) as DbSettings;
					//settings = JsonSerializer.Deserialize(secretsFile, typeof(DbSettings)) as DbSettings;
					if (settings is null)
					{
						throw new NullReferenceException();
					}
					
				}
				catch (Exception ex)
				{
					Log.Error(ex, "Serialization failed");
					forceSecretsCreation = true;
					Log.Information("Secrets invalid: Forcing generation of new values");
					goto ForcedSecretsCreation;
				}
			}

			NpgsqlConnectionStringBuilder strBuilder = new NpgsqlConnectionStringBuilder();

			strBuilder.Username = settings.Username;
			strBuilder.Password = settings.Password;
			strBuilder.Database = settings.Database;
			strBuilder.Host = settings.Host;
			strBuilder.Port = settings.Port;
			strBuilder.TcpKeepAlive = settings.TcpKeepAlive;
			strBuilder.KeepAlive = settings.KeepAlive;

			NpgsqlConnection conn = new(strBuilder.ConnectionString);

			return conn;
		}
	}
	[Serializable]
	[JsonSerializable(typeof(DbSettings))]
	public class DbSettings
	{
		public string Username = "user";
		public string Password = "pass";
		public string Database = "base";
		public string Host = "127.0.0.1";
		public int Port = 9001;
		public bool TcpKeepAlive = true;
		public int KeepAlive = 1;
	}

}
