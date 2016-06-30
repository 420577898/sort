using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using DotRas;
namespace SortConsole
{
    public class Dial:DialBase
    {

        public event Action DailCompletedHandle;


        public override void Process()
        {
            base.Dialer.EntryName = EntryName;
            base.Dialer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
            try
            {
                if (this.Dialer.IsBusy)
                    return;
                // Set the credentials the dialer should use.
                this.Dialer.Credentials = new NetworkCredential("81310336087", "320520");

                // NOTE: The entry MUST be in the phone book before the connection can be dialed.
                // Begin dialing the connection; this will raise events from the dialer instance.
                this.handle = this.Dialer.Dial();
            }
            catch (Exception ex)
            {
                LogUtil.Write(ex.ToString());

                //if (ex.Message.Contains("已经拨了这个连接"))
                //    System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + "系统重启.bat");
            }
        }

        int tryTimes = 0;
        public override void Dialer_DialCompleted(object sender, DialCompletedEventArgs e)
        {
            Console.WriteLine(e.Connected);
            if (e.Connected)
            {
                tryTimes = 0;
                if (DailCompletedHandle != null)
                    DailCompletedHandle();
            }
            else
            {
                if (tryTimes > 10)
                {
                    LogUtil.Write("拨号重试10次");
                    //    System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + "系统重启.bat");
                }
                else
                {
                    tryTimes++;
                    System.Threading.Thread.Sleep(2000);
                    Process();
                }
            }
        }

    }
}
