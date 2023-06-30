using System;
using System.Collections.Generic;

namespace UniFramework.Network
{
    public enum PackageType : int
    {
        Handshake = 1,
        HandshakeAck,
        Heartbeat,
        Data,
        Kick,
    }


    public class NanoNetPackageDecoder : INetPackageDecoder
    {
        //包头 4byte  1byte 表示包类型,3byte表示包长度
        private const int PackageHeaderLength = 4; //int类型
        private const int PackageTypeFielderLength = 1; //int类型
        private const int PackageHeaderLengthFiledSize = 3; //int类型
        private HandleErrorDelegate _handleErrorCallback;


        public int GetPackageHeaderSize()
        {
            return PackageHeaderLength;
        }

        public void RigistHandleErrorCallback(HandleErrorDelegate callback)
        {
            _handleErrorCallback = callback;
        }


        public void Decode(int packageBodyMaxSize, RingBuffer ringBuffer, List<INetPackage> outputPackages)
        {
            // 循环解包
            while (true)
            {
                // 如果数据不够判断消息长度
                if (ringBuffer.ReadableBytes < PackageHeaderLengthFiledSize)
                    break;

                ringBuffer.MarkReaderIndex();

                // 读取Package类型
                PackageType packageType = (PackageType)ringBuffer.ReadByte();
                NanoNetPackage package = new NanoNetPackage();
                package.PackageType = packageType;
                byte[] packageSizeBytes = ringBuffer.ReadBytes(3);

                // byte[] extendedArray = new byte[4];
                // Array.Copy(packageSizeBytes, 0, extendedArray, 1, packageSizeBytes.Length);
                // Array.Reverse(extendedArray);
                int packageSize = Utils.ByteToInt(packageSizeBytes);

                // 如果剩余可读数据小于Package长度
                if (ringBuffer.ReadableBytes < packageSize)
                {
                    ringBuffer.ResetReaderIndex();
                    break; //需要退出读够数据再解包
                }

                // 检测包体长度
                int bodySize = packageSize; // - PackageHeaderIDFiledSize;
                if (bodySize > packageBodyMaxSize)
                {
                    _handleErrorCallback(true,
                        $"The decode package {package.PackageType} body size is larger than {packageBodyMaxSize} !");
                    break;
                }

                // 读取包体
                {
                    package.BodyBytes = ringBuffer.ReadBytes(bodySize);
                    outputPackages.Add(package);
                }
            }

            // 注意：将剩余数据移至起始
            ringBuffer.DiscardReadBytes();
        }
    }
}