using System;
using System.Collections.Generic;
using System.Text;

namespace SemesterProject.DatabaseCommunication
{
    public class User: IComparable<User>
    {
        public int ID;
        public string FirstName;
        public string LastName;
        public string Email;
        public int? CardID;
        public int? CardPin;
        public DateTime CardValidStart;
        public DateTime? CardValidEnd;

        public int CompareTo(User other)
        {
            return this.ID.CompareTo(other.ID);
        }

        public override string ToString()
        {
            return $"{ID}: {FirstName} {LastName}";
        }
    }
}
