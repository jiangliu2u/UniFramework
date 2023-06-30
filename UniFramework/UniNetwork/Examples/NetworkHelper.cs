using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UniFramework.Network;
using UnityEngine;

public class NetworkHelper :MonoBehaviour
{
    // 登录请求消息
    class LoginRequestMessage
    {
        public string Name;
        public string Password;
    }

    class TestRequest
    {
        public int int_field;
        public string string_field;
    }
    
    class TestMessage
    {
        public int code;
        public string message;
    }

    
    public class VersionInfo
    {
        public int code;
        public string sys;
    }

    private IMessageDecoder msgDecoder = new NanoMessageDecoder();
    private IMessageEncoder msgEncoder = new NanoMessageEncoder();

// 登录反馈消息
    class LoginResponseMessage
    {
        public string Result;
    }
    UniFramework.Network.TcpClient _client = null;

// 创建TCP客户端
    void CreateClient()
    {
        // 初始化网络系统
        UniNetwork.Initalize();

        // 创建TCP客户端
        int packageMaxSize = short.MaxValue;
        var encoder = new NanoNetPackageEncoder();
        var decoder = new NanoNetPackageDecoder();
        _client = UniNetwork.CreateTcpClient(packageMaxSize, encoder, decoder);

        // 连接服务器
        var remote = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12200);
        _client.ConnectAsync(remote, OnConnectServer);
    }
    
    // 关闭TCP客户端
    void CloseClient()
    {
        if(_client != null)
        {
            _client.Dispose();
            _client = null; 
        }
    }

    void OnConnectServer(SocketError error)
    {
        Debug.Log($"Server connect result : {error}");
        if (error == SocketError.Success)
            Debug.Log("服务器连接成功！");
        else
            Debug.Log("服务器连接失败！");
    }


    private void Start()
    {
        CreateClient();
    }

    void Update()
    {
        // 每帧去获取解析的网络包
        var networkPackage = _client.PickPackage() as NanoNetPackage;
        
        if(networkPackage != null)
        {
            switch (networkPackage.PackageType)
            {
                case PackageType.Heartbeat:
                    Debug.Log("NanoNetPackage HeartBeat");
                    break;
                case PackageType.Data:
                    Debug.Log("NanoNetPackage Data");

                    var arr = networkPackage.BodyBytes;
                    NanoMessage a = msgDecoder.Decode(arr) as NanoMessage;
                    TestMessage t = JsonUtility.FromJson<TestMessage>(Encoding.UTF8.GetString(a.Data));
                    Debug.Log(t.code);
                    Debug.Log(t.message);
                    break;
                case PackageType.HandshakeAck:
                    Debug.Log("NanoNetPackage HandshakeAck");
                    break;
                case PackageType.Handshake:
                    Debug.Log("NanoNetPackage Handshake");
                    string json = Encoding.UTF8.GetString(networkPackage.BodyBytes);
                    VersionInfo message = JsonUtility.FromJson<VersionInfo>(json);
                    Debug.Log(message.sys);
                    SendHandshakeAck();
                    break;
                case PackageType.Kick:
                    Debug.Log("NanoNetPackage Kick");
                    break;
            }
        }
    }


    public void SendHandshake()
    {
        NanoNetPackage networkPackage = new NanoNetPackage
        {
            PackageType = PackageType.Handshake,
            BodyBytes = Array.Empty<byte>(),
        };
        _client.SendPackage(networkPackage);
    }
    
    public void SendHandshakeAck()
    {
        NanoNetPackage networkPackage = new NanoNetPackage
        {
            PackageType = PackageType.HandshakeAck,
            BodyBytes = Array.Empty<byte>(),
        };
        _client.SendPackage(networkPackage);
    }
    
// 发送登录请求消息
    public void SendLoginMessage()
    {

        var msg = new NanoMessage();
        msg.MsgType = NanoMessageType.Request;
        msg.Route = "Manager.TestRequest";
        msg.Id = 2222;

        TestRequest req = new TestRequest();
        req.int_field = 250;
        req.string_field = "在上述示例中，我们创建了一个 byte 数组 byteArray，其中含有一些字节数据。然后，使用 ArraySegment 的构造函数，指定原始数组、截取的起始索引和截取的长度，来创建一个字节数组的片段 segment。最后，使用 segment.ToArray() 将片段转换为一个新的字节数组 subArra";
        msg.Data = Encoding.UTF8.GetBytes(JsonUtility.ToJson(req)); 
        
        
        NanoNetPackage networkPackage = new NanoNetPackage
        {
            PackageType = PackageType.Data,
            BodyBytes = msgEncoder.Encode(msg),
        };
        _client.SendPackage(networkPackage);
    }
}