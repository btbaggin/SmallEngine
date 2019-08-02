using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SmallEngine.Components;

namespace SmallEngine.Serialization
{
    static class SerializationUtils
    {
        public readonly static Type DeserializeBeginType = typeof(OnDeserializeBeginAttribute);
        public readonly static Type DeserializeFinishType = typeof(OnDeserializeFinishAttribute);
        public readonly static Type NonSerializedType = typeof(NonSerializedAttribute);
        public readonly static Type ImportComponentType = typeof(ImportComponentAttribute);

        public static void WriteInt(this Stream pStream, int pInt)
        {
            const int size = sizeof(int);
            pStream.Write(BitConverter.GetBytes(pInt), 0, size);
        }

        public static void WriteString(this Stream pStream, string pString)
        {
            pStream.WriteInt(pString.Length);
            pStream.Write(System.Text.Encoding.ASCII.GetBytes(pString), 0, pString.Length);
        }

        public static void WriteFloat(this Stream pStream, float pFloat)
        {
            const int size = sizeof(float);
            pStream.Write(BitConverter.GetBytes(pFloat), 0, size);
        }

        public static void WriteBytes(this Stream pStream, byte[] pBytes)
        {
            pStream.WriteInt(pBytes.Length);
            pStream.Write(pBytes, 0, pBytes.Length);
        }

        public static int ReadInt(this Stream pStream)
        {
            byte[] buffer = new byte[4];

            pStream.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static string ReadString(this Stream pStream)
        {
            int length = pStream.ReadInt();

            byte[] buffer = new byte[length];
            pStream.Read(buffer, 0, length);

            return System.Text.Encoding.ASCII.GetString(buffer);
        }

        public static float ReadFloat(this Stream pStream)
        {
            const int size = sizeof(float);
            byte[] buffer = new byte[size];
            pStream.Read(buffer, 0, size);

            return BitConverter.ToSingle(buffer, 0);
        }

        public static byte[] ReadBytes(this Stream pStream)
        {
            int length = pStream.ReadInt();
            byte[] buffer = new byte[length];
            pStream.Read(buffer, 0, length);

            return buffer;
        }
    }
}
