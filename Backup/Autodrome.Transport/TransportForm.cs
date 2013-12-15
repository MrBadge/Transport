using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

using Autodrome.Base;

namespace Autodrome.Transport
{
   public partial class TransportForm : Form
   {
      private StreamInfo baseStream;
      private Dictionary<ulong, StreamInfo> autodromeStreams;
      private List<StreamInfo> navigationStreams;
      private List<StreamInfo> applicationStreams;
      private byte[] output_buffer;
      private Stream log;
      private DateTime log_start;
      private TimeSpan log_span;

      delegate void AcceptHandler(StreamInfo stream);
      private AcceptHandler acceptHandler;

      delegate void ReadHandler(StreamInfo stream, int count);
      private ReadHandler readHandler;

      delegate void ErrorHandler(StreamInfo stream, String str);
      private ErrorHandler errorHandler;

      void createListener(String address, String port, ClientType type)
      {
         ListenerInfo listener = new ListenerInfo();
         listener.Type = type;
         listener.Listener = new TcpListener(IPAddress.Parse(address), Int32.Parse(port));
         listener.Listener.Start();
         listener.Listener.BeginAcceptTcpClient(acceptCallback, listener);
      }

      public TransportForm()
      {
         InitializeComponent();
         trayIcon.Icon = Properties.Resources.IdleIcon;

         autodromeStreams = new Dictionary<ulong, StreamInfo>();
         navigationStreams = new List<StreamInfo>();
         applicationStreams = new List<StreamInfo>();
         output_buffer = new byte[3000];
         log_span = new TimeSpan(0, 30, 0);
         if (Properties.Settings.Default.LogEnabled)
         {
            DateTime now = DateTime.Now;
            TimeSpan keep_span = new TimeSpan(Properties.Settings.Default.LogDaysCount, 0, 0, 0);
            String log_dir = Path.GetFullPath(Properties.Settings.Default.LogPath);
            try
            {
               foreach (String file in Directory.GetFiles(log_dir))
               {
                  if (now.Subtract(File.GetCreationTime(file)) > keep_span)
                  {
                     File.Delete(file);
                  }
               }
            }
            catch { }
         }

         acceptHandler = new AcceptHandler(accept);
         errorHandler = new ErrorHandler(error);
         readHandler = new ReadHandler(read);
         availableHandler = new EventHandler(availabilityTimer_Tick);
      }

      private void TransportForm_Load(object sender, EventArgs e)
      {
         trayIcon.ShowBalloonTip(0, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "Приложение запущено", ToolTipIcon.Info);
         if (Properties.Settings.Default.AutoStart)
         {
            Start();
         }
      }

      private void startButton_Click(object sender, EventArgs e)
      {
         Start();
      }

      private void Start()
      {
         startButton.Enabled = false;
         String[] base_strings = basePortBox.Text.Split(new Char[] { ':' });

         baseStream = new StreamInfo();
         baseStream.Type = ClientType.Base;
         baseStream.InputBuffer = new byte[512];
         try
         {
            if (base_strings.Length == 1)
            {
               SerialPort base_port = new SerialPort(basePortBox.Text);
               base_port.RtsEnable = true;
               base_port.Open();
               baseStream.Tag = base_port;
               baseStream.Stream = base_port.BaseStream;
            }
            else
            {
               TcpClient base_client = new TcpClient(base_strings[0], Int32.Parse(base_strings[1]));
               baseStream.Stream = base_client.GetStream();
            }
         }
         catch (Exception)
         {
            baseStream.Stream = Stream.Null;
            trayIcon.ShowBalloonTip(5000, "Error", "GNSS-base port was not opened", ToolTipIcon.Error);
         }

         baseStream.Stream.BeginRead(baseStream.InputBuffer, 0, baseStream.InputBuffer.Length, readCallback, baseStream);

         try
         {
            createListener(gnssAddressBox.Text, gnssPortBox.Text, ClientType.Navigation);
            createListener(autodromAddressBox.Text, autodromPortBox.Text, ClientType.Autodrome);
            createListener(applicationAddressBox.Text, applicationPortBox.Text, ClientType.Application);
         }
         catch (Exception exception)
         {
            trayIcon.ShowBalloonTip(5000, "Execution exception", exception.ToString(), ToolTipIcon.Error);
         }
      }

