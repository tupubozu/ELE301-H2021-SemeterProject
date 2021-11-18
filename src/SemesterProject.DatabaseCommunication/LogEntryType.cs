using System;
using System.Collections.Generic;
using System.Text;

namespace SemesterProject.DatabaseCommunication
{
    public class LogEntryType
    {
        public static List<LogEntryLevel> Levels = new List<LogEntryLevel>();

        public int ID;
        public int LevelID;
        public string Name;

        public override string ToString()
        {
            return Name;
        }
    }
}
