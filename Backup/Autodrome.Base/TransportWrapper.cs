using System;
using System.Collections.Generic;
using System.Text;

namespace Autodrome.Base
{
   public class TransportWrapperEventArgs : DataEventArgs
   {
      public UInt64 Id;
      public TransportWrapperEventArgs(UInt64 id, byte[] data, int offset, int count)
         :base(data,offset,count)
      {
         Id = id;
      }
   }

   public class TransportWrapper : Wrapper
   {
      byte[] buffer;

      public TransportWrapper()
      {
         buffer = new byte[256];
      }

      int Wrap(UInt64 id, byte[] data, int offset, int count)
      {
         if(buffer.Length<count+12)
         {
            buffer = new byte[count + 12];
         }
         BitConverter.GetBytes(id).CopyTo(buffer, 0);
         BitConverter.GetBytes(count).CopyTo(buffer, 8);
         Array.Copy(data, offset, buffer, 12, count);
         return base.Wrap(new TransportWrapperEventArgs(id,buffer,0,count));
      }
   }
}
