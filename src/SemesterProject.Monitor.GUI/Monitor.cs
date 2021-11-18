using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Npgsql;
using SemesterProject.DatabaseCommunication;

namespace SemesterProject.Monitor.GUI
{
    public partial class Monitor : Form, IBased
    {
        public NpgsqlConnection? Database { get; set; }
        private List<LogEntry> entries = new List<LogEntry>();
        public Monitor()
        {
            InitializeComponent();
        }

        private async void timer1_Tick(object sender, EventArgs e) // Do _NOT_ enable. Here be (library) dragons.
        {
            timer1.Enabled = false;
            if (!(Database is null))
            {
                var cmd = Database.CreateCommand();
                cmd.CommandText = "select Logg.Logg_ID, Logg.LoggType_ID, Logg.Leser_ID, Logg.LeserTid, Logg.MeldingTid, Logg.SentralTid, Logg.Brukar_ID, Logg.LoggMelding from logg;"; //where current_timestamp - logg.sentraltid <= time \'01:00:00\';";

                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                        if (reader.Read())
                        {
                            entries.Clear();
                            listView1.Items.Clear();

                            do
                            {
                                entries.Add(new LogEntry()
                                {
                                    ID = reader.GetInt32(0),
                                    TypeID = reader.GetInt16(1),
                                    ReaderID = reader.GetInt32(2),
                                    ReaderTime = reader.GetDateTime(3),
                                    MessageTime = reader.GetDateTime(4),
                                    SentralTime = reader.GetDateTime(5),
                                    User_ID = reader.GetValue(6) as int?,
                                    LogMessage = reader.GetValue(7) as string,
                                });
                            } while (reader.Read());
                            reader.Close();
                        }

                    foreach (var entry in entries)
                    {
                        listView1.Items.Add(entry.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void loggInnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new LogIn(this).Show();
        }

        private void adgangToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new LogInspector(this, "Brukar", "Adgangslogg",LogInspector.QueryType.Generic).Show();
        }

        private void brukerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new LogInspector(this, "Node", "Avviste brukarar", LogInspector.QueryType.UserAuthFail).Show();
        }

        private void dørToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new LogInspector(this, "Node", "Innpasseringsforsøk", LogInspector.QueryType.DoorAccess).Show();
        }

        private void alarmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new LogInspector(this, "Alarmnivå", "Alarmar", LogInspector.QueryType.AlarmLevel).Show();
        }
    }
}