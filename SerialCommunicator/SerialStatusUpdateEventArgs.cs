﻿using System;
using ELE301.SemesterProject.Common.Core;

namespace ELE301.SemesterProject.SerialCommunication
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
