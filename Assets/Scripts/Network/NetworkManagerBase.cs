using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARPeerToPeerSample.Network
{
    public abstract class NetworkManagerBase : INetworkManager
    {
        // todo: polling system would be a better implementation, but this is fine for now
        public Action<string> ServiceFound;
        public Action<Color> ColorChangeMessageRecieved;
        public Action ConnectionEstablished;

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

        protected static bool VerifyColor(Color originalColor, Color deserializedColor)
        {
            return Math.Abs(originalColor.r - deserializedColor.r) < Mathf.Epsilon && Math.Abs(originalColor.g - deserializedColor.g) < Mathf.Epsilon && Math.Abs(originalColor.b - deserializedColor.b) < Mathf.Epsilon;
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
    }
}
