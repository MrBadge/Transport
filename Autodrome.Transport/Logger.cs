using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Autodrome.Transport
{
    public class Logger
    {
        public string LogPath { get; set; }
        public bool LogEnabled { get; set; }
        public int LogDaysCount { get; set; }
        public bool AutoStart { get; set; }
        private Stream log { get; set; }
        private DateTime log_start { get; set; }
        private TimeSpan log_span { get; set; }

        public Logger(bool LogEnabled = false, string LogPath = null, int LogDaysCount = 0, bool AutoStart = false)
        {
            this.LogEnabled = LogEnabled;
            this.LogPath = LogPath;
            this.LogDaysCount = LogDaysCount;
            this.AutoStart = AutoStart;

            log_span = new TimeSpan(0, 30, 0);
            if (LogEnabled)
            {
                DateTime now = DateTime.Now;
                TimeSpan keep_span = new TimeSpan(LogDaysCount, 0, 0, 0);
                String log_dir = Path.GetFullPath(LogPath);
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
                catch
                {
                }
            }
        }

        public void InitLog()
        {
            
        }

        public void DoLogging(byte[] output_buffer, int count)
        {
            if (LogEnabled)
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
                    String path = Path.Combine(LogPath,
                        String.Format("{0:yyyyMMdd_HHmmss}.log", log_start));
                    log = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read);
                }
                if (log != null)
                {
                    log.Write(output_buffer, 0, count + 12);
                }
            }  
        }
    }
}
