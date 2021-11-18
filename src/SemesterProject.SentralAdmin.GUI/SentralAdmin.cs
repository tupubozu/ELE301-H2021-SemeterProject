using SemesterProject.Monitor.GUI;
using SemesterProject.DatabaseCommunication;
using Npgsql;

namespace SemesterProject.SentralAdmin.GUI
{
    public partial class SentralAdmin : Form, IBased
    {
        public NpgsqlConnection? Database { get; set; }

        public SentralAdmin()
        {
            InitializeComponent();
        }
        private void loggInnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new LogIn(this).Show();
        }

        private void oppfriskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Placeholder");
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Placeholder");
        }

        private void btnRemoveUser_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Placeholder");
        }

        private void btnAddReader_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Placeholder");
        }

        private void btnRemoveReader_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Placeholder");
        }

        private void btnAddSone_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Placeholder");
        }

        private void btnRemoveSone_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Placeholder");
        }

        private void listUsers_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (CardReader.Sones.Count != 0)
                {
                    cbSoneSelector.Items.AddRange(CardReader.Sones.ToArray());
                }
                if (listUsers.SelectedValue != null)
                {
                    User user = (User)listUsers.SelectedItem;
                    txtUserID.Text = user.ID.ToString();
                    txtFirstName.Text = user.FirstName;
                    txtLastName.Text = user.LastName;
                    txtEmail.Text = user.Email;
                    StartTimePicker.Value = user.CardValidStart;
                    StopTimePicker.Value = user.CardValidEnd ?? DateTime.MaxValue;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void listReaders_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (CardReader.Sones.Count != 0)
                {
                    cbReaderSone.Items.AddRange(CardReader.Sones.ToArray());
                }
                if (listReaders.SelectedValue != null)
                {
                    CardReader reader = (CardReader)listReaders.SelectedItem;
                    txtReaderID.Text = reader.ID.ToString();
                    txtPlacement.Text = reader.Placement;
                    cbReaderSone.SelectedIndex = cbReaderSone.Items.IndexOf(CardReader.Sones.Find((AccessSone sone) => sone.ID == reader.SoneID));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void txtFirstName_Leave(object sender, EventArgs e)
        {
            MessageBox.Show("Send update query");
        }

        private void txtLastName_Leave(object sender, EventArgs e)
        {
            MessageBox.Show("Send update query");
        }

        private void txtEmail_Leave(object sender, EventArgs e)
        {
            MessageBox.Show("Send update query");
        }

        private void txtPlacement_Leave(object sender, EventArgs e)
        {
            MessageBox.Show("Send update query");
        }

        private void cbReaderSone_Leave(object sender, EventArgs e)
        {
            if(cbReaderSone.SelectedIndex >= 0)
            {
                MessageBox.Show("Send update query");
            }
        }
    }
}