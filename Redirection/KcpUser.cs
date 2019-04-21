using huqiang;
using huqiang.Data;
using MessagePack.LZ4;
using MsgPack.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class KcpUser : KcpLink
{
    public KcpUser(KcpServer server) : base(server)
    {
#if DEBUG
        Console.WriteLine("new link");
#endif
    }
    public override void Dispatch(byte[] dat, byte tag)
    {
        ServerDataControll.Dispatch(this, dat, tag);
    }
    public override void Disconnect()
    {
#if DEBUG
        Console.WriteLine("break link");
#endif
        KcpServer.Instance.RemoveLink(this);
    }
    public void Send(byte[][] data)
    {
        try
        {
            for (int i = 0; i < data.Length; i++)
                kcp.soc.Send(data[i], data[i].Length, endpPoint);
        }
        catch
        {
        }
    }
    public void SendNull(Int32 cmd, Int32 type)
    {
        var buf = KcpPack.PackNull(cmd, type);
        Send(buf, EnvelopeType.AesDataBuffer);
    }
    public void SendString(Int32 cmd, Int32 type, string obj)
    {
        var buf = KcpPack.PackString(cmd, type, obj);
        Send(buf, EnvelopeType.AesDataBuffer);
    }
    public void SendStruct<T>(Int32 cmd, Int32 type, T obj) where T : unmanaged
    {
        var buf = KcpPack.PackStruct<T>(cmd, type, obj);
        Send(buf, EnvelopeType.AesDataBuffer);
    }
    public void SendObject<T>(Int32 cmd, Int32 type, object obj) where T : class
    {
        var buf = KcpPack.PackObject<T>(cmd,type,obj);
        Send(buf, EnvelopeType.AesDataBuffer);
    }
}