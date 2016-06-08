using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;
using System.Diagnostics;
using DotRas;

namespace Sort
{
    public partial class Form1 : Form
    {
        WebBrowser wb = null;

        Random ran = new Random();

        int page = 1;
        int count = 0;
        Step step = Step.One;
        bool isProcessOk = true;

        string kwPath = AppDomain.CurrentDomain.BaseDirectory + "kw.txt";
        List<Keyword> list = new List<Keyword>();

        Keyword keyword = null;

        bool isConn = false;

        /// <summary>
        /// Defines the name of the entry being used by the example.
        /// </summary>
        public const string EntryName = "宽带连接";

        /// <summary>
        /// Holds a value containing the handle used by the connection that was dialed.
        /// </summary>
        private RasHandle handle = null;

        System.Timers.Timer timer = new System.Timers.Timer(20000);

        private RasPhoneBook AllUsersPhoneBook = new RasPhoneBook();
        private RasDialer Dialer = new RasDialer();

        [DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetProcessWorkingSetSize(IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetCurrentProcess", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr GetCurrentProcess();

        public Form1()
        {
            KillProcess();

            InitializeComponent();


            timer.Elapsed += timer_Elapsed;
            timer.AutoReset = false;

            this.Dialer.Timeout = 20000;
            this.Dialer.Credentials = null;
            this.Dialer.EapOptions = new DotRas.RasEapOptions(false, false, false);
            this.Dialer.HangUpPollingInterval = 0;
            this.Dialer.Options = new DotRas.RasDialOptions(false, false, false, false, false, false, false, false, false, false);
            this.Dialer.SynchronizingObject = this;
            //this.Dialer.StateChanged += new System.EventHandler<DotRas.StateChangedEventArgs>(this.Dialer_StateChanged);
            this.Dialer.DialCompleted += new System.EventHandler<DotRas.DialCompletedEventArgs>(this.Dialer_DialCompleted);
            string path = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
            //创建VPN
            this.AllUsersPhoneBook.Open(path);

            if (!this.AllUsersPhoneBook.Entries.Contains(EntryName))
            {
                RasEntry entry = RasEntry.CreateBroadbandEntry(EntryName,
                    RasDevice.GetDeviceByName("(PPPoE)", RasDeviceType.PPPoE));

                this.AllUsersPhoneBook.Entries.Add(entry);

            }



            if (File.Exists(kwPath))
            {
                using (StreamReader rd = new StreamReader(kwPath))
                {
                    string txt = rd.ReadToEnd();
                    string[] arr = txt.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string item in arr)
                    {
                        string[] arrs = item.Split(',');
                        if (arrs.Length != 3)
                            continue;
                        Keyword keyword = new Keyword();
                        keyword.kw = arrs[0];
                        keyword.url = arrs[1];
                        keyword.total = int.Parse(arrs[2]);
                        list.Add(keyword);
                    }
                }
            }

            #region
            var action = new Action(delegate()
            {
                while (true)
                {
                    foreach (Keyword item in list)
                    {
                        try
                        {
                            while (true)
                            {
                                if (isProcessOk == false)
                                {
                                    System.Threading.Thread.Sleep(1000);
                                    continue;
                                }
                                this.textBox2.Text = string.Empty;
                                ShowMsg("whileprocess");
                                isProcessOk = false;
                                break;
                            }
                            keyword = item;
                            if (keyword.showcount <= 0)
                            {
                                System.Threading.Thread.Sleep(1000);
                                continue;
                            }
                            System.Threading.Thread.Sleep(ran.Next(3, 9) * 1000);
#if !DEBUG

                            while (true)
                            {
                                if (isConn == false)
                                {
                                    ShowMsg("拨号中---" + keyword.kw);
                                    Dial();
                                    System.Threading.Thread.Sleep(1000);
                                    continue;
                                }
                                break;
                            }
#endif
                            if (this.InvokeRequired)
                            {
                                this.Invoke(new Action(delegate()
                                {
                                    if (wb != null && wb.IsDisposed == false)
                                    {
                                        ShowMsg("回收");
                                        wb.Dispose();
                                        wb = null;
                                        GC.Collect();
                                        GC.WaitForPendingFinalizers();
                                        IntPtr pHandle = GetCurrentProcess();
                                        SetProcessWorkingSetSize(pHandle, -1, -1);
                                    }
                                    wb = new WebBrowser();
                                    wb.ScriptErrorsSuppressed = true;
                                    wb.Navigating += wb_Navigating;
                                    wb.DocumentCompleted += wb_DocumentCompleted;

                                    wb.Navigate("https://www.baidu.com");
                                }));
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowMsg(ex.Message);
                        }
                    }
                }
            });
            action.BeginInvoke(null, null);
            #endregion
        }

        void timer_Elapsed(object sender, EventArgs e)
        {
            ShowMsg("timer_Elapsed");
            timer.Stop();
            Disconn();
            InitParam();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            
        }
        
        void wb_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            ShowMsg("wb_Navigating");
            if (e.Url.ToString().IndexOf("res://") == -1)
            {
                timer.Start();
            }
            else {
                timer.Stop();
                Disconn();
                InitParam();
            }
        }

        void Dial()
        {
            this.Dialer.EntryName = EntryName;
            this.Dialer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
            try
            {
                if (this.Dialer.IsBusy)
                    return;
                // Set the credentials the dialer should use.
                this.Dialer.Credentials = new NetworkCredential("99392214", "123456");

                // NOTE: The entry MUST be in the phone book before the connection can be dialed.
                // Begin dialing the connection; this will raise events from the dialer instance.
                this.handle = this.Dialer.DialAsync();

            }
            catch (Exception ex)
            {
                ShowMsg(ex.ToString());
            }
        }

        void Disconn()
        {
#if !DEBUG
            if (this.Dialer.IsBusy)
            {
                // The connection attempt has not been completed, cancel the attempt.
                this.Dialer.DialAsyncCancel();
            }
            else
            {
                // The connection attempt has completed, attempt to find the connection in the active connections.
                RasConnection connection = RasConnection.GetActiveConnectionByHandle(this.handle);
                if (connection != null)
                {
                    // The connection has been found, disconnect it.
                    connection.HangUp();
                }
            }
            isConn = false;
#endif
        }

        /// <summary>
        /// Occurs when the dialer has completed a dialing operation.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="DotRas.DialCompletedEventArgs"/> containing event data.</param>
        private void Dialer_DialCompleted(object sender, DialCompletedEventArgs e)
        {
            if (e.Connected)
            {
                isConn = true;
            }
            else
            {
                isConn = false;
            }
        }

        private void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            timer.Stop();
            ShowMsg("wb_DocumentCompleted");
            if (step == Step.One)
            {
                Init();
                return;
            }

            if (step == Step.Two)
            {
                FindCurrnt();
                return;
            }
            if (step == Step.Two)
            {
                FindCurrnt();
                return;
            }

            if (step == Step.Three)
            {
                StepThree();
            }
        }

