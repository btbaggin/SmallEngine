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
            pStream.Write(BitConverter.GetBytes(pInt), 0, 4);
        }

        public static void WriteString(this Stream pStream, string pString)
        {
            pStream.WriteInt(pString.Length);
            pStream.Write(System.Text.Encoding.ASCII.GetBytes(pString), 0, pString.Length);
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
    }
}
