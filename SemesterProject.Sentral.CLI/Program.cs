using Microsoft.Extensions.Configuration.UserSecrets;
using Npgsql;
using SemesterProject.Common.Core;
using SemesterProject.NetworkCommunication;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SemesterProject.Sentral.CLI
{
	class Program
	{
		static NpgsqlConnection DatabaseConnection;
		static Task dbConnector;
		static async Task Main(string[] args)
		{
			ProgramCore.SetupLoging();

			Log.Information("Starting program");
			Console.WriteLine("SENTRAL v0.0.1\n\tSemestergruppe 13");

			Console.CancelKeyPress += ProgramCore.Console_CancelKeyPress;
			{
				using Aes aes = AesSecret.GetAes();

				DatabaseConnection = GetDbConnection();

				dbConnector = GetDbConnectorTask();
				
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

				

				await ProgramCore.CheckStopFlag();
			}
			Log.Information("End of program");

#if DEBUG
			Console.ReadKey(true);
#endif
		}

		private static Task GetDbConnectorTask()
		{
			return Task.Run(async () =>
			{
				Log.Information("Attempting to connect to database");
			Retry:
				if (ProgramCore.ProgramCancel.IsCancellationRequested) return;
				try
				{
					Task dbConnerctionOpen = DatabaseConnection.OpenAsync(ProgramCore.ProgramCancel.Token);
					await dbConnerctionOpen;
				}
				catch (Exception ex)
				{
					Log.Debug(ex, "Database connection failed");
					Log.Information("Database connection failed. Retrying in 2 seconds");
					await Task.Delay(2000);
					goto Retry;
				}

				Log.Information("Database connection established");

			}, ProgramCore.ProgramCancel.Token);
		}

		private static void SocketServerSession_MessageRecieved(object sender, NetworkMessage e)
		{
			Log.Information("Network message recieved from node {0} ({2}): {1}", e.NodeNumber, e.Type, sender);
		}

		private static void SocketServerSession_OtherMessage(object sender, NetworkMessage e)
		{
			if (DatabaseConnection.State == ConnectionState.Open)
			{
				var s = sender as SocketServerSession;
				using var cmd = DatabaseConnection.CreateCommand();
				cmd.CommandText = $"insert into Logg (LoggType_ID, Leser_ID, LeserTid, MeldingTid, SentralTid, LoggMelding) values (0,{e.NodeNumber},\'{e.UnitTimestamp}\',\'{e.MessageTimestamp}\',\'{DateTime.Now.ToString("O")}\',\'{e.MessageObject as string}\');";
				var execute = cmd.ExecuteNonQuery();
			}
			else if (dbConnector.IsCompleted)
				dbConnector = GetDbConnectorTask(); 
		}

		private static void SocketServerSession_Breach(object sender, NetworkMessage e)
		{
			if (DatabaseConnection.State == ConnectionState.Open)
			{
				var s = sender as SocketServerSession;
				using var cmd = DatabaseConnection.CreateCommand();
				cmd.CommandText = $"insert into Logg (LoggType_ID, Leser_ID, LeserTid, MeldingTid, SentralTid) values (6,{e.NodeNumber},\'{e.UnitTimestamp}\',\'{e.MessageTimestamp}\',\'{DateTime.Now.ToString("O")}\');";
				var execute = cmd.ExecuteNonQuery();
			}
			else if (dbConnector.IsCompleted)
				dbConnector = GetDbConnectorTask();
		}

		private static void SocketServerSession_AuthSuccess(object sender, NetworkMessage e)
		{
			if (DatabaseConnection.State == ConnectionState.Open)
			{
				var s = sender as SocketServerSession;
				using var cmd = DatabaseConnection.CreateCommand();
				cmd.CommandText = $"insert into Logg (LoggType_ID, Leser_ID, LeserTid, MeldingTid, SentralTid) values (3,{e.NodeNumber},\'{e.UnitTimestamp}\',\'{e.MessageTimestamp}\',\'{DateTime.Now.ToString("O")}\');";
				var execute = cmd.ExecuteNonQuery();
			}
			else if (dbConnector.IsCompleted)
				dbConnector = GetDbConnectorTask();
		}

		private static void SocketServerSession_AuthTimeout(object sender, NetworkMessage e)
		{
			if (DatabaseConnection.State == ConnectionState.Open)
			{
				var s = sender as SocketServerSession;
				using var cmd = DatabaseConnection.CreateCommand();
				cmd.CommandText = $"insert into Logg (LoggType_ID, Leser_ID, LeserTid, MeldingTid, SentralTid) values (4,{e.NodeNumber},\'{e.UnitTimestamp}\',\'{e.MessageTimestamp}\',\'{DateTime.Now.ToString("O")}\');";
				var execute = cmd.ExecuteNonQuery();
			}
			else if (dbConnector.IsCompleted)
				dbConnector = GetDbConnectorTask();
		}

		private static void SocketServerSession_AuthFailure(object sender, NetworkMessage e)
		{
			if (DatabaseConnection.State == ConnectionState.Open)
			{
				var s = sender as SocketServerSession;
				using var cmd = DatabaseConnection.CreateCommand();
				cmd.CommandText = $"insert into Logg (LoggType_ID, Leser_ID, LeserTid, MeldingTid, SentralTid) values (5,{e.NodeNumber},\'{e.UnitTimestamp}\',\'{e.MessageTimestamp}\',\'{DateTime.Now.ToString("O")}\');";
				var execute = cmd.ExecuteNonQuery();
			}
			else if (dbConnector.IsCompleted)
				dbConnector = GetDbConnectorTask();
		}

		private static void SocketServerSession_KeypadPress(object sender, NetworkMessage e)
		{
			if (DatabaseConnection.State == ConnectionState.Open)
			{
				var s = sender as SocketServerSession;
				using var cmd = DatabaseConnection.CreateCommand();
				cmd.CommandText = $"insert into Logg (LoggType_ID, Leser_ID, LeserTid, MeldingTid, SentralTid) values (1,{e.NodeNumber},\'{e.UnitTimestamp}\',\'{e.MessageTimestamp}\',\'{DateTime.Now.ToString("O")}\');";
				var execute = cmd.ExecuteNonQuery();
			}
			else if (dbConnector.IsCompleted)
				dbConnector = GetDbConnectorTask();
		}

		private static async void SocketServerSession_UpdateAccessTable(object sender, NetworkMessage e)
		{
			if (DatabaseConnection.State == ConnectionState.Open)
			{
				var s = sender as SocketServerSession;
				using var cmd = DatabaseConnection.CreateCommand();
				cmd.CommandText = $"select bruker.bruker_id, bruker.kort_id, bruker.kort_pin, bruker.bruker_id in (select bruker_id from bruker inner join tilgang on tilgang.bruker_id = bruker.bruker_id inner join kortleser on kortleser.sone_id = tilgang.sone_id where kortleser.leser_id = {e.NodeNumber} and (bruker.kort_gyldig_start <= current_date and not bruker.kort_gyldig_stop < current_date );) as aksess from bruker;";
				var execute = cmd.ExecuteReaderAsync();
				SortedSet<UserPermission> authTab = new();

				using (var reader = await execute)
					while (reader.Read())
						authTab.Add(new UserPermission()
						{
							UserId = reader.GetInt32(0),
							CardId = reader.GetInt32(1),
							CardCode = reader.GetInt32(2),
							IsAllowed = reader.GetBoolean(3)
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
			else if (dbConnector.IsCompleted)
				dbConnector = GetDbConnectorTask();
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
				catch (Exception ex)
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
