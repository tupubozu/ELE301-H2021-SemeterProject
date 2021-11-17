using System;
using Npgsql;

namespace SemesterProject.Monitor.GUI
{
    public partial class Monitor : Form
    {
        public NpgsqlConnection? Database;
        public Monitor()
        {
            InitializeComponent();
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            if (!(Database is null))
            {
                var cmd = Database.CreateCommand();
                cmd.CommandText = "select * from logg where curr_timestamp - logg.sentraltid <= time \'01:00:00\';";

                try
                {
                    var tsk = cmd.ExecuteReaderAsync();
                    using var reader = await tsk;
                    reader.Read();

                    do
                    {
                        //listView1.Items.Add((reader.GetString(0),reader.GetString(1),reader.GetString(2)));  
                    } while (reader.Read());
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        private void loggInnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var login = new LogIn(this);
            var result = login.ShowDialog();
        }
    }
}