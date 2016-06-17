using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace Sort
{
    public partial class Form3 : Form
    {
        string sid = "";
        System.Net.CookieCollection cookies = new System.Net.CookieCollection();

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            //System.Threading.Thread.Sleep(2000);

            //System.Diagnostics.Process.Start("一键清理系统.bat");
            
            //System.Diagnostics.Process.Start("sort.exe");
            //Application.Exit();

            //HttpHelper http = new HttpHelper();
            //http.GetHtml("https://www.baidu.com");
            //cookies = http.Cookies;

            //His();

            Index bd = new Index();
            bd.Keyword = "seo";
            bd.MatchUrl = "seo.chinaz.com";
            bd.NextHandler += new Action<string,string,string>(delegate(string html,string keyword,string matchUrl) {
                Find find = new Find(html,keyword,matchUrl);
                find.Process();
            });
            bd.Process();
        }

        void His()
        {
            string url = string.Concat("https://www.baidu.com/his?wd=&from=pc_web&hisdata=&json=1&p=3&sid=", cookies["H_PS_PSSID"].Value, "&csor=0&cb=jQuery1102007811810495217641_",GetUNIX_TIMESTAMP(),"&_="+GetUNIX_TIMESTAMP());
            HttpHelper http = new HttpHelper();
            http.GetHtml(url);
        }
        public static long GetUNIX_TIMESTAMP()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }
    }
}
