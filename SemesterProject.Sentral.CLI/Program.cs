using System;
using System.Collections.Generic;
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
		static NpgsqlConnection DatabaseConnection;
		static async Task Main(string[] args)
		{
			ProgramCore.SetupLoging();

			Log.Information("Starting program");
			Console.WriteLine("SENTRAL v0.0.1\n\tSemestergruppe 13");

			Console.CancelKeyPress += ProgramCore.Console_CancelKeyPress;
			{
				using Aes aes = AesSecret.GetAes();

				DatabaseConnection = GetDbConnection();
				Task dbConnerctionOpen = DatabaseConnection.OpenAsync(ProgramCore.ProgramCancel.Token);
				Log.Information("Creating server object");
				using SocketServer socketServer = new(aes);
				
				SocketServerSession.MessageRecieved += SocketServerSession_MessageRecieved;
				SocketServerSession.UpdateAccessTable += SocketServerSession_UpdateAccessTable;
				SocketServerSession.KeypadPress += SocketServerSession_KeypadPress;
				SocketServerSession.AuthFailure += SocketServerSession_AuthFailure;
				SocketServerSession.AuthTimeout += SocketServerSession_AuthTimeout;
				SocketServerSession.AuthSuccess += SocketServerSession_AuthSuccess;
				SocketServerSession.Breach += SocketServerSession_Breach;
				SocketServerSession.OtherMessage += SocketServerSession_OtherMessage;

				await dbConnerctionOpen;

				await ProgramCore.CheckStopFlag();
			}
			Log.Information("End of program");

#if DEBUG
			Console.ReadKey(true);
#endif
		}

		private static void SocketServerSession_MessageRecieved(object sender, NetworkMessage e)
		{
			Log.Information("Network message recieved from node {0} ({2}): {1}", e.NodeNumber, e.Type, sender);
		}

		private static void SocketServerSession_OtherMessage(object sender, NetworkMessage e)
		{
			throw new NotImplementedException();
		}

		private static void SocketServerSession_Breach(object sender, NetworkMessage e)
		{
			throw new NotImplementedException();
		}

		private static void SocketServerSession_AuthSuccess(object sender, NetworkMessage e)
		{
			throw new NotImplementedException();
		}

		private static void SocketServerSession_AuthTimeout(object sender, NetworkMessage e)
		{
			throw new NotImplementedException();
		}

		private static void SocketServerSession_AuthFailure(object sender, NetworkMessage e)
		{
			throw new NotImplementedException();
		}

		private static void SocketServerSession_KeypadPress(object sender, NetworkMessage e)
		{
			throw new NotImplementedException();
		}

		private static async void SocketServerSession_UpdateAccessTable(object sender, NetworkMessage e)
		{
			var s = sender as SocketServerSession;
			using var cmd = DatabaseConnection.CreateCommand();
			cmd.CommandText = $"select bruker.bruker_id, bruker.kode, bruker.bruker_id in (select bruker_id from bruker inner join tilgang on tilgang.bruker_id = tilgang.bruker_id inner join kortleser on kortleser.leser_id = tilgang.leser_id where kortleser.leser_id = {e.NodeNumber};) as aksess from users;";
			var execute = cmd.ExecuteReaderAsync();
			SortedSet<UserPermission> authTab = new();

			using (var reader = await execute)
				while (reader.Read())
					authTab.Add(new UserPermission()
					{
						UserId = reader.GetInt32(0),
						PassCode = reader.GetInt32(1),
						IsAllowed = reader.GetBoolean(2)
					});

			DateTime current = DateTime.Now;
			s.EnqueueNetworkData(new NetworkMessage()
			{
				MessageObject = authTab,
				MessageTimestamp = current,
				NodeNumber = 0,
				UnitTimestamp = current,
				Type = NetworkMessage.MessageType.UpdateAccessTable
			});

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
