using System;

namespace SemesterProject.Common.Core
{
	public class UserPermission : IComparable<UserPermission>
	{
		public int UserId { get; set; }
		public int CardId { get; set; }
		public int CardCode { get; set; }
		public bool IsAllowed { get; set; } = false;

		//public UserPermission(int id, int code, bool allowed)
		//{
		//    UserId = id;
		//    PassCode = code;
		//    IsAllowed = allowed;
		//}

		public int CompareTo(UserPermission other)
		{
			return this.UserId.CompareTo(other.UserId);
		}
	}
}
