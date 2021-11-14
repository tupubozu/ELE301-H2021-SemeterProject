using System;

namespace SemesterProject.NetworkCommunication
{
	[Serializable]
	public class NetworkMessage
	{
		public enum MessageType { UpdateAccessTable, UpdateUnitTime, RequestAccessTable, Breach, KeypadPress, AuthSuccess, AuthFailure, AuthTimeout, Closed, Other }
		public MessageType Type;
		public DateTime MessageTimestamp;
		public int NodeNumber;
		public DateTime UnitTimestamp;
		public object MessageObject;
	}
}
