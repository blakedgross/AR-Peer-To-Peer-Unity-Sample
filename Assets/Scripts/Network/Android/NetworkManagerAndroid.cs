using UnityEngine;

namespace ARPeerToPeerSample.Network
{
    public class NetworkManagerAndroid : INetworkManager
    {
        private WifiDirectImpl _wifiDirectImpl;
        public NetworkManagerAndroid(WifiDirectImpl wifiDirectImpl)
        {
            _wifiDirectImpl = wifiDirectImpl;
        }

        public void Start()
        {
            Debug.Log("Starting network manager android");
            _wifiDirectImpl.StartWifiDirectConnection();
        }
    }
}