        void InitParam()
        {
            step = Step.One;
            isProcessOk = true;
            this.page = 1;
        }

        void Init()
        {
            ShowMsg("Init");
            var action = new Action(delegate()
            {
                System.Threading.Thread.Sleep(ran.Next(2, 4) * 1000);
                if (this.wb.InvokeRequired)
                {
                    this.wb.Invoke(new Action(delegate()
                    {
                        HtmlDocument doc = wb.Document;
                        HtmlElement kw = doc.GetElementById("kw");
                        HtmlElement su = doc.GetElementById("su"); 
                        //kw.Focus();
                        kw.InnerText = keyword.kw;
                        step = Step.Two;
                        su.InvokeMember("click");
                        return;
                    }))
                    ;
                }
            });
            action.BeginInvoke(null, null);
        }

        void FindCurrnt()
        {
            ShowMsg("FindCurrnt");
            if (step == Step.Two)//获取当前页
            {
                var action = new Action(delegate()
                {
                    System.Threading.Thread.Sleep(ran.Next(2, 5) * 1000);
                    if (this.wb.InvokeRequired)
                    {
                        this.wb.Invoke(new Action(delegate()
                        {
                            HtmlDocument doc = wb.Document;
                            HtmlElementCollection els = doc.GetElementsByTagName("a");
                            foreach (HtmlElement el in els)
                            {
                                if (el.OuterHtml.IndexOf("c-showurl") > 0)
                                {
                                    if (el.InnerText.StartsWith(keyword.url, StringComparison.OrdinalIgnoreCase))
                                    {
                                        el.ScrollIntoView(false);
                                        //System.Threading.Thread.Sleep(5000);
                                        //el.SetAttribute("target", "_self");
                                        //el.InvokeMember("click");
                                        //isExists = true;
                                        count++;
                                        bool isShow = false;
#if !DEBUG
                                        if (ran.Next(0, 5)== 1)
                                        {
#endif
                                            if (keyword.total > 0)
                                            {
                                                HtmlElement divParent = el.Parent.Parent;
                                                VisitWgif(divParent,el, doc);
                                            }
#if !DEBUG
                                            else
                                                isShow = true;
                                        }
                                        else
                                            isShow = true;
#endif
                                        if (isShow)
                                        {
                                            keyword.showcount--;
                                            Disconn();
                                            InitParam();

                                            ShowMsg(string.Format("{3}，剩余刷 {0} 次，剩余展示 {1} 次，总共 {2} 次", keyword.total, keyword.showcount, count, keyword.kw));
                                        }
                                        
                                        return;
                                    }
                                }
                            }
                            Page();
                        }));

                    }
                });
                action.BeginInvoke(null, null);
            }
        }

