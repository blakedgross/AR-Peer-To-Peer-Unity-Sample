using UnityEngine;
using UnityMultipeerConnectivity;

namespace ARPeerToPeerSample.Network
{
    public class NetworkManageriOS : NetworkManagerBase
    {
        public NetworkManageriOS()
        {
            // todo: clean these up
            UnityMCSessionNativeInterface.GetMcSessionNativeInterface().DataReceivedEvent += OnDataReceivedEvent;
            UnityMCSessionNativeInterface.GetMcSessionNativeInterface().StateChangedEvent += OnStateChangedEvent;;
        }

        public override void Connect()
        {
            Debug.Log("Connection is out of our control");
        }

        public override void SendMessage(byte[] message)
        {
            UnityMCSessionNativeInterface.GetMcSessionNativeInterface().SendToAllPeers(message);
        }

        public override void Start()
        {
            Debug.Log("Start is out of our control");
        }

        private void OnDataReceivedEvent(byte[] obj)
        {
            Debug.Log("recieved message");
            MessageReceived?.Invoke(obj);
        }

        private void OnStateChangedEvent(UnityMCPeerID arg1, UnityMCSessionState arg2)
        {
            // todo: handle multi-user
            Debug.Log("State: " + arg2.ToString() + " with user: " + arg1.DisplayName);
            switch (arg2)
            {
                case UnityMCSessionState.Connecting:
                    ServiceFound?.Invoke(arg1.DisplayName);
                    break;
                case UnityMCSessionState.Connected:
                    ConnectionEstablished?.Invoke();
                    break;
                default:
                    Debug.Log("Unexpected state: " + arg2);
                    break;
            }
        }

    }
}