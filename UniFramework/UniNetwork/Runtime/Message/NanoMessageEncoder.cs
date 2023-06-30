using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UniFramework.Network
{
    public class NanoMessageEncoder : IMessageEncoder
    {
        public byte[] Encode(IMessage message)
        {
            NanoMessage msg = message as NanoMessage;
            var flag = (byte)msg.MsgType << 1;
            MemoryStream stream = new MemoryStream();
            stream.WriteByte((byte)flag);
            var arr = Utils.VarintToBytes(msg.Id);
            stream.Write(arr, 0, arr.Length);
            var route = Encoding.UTF8.GetBytes(msg.Route);
            stream.WriteByte((byte)route.Length);
            stream.Write(route, 0, route.Length);
            stream.Write(msg.Data, 0, msg.Data.Length);
            var a = stream.ToArray();
            return a;
        }
    }
}