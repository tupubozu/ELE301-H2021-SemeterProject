using System;
using System.Collections.Generic;
using System.Text;

namespace SemesterProject.DatabaseCommunication
{
    public class User
    {
        public int ID;
        public string FirstName;
        public string LastName;
        public string Email;
        public int? CardID;
        public int? CardPin;
        public DateTime CardValidStart;
        public DateTime? CardValidEnd;

        public override string ToString()
        {
            return $"{ID}: {FirstName} {LastName}";
        }
    }
}
