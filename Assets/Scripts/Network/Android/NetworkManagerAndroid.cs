using ARPeerToPeerSample.Utility;
using GoogleARCore.CrossPlatform;
using System;
using UnityEngine;

namespace ARPeerToPeerSample.Network
{
    public class NetworkManagerAndroid : NetworkManagerBase
    {
        private WifiDirectImpl _wifiDirectImpl;
        private string _addr;

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

        public override void CreateAnchor(IntPtr anchorNativePtr)
        {
            // raw anchor data stored at a 4 byte offset: https://forum.unity.com/threads/arfoundation-2-1-azure-cloud-spatial-anchors.679639/
            XPSession.CreateCloudAnchor((IntPtr)(anchorNativePtr.ToInt64() + sizeof(int))).ThenAction((CloudAnchorResult result) =>
            {
                AnchorPostComplete?.Invoke(result.Response.ToString());
            });
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