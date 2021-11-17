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

namespace SemesterProject.Monitor.GUI
{
    public partial class LogIn : Form
    {
        Monitor MainForm;
        public LogIn(Monitor monitor)
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
            NpgsqlConnectionStringBuilder constrBuilder = new NpgsqlConnectionStringBuilder();
            constrBuilder.Host = txtIp.Text;
            constrBuilder.Database = txtDB.Text;
            constrBuilder.Username = txtUser.Text;
            constrBuilder.Password = txtPass.Text;

            try
            {
                MainForm.Database = new NpgsqlConnection(constrBuilder.ConnectionString);

                var tsk = MainForm.Database.OpenAsync();
                await tsk;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,ex.GetType().Name);
            }
        }
    }
}
