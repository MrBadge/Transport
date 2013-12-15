using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Text;
using Autodrome.Base;
using Autodrome.Transport;

namespace Autodrome.Transport.Logic
{
    public class MainLogic
    {
        private StreamInfo baseStream;
        public Dictionary<ulong, StreamInfo> autodromeStreams { get; set; }
        public List<StreamInfo> navigationStreams { get; set; }

        public List<StreamInfo> applicationStreams;
        private byte[] output_buffer;
        //private Stream log;
        //private DateTime log_start;
        //private TimeSpan log_span;

        private delegate void AcceptHandler(StreamInfo stream);

        private AcceptHandler acceptHandler;

        private delegate void ReadHandler(StreamInfo stream, int count);

        private ReadHandler readHandler;

        private delegate void ErrorHandler(StreamInfo stream, String str);

        private ErrorHandler errorHandler;

        public MainLogic()
        {
            autodromeStreams = new Dictionary<ulong, StreamInfo>();
            navigationStreams = new List<StreamInfo>();
            applicationStreams = new List<StreamInfo>();
            output_buffer = new byte[3000];

            //acceptHandler = new AcceptHandler(accept);
            errorHandler = new ErrorHandler(error);
            readHandler = new ReadHandler(read);
            availableHandler = new EventHandler(availabilityTimer_Tick);
        }

        private void createListener(String address, String port, ClientType type)
        {
            ListenerInfo listener = new ListenerInfo();
            listener.Type = type;
            listener.Listener = new TcpListener(IPAddress.Parse(address), Int32.Parse(port));
            listener.Listener.Start();
            listener.Listener.BeginAcceptTcpClient(acceptCallback, listener);
        }

        public void FormInitialization(Logger lg)
        {
            autodromeStreams = new Dictionary<ulong, StreamInfo>();
            navigationStreams = new List<StreamInfo>();
            applicationStreams = new List<StreamInfo>();
            output_buffer = new byte[3000];
            
            //acceptHandler = new AcceptHandler(accept);
            errorHandler = new ErrorHandler(error);
            readHandler = new ReadHandler(read);
            availableHandler = new EventHandler(availabilityTimer_Tick);
        }

        public void Accept_Logic(StreamInfo stream)
        {
            switch (stream.Type)
            {
                case ClientType.Application:
                    applicationStreams.Add(stream);
                    break;
                case ClientType.Navigation:
                    autodromeStreams.Add(stream.Key, stream);
                    navigationStreams.Add(stream);
                    break;
                case ClientType.Autodrome:
                    autodromeStreams.Add(stream.Key, stream);
                    break;
                default:
                    //MessageBox.Show("undefined stream accepted");
                    break;
            }

            try
            {
                stream.Stream.BeginRead(stream.InputBuffer, 0, stream.InputBuffer.Length, readCallback, stream);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception accept: {0}", e);
                error(stream, "accept failed");
            }
        }

        private void readCallback(IAsyncResult ar)
        {
            StreamInfo stream = ar.AsyncState as StreamInfo;
            try
            {
                int count = stream.Stream.EndRead(ar); 
                //Invoke(readHandler, new object[] { stream, count });
                stream.Stream.BeginRead(stream.InputBuffer, 0, stream.InputBuffer.Length, readCallback, stream);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception readCallback: {0}", e);
                error(stream, "readCallback failed");
            }
        }

        private void acceptCallback(IAsyncResult ar)
        {
            ListenerInfo listener = ar.AsyncState as ListenerInfo;
            try
            {
                TcpClient client = listener.Listener.EndAcceptTcpClient(ar);
                StreamInfo stream = new StreamInfo();
                stream.Type = listener.Type;
                stream.InputBuffer = new byte[(stream.Type == ClientType.Application) ? 2048 : 1024];
                stream.Client = client;
                stream.Client.ReceiveTimeout = 5000;
                stream.Client.SendTimeout = 5000;
                IPEndPoint ip = stream.Client.Client.RemoteEndPoint as IPEndPoint;
                Int32 address = ip.Address.GetHashCode();
                Int32 port = ip.Port;
                stream.Key = (((UInt64)address) << 32) + (UInt64)port;
                stream.Stream = stream.Client.GetStream();
                if (stream.Type == ClientType.Application)
                {
                    ApplicationInputSpliceBuffer splice = new ApplicationInputSpliceBuffer(2048);
                    stream.Tag = splice;
                    splice.PacketReceived += processApplicationPacket;
                }
                //Invoke(acceptHandler, new object[] { stream });
            }
            catch (Exception e)
            {
                Console.WriteLine("exception acceptCallback: {0}", e);
            }
            listener.Listener.BeginAcceptTcpClient(acceptCallback, listener);
        }

        private void processApplicationPacket(object sender, DataEventArgs args)
        {
            ulong key = BitConverter.ToUInt64(args.Data, args.Offset);
            int count = BitConverter.ToInt32(args.Data, args.Offset + 8);
            StreamInfo stream;
            if (autodromeStreams.TryGetValue(key, out stream))
            {
                byte[] copy = new byte[count];
                Array.Copy(args.Data, args.Offset + 12, copy, 0, count);
                try
                {
                    stream.Write(copy, 0, count);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception write: {0}", e);
                    error(stream, "write failed");
                }
            }
        }

