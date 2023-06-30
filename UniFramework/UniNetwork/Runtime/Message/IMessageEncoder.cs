namespace UniFramework.Network
{
    public interface IMessageEncoder
    {
        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="message">要编码的message</param>
        byte[] Encode(IMessage message);
    }
}