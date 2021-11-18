using System;
using System.Collections.Generic;
using System.Text;

namespace SemesterProject.DatabaseCommunication
{
    public class CardReader : IComparable<CardReader>
    {
        public static List<AccessSone> Sones = new List<AccessSone>();

        public int ID;
        public int SoneID;
        public string Placement;

        public int CompareTo(CardReader other)
        {
            return this.ID.CompareTo(other.ID);
        }

        public override string ToString()
        {
            return $"Node {ID}";
        }
    }
}