      void acceptCallback(IAsyncResult ar)
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
            Invoke(acceptHandler, new object[] { stream });
         }
         catch(Exception e)
         {
            Console.WriteLine("exception acceptCallback: {0}", e);
         }
         listener.Listener.BeginAcceptTcpClient(acceptCallback, listener);
      }

      void accept(StreamInfo stream)
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
         DataGridViewRow row = new DataGridViewRow();
         row.CreateCells(dataGridView1);
         row.Tag = stream;
         row.Cells[columnAddress.Index].Value = stream.Client.Client.RemoteEndPoint.ToString();
         row.Cells[columnType.Index].Value = stream.Type.ToString();
         dataGridView1.Rows.Add(row);
         try
         {
            stream.Stream.BeginRead(stream.InputBuffer, 0, stream.InputBuffer.Length, readCallback, stream);
         }
         catch(Exception e)
         {
            Console.WriteLine("Exception accept: {0}", e);
            error(stream, "accept failed");
         }
      }

      void readCallback(IAsyncResult ar)
      {
         StreamInfo stream = ar.AsyncState as StreamInfo;
         try
         {
            int count = stream.Stream.EndRead(ar);
            Invoke(readHandler, new object[] { stream, count });
            stream.Stream.BeginRead(stream.InputBuffer, 0, stream.InputBuffer.Length, readCallback, stream);
         }
         catch(Exception e)
         {
            Console.WriteLine("Exception readCallback: {0}", e);
            error(stream, "readCallback failed");
         }
      }
      
      void read(StreamInfo stream, int count)
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
                     if (Properties.Settings.Default.LogEnabled)
                     {
                        if (DateTime.Now.Subtract(log_start) > log_span)
                        {
                           if (log != null)
                           {
                              log.Close();
                              log = null;
                           }
                        }
                        if (log == null)
                        {
                           log_start = DateTime.Now;
                           String path = Path.Combine(Properties.Settings.Default.LogPath, String.Format("{0:yyyyMMdd_HHmmss}.log", log_start));
                           log = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read);
                        }
                        if (log != null)
                        {
                           log.Write(output_buffer, 0, count + 12);
                        }
                     }
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
                     catch(Exception e)
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
         catch(Exception e)
         {
            Console.WriteLine("Exception read: {0}", e);
            error(stream, "read failed");
         }
      }

      void processApplicationPacket(object sender, DataEventArgs args)
      {
         ulong key = BitConverter.ToUInt64(args.Data, args.Offset);
         int count = BitConverter.ToInt32(args.Data, args.Offset + 8);
         StreamInfo stream;
         if (autodromeStreams.TryGetValue(key, out stream))
         {
            byte[] copy = new byte[count];
            Array.Copy(args.Data, args.Offset+12, copy, 0, count);
            try
            {
               stream.Write(copy, 0, count);
            }
            catch(Exception e)
            {
               Console.WriteLine("Exception write: {0}", e);
               error(stream, "write failed");
            }
         }
      }

      void error(StreamInfo stream, String str)
      {

         if (InvokeRequired)
         {
            Invoke(errorHandler, new object[] { stream, str });
            return;
         }
         try
         {
            stream.Stream.Close();
            //stream.Client.Client.Disconnect(true);
         }
         catch { }
         try
         {
            logView.Items.Add(String.Format("{0}: {1} {2}", new object[]{
               DateTime.Now.ToLongTimeString(),
               stream.Client.Client.RemoteEndPoint,
               str}));

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
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
               if (row.Tag == stream)
               {
                  dataGridView1.Rows.Remove(row);
               }
            }
         }
         catch (Exception e)
         {
            Console.WriteLine("Exception error: {0}", e);
         }
      }
      
      private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
      {
         Visible = !Visible;
         if (Visible) Activate();
      }

      private void exitMenuItem_Click(object sender, EventArgs e)
      {
         Application.Exit();
      }

      private void serial_DropDown(object sender, EventArgs e)
      {
         basePortBox.Items.Clear();
         foreach (string str in SerialPort.GetPortNames())
         {
            basePortBox.Items.Add(str);
         }
      }

      private void localIP_DropDown(object sender, EventArgs e)
      {
         ComboBox box = (ComboBox)sender;
         box.Items.Clear();
         box.Items.AddRange(SystemUtil.GetLocalAddresses());
      }

      private void TransportForm_Resize(object sender, EventArgs e)
      {
         if (WindowState == FormWindowState.Minimized)
         {
            Visible = false;
         }
      }


      EventHandler availableHandler;      
      private void availabilityTimer_Tick(object sender, EventArgs e)
      {
         if (InvokeRequired)
         {
            Invoke(availableHandler, new object[] { sender, e });
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

      private void TransportForm_FormClosed(object sender, FormClosedEventArgs e)
      {
         SerialPort base_serial = baseStream.Tag as SerialPort;
         if (base_serial != null)
         {
            base_serial.RtsEnable = false;
         }
      }

      private void updateTimer_Tick(object sender, EventArgs e)
      {
         Invoke(new EventHandler(update), new object[] { this, EventArgs.Empty });
      }

      private void update(object sender, EventArgs args)
      {
         foreach (DataGridViewRow row in dataGridView1.Rows)
         {
            StreamInfo stream = row.Tag as StreamInfo;
            if (stream != null)
            {
               Int64 read = stream.BytesRead;
               Int64 written = stream.BytesWritten;

               Int64 prev_read = (Int64)row.Cells[columnReading.Index].Tag;
               Int64 prev_written = (Int64)row.Cells[columnWriting.Index].Tag;

               row.Cells[columnReading.Index].Value = read;
               row.Cells[columnReading.Index].Tag = read;
               row.Cells[columnWriting.Index].Value = written;
               row.Cells[columnWriting.Index].Tag = written;

               double step = 0.001*updateTimer.Interval;

               row.Cells[columnReadBPS.Index].Value = (read-prev_read)*step;
               row.Cells[columnWriteBPS.Index].Value = (written-prev_written)*step;
            }
         }
      }

      private void clearButton_Click(object sender, EventArgs e)
      {
         foreach (StreamInfo s in applicationStreams)
         {
            s.BytesRead = 0;
            s.BytesWritten = 0;
         }
         foreach (StreamInfo s in autodromeStreams.Values)
         {
            s.BytesRead = 0;
            s.BytesWritten = 0;
         }
      }

      private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
      {
         for (int i = 0; i < e.RowCount; i++)
         {
            dataGridView1.Rows[e.RowIndex + i].Cells[columnReading.Index].Tag = (Int64)0;
            dataGridView1.Rows[e.RowIndex + i].Cells[columnWriting.Index].Tag = (Int64)0;
         }
      }
   }
}