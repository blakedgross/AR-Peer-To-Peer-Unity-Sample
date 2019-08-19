using UnityEngine;
using UnityMultipeerConnectivity;
using UnityEngine.XR.ARFoundation;
using System;
using System.Threading.Tasks;
using UnityEngine.XR.ARKit;
using System.Threading;
using UnityEngine.XR.ARSubsystems;

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
            // todo: all byte to "stream" conversion should be made more effecient through less copying
            byte[] planeTrackableId1 = BitConverter.GetBytes(plane.trackableId.subId1);
            byte[] planeTrackableId2 = BitConverter.GetBytes(plane.trackableId.subId2);
            byte[] messagePacket = new byte[planeTrackableId1.Length + planeTrackableId2.Length];
            planeTrackableId1.CopyTo(messagePacket, 0);
            planeTrackableId2.CopyTo(messagePacket, planeTrackableId1.Length);
            NetworkMessageStruct networkMessageStruct = new NetworkMessageStruct
            {
                Type = MessageType.Anchor,
                Message = messagePacket
            };
            SendMessage(networkMessageStruct);
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
                    Debug.Log("recieved world map");
                    ARWorldMap arWorldMap = (ARWorldMap)new PackableARWorldMap(networkMessage.Message);
                    RestartSessionWithWorldMap(arWorldMap);
                    break;
                case MessageType.Anchor:
                    Debug.Log("recieved anchor");
                    ulong trackableId1 = BitConverter.ToUInt64(networkMessage.Message, 0);
                    ulong trackableId2 = BitConverter.ToUInt64(networkMessage.Message, sizeof(ulong));
                    TrackableId trackableId = new TrackableId(trackableId1, trackableId2);
                    AnchorRecieved?.Invoke(trackableId);
                    break;
                case MessageType.SpawnedObject:
                    DeserializeObjectSpawnAndSendEvent(networkMessage.Message);
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