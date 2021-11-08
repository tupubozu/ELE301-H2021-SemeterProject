using System;

namespace SemesterProject.Common.Core
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
