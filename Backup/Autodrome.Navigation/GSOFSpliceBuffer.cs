using System;
using System.Collections.Generic;
using System.Text;

using Autodrome.Base;

namespace Autodrome.Navigation
{
   public class GSOFEventArgs : EventArgs
   {
      public GSOF.Record[] Records;
      public GSOFEventArgs(GSOF.Record[] records)
      {
         Records = records;
      }
   }

   public class GSOFSpliceBuffer : SignatureSpliceBuffer
   {
      byte[] output_buffer;
      int output_size;
      Byte output_number;
      
      public event EventHandler<GSOFEventArgs> RecordsReceived;

      private void packetReceived(object sender, DataEventArgs args)
      {
         Byte length = args.Data[args.Offset + 3];
         Byte number = args.Data[args.Offset + 4];
         Byte page = args.Data[args.Offset + 5];
         Byte max_page = args.Data[args.Offset + 6];

         int data_length = length - 3;

         if (data_length < 1) return;

         if (page == 0)
         {
            //начало пакета
            output_number = number;
            output_size = 0;
         }
         if (number == output_number)
         {
            //продолжение пакета
            if (output_buffer.Length < output_size + data_length)
            {
               byte[] old_buffer = output_buffer;
               output_buffer = new byte[output_size + data_length + 256];
               Array.Copy(old_buffer, 0, output_buffer, 0, output_size);
            }
            Array.Copy(args.Data, args.Offset + 7, output_buffer, output_size, data_length);
            output_size += data_length;
            if (page == max_page)
            {
               if (RecordsReceived != null)
               {
                  RecordsReceived(this, new GSOFEventArgs(GSOF.ReadRecords(output_buffer, 0, output_size)));
               }
            }
         }
      }
      
      public GSOFSpliceBuffer(int size)
         : base(new byte[] { 0x02 }, size)
      {
         output_buffer = new byte[256];
         PacketReceived += packetReceived;
      }
      protected override bool PacketReady(int position)
      {
         int bytes_left = Offset - position;
         return (bytes_left > 3) && (bytes_left >= PacketSize(position));
      }
      protected override bool PacketValid(int position)
      {
         int control_size = Data[position + 3] + 4;
         byte checksum = 0;
         for (int i = 1; i < control_size; i++)
         {
            checksum += Data[position + i];
         }
         return (Data[position + control_size] == checksum) && (Data[position + control_size + 1] == 0x03);
      }
      protected override int PacketSize(int position)
      {
         int result = (Int16)Data[position + 3]+6;
         return result;
      }
   }
}
