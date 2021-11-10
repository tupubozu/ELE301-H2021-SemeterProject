using System;
using System.Collections.Generic;
using System.Text;

namespace SemesterProject.NetworkCommunication
{
    [Serializable]
    public class NetworkMessage
    {
        public enum MessageType { UpdateAccessTable, Breach, KeypadPress, AuthSuccess, AuthFailure, AuthTimeout, Other }
        public MessageType Type;
        public int UnitNumber;
        public object MessageObject;
    }
}
