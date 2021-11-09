using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;
using Microsoft.Extensions.Configuration.UserSecrets;
using Serilog;

namespace SemesterProject.Common.Core
{ 
	[Serializable]
	public class AesSecret
	{
		public byte[] Key;
		public byte[] IV;

		public static AesSecret GetSecret(Aes aes)
		{
			bool forceSecretsCreation = false;
			AesSecret aesSecret = null;
			string secretsPath = Path.Combine(Path.GetDirectoryName(PathHelper.GetSecretsPathFromSecretsId("687cbd63-3b58-4935-818b-22d3db77947e")), "aesSecrets.xml");
		ForcedSecretsCreation:
			if (!File.Exists(secretsPath) || forceSecretsCreation)
			{
				if (!forceSecretsCreation) Log.Information("Secrets not found");
				Directory.CreateDirectory(Path.GetDirectoryName(secretsPath));

				Log.Information("Generating secrets");
				aes.GenerateKey();
				aes.GenerateIV();
				aesSecret = new AesSecret()
				{
					Key = aes.Key,
					IV = aes.IV
				};

				Log.Information("Generating serializeable sectrets object");
				using (var secretsFile = new FileStream(secretsPath, FileMode.Create, FileAccess.Write))
				{
					Log.Information("Creating new secrets file");
					var serializer = new XmlSerializer(typeof(AesSecret));
					serializer.Serialize(secretsFile, aesSecret);
					Log.Information("Secrets file created");
				}
			}
			else
			{
				Log.Information("Serializing secrets from secrets file");

				try
				{
					using (var secretsFile = new FileStream(secretsPath, FileMode.Open, FileAccess.Read))
					{
						var serializer = new XmlSerializer(typeof(AesSecret));
						aesSecret = serializer.Deserialize(secretsFile) as AesSecret;
					}

					//aesSecret = JsonSerializer.Deserialize<AesSecret>(secretPath);
				}
				catch (Exception ex)
				{
					Log.Error(ex, "Serialization failed");
					forceSecretsCreation = true;
					Log.Information("Secrets invalid: Forcing generation of new values");
					goto ForcedSecretsCreation;
					throw;
				}

				aes.Key = aesSecret.Key;
				aes.IV = aesSecret.IV;
			}
			return aesSecret;
		}
	}
}
