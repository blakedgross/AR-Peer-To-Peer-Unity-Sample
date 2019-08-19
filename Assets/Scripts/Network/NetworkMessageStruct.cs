using System;

namespace ARPeerToPeerSample.Network
{
	public enum MessageType : Int32
	{
        ColorChange,
        WorldMap,
        Anchor,
        SpawnedObject
	}

	public struct NetworkMessageStruct
	{
		public MessageType Type;
		public byte[] Message;

        public byte[] Serialize()
		{
			byte[] serializedStream = new byte[sizeof(MessageType) + Message.Length];
			byte[] typeBytes = BitConverter.GetBytes((Int32)Type);
			typeBytes.CopyTo(serializedStream, 0);
			Message.CopyTo(serializedStream, typeBytes.Length);
			return serializedStream;
		}

        public static NetworkMessageStruct Deserialize(byte[] data)
		{
			MessageType type = (MessageType)BitConverter.ToInt32(data, 0);
			byte[] message = new byte[data.Length - sizeof(MessageType)];
            Array.Copy(data, sizeof(MessageType), message, 0, data.Length - sizeof(MessageType));   
			return new NetworkMessageStruct
			{
				Type = type,
                Message = message
			};
		}
	}
}