        private void error(StreamInfo stream, String str)
        {
            //if (InvokeRequired)
            //{
            //    Invoke(errorHandler, new object[] { stream, str });
            //    return;
            //}
            try
            {
                stream.Stream.Close();
                //stream.Client.Client.Disconnect(true);
            }
            catch
            {
            }
            try
            {
                //logView.Items.Add(String.Format("{0}: {1} {2}", new object[]
                //{
                //    DateTime.Now.ToLongTimeString(),
                //    stream.Client.Client.RemoteEndPoint,
                //    str
                //}));

                if ((stream.Type == ClientType.Autodrome) || (stream.Type == ClientType.Navigation))
                {
                    byte[] packet = new byte[12];
                    BitConverter.GetBytes(stream.Key).CopyTo(packet, 0);
                    BitConverter.GetBytes((Int32)0).CopyTo(packet, 8);
                    foreach (StreamInfo application in applicationStreams)
                    {
                        try
                        {
                            application.Write(packet, 0, 12);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception writing zero-buffer to application: {0}", e);
                            error(application, "writing zero-buffer to application failed");
                        }
                    }
                }

                switch (stream.Type)
                {
                    case ClientType.Application:
                        applicationStreams.Remove(stream);
                        break;
                    case ClientType.Navigation:
                        navigationStreams.Remove(stream);
                        autodromeStreams.Remove(stream.Key);
                        break;
                    case ClientType.Autodrome:
                        autodromeStreams.Remove(stream.Key);
                        break;
                }
                //foreach (DataGridViewRow row in dataGridView1.Rows)
                //{
                //    if (row.Tag == stream)
                //    {
                //        dataGridView1.Rows.Remove(row);
                //    }
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception error: {0}", e);
            }
        }

        private EventHandler availableHandler;

        private void availabilityTimer_Tick(object sender, EventArgs e)
        {
            //if (InvokeRequired)
            //{
            //    Invoke(availableHandler, new object[] { sender, e });
            //    return;
            //}
            foreach (StreamInfo s in autodromeStreams.Values)
            {
                if (!s.Connected)
                {
                    read(s, 0);
                    autodromeStreams.Remove(s.Key);
                    if (s.Type == ClientType.Navigation)
                    {
                        navigationStreams.Remove(s);
                    }
                }
            }
            foreach (StreamInfo s in applicationStreams)
            {
                if (!s.Connected)
                {
                    applicationStreams.Remove(s);
                }
            }
        }

        private void read(StreamInfo stream, int count/*, bool PropSetDefLogEnabled, string PropSetDefLogPath*/)
        {
            try
            {
                stream.BytesRead += count;
                switch (stream.Type)
                {
                    case ClientType.Autodrome:
                    case ClientType.Navigation:
                        try
                        {
                            BitConverter.GetBytes(stream.Key).CopyTo(output_buffer, 0);
                            BitConverter.GetBytes(count).CopyTo(output_buffer, 8);
                            Array.Copy(stream.InputBuffer, 0, output_buffer, 12, count);
                            //if (Properties.Settings.Default.LogEnabled)
                            //{
                            //    if (DateTime.Now.Subtract(log_start) > log_span)
                            //    {
                            //        if (log != null)
                            //        {
                            //            log.Close();
                            //            log = null;
                            //        }
                            //    }
                            //    if (log == null)
                            //    {
                            //        log_start = DateTime.Now;
                            //        String path = Path.Combine(PropSetDefLogPath/*Properties.Settings.Default.LogPath*/,
                            //            String.Format("{0:yyyyMMdd_HHmmss}.log", log_start));
                            //        log = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read);
                            //    }
                            //    if (log != null)
                            //    {
                            //        log.Write(output_buffer, 0, count + 12);
                            //    }
                            //}
                        }
                        catch (Exception e)
                        {
                        }
                        foreach (StreamInfo application in applicationStreams)
                        {
                            try
                            {
                                //application.Write(BitConverter.GetBytes(stream.Key), 0, 8);
                                //application.Write(BitConverter.GetBytes(count), 0, 4);
                                //application.Write((byte[])stream.InputBuffer.Clone(), 0, count);
                                application.Write(output_buffer, 0, count + 12);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Exception write to application: {0}", e);
                                error(application, "write to application failed");
                            }
                        }
                        break;
                    case ClientType.Application:
                        ApplicationInputSpliceBuffer splice = stream.Tag as ApplicationInputSpliceBuffer;
                        int left = count;
                        int offset = 0;
                        int free = splice.Data.Length - splice.Offset;
                        while ((free > 0) && (left > 0))
                        {
                            int min = Math.Min(free, left);
                            splice.Write(new DataEventArgs(stream.InputBuffer, offset, min));
                            left -= min;
                            offset += min;
                            free = splice.Data.Length - splice.Offset;
                        }
                        break;
                    case ClientType.Base:
                        foreach (StreamInfo client in navigationStreams)
                        {
                            try
                            {
                                client.Write((byte[])stream.InputBuffer.Clone(), 0, count);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Exception write to navigation: {0}", e);
                                error(client, "write to navigation failed");
                            }
                        }
                        break;
                    default:
                        //MessageBox.Show("undefined stream read");
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception read: {0}", e);
                error(stream, "read failed");
            }
        }
    }
}
