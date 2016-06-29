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

        static void Main(string[] args)
        {
            Start();
            
            Console.ReadKey();
        }

        static void Start()
        {
            Dial dial = new Dial();
            dial.DailCompletedHandle += new Action(delegate
            {
                Index bd = new Index();
                bd.Keyword = "360医疗保险";
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
                    DialDisconn disconn = new DialDisconn();
                    disconn.DisconnHandle += Start;
                    disconn.Process();
                });
                bd.Process();
            });
            dial.Process();
            //Index bd = new Index();
            //bd.Keyword = "360医疗保险";
            //bd.MatchUrl = "abc.com";
            //bd.NextHandler += new Action<string, string, string, string>(delegate(string html, string keyword, string matchUrl, string locationUrl)
            //{
            //    System.Threading.Thread.Sleep(ran.Next(1, 3) * 1000);
            //    Start();
            //});
            //bd.Process();
        }
    }
}
