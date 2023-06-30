using System.Collections.Generic;
using System.IO;

namespace UniFramework.Network
{
    public class Utils
    {
        public static int ByteToInt(byte[] arr)
        {
            int result = 0;
            foreach (var b in arr)
            {
                result = (result << 8) + b;
            }

            return result;
        }


        public static byte[] IntToBytes(int b, int bit = 3)
        {
            byte[] bytes = new byte[3];

            bytes[0] = (byte)((b >> 16) & 0xFF);    // 获取高位字节
            bytes[1] = (byte)((b >> 8) & 0xFF);     // 获取中位字节
            bytes[2] = (byte)(b & 0xFF);            // 获取低位字节

            return bytes;
        }

        public static long BytesToVarint(byte[] arr)
        {
            long id = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                var b = arr[i];
                var t = (b & 0x7f) << i;
                id += t;
            }
            return id;
        }

        public static byte[] VarintToBytes(long id)
        {
            MemoryStream stream = new MemoryStream();
            while (true)
            {
                var b = id % 128;
                id >>= 7;
                if (b != 0)
                {
                    stream.WriteByte((byte)(b + 128));
                }
                else
                {
                    stream.WriteByte((byte)b);
                    break;
                }
            }

            return stream.ToArray();
        }
    }
}