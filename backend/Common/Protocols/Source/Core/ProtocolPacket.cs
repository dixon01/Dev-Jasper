using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Gorba.Common.Protocols.Core
{
    public class ProtocolPacket
    {
        /*static public byte[] StructureToByteArray(object obj)
        {

            int len = Marshal.SizeOf(obj);

            byte[] arr = new byte[len];

            IntPtr ptr = Marshal.AllocHGlobal(len);

            Marshal.StructureToPtr(obj, ptr, true);

            Marshal.Copy(ptr, arr, 0, len);

            Marshal.FreeHGlobal(ptr);

            return arr;
        }
         * */

        public static byte[] StructureToByteArray(object obj)
        {
            int rawsize = Marshal.SizeOf(obj);
            byte[] rawdata = new byte[rawsize];
            GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            try {
                Marshal.StructureToPtr(obj, handle.AddrOfPinnedObject(), false);
            } finally {
                handle.Free();
            }
            return rawdata;
        }

        public static T ByteArrayToStruct<T>(byte[] data)
        {
            //byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];
            int len = Marshal.SizeOf(typeof(T));
            if (data.Length > len) {
                return System.Activator.CreateInstance<T>();
                // Perhaps throw exception ????
            } // if
            GCHandle handle =
                GCHandle.Alloc(data,
                GCHandleType.Pinned);
            T temp = (T)
                Marshal.PtrToStructure(
                handle.AddrOfPinnedObject(),
                typeof(T));
            handle.Free();
            return temp;
        }

        private struct Region
        {
            public int Offset;
            public int Length;
        }


        private int m_MaxPacketSize;
        private Region m_Header;
        private Region m_Body;
        private byte[] m_Data;

        public int Length
        {
            get { return GetLength(); }
        }

        public ProtocolPacket(int bodyLength, int bodyOffset, int maxPacketSize)
        {
            m_MaxPacketSize = maxPacketSize;
            m_Data = new byte[m_MaxPacketSize];
            m_Header.Length = 0;
            m_Body.Offset = bodyOffset;
            m_Body.Length = bodyLength;
        }


        public void AddHeader(int length)
        {
            // Consider the header of the higher layer to be a part of
            // the body for the this layer.
            m_Body.Offset -= m_Header.Length;

            m_Body.Length += m_Header.Length;
            // Save the length and header offset of the new layer. Addition
            // of the header would move up the header offset.            
            m_Header.Length = length;
            if (m_Body.Offset != 0) {
                m_Header.Offset = m_Body.Offset - length;
            } // if
        }

        public void ExtractHeader(int length)
        {
            // Update the new header. The header begins at current
            // body start offset.

            m_Header.Offset = m_Body.Offset;
            m_Header.Length = length;


            // Reduce the body size to account for the removed header.
            m_Body.Offset += length;
            m_Body.Length -= length;
        }

        public byte[] GetHeader()
        {
            byte[] header = new byte[m_Header.Length];
            Array.Copy(m_Data, m_Header.Offset, header, 0, m_Header.Length);
            return header;
        }

        public byte[] GetBody()
        {
            byte[] body = new byte[m_Body.Length];
            Array.Copy(m_Data, m_Body.Offset, body, 0, m_Body.Length);
            return body;
        }

        private int GetLength()
        {
            return (m_Header.Length + m_Body.Length);
        }

        public void SetHeader(byte[] header)
        {
            Array.Copy(header, 0, m_Data, m_Header.Offset, m_Header.Length);
        }

        public void SetBody(byte[] body)
        {
            Array.Copy(body, 0, m_Data, m_Body.Offset, m_Body.Length);
        }

    }
}
