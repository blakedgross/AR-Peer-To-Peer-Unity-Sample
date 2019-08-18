﻿using UnityEngine;
using UnityMultipeerConnectivity;
using UnityEngine.XR.ARFoundation;
using System;
using System.Threading.Tasks;
using UnityEngine.XR.ARKit;
using System.Threading;

namespace ARPeerToPeerSample.Network
{
    public class NetworkManageriOS : NetworkManagerBase
    {
        private ARSession _arSession;
        public NetworkManageriOS(ARSession arSession)
        {
            // todo: clean these up
            UnityMCSessionNativeInterface.GetMcSessionNativeInterface().DataReceivedEvent += OnDataReceivedEvent;
            UnityMCSessionNativeInterface.GetMcSessionNativeInterface().StateChangedEvent += OnStateChangedEvent;

            _arSession = arSession;
        }

        public override void Connect()
        {
            Debug.Log("Connection is out of our control");
        }

        protected override void SendMessage(NetworkMessageStruct message)
        {
            Debug.Log("Send message to all friends");
            UnityMCSessionNativeInterface.GetMcSessionNativeInterface().SendToAllPeers(message.Serialize());
        }

        public override void Start()
        {
            Debug.Log("Start is out of our control");
        }

        public void SendWorldMap()
        {
            SendWorldMapAsync();
        }

        public override void SendAnchor(ARPlane plane)
        {
            SendAnchorAsync(plane);
        }

        private async void SendAnchorAsync(ARPlane plane)
        {
            // send anchor info by serializing plane and sending it over
        }

        private async void SendWorldMapAsync()
        {
            ARWorldMap worldMap = await GetARWorldMapAsync();
            PackableARWorldMap packableARWorldMap = (PackableARWorldMap)worldMap;
            NetworkMessageStruct networkMessageStruct = new NetworkMessageStruct
            {
                Type = MessageType.WorldMap,
                Message = packableARWorldMap.ARWorldMapData
            };
            SendMessage(networkMessageStruct);
        }

        private void OnDataReceivedEvent(byte[] obj)
        {
            NetworkMessageStruct networkMessage = NetworkMessageStruct.Deserialize(obj);
            switch (networkMessage.Type)
            {
                case MessageType.ColorChange:
                    DeserializeColorAndSendEvent(networkMessage.Message);
                    break;
                case MessageType.WorldMap:
                    ARWorldMap arWorldMap = (ARWorldMap)new PackableARWorldMap(networkMessage.Message);
                    RestartSessionWithWorldMap(arWorldMap);
                    break;
            }
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

        private async Task<ARWorldMap> GetARWorldMapAsync()
        {
            if (!(_arSession.subsystem is ARKitSessionSubsystem arKitSessionSubsystem))
            {
                throw new Exception("No session subsystem available. Could not load.");
            }
            return await arKitSessionSubsystem.GetARWorldMapTask();
        }

        private void RestartSessionWithWorldMap(ARWorldMap arWorldMap)
        {
            if (_arSession.subsystem is ARKitSessionSubsystem arKitSessionSubsystem)
            {
                arKitSessionSubsystem.ApplyWorldMap(arWorldMap);
            }
        }
    }

    public static class ARKitSessionSubsystemExtensions
    {
        public static async Task<ARWorldMap> GetARWorldMapTask(this ARKitSessionSubsystem session, CancellationToken cancellationToken = default)
        {
            using (var request = session.GetARWorldMapAsync())
            {
                while (!request.status.IsDone())
                {
                    await Task.Yield();
                    cancellationToken.ThrowIfCancellationRequested();
                }

                if (request.status.IsError())
                {
                    throw new Exception($"Session getting AR world map failed with status {request.status}");
                }

                return request.GetWorldMap();
            }
        }
    }
}