        void Page()
        {
            ShowMsg("Page");
            var action = new Action(delegate()
            {
                System.Threading.Thread.Sleep(ran.Next(3, 5) * 1000);
                if (this.wb.InvokeRequired)
                {
                    this.wb.Invoke(new Action(delegate()
                    {
                        HtmlDocument doc = wb.Document;
                        HtmlElement page = doc.GetElementById("page");
                        HtmlElementCollection els = page.GetElementsByTagName("a");
                        foreach (HtmlElement el in els)
                        {
                            foreach (HtmlElement cl in el.Children)
                            {
                                if (cl.InnerText == (this.page + 1).ToString())
                                {
                                    cl.ScrollIntoView(false);
                                    this.page++;
                                    if (this.page > 5)
                                    {
                                        Disconn();
                                        InitParam();
                                        return;
                                    }
                                    step = Step.Two;
                                    el.InvokeMember("click");
                                    return;
                                }
                            }
                        }
                    }));
                }
            });
            action.BeginInvoke(null, null);
        }

        void StepThree()
        { 
            var action = new Action(delegate()
            {
                System.Threading.Thread.Sleep(ran.Next(3, 5) * 1000);
                keyword.total--;
                keyword.showcount--;
                Disconn();
                InitParam();

                ShowMsg(string.Format("{3}，剩余刷 {0} 次，剩余展示 {1} 次，总共 {2} 次", keyword.total, keyword.showcount, count, keyword.kw));
            });
            action.BeginInvoke(null, null);
        }

