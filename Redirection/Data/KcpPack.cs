using huqiang;
using huqiang.Data;
using MessagePack.LZ4;
using MsgPack.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class KcpPack
{
    public static T UnPackMsg<T>(byte[] dat) where T : class
    {
        var table = MessagePackSerializer.Get<T>();
        var ms = new MemoryStream(dat);
        try
        {
            var t = table.Unpack(ms);
            ms.Dispose();
            return t;
        }
        catch
        {
            ms.Dispose();
            return null;
        }
    }
    public static byte[] PackNull(Int32 cmd, Int32 type)
    {
        DataBuffer db = new DataBuffer(4);
        var fake = new FakeStruct(db, Req.Length);
        fake[Req.Cmd] = cmd;
        fake[Req.Type] = type;
        db.fakeStruct = fake;
        var dat = db.ToBytes();
        byte[] buf = new byte[dat.Length];
        int len = LZ4Codec.Encode32Unsafe(dat, 0, dat.Length, buf, 0, buf.Length);
        dat = WriteLen(dat.Length, len, buf);
        buf = AES.Instance.Encrypt(dat);
        return buf;
    }
    public static byte[] PackString(Int32 cmd, Int32 type, string str)
    {
        DataBuffer db = new DataBuffer(4);
        var fake = new FakeStruct(db, Req.Length);
        fake[Req.Cmd] = cmd;
        fake[Req.Type] = type;
        fake.SetData(Req.Args, str);
        db.fakeStruct = fake;
        var dat = db.ToBytes();
        byte[] buf = new byte[dat.Length];
        int len = LZ4Codec.Encode32Unsafe(dat, 0, dat.Length, buf, 0, buf.Length);
        dat = WriteLen(dat.Length, len, buf);
        buf = AES.Instance.Encrypt(dat);
        return buf;
    }
    public static unsafe byte[] PackStruct<T>(Int32 cmd, Int32 type, T obj) where T : unmanaged
    {
        DataBuffer db = new DataBuffer(4);
        var fake = new FakeStruct(db, Req.Length);
        fake[Req.Cmd] = cmd;
        fake[Req.Type] = type;
        db.fakeStruct = fake;
        int len = sizeof(T);
        FakeStruct fs = new FakeStruct(db, len / 4);
        *(T*)fs.ip = obj;
        fake.SetData(Req.Args, fs);
        var dat = db.ToBytes();
        byte[] buf = new byte[dat.Length];
        len = LZ4Codec.Encode32Unsafe(dat, 0, dat.Length, buf, 0, buf.Length);
        dat = WriteLen(dat.Length, len, buf);
        buf = AES.Instance.Encrypt(dat);
        return buf;
    }
    public static byte[] PackObject<T>(Int32 cmd, Int32 type, object obj) where T : class
    {
        DataBuffer db = new DataBuffer(4);
        var fake = new FakeStruct(db, Req.Length);
        fake[Req.Cmd] = cmd;
        fake[Req.Type] = type;
        db.fakeStruct = fake;
        var ser = MessagePackSerializer.Get<T>();
        MemoryStream ms = new MemoryStream();
        ser.Pack(ms, obj);
        fake.SetData(Req.Args, ms.ToArray());
        ms.Dispose();
        var dat = db.ToBytes();
        byte[] buf = new byte[dat.Length];
        int len = LZ4Codec.Encode32Unsafe(dat, 0, dat.Length, buf, 0, buf.Length);
        dat = WriteLen(dat.Length, len, buf);
        buf = AES.Instance.Encrypt(dat);
        return buf;
    }
    static byte[] WriteLen(int all, int len, byte[] dat)
    {
        var tmp = new byte[dat.Length + 8];
        var l = len.ToBytes();
        var a = all.ToBytes();
        int s = 4;
        for (int i = 0; i < 4; i++)
        {
            tmp[i] = l[i];
            tmp[s] = a[i];
            s++;
        }
        for (int i = 0; i < len; i++)
        {
            tmp[s] = dat[i];
            s++;
        }
        return tmp;
    }
    public static byte[] Pack(DataBuffer data)
    {
        var dat = data.ToBytes();
        byte[] buf = new byte[dat.Length];
        int len = LZ4Codec.Encode32Unsafe(dat, 0, dat.Length, buf, 0, buf.Length);
        dat = WriteLen(dat.Length, len, buf);
        buf = AES.Instance.Encrypt(dat);
        return buf;
    }
    public static DataBuffer UnPack(byte[] dat)
    {
        if (dat.Length > 2)
        {
            try
            {
                dat = AES.Instance.Decrypt(dat);
                int len = dat.ReadInt32(0);
                int all = dat.ReadInt32(4);
                byte[] buf = new byte[all];
                int o = LZ4Codec.Decode32Unsafe(dat, 8, len, buf, 0, all);
                dat = new byte[o];
                for (int i = 0; i < o; i++)
                    dat[i] = buf[i];
                return new DataBuffer(dat);
            }
            catch
            {
            }
        }
        return null;
    }
}
