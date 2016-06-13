using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using DotRas;
using System.Runtime.InteropServices;
using System.IO;

namespace Sort
{
    public partial class Form2 : Form
    {
        string keyword = "天天乐乐";
        string url = "www.7edown.com";

        Step step = Step.One;

        bool isConn = false;

        object isConnObj = new object();

        bool isProcess = false;

        int page = 1;

        Random ran = new Random();

        /// <summary>
        /// Defines the name of the entry being used by the example.
        /// </summary>
        public const string EntryName = "宽带连接";

        /// <summary>
        /// Holds a value containing the handle used by the connection that was dialed.
        /// </summary>
        private RasHandle handle = null;

        System.Timers.Timer timer = new System.Timers.Timer(15000);

        private RasPhoneBook AllUsersPhoneBook = new RasPhoneBook();
        private RasDialer Dialer = new RasDialer();

        [DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetProcessWorkingSetSize(IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetCurrentProcess", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr GetCurrentProcess();

        public Form2()
        {
            InitializeComponent();
            KillMySelf();

            timer.Elapsed += timer_Elapsed;
            timer.AutoReset = false;

            
        }

        private void Form2_Load(object sender, EventArgs e)
        {
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

            var action = new Action(delegate()
            {
                while (true)
                {
                    try
                    {
                        if (isProcess)
                        {
                            System.Threading.Thread.Sleep(200);
                            continue;
                        }
                        this.isProcess=true;
                        this.page = 1;
                        this.step = Step.One;
#if !DEBUG

                        while (true)
                        {
                            lock (isConnObj)
                            {
                                if (isConn == false)
                                {
                                    Dial();
                                    System.Threading.Thread.Sleep(200);
                                    continue;
                                }
                            }
                            break;
                        }
                        //if (isConn == false)
                        //{
                        //    Dial();
                        //    //System.Threading.Thread.Sleep(200);
                        //    //continue;
                        //}
#endif


                        //List<string> paths = new List<string>() { 
                        //@"C:\Documents and Settings\Administrator\Cookies",
                        //Environment.GetFolderPath(Environment.SpecialFolder.InternetCache),
                        //@"C:\Documents and Settings\Administrator\UserData",
                        //@"C:\Documents and Settings\Administrator\Local Settings\Application Data\Microsoft\Internet Explorer"

                        //};

                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(delegate()
                            {
                                if (webBrowser1 != null && webBrowser1.IsDisposed == false)
                                {
                                    webBrowser1.Dispose();
                                    this.Controls.RemoveByKey("webBrowser1");
                                    GC.Collect();
                                    GC.WaitForPendingFinalizers();
                                    IntPtr pHandle = GetCurrentProcess();
                                    SetProcessWorkingSetSize(pHandle, -1, -1);



                                    //foreach (string item in paths)
                                    //{
                                        
                                    //        DirectoryInfo dir = new DirectoryInfo(item);
                                    //        DirectoryInfo[] infos = dir.GetDirectories();
                                    //        foreach (DirectoryInfo info in infos)
                                    //        {
                                    //            try
                                    //            {
                                    //                Directory.Delete(info.FullName, true);
                                    //            }
                                    //            catch (Exception ex) { LogUtil.Write(ex.Message); }
                                    //        }

                                    //        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);
                                    //        foreach (FileInfo file in files)
                                    //        {
                                    //            try
                                    //            {
                                    //                File.Delete(file.FullName);
                                    //            }
                                    //            catch (Exception ex) { LogUtil.Write(ex.Message); }
                                    //        }
                                    //}

                                }
                                this.SuspendLayout();
                                webBrowser1 = new WebBrowser();
                                //webBrowser1.Location = new System.Drawing.Point(0, 0);
                                //webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
                                //webBrowser1.Name = "webBrowser1";
                                //webBrowser1.Size = new System.Drawing.Size(884, 528);
                                webBrowser1.Size = this.ClientSize;
                                webBrowser1.ScriptErrorsSuppressed = true;
                                webBrowser1.Navigating += webBrowser1_Navigating;
                                webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
                                this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
                                this.webBrowser1.Name = "webBrowser1";
                                this.Controls.Add(this.webBrowser1);
                                this.ResumeLayout(false);
                                webBrowser1.Navigate("https://www.baidu.com");
                            }));

                        }
                    }
                    catch (Exception ex)
                    {
                        this.isProcess=false;
                        LogUtil.Write(ex.Message);
                    }
                }

            });
            action.BeginInvoke(null,null);

        }
        void timer_Elapsed(object sender, EventArgs e)
        {
            LogUtil.Write("timer_Elapsed");
            timer.Stop();
            Disconn();
            isProcess = false;
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //LogUtil.Write("webBrowser1_DocumentCompleted");
            timer.Stop();
            if (step == Step.One)
                Init();
            else if (step == Step.Two)
                FindCurrnt();
        }
        void Init()
        {
            //if (this.webBrowser1.InvokeRequired)
            //{
            //    this.webBrowser1.Invoke(new Action(delegate()
            //    {
            //        HtmlDocument doc = this.webBrowser1.Document;
            //        HtmlElement kw = doc.GetElementById("kw");
            //        HtmlElement su = doc.GetElementById("su");
            //        kw.InnerText = keyword;
            //        step = Step.Two;
            //        su.InvokeMember("click");
            //        return;
            //    }))
            //    ;
            //}
            //else
            //{
                HtmlDocument doc = this.webBrowser1.Document;
                HtmlElement kw = doc.GetElementById("kw");
                HtmlElement su = doc.GetElementById("su");
                if (doc == null || kw == null || su == null)
                {
                    Disconn();
                    isProcess = false;
                    return;
                }
                kw.InnerText = keyword;
                step = Step.Two;
                su.InvokeMember("click");
            
            //}
        }

