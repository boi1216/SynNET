using System;
using System.Net;
using System.IO;
using MiNET.Net;
using MiNET.Utils;
using System.Linq;
using System.Text;

namespace SynapseMiNET.network.protocol 
{
    abstract class Packet : ICloneable
    {

        public MemoryStream _buffer;
        protected BinaryReader _reader;
        protected BinaryWriter _writer;

        public int Id;
        public Packet()
        {
            _buffer = new MemoryStream();
            _reader = new BinaryReader(_buffer);
            _writer = new BinaryWriter(_buffer);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void setBuffer(byte[] buffer)
        {
            _buffer = new MemoryStream(buffer);
            _reader = new BinaryReader(_buffer);
            _writer = new BinaryWriter(_buffer);
        }

        public void writeByteArray(byte[] array)
        {
            _writer.Write(array);
        }

        public byte[] readByteArray(int len)
        {
           return  _reader.ReadBytes(len);
        }

        public void writeBool(bool b)
        {
            byte byteB =  (byte)(b ? 0x01 : 0x00);
            _writer.Write(byteB);

        }

        public bool readBool()
        {
            return _reader.ReadByte() == 0;
        }

        public void writeByte(byte value)
        {
            _writer.Write(new byte[] { value });
        }

        public byte readByte()
        {
           return  _reader.ReadByte();
        }

        public void writeString(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            writeUnsignedVarInt(bytes.Length);
            _writer.Write(bytes);
        }

        public string getString()
        {
            return Encoding.UTF8.GetString(getByteArray());
        }

        public byte[] getByteArray()
        {
            return _reader.ReadBytes((int)VarInt.ReadUInt32(_buffer));
        }

        public void writeUnsignedVarInt(int i)
        {
            VarInt.WriteUInt32(_buffer, (uint)i);
        }

        public long getUnsignedVarInt()
        {
            return VarInt.ReadUInt32(_buffer);
        }

        public void writeInt(int i)
        {
            var bytes = new byte[]{
                (byte) ((i >> 24) & 0xFF),
                (byte) ((i >> 16) & 0xFF),
                (byte) ((i >> 8) & 0xFF),
                (byte) (i & 0xFF)
        };
            _writer.Write(bytes);
        }

        public int readInt()
        {
            return _reader.ReadInt32();
        }

        public void writeFloat(float i)
        {
            writeInt(BitConverter.ToInt32(BitConverter.GetBytes(i), 0));
        }

        public void writeLong(long l)
        {
            var bytes = new byte[]{
                (byte) (tripleRightShift((int)l, 56)),
                (byte) (tripleRightShift((int)l, 48)),
                (byte) (tripleRightShift((int)l, 40)),
                (byte) (tripleRightShift((int)l, 32)),
                (byte) (tripleRightShift((int)l, 24)),
                (byte) (tripleRightShift((int)l, 16)),
                (byte) (tripleRightShift((int)l, 8)),
                (byte) (l)
        };
            _writer.Write(bytes);
        }

        public static byte[] writeLLong(long l)
        {
            return new byte[]{
                (byte) (l),
                (byte) (l >> 8),
                (byte) (l >> 16),
                (byte) (l >> 24),
                (byte) (l >> 32),
                (byte) (l >> 40),
                (byte) (l >> 48),
                (byte) (l >> 56),
        };
        }

        public static long readLLong(byte[] bytes)
        {
            return (((long)bytes[7] << 56) +
                    ((long)(bytes[6] & 0xFF) << 48) +
                    ((long)(bytes[5] & 0xFF) << 40) +
                    ((long)(bytes[4] & 0xFF) << 32) +
                    ((long)(bytes[3] & 0xFF) << 24) +
                    ((bytes[2] & 0xFF) << 16) +
                    ((bytes[1] & 0xFF) << 8) +
                    ((bytes[0] & 0xFF)));
        }


        public int tripleRightShift(int value, int pos)
        {
            if (pos != 0)
            {
                int mask = 0x7fffffff;
                value >>= 1;
                value &= mask;
                value >>= pos - 1;
            }
            return value;
        }

        public void writeUUID(UUID uuid)
        {
            _writer.Write(uuid.GetBytes());
        }

        public UUID readUUID()
        {
            byte[] bytes = _reader.ReadBytes(16);


            return new UUID(bytes);
        }

        public static byte[] appendBytes(byte[][] bytes)
        {
            int length = 0;
            foreach (byte[] b in bytes)
            {
                length += b.Length;
            }
            MemoryStream buffer = new MemoryStream(length);
            foreach (byte[] b in bytes)
            {
                buffer.Write(b);
            }
            return buffer.ToArray();
        }


        public void writeId()
        {
            writeUnsignedVarInt(Id);
        }

        public byte[] getEncoded()
        {
            return this._buffer.ToArray();
        }

        public abstract void encode();

        public abstract void decode();
    }
}
