using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using SemesterProject.DatabaseCommunication;

namespace SemesterProject.Monitor.GUI
{
    public partial class LogIn : Form
    {
        IBased MainForm;

        private static string serverIP = string.Empty;
        private static int serverPort = 5432;
        private static string database = string.Empty;
        private static string username = string.Empty;
        private static string password = string.Empty;

        public LogIn(IBased monitor)
        {
            InitializeComponent();
            MainForm = monitor;

            txtIp.Text = serverIP;
            txtPort.Text = serverPort.ToString();
            txtDB.Text = database;
            txtUser.Text = username;
            txtPass.Text = password;
        }

        private void btnAvbryt_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnLogIn_Click(object sender, EventArgs e)
        {
            try
            {
                serverIP = txtIp.Text;
                serverPort = Convert.ToInt32(txtPort.Text, 10);
                database = txtDB.Text;
                username = txtUser.Text;
                password = txtPass.Text;

                NpgsqlConnectionStringBuilder constrBuilder = new NpgsqlConnectionStringBuilder();
                constrBuilder.Host = serverIP;
                constrBuilder.Port = serverPort;
                constrBuilder.Database = database;
                constrBuilder.Username = username;
                constrBuilder.Password = password;

                MainForm.Database = new NpgsqlConnection(constrBuilder.ConnectionString);

                Task tsk = MainForm.Database.OpenAsync();
                await tsk;

                using var cmd = MainForm.Database.CreateCommand();

                cmd.CommandText = "select LoggType_ID, AlarmNivaa_ID, TypeNamn from LoggType;";
                var rd_tsk = cmd.ExecuteReaderAsync();
                using (var reader = await rd_tsk)
                    if (reader.Read())
                    {
                        do
                        {
                            LogEntry.EntryTypes.Add(new LogEntryType()
                            {
                                ID = reader.GetInt32(0),
                                LevelID = reader.GetInt32(1),
                                Name = reader.GetString(2)
                            });
                        } while (reader.Read());
                    }

                cmd.CommandText = "select AlarmNivaa_ID, Namn from AlarmNivaa;";
                rd_tsk = cmd.ExecuteReaderAsync();
                using (var reader = await rd_tsk)
                    if (reader.Read())
                    {
                        do
                        {
                            LogEntryType.Levels.Add(new LogEntryLevel()
                            {
                                ID = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        } while (reader.Read());
                    }

                cmd.CommandText = "select Sone_ID, SoneNamn from Tilgangssone;";
                rd_tsk = cmd.ExecuteReaderAsync();
                using (var reader = await rd_tsk)
                    if (reader.Read())
                    {
                        do
                        {
                            CardReader.Sones.Add(new AccessSone()
                            {
                                ID = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        } while (reader.Read());
                    }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
    }
}
