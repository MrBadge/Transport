using System;
using System.Windows.Forms;

namespace Autodrome.Transport
{
    static class Transport
    {
        public static TransportForm form;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form = new TransportForm();
            Application.Run(form);
        }

    }
}