using System;
using UnityEngine.XR.ARFoundation;

namespace ARPeerToPeerSample.Network
{
    public abstract class NetworkManagerBase : INetworkManager
    {
        // todo: polling system would be a better implementation, but this is fine for now
        public Action<string> ServiceFound;
        public Action<byte[]> MessageReceived;
        public Action ConnectionEstablished;

        public virtual void Connect()
        {
            throw new NotImplementedException();
        }

        public virtual void SendMessage(byte[] message)
        {
            throw new NotImplementedException();
        }

        public virtual void Start()
        {
            throw new NotImplementedException();
        }

        public virtual void SendAnchor(ARPlane plane)
        {
            throw new NotImplementedException();
        }
    }
}
