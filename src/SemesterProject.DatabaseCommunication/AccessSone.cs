using System;
using System.Collections.Generic;
using System.Text;

namespace SemesterProject.DatabaseCommunication
{
    public class AccessSone : IComparable<AccessSone>
    {
        public int ID;
        public string Name;

        public int CompareTo(AccessSone other)
        {
            return this.ID.CompareTo(other.ID);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
