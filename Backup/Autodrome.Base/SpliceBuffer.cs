using System;
using System.Collections.Generic;
using System.Text;

using Autodrome.Interfaces;

namespace Autodrome.Base
{
   public interface ISpliceBuffer
   {
      void Write(DataEventArgs args);
      //UInt64 Tag { get;set;}
      Object Tag { get;set;}
   }

   public class SpliceBuffer : ISpliceBuffer
   {
      private byte[] data;
      private int offset;

      //private UInt64 tag;
      //public UInt64 Tag { get { return tag; } set { tag = value; } }

      private Object tag;
      public Object Tag { get { return tag; } set { tag = value; } }

      public byte[] Data { get { return data; } }
      public int Offset { get { return offset; } }

      public SpliceBuffer(int size)
      {
         data = new byte[size];
         offset = 0;
      }

      public event EventHandler<DataEventArgs> PacketReceived;

      protected virtual bool PacketFound(ref int position) { return position < offset; } // найдено ли начало пакета
      protected virtual bool PacketReady(int position) { return true; } // целиком ли собран пакет
      protected virtual bool PacketValid(int position) { return true; } // не битый ли пакет
      protected virtual int PacketSize(int position) { return 1; } // размер пакета

      static public UInt32 CalculateCRC(byte[] data, int offset, int size) // вычисляет crc
      {
         UInt32 ulTemp1;
         UInt32 ulTemp2;
         UInt32 ulCRC = 0;
         int end = offset + size;
         for (int i = offset; i < end; i++)
         {
            ulTemp1 = (ulCRC >> 8) & 0x00FFFFFF;
            ulTemp2 = crc32value((ulCRC ^ (UInt32)(data[i])) & 0xFF);
            ulCRC = ulTemp1 ^ ulTemp2;
         }
         return (ulCRC);
      }
      static private UInt32 crc32value(UInt32 value)
      {
         for (int j = 8; j > 0; j--)
         {
            if ((value & 1) != 0)
               value = (value >> 1) ^ 0xEDB88320;
            else
               value >>= 1;
         }
         return value;
      }

      virtual public void Write(DataEventArgs args)
      {
         if (data.Length - offset < args.Count)
            throw new Exception("received data too long");
         if ((args.Data != data) || (args.Offset != offset))
         {
            Array.Copy(args.Data, args.Offset, data, offset, args.Count);
         }
         offset += args.Count;
         int position = 0;
         while (PacketFound(ref position) && PacketReady(position))
         {
            if (PacketValid(position))
            {
               int size = PacketSize(position);
               if (PacketReceived != null)
                  PacketReceived(this, new DataEventArgs(data, position, size));
               position += size;
            }
            else
            {
               position++;
            }
         }
         if (position > 0)
         {
            offset -= position;
            if (offset > 0)
            {
               Array.Copy(data, position, data, 0, offset);
            }
         }
      }
   }

   public class ApplicationInputSpliceBuffer : SpliceBuffer
   {
      protected override bool PacketReady(int position)
      {
         int bytes_left = Offset - position;
         return (bytes_left > 12) && (bytes_left >= PacketSize(position));
      }

      protected override int PacketSize(int position)
      {
         return BitConverter.ToInt16(Data, position + 8) + 12;
      }

      public ApplicationInputSpliceBuffer(int size)
         : base(size)
      {
      }
   }

   public class SignatureSpliceBuffer : SpliceBuffer
   {
      private byte[] signature;

      public SignatureSpliceBuffer(byte[] _signature, int size)
         : base(size)
      {
         signature = _signature;
      }

      protected override bool PacketFound(ref int position)
      {
         bool result = false;
         int end = Offset - signature.Length;
         while(position<=end)
         {
            int i = 0;
            for (; i < signature.Length; i++)
            {
               if (Data[position + i] != signature[i]) break;
            }
            if (i == signature.Length)
            {
               result = true;
               break;
            }
            position++;
         }
         return result;
      }
   }
}
