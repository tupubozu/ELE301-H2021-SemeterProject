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
        public LogIn(IBased monitor)
        {
            InitializeComponent();
            MainForm = monitor;
        }

        private void btnAvbryt_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnLogIn_Click(object sender, EventArgs e)
        {
            try
            {
                NpgsqlConnectionStringBuilder constrBuilder = new NpgsqlConnectionStringBuilder();
                constrBuilder.Host = txtIp.Text;
                constrBuilder.Port = Convert.ToInt32(txtPort.Text, 10);
                constrBuilder.Database = txtDB.Text;
                constrBuilder.Username = txtUser.Text;
                constrBuilder.Password = txtPass.Text;

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
