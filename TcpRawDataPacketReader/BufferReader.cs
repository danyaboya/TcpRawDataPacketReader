using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class BufferReader : BinaryReader
{
    public BufferReader(byte[] buffer)
        : this(new MemoryStream(buffer))
    {
    }

    public BufferReader(MemoryStream stream)
        : base(stream)
    {
    }

    public override string ReadString()
    {
        int num = ReadLength();
        if (num > 0)
        {
            byte[] array = new byte[num];
            Read(array, 0, array.Length);
            Encoding uTF = Encoding.UTF8;
            return uTF.GetString(array);
        }
        return string.Empty;
    }

    public string[] ReadStringArray()
    {
        int num = ReadLength();
        string[] array = new string[num];
        for (int i = 0; i < num; i++)
        {
            array[i] = ReadString();
        }
        return array;
    }

    public byte[] ReadByteArray()
    {
        int num = ReadLength();
        byte[] array = new byte[num];
        for (int i = 0; i < num; i++)
        {
            array[i] = ReadByte();
        }
        return array;
    }

    public T ReadDesc<T>() where T : IBufferRead, new()
    {
        T result = new T();
        result.Read(this);
        return result;
    }

    public List<T> ReadDescList<T>() where T : IBufferRead, new()
    {
        int num = ReadLength();
        List<T> list = new List<T>();
        for (int i = 0; i < num; i++)
        {
            list.Add(ReadDesc<T>());
        }
        return list;
    }

    public List<ushort> ReadUInt16List()
    {
        int num = ReadLength();
        List<ushort> list = new List<ushort>();
        for (int i = 0; i < num; i++)
        {
            list.Add(ReadUInt16());
        }
        return list;
    }

    public List<uint> ReadUInt32List()
    {
        int num = ReadLength();
        List<uint> list = new List<uint>();
        for (int i = 0; i < num; i++)
        {
            list.Add(ReadUInt32());
        }
        return list;
    }

    public T[] ReadDescArray<T>() where T : IBufferRead, new()
    {
        int num = ReadLength();
        T[] array = new T[num];
        for (int i = 0; i < num; i++)
        {
            array[i] = ReadDesc<T>();
        }
        return array;
    }

    public HashSet<T> ReadSet<T>()
    {
        ushort num = ReadUInt16();
        HashSet<T> hashSet = new HashSet<T>();
        for (int num2 = 1; num2 < 65536; num2 <<= 1)
        {
            if ((num & num2) != 0)
            {
                hashSet.Add((T)Enum.ToObject(typeof(T), num2));
            }
        }
        return hashSet;
    }

    public int ReadLength()
    {
        return ReadUInt16();
    }


    public DateTime ReadDateTime()
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(ReadUInt32());
    }

    public DateTime ReadLongDateTime()
    {
        ulong num = ReadUInt64();
        if (num != 0L)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(num);
        }
        return default(DateTime);
    }

    public uint ReadGUID()
    {
        return ReadUInt32();
    }

    public static ushort ReadBufferSize(byte[] data)
    {
        ushort num = data[0];
        ushort num2 = data[1];
        return (ushort)((num << 8) | num2);
    }

}