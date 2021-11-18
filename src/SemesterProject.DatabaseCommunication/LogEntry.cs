using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemesterProject.DatabaseCommunication
{
	public class LogEntry
	{
		public static List<LogEntryType> EntryTypes = new List<LogEntryType>();


		public long ID;
		public short TypeID;
		public int ReaderID;
		public DateTime ReaderTime;
		public DateTime MessageTime;
		public DateTime SentralTime;
		public int? User_ID;
		public string LogMessage;

        public override string ToString()
        {
			return $"{SentralTime}: {EntryTypes.Find( (LogEntryType element) => element.ID == TypeID).Name}: Node {ReaderID}{(User_ID is null ? string.Empty : $": User {User_ID}")}{(string.IsNullOrEmpty(LogMessage)? string.Empty: $", Message: \"{LogMessage}\"")}";
        }
    }
}
