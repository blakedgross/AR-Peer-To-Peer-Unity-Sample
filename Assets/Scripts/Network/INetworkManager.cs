using System;

namespace ARPeerToPeerSample.Network
{
    /// <summary>
    /// Interface for different platform implementation of network manager
    /// </summary>
    public interface INetworkManager
    {
        void Start();
        void Connect();
        void SendMessage(byte[] message);
    }
}