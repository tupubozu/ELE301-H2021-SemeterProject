using SemesterProject.Common.Core;
using System;

namespace SemesterProject.SerialCommunication
{
	public class AuthControllerEventArgs : EventArgs
	{
		public SerialStatusData StatusData;
		public UserPermission Permission;

		public static new AuthControllerEventArgs Empty
		{
			get => EventArgs.Empty as AuthControllerEventArgs;
		}
	}
}
