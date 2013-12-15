using System;
using System.IO.Ports;
using System.Reflection;
using System.Windows.Forms;
using Autodrome.Base;
using Autodrome.Transport.Properties;

//using Autodrome.Transport.Logic;

namespace Autodrome.Transport
{
    public partial class TransportForm : Form
    {
        private readonly Logger LogManager;
        private readonly MainLogic Logic;
        public readonly EventHandler availableHandler;

        //public delegate void AcceptHandler(StreamInfo stream);
        //public AcceptHandler acceptHandler;

        //public delegate void ReadHandler(StreamInfo stream, int count);
        //public ReadHandler readHandler;

        //public delegate void ErrorHandler(StreamInfo stream, String str);
        //public ErrorHandler errorHandler;

        //private TForm getForm<TForm>()
        //where TForm : Form
        //{
        //    return (TForm)Application.OpenForms.OfType<TForm>().FirstOrDefault();
        //}

        public TransportForm()
        {
            LogManager = new Logger(Settings.Default.LogEnabled, Settings.Default.LogPath,
                Settings.Default.LogDaysCount, Settings.Default.AutoStart);
            Logic = new MainLogic(LogManager);
            availableHandler = availabilityTimer_Tick;

            InitializeComponent();
            trayIcon.Icon = Resources.IdleIcon;
        }

        private void TransportForm_Load(object sender, EventArgs e)
        {
            trayIcon.ShowBalloonTip(0, Assembly.GetExecutingAssembly().GetName().Name,
                "Приложение запущено", ToolTipIcon.Info);
            if (LogManager.AutoStart)
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
            try
            {
                Logic.Start(basePortBox.Text);
            }
            catch (Exception)
            {
                trayIcon.ShowBalloonTip(5000, "Error", "GNSS-base port was not opened", ToolTipIcon.Error);
            }

            try
            {
                Logic.createListener(gnssAddressBox.Text, gnssPortBox.Text, ClientType.Navigation);
                Logic.createListener(autodromAddressBox.Text, autodromPortBox.Text, ClientType.Autodrome);
                Logic.createListener(applicationAddressBox.Text, applicationPortBox.Text, ClientType.Application);
            }
            catch (Exception exception)
            {
                trayIcon.ShowBalloonTip(5000, "Execution exception", exception.ToString(), ToolTipIcon.Error);
            }
        }

        public void DataGridUpdate(StreamInfo stream)
        {
            var row = new DataGridViewRow();
            row.CreateCells(dataGridView1);
            row.Tag = stream;
            row.Cells[columnAddress.Index].Value = stream.Client.Client.RemoteEndPoint.ToString();
            row.Cells[columnType.Index].Value = stream.Type.ToString();
            dataGridView1.Rows.Add(row);
        }

        public void After_error_update(StreamInfo stream, String str)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Tag == stream)
                {
                    logView.Items.Add(String.Format("{0}: {1} {2}", new object[]
                    {
                        DateTime.Now.ToLongTimeString(),
                        /*stream.Client.Client.RemoteEndPoint*/row.Cells[columnAddress.Index].Value,
                        str
                    }));
                    dataGridView1.Rows.Remove(row);
                }
            }
        }

        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Visible = !Visible;
            if (Visible)
                Activate();
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
            var box = (ComboBox) sender;
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

        private void TransportForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                var base_serial = Logic.baseStream.Tag as SerialPort;
                if (base_serial != null)
                {
                    base_serial.RtsEnable = false;
                }
            }
            catch (Exception)
            {
             //Application was not launched     
            }
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            Invoke(new EventHandler(update), new object[] {this, EventArgs.Empty});
        }

        private void update(object sender, EventArgs args)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var stream = row.Tag as StreamInfo;
                if (stream != null)
                {
                    var dead = MainLogic.IsClientDead(stream.Client);
                    if (dead)
                    {
                        dataGridView1.Rows.Remove(row);
                        //update(sender, args);
                        return;
                    }
                    Int64 read = stream.BytesRead;
                    Int64 written = stream.BytesWritten;

                    var prev_read = (Int64) row.Cells[columnReading.Index].Tag;
                    var prev_written = (Int64) row.Cells[columnWriting.Index].Tag;

                    row.Cells[columnReading.Index].Value = read;
                    row.Cells[columnReading.Index].Tag = read;
                    row.Cells[columnWriting.Index].Value = written;
                    row.Cells[columnWriting.Index].Tag = written;

                    double step = 0.001*updateTimer.Interval;

                    row.Cells[columnReadBPS.Index].Value = (read - prev_read)*step;
                    row.Cells[columnWriteBPS.Index].Value = (written - prev_written)*step;
                }
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            foreach (StreamInfo s in Logic.applicationStreams)
            {
                s.BytesRead = 0;
                s.BytesWritten = 0;
            }
            foreach (StreamInfo s in Logic.autodromeStreams.Values)
            {
                s.BytesRead = 0;
                s.BytesWritten = 0;
            }
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = 0; i < e.RowCount; i++)
            {
                dataGridView1.Rows[e.RowIndex + i].Cells[columnReading.Index].Tag = (Int64) 0;
                dataGridView1.Rows[e.RowIndex + i].Cells[columnWriting.Index].Tag = (Int64) 0;
            }
        }

        private void availabilityTimer_Tick(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(availableHandler, new[] {sender, e});
                return;
            }
            Logic.availabilityTimer_Tick(sender, e);
        }
    }
}