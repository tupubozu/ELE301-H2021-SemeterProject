using System;
using System.Collections.Generic;
using System.Text;

namespace SemesterProject.Common.Core
{
    public class UserPermission: IComparable<UserPermission>
    {
        public int UserId { get; set; }
        public int PassCode { get; set; }
        public bool IsAllowed { get; set; } = false;

        public int CompareTo(UserPermission other)
        {
            return this.UserId.CompareTo(other.UserId);
        }
    }
}