        void VisitWgif(HtmlElement el,HtmlElement currentEl, HtmlDocument doc)
        {
            try
            {
                ShowMsg("VisitWgif");
                ParamModel pm = new ParamModel();
                pm.q = keyword.kw;
                pm.rsv_xpath = "h3-a(title)";
                pm.rsv_height = "107";
                pm.rsv_width = "538";
                pm.rsv_tpl = el.GetAttribute("tpl");
                pm.p1 = el.GetAttribute("id");
                pm.rsv_srcid = el.GetAttribute("srcid");

                string divClickStr = el.GetAttribute("data-click");
                divClick divClick = LitJson.JsonMapper.ToObject<divClick>(divClickStr);
                pm.divClick = divClick;

                HtmlElement h3_a = el.FirstChild.FirstChild;
                string h3ClickStr = h3_a.GetAttribute("data-click");
                h3Click h3Click = LitJson.JsonMapper.ToObject<h3Click>(h3ClickStr);
                pm.h3Click = h3Click;

                pm.fm = "as";
                pm.rsv_sid = doc.InvokeScript("eval", new String[] { "bds.comm.sid" }).ToString();
                pm.cid = "0";

                pm.url = h3_a.GetAttribute("href");

                pm.title = h3_a.InnerText;

                pm.qid = doc.InvokeScript("eval", new String[] { "bds.comm.qid" }).ToString();

                pm.t = GetUNIX_TIMESTAMP().ToString();
                pm.rsv_cftime = "1";
                pm.rsv_iorr = "0";
                pm.rsv_tn = doc.InvokeScript("eval", new String[] { "bds.comm.tn" }).ToString();
                pm.rsv_isid = pm.rsv_sid;
                pm.rsv_ssl = "1";
                pm.path = doc.Url.ToString();

                pm.usburl = doc.InvokeScript("eval", new String[] { "bds.comm.ubsurl" }).ToString();

                pm.cookie = doc.Cookie;
                var gifAction = new Action(delegate()
                {
                    HttpHelper http = new HttpHelper();
                    System.Net.CookieCollection cookies = HttpHelper.StringToCookieCollection(pm.cookie, ".baidu.com");

                    http.CookieContainer.Add(cookies);
                    http.Referer = pm.path;
                    http.Accept = "image/webp,image/*,*/*;q=0.8";
                    string url = string.Concat(pm.usburl, "?", pm.ToString());
                    ShowMsg(url);
                    http.GetHtml(url);

                    http = new HttpHelper();
                    http.Referer = pm.path;
                    http.CookieContainer.Add(cookies);
                    http.GetHtml(pm.url);

                    currentEl.InvokeMember("click");

                    step = Step.Three;
                    
                });
                gifAction.BeginInvoke(null, null);
            }
            catch (Exception ex)
            {
                LogUtil.Write("VisitWgif:"+ex.Message);
            }
        }

        void ShowMsg(string msg)
        {
            if (this.InvokeRequired)
            {
                this.textBox2.Invoke(new Action<string>(delegate(string mm)
                {
                    this.textBox2.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n\r\n" + mm + "\r\n\r\n");
                    }), msg);
            }
            else
            {
                this.textBox2.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n\r\n" + msg + "\r\n\r\n");
               
            }
        }
        public static long GetUNIX_TIMESTAMP()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            this.textBox2.SelectionStart = this.textBox2.Text.Length;
            this.textBox2.SelectionLength = 0;
            this.textBox2.ScrollToCaret();
        }

        private void KillProcess()
        {
            Process[] myproc = Process.GetProcesses();
            Process current = Process.GetCurrentProcess(); 
            foreach (Process item in myproc)
            {

                if (string.Compare(item.ProcessName, current.ProcessName, true) == 0)
                {
                    if (item.Id != current.Id)
                    {
                        item.Kill();
                        break;
                    }
                }
            }
        }
    }

    public enum Step
    { 
        One,

        Two,

        Three,

        Four
    }

    public class Keyword
    {
        static Random random = new Random();

        public string kw { get; set; }

        public string url { get; set; }

        /// <summary>
        /// 刷的次数
        /// </summary>
        public int total { get; set; }

        private int? m_showcount;

        public int showcount
        {
            get {
                if (m_showcount == null)
                    m_showcount = total * random.Next(5, 20);
                return m_showcount.Value;
            }
            set { m_showcount = value; }
        }
    }
}
