using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;

namespace Sort
{
    public abstract class BaiduBase
    {
        public string Keyword { get; set; }

        public string MatchUrl { get; set; }

        public HttpHelper Http { get; set; }

        public static CookieCollection Cookies = new CookieCollection();

        public string Html { get; protected set; }

        public string Url { get; set; }

        public BaiduBase() {
            InitHttp();
        }

        public void  GetResult()
        {
            if (string.IsNullOrEmpty(Url))
                return;
            Html = Http.GetHtml(Url);
            MergeCookie(Http.Cookies);
        }

        void MergeCookie(CookieCollection cookies)
        {
            //特殊处理H_PS_645EC
            Match match = RegexUitl.rsvtReg.Match(Html);
            if (match.Success)
            {
                if (Cookies["H_PS_645EC"] != null)
                    Cookies["H_PS_645EC"].Value = match.Groups[1].Value.Trim();
                else
                    Cookies.Add(new Cookie("H_PS_645EC", match.Groups[1].Value.Trim(), "/", "www.baidu.com"));
            }

            foreach (Cookie item in cookies)
            {
                if (item.Expires != null && item.Expires.Year == 1938)
                    item.Expires = DateTime.MaxValue;
                if (Cookies[item.Name] == null)
                    Cookies.Add(item);
                else
                    Cookies[item.Name].Value = item.Value;
            }
        }

        public void ClearCookie()
        {
            Cookies = new CookieCollection();
        }

        public void InitHttp()
        {
            Http = new HttpHelper();
            Http.CookieContainer.Add(Cookies);
            Http.Encoding = Encoding.UTF8;
        }

        public string EncodeKeyword()
        {
            return System.Web.HttpUtility.UrlEncode(Keyword, Encoding.UTF8);
        }

        public abstract void Process();

    }
}
