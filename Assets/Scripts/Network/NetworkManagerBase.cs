using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARPeerToPeerSample.Network
{
    public abstract class NetworkManagerBase : INetworkManager
    {
        // todo: polling system would be a better implementation, but this is fine for now
        public Action<string> ServiceFound;
        public Action<Color> ColorChangeMessageRecieved;
        public Action ConnectionEstablished;
        public Action<TrackableId> AnchorRecieved;
        public Action<Pose> ObjectSpawned;

        public virtual void Connect()
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

        /// <summary>
        /// Test method for color serialization
        /// </summary>
        /// <returns></returns>
        public bool TestColorSerialization()
        {
            Color originalColor = new Color(1, .5f, 0);
            byte[] byteColor = SerializeColor(originalColor);
            Color deserializedColor = DeserializeColor(byteColor);
            return VerifyColor(originalColor, deserializedColor);
        }

        public bool TestNetworkPacketSerialization()
        {
            Color sourceColor = new Color(.1f, .2f, .3f);
            NetworkMessageStruct networkMessageStruct = new NetworkMessageStruct
            {
                Type = MessageType.ColorChange,
                Message = SerializeColor(sourceColor)
            };

            byte[] serializeMessage = networkMessageStruct.Serialize();
            NetworkMessageStruct deserializedStruct = NetworkMessageStruct.Deserialize(serializeMessage);
            return networkMessageStruct.Type == deserializedStruct.Type && VerifyColor(sourceColor, DeserializeColor(deserializedStruct.Message));
        }

        public void SendColorMessage(Color colorToSend)
        {
            byte[] colorData = SerializeColor(colorToSend);
            SendMessage(new NetworkMessageStruct
            {
                Type = MessageType.ColorChange,
                Message = colorData
            });
        }

        public void SendModelSpawn(Vector3 localPosition, Quaternion rotation)
        {
            SendMessage(new NetworkMessageStruct
            {
                Type = MessageType.SpawnedObject,
                Message = SerializeSpawnedObjectMessage(localPosition, rotation)
            });
        }

        protected static bool VerifyColor(Color originalColor, Color deserializedColor)
        {
            return Math.Abs(originalColor.r - deserializedColor.r) < Mathf.Epsilon && Math.Abs(originalColor.g - deserializedColor.g) < Mathf.Epsilon && Math.Abs(originalColor.b - deserializedColor.b) < Mathf.Epsilon;
        }

        protected byte[] SerializeSpawnedObjectMessage(Vector3 localPosition, Quaternion rotation)
        {
            byte[] message = new byte[sizeof(float) * 7];
            byte[] posX = BitConverter.GetBytes(localPosition.x);
            byte[] posY = BitConverter.GetBytes(localPosition.y);
            byte[] posZ = BitConverter.GetBytes(localPosition.z);
            byte[] rotX = BitConverter.GetBytes(rotation.x);
            byte[] rotY = BitConverter.GetBytes(rotation.y);
            byte[] rotZ = BitConverter.GetBytes(rotation.z);
            byte[] rotW = BitConverter.GetBytes(rotation.w);
            posX.CopyTo(message, 0);
            posY.CopyTo(message, sizeof(float));
            posZ.CopyTo(message, sizeof(float) * 2);
            rotX.CopyTo(message, sizeof(float) * 3);
            rotY.CopyTo(message, sizeof(float) * 4);
            rotZ.CopyTo(message, sizeof(float) * 5);
            rotW.CopyTo(message, sizeof(float) * 6);

            return message;
        }

        protected Pose DeserializeObjectSpawn(byte[] objectSpawn)
        {
            float posX = BitConverter.ToSingle(objectSpawn, 0);
            float posY = BitConverter.ToSingle(objectSpawn, sizeof(float));
            float posZ = BitConverter.ToSingle(objectSpawn, sizeof(float) * 2);
            float rotX = BitConverter.ToSingle(objectSpawn, sizeof(float) * 3);
            float rotY = BitConverter.ToSingle(objectSpawn, sizeof(float) * 4);
            float rotZ = BitConverter.ToSingle(objectSpawn, sizeof(float) * 5);
            float rotW = BitConverter.ToSingle(objectSpawn, sizeof(float) * 6);

            return new Pose(new Vector3(posX, posY, posZ), new Quaternion(rotX, rotY, rotZ, rotW));
        }

        protected byte[] SerializeColor(Color color)
        {
            byte[] colorData = new byte[sizeof(float) * 3];
            byte[] r = BitConverter.GetBytes(color.r);
            byte[] g = BitConverter.GetBytes(color.g);
            byte[] b = BitConverter.GetBytes(color.b);
            r.CopyTo(colorData, 0);
            g.CopyTo(colorData, sizeof(float));
            b.CopyTo(colorData, sizeof(float) * 2);
            return colorData;
        }

        protected Color DeserializeColor(byte[] colorData)
        {
            float r = BitConverter.ToSingle(colorData, 0);
            float g = BitConverter.ToSingle(colorData, sizeof(float));
            float b = BitConverter.ToSingle(colorData, sizeof(float) * 2);

            return new Color(r, g, b);
        }

        protected virtual void SendMessage(NetworkMessageStruct message)
        {
            throw new NotImplementedException();
        }

        protected void DeserializeColorAndSendEvent(byte[] colorData)
        {
            Color color = DeserializeColor(colorData);
            ColorChangeMessageRecieved?.Invoke(color);
        }

        protected void DeserializeObjectSpawnAndSendEvent(byte[] objectSpawnData)
        {
            Pose pose = DeserializeObjectSpawn(objectSpawnData);
            ObjectSpawned?.Invoke(pose);
        }
    }
}
