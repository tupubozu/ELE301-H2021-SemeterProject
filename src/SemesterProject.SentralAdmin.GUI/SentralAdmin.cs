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

        private async void oppfriskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(Database is null))
            {
                try
                {
                    using var cmd = Database.CreateCommand();

                    cmd.CommandText = $"select Brukar_ID, Fornamn, Etternamn, Epost, Kort_ID, Kort_pin, Kort_gyldig_start, Kort_gyldig_stopp from Brukar;";
                    using (var reader = await cmd.ExecuteReaderAsync())
                        if (reader.Read())
                        {
                            listUsers.Items.Clear();
                            listUsers.SelectedIndex = -1;
                            do
                            {
                                listUsers.Items.Add(new User()
                                {
                                    ID = reader.GetInt32(0),
                                    FirstName = reader.GetValue(1) as string,
                                    LastName = reader.GetValue(2) as string,
                                    Email = reader.GetValue(3) as string,
                                    CardID = reader.GetValue(4) as int?,
                                    CardPin = reader.GetValue(5) as int?,
                                    CardValidStart = reader.GetDateTime(6),
                                    CardValidEnd = reader.GetValue(7) as DateTime? ?? DateTime.MaxValue.AddYears(-1).Date,
                                });
                            } while (reader.Read());
                            reader.Close();
                        }

                    cmd.CommandText = $"select Leser_ID, Sone_ID, Plassering from Kortleser;";
                    using (var reader = cmd.ExecuteReader())
                        if (reader.Read())
                        {
                            listReaders.Items.Clear();
                            listReaders.SelectedIndex = -1;
                            do
                            {
                                listReaders.Items.Add(new CardReader()
                                {
                                    ID = reader.GetInt32(0),
                                    SoneID = reader.GetInt32(1),
                                    Placement = reader.GetValue(2) as string,
                                });
                            } while (reader.Read());
                            reader.Close();
                        }

                    cbReaderSone.Items.Clear();
                    foreach (var sone in CardReader.Sones) cbReaderSone.Items.Add(sone);
                    if (cbReaderSone.Items.Count != 0) cbReaderSone.SelectedIndex = 0;

                    cbSoneSelector.Items.Clear();
                    foreach (var sone in CardReader.Sones) cbSoneSelector.Items.Add(sone);
                    if (cbSoneSelector.Items.Count != 0) cbSoneSelector.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                }
            }
            else if (Database is null) MessageBox.Show("Logg inn i databaseserveren", "Viktig");
        }

        private async void btnAddUser_Click(object sender, EventArgs e)
        {
            if (!(Database is null))
            {
                try
                {
                    using var cmd = Database.CreateCommand();
                    cmd.CommandText = $"insert into brukar (brukar_id) values ({Convert.ToInt32(txtUserID.Text)});";
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                }
                
            }
            else if (Database is null) MessageBox.Show("Logg inn i databaseserveren", "Viktig");
        }

        private async void btnRemoveUser_Click(object sender, EventArgs e)
        {
            if (!(Database is null) && !(listUsers.SelectedItem is null))
            {
                try
                {
                    var usr = listUsers!.SelectedItem as User;
                    using var cmd = Database!.CreateCommand();
                    cmd.CommandText = $"delete from brukar where brukar_id = {usr!.ID} cascade;";
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                }
            }
            else if (Database is null) MessageBox.Show("Logg inn i databaseserveren", "Viktig");
        }

        private async void btnAddReader_Click(object sender, EventArgs e)
        {
            if (!(Database is null) && !(cbReaderSone.SelectedItem is null))
            {
                try
                {
                    var obj = cbReaderSone!.SelectedItem as AccessSone;
                    using var cmd = Database!.CreateCommand();
                    cmd.CommandText = $"insert into kortleser (leser_ID, Sone_ID) values ({Convert.ToInt32(txtReaderID.Text)},{obj!.ID});";
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                }
            }
            else if (Database is null) MessageBox.Show("Logg inn i databaseserveren", "Viktig");
        }

        private async void btnRemoveReader_Click(object sender, EventArgs e)
        {
            if (!(Database is null) && !(listReaders.SelectedItem is null))
            {
                var rdr = listReaders!.SelectedItem as CardReader;

                using var cmd = Database!.CreateCommand();
                cmd.CommandText = $"delete from kortleser where leser_id = {rdr!.ID} cascade;";
                await cmd.ExecuteNonQueryAsync();
            }
            else if (Database is null) MessageBox.Show("Logg inn i databaseserveren", "Viktig");
        }

        private async void btnAddSone_Click(object sender, EventArgs e)
        {
            if (!(Database is null) && !(cbSoneSelector.SelectedItem is null) && !(listUsers.SelectedItem is null))
            {
                var obj = cbSoneSelector!.SelectedItem as AccessSone;
                var usr = listUsers!.SelectedItem as User;
                using var cmd = Database!.CreateCommand();
                cmd.CommandText = $"insert into tilgang (sone_id, brukar_id) values ({obj!.ID}, {usr!.ID});";
                await cmd.ExecuteNonQueryAsync();

                listSones.Items.Add(obj);
            }
            else if (Database is null)  MessageBox.Show("Logg inn i databaseserveren", "Viktig");
        }

        private async void btnRemoveSone_Click(object sender, EventArgs e)
        {
            if (!(Database is null) && !(listSones.SelectedItem is null) && !(listUsers.SelectedItem is null))
            {
                try
                {
                    var obj = listSones!.SelectedItem as AccessSone;
                    var usr = listUsers!.SelectedItem as User;
                    using var cmd = Database!.CreateCommand();
                    cmd.CommandText = $"delete from tilgang where tilgang.sone_id = {obj!.ID} and tilgang.brukar_id = {usr!.ID};";
                    await cmd.ExecuteNonQueryAsync();

                    listSones.Items.Remove(obj);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                }
               
            }
            else if (Database is null) MessageBox.Show("Logg inn i databaseserveren", "Viktig");
        }

        private async void listUsers_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (CardReader.Sones.Count != 0)
                {
                    cbSoneSelector.Items.Clear();
                    cbSoneSelector.Items.AddRange(CardReader.Sones.ToArray());
                }
                if (listUsers.SelectedItem != null)
                {
                    User user = (User)listUsers.SelectedItem;
                    txtUserID.Text = user.ID.ToString();
                    txtFirstName.Text = user.FirstName;
                    txtLastName.Text = user.LastName;
                    txtEmail.Text = user.Email;
                    txtCardID.Text = user.CardID?.ToString();
                    txtCardPin.Text = user.CardPin?.ToString();
                    StartTimePicker.Value = user.CardValidStart;
                    StopTimePicker.Value = user.CardValidEnd ?? DateTime.Now;

                    listSones.Items.Clear();

                    if (!(Database is null))
                    {
                        List<AccessSone> sones = new List<AccessSone>();

                        using var cmd = Database!.CreateCommand();
                        cmd.CommandText = $"select Tilgangssone.Sone_ID, Tilgangssone.SoneNamn from Tilgangssone inner join Tilgang on Tilgangssone.Sone_ID = Tilgang.Sone_ID where Tilgang.Brukar_ID = {user.ID};";

                        var rd_tsk = cmd.ExecuteReaderAsync();
                        using (var reader = await rd_tsk)
                            if (reader.Read())
                            {
                                do
                                {
                                    sones.Add(new AccessSone()
                                    {
                                        ID = reader.GetInt32(0),
                                        Name = reader.GetString(1)
                                    });
                                } while (reader.Read());
                            }
                        
                        listSones.Items.AddRange(sones.ToArray());
                    }
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
                    cbReaderSone.Items.Clear();
                    cbReaderSone.Items.AddRange(CardReader.Sones.ToArray());
                }
                if (listReaders.SelectedItem != null)
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

        private async void txtFirstName_Leave(object sender, EventArgs e)
        {
            if (!(Database is null) && !(listUsers.SelectedItem is null))
            {
                var usr = listUsers!.SelectedItem as User;
                if (usr!.FirstName != txtFirstName.Text.Trim())
                {
                    using var cmd = Database.CreateCommand();
                    cmd.CommandText = $"update brukar set fornamn = \'{txtFirstName.Text.Trim()}\' where brukar_id = {usr!.ID}";
                    await cmd.ExecuteNonQueryAsync();
                    usr!.FirstName = txtFirstName.Text.Trim();
                }
            }
            else if (Database is null) MessageBox.Show("Logg inn i databaseserveren", "Viktig");
        }

        private async void txtLastName_Leave(object sender, EventArgs e)
        {
            if (!(Database is null) && !(listUsers.SelectedItem is null))
            {
                var usr = listUsers!.SelectedItem as User;
                if (usr!.LastName != txtLastName.Text.Trim())
                {
                    using var cmd = Database.CreateCommand();
                    cmd.CommandText = $"update brukar set etternamn = \'{txtLastName.Text.Trim()}\' where brukar_id = {usr!.ID}";
                    await cmd.ExecuteNonQueryAsync();
                    usr!.LastName = txtLastName.Text.Trim();
                }
            }
            else if (Database is null) MessageBox.Show("Logg inn i databaseserveren", "Viktig");
        }

        private async void txtEmail_Leave(object sender, EventArgs e)
        {
            if (!(Database is null) && !(listUsers.SelectedItem is null))
            {
                try
                {
                    var usr = listUsers!.SelectedItem as User;
                    if (usr!.Email != txtEmail.Text.Trim())
                    {
                        using var cmd = Database.CreateCommand();
                        cmd.CommandText = $"update brukar set epost = \'{txtEmail.Text.Trim()}\' where brukar_id = {usr!.ID}";
                        await cmd.ExecuteNonQueryAsync();
                        usr!.Email = txtEmail.Text.Trim();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,ex.GetType().Name);
                }
            }
            else if (Database is null) MessageBox.Show("Logg inn i databaseserveren", "Viktig");
        }


        private async void txtCardID_Leave(object sender, EventArgs e)
        {
            if (!(Database is null) && !(listUsers.SelectedItem is null))
            {
                try
                {
                    var usr = listUsers!.SelectedItem as User;
                    int val = int.Parse(txtCardID.Text);
                    if (usr!.CardID != val)
                    {
                        using var cmd = Database.CreateCommand();
                        cmd.CommandText = $"update brukar set kort_id = {val} where brukar_id = {usr!.ID}";
                        await cmd.ExecuteNonQueryAsync();
                        usr!.CardID = val;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                }
            }
            else if (Database is null) MessageBox.Show("Logg inn i databaseserveren", "Viktig");
        }

        private async void txtCardPin_Leave(object sender, EventArgs e)
        {
            if (!(Database is null) && !(listUsers.SelectedItem is null))
            {
                try
                {
                    var usr = listUsers!.SelectedItem as User;
                    int val = int.Parse(txtCardPin.Text);
                    if (usr!.CardPin != val)
                    {
                        using var cmd = Database.CreateCommand();
                        cmd.CommandText = $"update brukar set kort_pin = {val} where brukar_id = {usr!.ID}";
                        await cmd.ExecuteNonQueryAsync();
                        usr!.CardPin = val;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                }
            }
            else if (Database is null) MessageBox.Show("Logg inn i databaseserveren", "Viktig");
        }

        private async void StartTimePicker_Leave(object sender, EventArgs e)
        {
            if (!(Database is null) && !(listUsers.SelectedItem is null))
            {
                try
                {
                    var usr = listUsers!.SelectedItem as User;
                    if (usr!.CardValidStart != StartTimePicker.Value)
                    {
                        using var cmd = Database.CreateCommand();
                        cmd.CommandText = $"update brukar set Kort_gyldig_start = timestamp \'{StartTimePicker.Value.ToString("O")}\' where brukar_id = {usr!.ID}";
                        await cmd.ExecuteNonQueryAsync();
                        usr!.CardValidStart = StartTimePicker.Value;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                }
            }
            else if (Database is null) MessageBox.Show("Logg inn i databaseserveren", "Viktig");
        }

        private async void StopTimePicker_Leave(object sender, EventArgs e)
        {
            if (!(Database is null) && !(listUsers.SelectedItem is null))
            {
                try
                {
                    var usr = listUsers!.SelectedItem as User;
                    if (usr!.CardValidStart != StopTimePicker.Value)
                    {
                        using var cmd = Database.CreateCommand();
                        cmd.CommandText = $"update brukar set Kort_gyldig_stopp = timestamp \'{StopTimePicker.Value.ToString("O")}\' where brukar_id = {usr!.ID}";
                        await cmd.ExecuteNonQueryAsync();
                        usr!.CardValidStart = StopTimePicker.Value;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                }
            }
            else if (Database is null) MessageBox.Show("Logg inn i databaseserveren", "Viktig");
        }

        private async void txtPlacement_Leave(object sender, EventArgs e)
        {
            if (!(Database is null) && !(listReaders.SelectedItem is null))
            {
                try
                {
                    var rdr = listReaders!.SelectedItem as CardReader;
                    if (rdr!.Placement != txtPlacement.Text.Trim())
                    {
                        using var cmd = Database.CreateCommand();
                        cmd.CommandText = $"update kortleser set plassering = \'{txtPlacement.Text.Trim()}\' where leser_id = {rdr!.ID}";
                        await cmd.ExecuteNonQueryAsync();
                        rdr!.Placement = txtPlacement.Text.Trim();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                }
            }
            else if (Database is null) MessageBox.Show("Logg inn i databaseserveren", "Viktig");
        }

        private async void cbReaderSone_Leave(object sender, EventArgs e)
        {
            if (!(Database is null) && !(listReaders.SelectedItem is null) && !(cbReaderSone.SelectedItem is null))
            {
                try
                {
                    var rdr = listReaders!.SelectedItem as CardReader;
                    var rsn = cbReaderSone!.SelectedItem as AccessSone;
                    if (rdr!.SoneID != rsn!.ID)
                    {
                        using var cmd = Database.CreateCommand();
                        cmd.CommandText = $"update korleser set sone_id = {rsn!.ID} where leser_id = {rdr!.ID}";
                        await cmd.ExecuteNonQueryAsync();
                        rdr!.SoneID = rsn!.ID;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                }
            }
            else if (Database is null) MessageBox.Show("Logg inn i databaseserveren", "Viktig");
        }
    }
}