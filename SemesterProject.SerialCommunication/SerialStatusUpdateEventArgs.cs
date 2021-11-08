using System;
using SemesterProject.Common.Core;

namespace SemesterProject.SerialCommunication
{
	public class SerialStatusUpdateEventArgs : EventArgs
	{
		public SerialStatusData statusData;

		public static new SerialStatusUpdateEventArgs Empty
		{
			get => EventArgs.Empty as SerialStatusUpdateEventArgs;
		}
	}
}
