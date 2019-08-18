using System;
using UnityEngine.XR.ARFoundation;

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
        void SendAnchor(ARPlane plane);
    }
}