using System;
using System.Collections.Generic;
using System.Text;

namespace SemesterProject.NetworkCommunication
{
	[Serializable]
	public class NetworkMessage
	{
		public enum MessageType { UpdateAccessTable, UpdateUnitTime, RequestAccessTable, Breach, KeypadPress, AuthSuccess, AuthFailure, AuthTimeout, Other }
		public MessageType Type;
		public DateTime MessageTimestamp;
		public int NodeNumber;
		public DateTime UnitTimestamp;
		public object MessageObject;
	}
}