        void FindCurrnt()
        {
            var action = new Action(delegate()
            {
                System.Threading.Thread.Sleep(ran.Next(2, 4) * 1000);
                if (this.webBrowser1.InvokeRequired)
                {
                    this.webBrowser1.Invoke(new Action(delegate()
                    {
                        HtmlDocument doc = webBrowser1.Document;
                        HtmlElementCollection els = doc.GetElementsByTagName("a");
                        foreach (HtmlElement el in els)
                        {
                            if (el.OuterHtml.IndexOf("c-showurl") > 0)
                            {
                                if (el.InnerText.StartsWith(url, StringComparison.OrdinalIgnoreCase))
                                {
                                    el.ScrollIntoView(false);
#if !DEBUG
                                    if (ran.Next(0, 1) == 0)
                                    {
#endif
                                        HtmlElement divParent = el.Parent.Parent;
                                        while (true)
                                        {
                                            if (divParent == null)
                                            {
                                                isProcess = false;
                                                return;
                                            }
                                            string id = divParent.GetAttribute("id");
                                            System.Text.RegularExpressions.Regex idReg = new System.Text.RegularExpressions.Regex(@"\d{1,2}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                                            if (idReg.IsMatch(id))
                                                break;
                                            divParent = divParent.Parent;
                                        }

                                        VisitWgif(divParent, el, doc);
#if !DEBUG
                                    }
                                    else
                                    {
                                        Disconn();
                                        isProcess = false;
                                    }
                            
#endif
                                    return;
                                }
                            }
                        }
                        Page();
                    }));

                }
            });
            action.BeginInvoke(null,null);
        }

        void Page()
        {
            var action = new Action(delegate()
           {
               System.Threading.Thread.Sleep(ran.Next(2, 4) * 1000);
               if (this.webBrowser1.InvokeRequired)
               {
                   this.webBrowser1.Invoke(new Action(delegate()
                   {
                       HtmlDocument doc = webBrowser1.Document;
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
                                       isProcess = false;
                                       return;
                                   }
                                   step = Step.Two;
                                   el.InvokeMember("click");
                                   return;
                               }
                           }
                       }
                       Disconn();
                       isProcess = false;
                   }));

               }
           });
            action.BeginInvoke(null,null);
        }

        void VisitWgif(HtmlElement el, HtmlElement currentEl, HtmlDocument doc)
        {
            try
            {
                ParamModel pm = new ParamModel();
                pm.q = keyword;
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
                    //currentEl.InvokeMember("click");

                    HttpHelper http = new HttpHelper();
                    System.Net.CookieCollection cookies = HttpHelper.StringToCookieCollection(pm.cookie, ".baidu.com");
                    LogUtil.Write(pm.cookie);
                    http.CookieContainer.Add(cookies);
                    http.Referer = pm.path;
                    http.Accept = "image/webp,image/*,*/*;q=0.8";
                    string url = string.Concat(pm.usburl, "?", pm.ToString());
                    http.GetHtml(url);

                    http = new HttpHelper();
                    http.CookieContainer.Add(cookies);
                    http.Referer = pm.path;
                    url = string.Concat("https://sp2.baidu.com/8LUYsjW91Qh3otqbppnN2DJv",
                        "?",
                        pm.url.Substring(pm.url.IndexOf("url=")),
                        "&cb=jQuery110207832295363147099_1465803898681&ie=utf-8&oe=utf-8&format=json&t=", GetUNIX_TIMESTAMP());
                    http.GetHtml(url);

                    http = new HttpHelper();
                    http.CookieContainer.Add(cookies);
                    http.Referer = pm.path;
                    http.GetHtml(pm.url);

                    //System.Threading.Thread.Sleep(ran.Next(1,3)*1000);

                    //KillIe();
#if !DEBUG
                    System.Diagnostics.Process.Start("sort1.exe");

                    Disconn();

                    //isProcess = false;
#endif
                    Application.Exit();



                });
                gifAction.BeginInvoke(null, null);
            }
            catch (Exception ex)
            {
                LogUtil.Write("VisitWgif:" + ex.Message);
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
                this.Dialer.Credentials = new NetworkCredential("14ab1", "735435");

                // NOTE: The entry MUST be in the phone book before the connection can be dialed.
                // Begin dialing the connection; this will raise events from the dialer instance.
                this.handle = this.Dialer.DialAsync();

            }
            catch (Exception ex)
            {
                LogUtil.Write(ex.ToString());

                if (ex.Message.Contains("已经拨了这个连接"))
                    System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory+"系统重启.bat");
            }
        }

        /// <summary>
        /// Occurs when the dialer has completed a dialing operation.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="DotRas.DialCompletedEventArgs"/> containing event data.</param>
        private void Dialer_DialCompleted(object sender, DialCompletedEventArgs e)
        {
            lock (isConnObj)
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
            lock (isConnObj)
            {
                isConn = false;
            }
#endif
        }

        public static long GetUNIX_TIMESTAMP()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        private void KillMySelf()
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

        private void KillIe()
        {
            Process[] myproc = Process.GetProcesses();
            Process current = Process.GetCurrentProcess();
            foreach (Process item in myproc)
            {

                if (string.Compare(item.ProcessName, "iexplore", true) == 0)
                {
                    item.Kill();
                    continue;
                }
            }
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.ToString().IndexOf("res://") == -1)
            {
                timer.Start();
            }
            else
            {
                timer.Stop();
                Disconn();
                isProcess = false;
            }
        }
    }
}
