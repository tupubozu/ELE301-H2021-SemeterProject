using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SemesterProject.DatabaseCommunication;
using Npgsql;

namespace SemesterProject.Monitor.GUI
{
    public partial class LogInspector : Form
    {
        public enum QueryType { AlarmLevel, UserAuthFail, Generic, DoorAccess}
        IBased MainForm;

        QueryType query;

        List<User> users;
        List<CardReader> cardReaders;
        public LogInspector(IBased parent, string selector, string title, QueryType query) // Warning: Here be (SQL) dragons
        {
            InitializeComponent();

            users = new List<User>();
            cardReaders = new List<CardReader>();

            MainForm = parent;
            label1.Text = selector;
            this.Text = title;
            this.query = query;

            if (!(MainForm.Database is null))
            {
                try
                {
                    using var cmd = MainForm.Database.CreateCommand();

                    cmd.CommandText = $"select Brukar_ID, Fornamn, Etternamn, Epost, Kort_ID, Kort_pin, Kort_gyldig_start, Kort_gyldig_stopp from Brukar;";

                    using (var reader = cmd.ExecuteReader())
                        if (reader.Read())
                        {
                            do
                            {
                                users.Add(new User()
                                {
                                    ID = reader.GetInt32(0),
                                    FirstName = reader.GetValue(1) as string,
                                    LastName = reader.GetValue(2) as string,
                                    Email = reader.GetValue(3) as string,
                                    CardID = reader.GetValue(4) as int?,
                                    CardPin = reader.GetValue(5) as int?,
                                    CardValidStart = reader.GetDateTime(6),
                                    CardValidEnd = reader.GetValue(7) as DateTime?,
                                });
                            } while (reader.Read());
                            reader.Close();
                        }

                    cmd.CommandText = $"select Leser_ID, Sone_ID, Plassering from Kortleser;";
                    using (var reader = cmd.ExecuteReader())
                        if (reader.Read())
                        {
                            do
                            {
                                cardReaders.Add(new CardReader()
                                {
                                    ID = reader.GetInt32(0),
                                    SoneID = reader.GetInt32(1),
                                    Placement = reader.GetValue(2) as string,
                                });
                            } while (reader.Read());
                            reader.Close();
                        }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }


            switch (query)
            {
                case QueryType.AlarmLevel:
                    comboBox1.Items.AddRange(LogEntryType.Levels.ToArray());
                    break;
                case QueryType.UserAuthFail:
                    comboBox1.Items.AddRange(cardReaders.ToArray());
                    break;
                case QueryType.Generic:
                    comboBox1.Items.AddRange(users.ToArray());
                    break;
                case QueryType.DoorAccess:
                default:
                    comboBox1.Items.Clear();
                    break;
            }
        }

        private async void button1_Click(object sender, EventArgs e) // Warning: Here be (SQL) elder dragons
        {
            listBox1.Items.Clear();

            if (!(MainForm.Database is null))
            {
                try
                {
                    using var cmd = MainForm.Database.CreateCommand();

                    switch (query)
                    {
                        case QueryType.AlarmLevel:
                            cmd.CommandText = $"select Logg.Logg_ID, Logg.LoggType_ID, Logg.Leser_ID, Logg.LeserTid, Logg.MeldingTid, Logg.SentralTid, Logg.Brukar_ID, Logg.LoggMelding from Logg inner join LoggType on Logg.LoggType_ID = LoggType.LoggType_ID where timestamp \'{startTimePicker.Value.ToString("O")}\' <= logg.sentraltid  and timestamp \'{stopTimePicker.Value.ToString("O")}\' >= logg.sentraltid{(comboBox1.SelectedItem is null ? string.Empty : $" and loggtype.alarmnivaa_id = {(comboBox1.SelectedItem as LogEntryLevel)?.ID}")};";
                            await GetLogg(cmd);
                            break;
                        case QueryType.UserAuthFail:
                            cmd.CommandText = $"select Brukar_ID, Fornamn, Etternamn, Epost, Kort_ID, Kort_pin, Kort_gyldig_start, Kort_gyldig_stopp from Brukar inner join Logg on Brukar.Brukar_ID = Logg.Brukar_ID where timestamp \'{ startTimePicker.Value.ToString("O")} <= Logg.SentralTid and timestamp \'{ stopTimePicker.Value.ToString("O")}\' >= Logg.SentralTid having count(Brukar.Brukar_ID) >= 10;";
                            break;
                        case QueryType.Generic:
                            cmd.CommandText = $"select Logg.Logg_ID, Logg.LoggType_ID, Logg.Leser_ID, Logg.LeserTid, Logg.MeldingTid, Logg.SentralTid, Logg.Brukar_ID, Logg.LoggMelding from logg where timestamp \'{startTimePicker.Value.ToString("O")}\' <= logg.sentraltid  and timestamp \'{stopTimePicker.Value.ToString("O")}\' >= logg.sentraltid;";
                            await GetLogg(cmd);
                            break;
                        case QueryType.DoorAccess:
                            cmd.CommandText = $"select Logg.Logg_ID, Logg.LoggType_ID, Logg.Leser_ID, Logg.LeserTid, Logg.MeldingTid, Logg.SentralTid, Logg.Brukar_ID, Logg.LoggMelding from logg where timestamp \'{startTimePicker.Value.ToString("O")}\' <= logg.sentraltid  and timestamp \'{stopTimePicker.Value.ToString("O")}\' >= logg.sentraltid{(comboBox1.SelectedItem is null ? string.Empty : $" and logg.leser_id = {(comboBox1.SelectedItem as CardReader)?.ID}")};";
                            break;
                        default:
                            cmd.CommandText = $"select Logg.Logg_ID, Logg.LoggType_ID, Logg.Leser_ID, Logg.LeserTid, Logg.MeldingTid, Logg.SentralTid, Logg.Brukar_ID, Logg.LoggMelding from logg where timestamp \'{startTimePicker.Value.ToString("O")}\' <= logg.sentraltid  and timestamp \'{stopTimePicker.Value.ToString("O")}\' >= logg.sentraltid;";
                            await GetLogg(cmd);
                            break;
                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private async Task GetLogg(NpgsqlCommand cmd)
        {
            var rd_tsk = cmd.ExecuteReaderAsync();
            using (var reader = await rd_tsk)
                if (reader.Read())
                {
                    do
                    {
                        listBox1.Items.Add(new LogEntry()
                        {
                            ID = reader.GetInt32(0),
                            TypeID = reader.GetInt16(1),
                            ReaderID = reader.GetInt32(2),
                            ReaderTime = reader.GetDateTime(3),
                            MessageTime = reader.GetDateTime(4),
                            SentralTime = reader.GetDateTime(5),
                            User_ID = reader.GetValue(6) as int?,
                            LogMessage = reader.GetString(7),
                        });
                    } while (reader.Read());
                    reader.Close();
                }
            
        }
    }
}
