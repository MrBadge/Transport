using System;
using System.Collections.Generic;
using System.Text;

namespace Autodrome.Base
{
   public class DataEventArgs : EventArgs
   {
      private byte[] data;
      private int offset;
      private int count;

      public byte[] Data { get { return data; } }
      public int Offset { get { return offset; } }
      public int Count { get { return count; } }

      public DataEventArgs(byte[] _data, int _offset, int _count)
      {
         data = _data;
         offset = _offset;
         count = _count;
      }
   }
}
