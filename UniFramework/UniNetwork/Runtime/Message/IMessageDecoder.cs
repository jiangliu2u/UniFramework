namespace UniFramework.Network
{
    public interface IMessageDecoder
    {
        IMessage Decode(byte[] bytes);
    }
}