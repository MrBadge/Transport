using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using Autodrome.Base;

namespace Autodrome.Transport
{
    public class MainLogic
    {
        private readonly Logger LogManager;

        private readonly AcceptHandler acceptHandler;

        private readonly ErrorHandler errorHandler;
        private readonly byte[] output_buffer;
        private readonly ReadHandler readHandler;
        public List<StreamInfo> applicationStreams;
        private EventHandler availableHandler;
        public StreamInfo baseStream;

        public MainLogic(Logger LogManager)
        {
            autodromeStreams = new Dictionary<ulong, StreamInfo>();
            navigationStreams = new List<StreamInfo>();
            applicationStreams = new List<StreamInfo>();
            output_buffer = new byte[3000];

            acceptHandler = accept;
            errorHandler = error;
            readHandler = read;
            //availableHandler = new EventHandler(availabilityTimer_Tick);

            this.LogManager = LogManager;
        }

        public Dictionary<ulong, StreamInfo> autodromeStreams { get; set; }
        private List<StreamInfo> navigationStreams { get; set; }

        public void Start(String basePorts)
        {
            String[] base_strings = basePorts.Split(new[] {':'});

            baseStream = new StreamInfo {Type = ClientType.Base, InputBuffer = new byte[512]};
            try
            {
                if (base_strings.Length == 1)
                {
                    var base_port = new SerialPort(basePorts);
                    base_port.RtsEnable = true;
                    base_port.Open();
                    baseStream.Tag = base_port;
                    baseStream.Stream = base_port.BaseStream;
                }
                else
                {
                    var base_client = new TcpClient(base_strings[0], Int32.Parse(base_strings[1]));
                    baseStream.Stream = base_client.GetStream();
                }
            }
            catch (Exception)
            {
                baseStream.Stream = Stream.Null;
                throw;
            }

            baseStream.Stream.BeginRead(baseStream.InputBuffer, 0, baseStream.InputBuffer.Length, readCallback,
                baseStream);
        }

        public void createListener(String address, String port, ClientType type)
        {
            var listener = new ListenerInfo();
            listener.Type = type;
            listener.Listener = new TcpListener(IPAddress.Parse(address), Int32.Parse(port));
            listener.Listener.Start();
            listener.Listener.BeginAcceptTcpClient(acceptCallback, listener);
        }

        public void accept(StreamInfo stream)
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
                    MessageBox.Show("undefined stream accepted");
                    break;
            }

            Transport.form.DataGridUpdate(stream);
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
            var stream = ar.AsyncState as StreamInfo;
            try
            {
                int count = stream.Stream.EndRead(ar);
                //read(stream, count);
                Transport.form.Invoke(readHandler, new object[] {stream, count});
                //Dispatcher.CurrentDispatcher.BeginInvoke(Transport.form.readHandler, new object[] {stream, count});
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
            var listener = ar.AsyncState as ListenerInfo;
            try
            {
                TcpClient client = listener.Listener.EndAcceptTcpClient(ar);
                var stream = new StreamInfo();
                stream.Type = listener.Type;
                stream.InputBuffer = new byte[(stream.Type == ClientType.Application) ? 2048 : 1024];
                stream.Client = client;
                stream.Client.ReceiveTimeout = 5000;
                stream.Client.SendTimeout = 5000;
                var ip = stream.Client.Client.RemoteEndPoint as IPEndPoint;
                Int32 address = ip.Address.GetHashCode();
                Int32 port = ip.Port;
                stream.Key = (((UInt64) address) << 32) + (UInt64) port;
                stream.Stream = stream.Client.GetStream();
                if (stream.Type == ClientType.Application)
                {
                    var splice = new ApplicationInputSpliceBuffer(2048);
                    stream.Tag = splice;
                    splice.PacketReceived += processApplicationPacket;
                }
                //accept(stream);
                //Dispatcher.CurrentDispatcher.BeginInvoke(acceptHandler, new object[] {stream});
                //TransportForm.BeginInvoke(acceptHandler, new object[] { stream });
                Transport.form.Invoke(acceptHandler, new object[] {stream});
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
                var copy = new byte[count];
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

        public void error(StreamInfo stream, String str)
        {
            //if (Dispatcher.CurrentDispatcher.CheckAccess())
            //{
            //    error(stream, str);
            //    Dispatcher.CurrentDispatcher.BeginInvoke(errorHandler, new object[] { stream, str });
            //    return;
            //}
            if (Transport.form.InvokeRequired)
            {
                Transport.form.Invoke(errorHandler, new object[] {stream, str});
                return;
            }
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
                if ((stream.Type == ClientType.Autodrome) || (stream.Type == ClientType.Navigation))
                {
                    var packet = new byte[12];
                    BitConverter.GetBytes(stream.Key).CopyTo(packet, 0);
                    BitConverter.GetBytes(0).CopyTo(packet, 8);
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
                    try
                    {
                        Transport.form.After_error_update(stream, str);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception error: {0}", e);
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
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception error: {0}", e);
            }
        }

        public void availabilityTimer_Tick(object sender, EventArgs e)
        {
            //if (Dispatcher.CurrentDispatcher.CheckAccess())
            //{
            //    Dispatcher.CurrentDispatcher.BeginInvoke(availableHandler, new object[] { sender, e });
            //    return;
            //}
            if (Transport.form.InvokeRequired)
            {
                Transport.form.Invoke(availableHandler, new[] {sender, e});
                return;
            }
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

        public void read(StreamInfo stream, int count)
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
                            LogManager.DoLogging(output_buffer, count);
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
                        var splice = stream.Tag as ApplicationInputSpliceBuffer;
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
                                client.Write((byte[]) stream.InputBuffer.Clone(), 0, count);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Exception write to navigation: {0}", e);
                                error(client, "write to navigation failed");
                            }
                        }
                        break;
                    default:
                        MessageBox.Show("undefined stream read");
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception read: {0}", e);
                error(stream, "read failed");
            }
        }

        private delegate void AcceptHandler(StreamInfo stream);

        private delegate void ErrorHandler(StreamInfo stream, String str);

        private delegate void ReadHandler(StreamInfo stream, int count);
    }
}