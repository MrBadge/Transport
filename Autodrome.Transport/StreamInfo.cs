using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Autodrome.Transport
{
   delegate void AcceptHandler(TcpClient client, StreamReadHandler read_handler);
   delegate void StreamReadHandler(StreamInfo stream, int count);

   public enum ClientType
   {
      Undefined = 0,
      Autodrome,
      Navigation,
      Application,
      Base,
   }
   
   public class StreamInfo
   {
      public ulong Key;
      public byte[] InputBuffer;
      public Stream Stream;
      public TcpClient Client;
      public ClientType Type;
      public object Tag;
      public Int64 BytesRead;
      public Int64 BytesWritten;

      public bool Connected
      {
         get
         {
            try
            {
               byte[] tmp = new byte[1];
               Client.Client.Send(tmp, 0, 0);
            }
            catch (SocketException e)
            {
               if (!e.NativeErrorCode.Equals(10035))
               {
                  return false;
               }
            }
            return true;
         }
      }

      public void Write(byte[] data, int offset, int count)
      {
         BytesWritten += count;
         Stream.Write(data, offset, count);
      }
   }

   class ListenerInfo
   {
      public TcpListener Listener;
      public ClientType Type;
   }
}
