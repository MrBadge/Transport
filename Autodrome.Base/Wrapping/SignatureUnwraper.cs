using System;
using System.Collections.Generic;
using System.Text;

namespace Autodrome.Base.Wrapping
{
   class SignatureUnwrapper : BufferedUnwrapper
   {
      private byte[] signature;

      public SignatureUnwrapper(int size, byte[] _signature)
         : base(size)
      {
         signature = _signature;
      }

      protected override int Process(byte[] data, int offset, int size)
      {
         //int position = offset;
         //while (PacketFound(ref position) && PacketReady(position))
         //{
         //   if (PacketValid(position))
         //   {
         //      int count = PacketSize(position);
         //      if (PacketReceived != null)
         //         PacketReceived(this, new DataEventArgs(data, position, count));
         //      position += count;
         //   }
         //   else
         //   {
         //      position++;
         //   }
         //}
         //return position;
         return offset+size;
      }
   }
}
