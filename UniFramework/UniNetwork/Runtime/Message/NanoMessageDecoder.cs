using System;
using System.Text;

namespace UniFramework.Network
{
    public class NanoMessageDecoder : IMessageDecoder
    {
        public IMessage Decode(byte[] bytes)
        {
            NanoMessage msg = new NanoMessage();

            var flag = bytes[0];
            var offset = 1;

            msg.MsgType = (NanoMessageType)((flag >> 1) & 0x07);

            if (msg.MsgType == NanoMessageType.Request || msg.MsgType == NanoMessageType.Response)
            {
                long id = 0;
                for (int i = offset; i < bytes.Length; i++)
                {
                    var b = bytes[i];
                    var t = ((long)(b & 0x7f)) << (7 * (i - offset));
                    id += t;
                    if (b < 128)
                    {
                        offset = i + 1;
                        break;
                    }
                }
                msg.Id = id;
            }
            if (NanoMessage.Routable(msg.MsgType))
            {
                var routeLength = (int)bytes[offset];
                offset++;
                ArraySegment<byte> segment = new ArraySegment<byte>(bytes, offset, routeLength);
                msg.Route = Encoding.UTF8.GetString(segment);
                offset += routeLength;
            }
            ArraySegment<byte> segment2 = new ArraySegment<byte>(bytes, offset, bytes.Length - offset);
            msg.Data = segment2.ToArray();
            return msg;
        }
    }
}