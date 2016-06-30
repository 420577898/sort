using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortConsole
{
    class Program
    {
        static Random ran = new Random();

        static int count = 0;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            Start();
            
            Console.ReadKey();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                LogUtil.Write(ex.Message + "\t" + ex.StackTrace + "\tcount:" + count);
            }
        }

        static void Start()
        {
#if !DEBUG
            Dial dial = new Dial();
            dial.DailCompletedHandle += new Action(delegate
            {
                Console.WriteLine("连接成功");
                Index bd = new Index();
                bd.Keyword = "帝爵电玩城官网xq66cn";
                bd.MatchUrl = "abc.com";
                bd.ErrorHandler += new Action<string>(delegate(string msg)
                {
                    Console.WriteLine(msg);
                    DialDisconn disconn = new DialDisconn();
                    disconn.DisconnHandle += Start;
                    disconn.Process();
                });
                bd.NextHandler += new Action<string, string, string, string>(delegate(string html, string keyword, string matchUrl, string locationUrl)
                {
                    Console.WriteLine(keyword+"\tOK\tCount:"+count);
                    count++;
                    System.Threading.Thread.Sleep(ran.Next(3,5)*1000);
                    DialDisconn disconn = new DialDisconn();
                    disconn.handle = dial.handle;
                    disconn.DisconnHandle += Start;
                    disconn.Process();
                    Console.WriteLine("已断开");
                });
                bd.Process();
            });
            dial.Process();
#else
            Index bd = new Index();
            bd.Keyword = "360医疗保险";
            bd.MatchUrl = "abc.com";
            bd.NextHandler += new Action<string, string, string, string>(delegate(string html, string keyword, string matchUrl, string locationUrl)
            {
                //System.Threading.Thread.Sleep(ran.Next(1, 3) * 1000);
                //Start();
            });
            bd.Process();

#endif
        }
    }
}
