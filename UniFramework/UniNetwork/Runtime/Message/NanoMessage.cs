namespace UniFramework.Network
{
    public enum NanoMessageType : byte
    {
        Request = 0x00,
        Notify = 0x01,
        Response = 0x02,
        Push = 0x03,
    }

    public class NanoMessage : IMessage
    {
        public NanoMessageType MsgType { get; set; }
        public long Id { get; set; }
        public string Route { get; set; }
        public byte[] Data { get; set; }
        private bool compressed;


        public static bool Routable(NanoMessageType msgType)
        {
            return msgType == NanoMessageType.Request || msgType == NanoMessageType.Push ||
                   msgType == NanoMessageType.Notify;
        }
    }
}