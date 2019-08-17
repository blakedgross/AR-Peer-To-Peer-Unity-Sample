using ARPeerToPeerSample.Utility;
using UnityEngine;

namespace ARPeerToPeerSample.Network
{
    public class NetworkManagerAndroid : NetworkManagerBase
    {
        private WifiDirectImpl _wifiDirectImpl;
        private string _addr;
        //private XPSession

        public NetworkManagerAndroid(WifiDirectImpl wifiDirectImpl)
        {
            DependencyRegistry.AddDependency<INetworkManager>(this);
            _wifiDirectImpl = wifiDirectImpl;
            _wifiDirectImpl.ServiceFound += OnServiceFound;
            _wifiDirectImpl.ConnectionEstablished += OnConnectionEstablished;
            _wifiDirectImpl.MessageReceived += OnMessageReceived;
        }

        public override void Start()
        {
            Debug.Log("Starting network manager android");
            _wifiDirectImpl.StartWifiDirectConnection();
        }

        public override void Connect()
        {
            _wifiDirectImpl.connectToService(_addr);
        }

        public override void SendMessage(string message)
        {
            _wifiDirectImpl.sendMessage(message);
        }

        private void OnServiceFound(string addr)
        {
            _addr = addr;
            ServiceFound?.Invoke(addr);
        }

        private void OnConnectionEstablished()
        {
            ConnectionEstablished?.Invoke();
        }

        private void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(message);
        }
    }
}