using System;
using System.Collections.Generic;
using System.Text;

using Autodrome.Base;

namespace Autodrome.Base
{
   abstract class BufferedUnwrapper : Unwrapper
   {
      private byte[] buffer; // буфер с принятыми байтами
      private int count;    // количество неразобранных байт

      abstract protected int Process(byte[] data, int offset, int size);

      public BufferedUnwrapper(int size)
      {
         buffer = new byte[size];
      }
      
      public void Write(DataEventArgs args)
      {
         Array.Copy(args.Data, args.Offset, buffer, count, args.Count);
         int size = count + args.Count;
         int processed = Process(buffer, 0, size);
         count = size - processed;
         if ((count > 0) && (processed > 0))
         {
            Array.Copy(buffer, processed, buffer, 0, count);
         }
      }
   }
}
