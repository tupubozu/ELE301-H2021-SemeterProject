using System;
using System.Collections.Generic;
using System.Text;

namespace SemesterProject.DatabaseCommunication
{
    public class CardReader
    {
        public static List<AccessSone> Sones = new List<AccessSone>();

        public int ID;
        public int SoneID;
        public string Placement;

        public override string ToString()
        {
            return $"Node {ID}";
        }
    }
}
