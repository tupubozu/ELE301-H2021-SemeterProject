using System;

namespace SemesterProject.NetworkCommunication
{
    public class NetworkMessageUpdateEventArgs : EventArgs
    {
        public NetworkMessage MessageData;

        public static new NetworkMessageUpdateEventArgs Empty => EventArgs.Empty as NetworkMessageUpdateEventArgs;

    }
}