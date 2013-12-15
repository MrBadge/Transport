using System;
using System.Collections.Generic;
using System.Text;

using Autodrome.Base;

namespace Autodrome.Navigation
{
   public class GSOFPacketWrapper : Wrapper
   {
      public int Unwrap(byte[] data, int offset, int count)
      {
         return count;
      }
   }
}
