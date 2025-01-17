using System;
using System.Diagnostics;
using System.Windows.Forms;
using Medicare.Main;

namespace Medicare
{
    static class Program
    {
        static void exceptionDump(object sender, UnhandledExceptionEventArgs args)
        {
            MinidumpHelp.Minidump.install_self_mini_dump();
        }

        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(exceptionDump);

            //Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var w = new CTPMain();
            w.Show();
            Application.Run(w);
        }
    }
}
