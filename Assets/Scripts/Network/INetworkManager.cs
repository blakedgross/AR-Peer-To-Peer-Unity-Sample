using System;
using UnityEngine;
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
        void SendColorMessage(Color colorToSend);
        void SendAnchor(ARPlane plane);
    }
}