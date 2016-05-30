using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Sort
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException+=CurrentDomain_UnhandledException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                LogUtil.Write(ex.Message + "\tdsfsdfsdf" + ex.StackTrace);
            }
        }
    }
}
