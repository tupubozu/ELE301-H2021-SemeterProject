﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SemesterProject.DatabaseCommunication
{
    public class LogEntryLevel
    {
        public int ID;
        public string Name;

        public override string ToString()
        {
            return Name;
        }
    }
}
