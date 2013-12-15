using System;
using System.Collections.Generic;
using System.Text;

namespace Autodrome.Base
{
   public class LGS2_SpliceBuffer : SignatureSpliceBuffer
   {
      public LGS2_SpliceBuffer(int size)
         : base(new byte[] { 0xa8 }, size)
      {
      }
      protected override bool PacketReady(int position)
      {
         int bytes_left = Offset - position;
         return (bytes_left > 3) && (bytes_left >= PacketSize(position));
      }
      protected override bool PacketValid(int position)
      {
         int control_size = Data[position + 1];
         byte checksum = 0;
         for (int i = 0; i < control_size; i++)
         {
            checksum ^= Data[position + i + 2];
         }
         return (Data[position + control_size + 2] == checksum) && (Data[position + control_size + 3] == 0xa9);
      }
      protected override int PacketSize(int position)
      {
         int result = (Int16)Data[position + 1] + 4;
         return result;
      }
   }
}
