using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPeerToPeerSample.Network
{
    public class WifiDirectImpl : WifiDirectBase
    {
        public Action<string> ServiceFound;
        public Action<string> MessageReceived;
        public Action ConnectionEstablished;

        public void StartWifiDirectConnection()
        {
            base.initialize(this.gameObject.name);
        }

        //when the WifiDirect services is connected to the phone, begin broadcasting and discovering services
        public override void onServiceConnected()
        {
            print("service connected");
            Dictionary<string, string> record = new Dictionary<string, string>();
            record.Add("demo", "unity");
            base.broadcastService("hello", record);
            base.discoverServices();
        }

        public override void onServiceFound(string addr)
        {
            print("service found: " + addr);
            ServiceFound?.Invoke(addr);
        }

        public override void onMessage(string message)
        {
            print("message received: " + message);
            MessageReceived?.Invoke(message);
        }

        public override void onConnect()
        {
            print("connected to other device");
            ConnectionEstablished?.Invoke();
        }
    }
}