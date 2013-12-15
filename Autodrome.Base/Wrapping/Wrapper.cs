using System;
using System.Collections.Generic;
using System.Text;

namespace Autodrome.Base
{
   public class Wrapper
   {
      // кладет данные в обертку
      public int Wrap(DataEventArgs args)
      {
         if (DataWrapped != null)
         {
            DataWrapped(this, args);
         }
         return args.Count;
      }
      // вытаскивает данные из обертки
      // возращает количество просмотренных данных
      public int Unwrap(DataEventArgs args)
      {
         if (DataUnwrapped != null)
         {
            DataUnwrapped(this, args);
         }
         return args.Count;
      }

      public event EventHandler<DataEventArgs> DataWrapped;
      public event EventHandler<DataEventArgs> DataUnwrapped;
   }
}
