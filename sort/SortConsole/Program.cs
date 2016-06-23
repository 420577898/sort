using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Index bd = new Index();
            bd.Keyword = "360医疗保险";
            bd.MatchUrl = "seo.chinaz.com";
            bd.NextHandler += new Action<string, string, string,string>(delegate(string html, string keyword, string matchUrl,string locationUrl)
            {
                Find find = new Find(html, keyword, matchUrl,locationUrl);
                find.Process();
            });
            bd.Process();
            Console.ReadKey();
        }
    }
}
