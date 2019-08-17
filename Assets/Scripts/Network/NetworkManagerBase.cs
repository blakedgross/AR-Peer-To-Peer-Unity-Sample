using System;

namespace ARPeerToPeerSample.Network
{
    public abstract class NetworkManagerBase : INetworkManager
    {
        // todo: polling system would be a better implementation, but this is fine for now
        public Action<string> ServiceFound;
        public Action<string> MessageReceived;
        public Action ConnectionEstablished;
        public Action<string> AnchorPostComplete;

        public virtual void Connect()
        {
            throw new NotImplementedException();
        }

        public virtual void CreateAnchor(IntPtr anchorNativePtr)
        {
            throw new NotImplementedException();
        }

        public virtual void SendMessage(string message)
        {
            throw new NotImplementedException();
        }

        public virtual void Start()
        {
            throw new NotImplementedException();
        }
    }
}
