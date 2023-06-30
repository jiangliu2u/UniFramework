namespace UniFramework.Network
{
    public class NanoNetPackage:INetPackage
    {
        /// <summary>
        /// 包体类型
        /// </summary>
        public PackageType PackageType { set; get; }

        /// <summary>
        /// 包体数据
        /// </summary>
        public byte[] BodyBytes { set; get; }
    }
}