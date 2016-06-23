using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;

namespace SortConsole
{
    public abstract class BaiduBase
    {
        public string Keyword { get; set; }

        public string MatchUrl { get; set; }

        public HttpUtil Http { get; set; }

        public static CookieCollection Cookies = new CookieCollection();


        public string Html { get; protected set; }

        public BdComm BdComm { get; set; }

        public string Url { get; set; }

        public string EncodeUrl { get; set; }

        public BaiduBase() {
            InitHttp();
        }

        public void  GetResult()
        {
            if (string.IsNullOrEmpty(Url))
                return;
            Html = Http.GetHtml(Url);
            EncodeUrl = Http.ResponseUri.AbsoluteUri;
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
            Http = new HttpUtil();
            Http.CookieContainer.Add(Cookies);
            Http.Encoding = Encoding.UTF8;
        }

        public HttpUtil CreateHttp()
        {
            HttpUtil http = new HttpUtil();
            http.CookieContainer.Add(Cookies);
            http.Encoding = Encoding.UTF8;
            return http;
        }

        public string EncodeKeyword()
        {
            return StringUtil.UrlEncode(Keyword, Encoding.GetEncoding("GB2312"));
        }

        public bool InitBdComm()
        {
            BdComm = new BdComm();
            BdComm.path = StringUtil.UrlEncode(EncodeUrl);

            Match cidMatch = RegexUitl.cidReg.Match(Html);
            if (cidMatch.Success)
            {
                BdComm.cid = cidMatch.Groups[1].Value.Trim();
            }

            Match qidMatch = RegexUitl.qidReg.Match(Html);
            if (qidMatch.Success)
            {
                BdComm.qid = qidMatch.Groups[1].Value.Trim();
            }

            Match usburlMatch = RegexUitl.ubsurlReg.Match(Html);
            if (usburlMatch.Success==false)
            {
                return false;
            }
            BdComm.usburl = usburlMatch.Groups[1].Value.Trim();

            Match nsclickMatch = RegexUitl.nsclickReg.Match(Html);
            if (nsclickMatch.Success==false)
            {
                return false;
            }
            BdComm.nsclickurl = nsclickMatch.Groups[1].Value.Trim();

            Match tnMatch = RegexUitl.tnReg.Match(Html);
            if (tnMatch.Success)
                BdComm.tn = tnMatch.Groups[1].Value.Trim();

            Match cgifMatch = RegexUitl.cgifReg.Match(Html);
            if (cgifMatch.Success)
                BdComm.cgif = cgifMatch.Groups[1].Value.Trim();

            Match sidMatch = RegexUitl.sidReg.Match(Html);
            if (sidMatch.Success)
                BdComm.sid = sidMatch.Groups[1].Value.Trim();

            Match seInfoMatch = RegexUitl.seInfoReg.Match(Html);
            if (!seInfoMatch.Success)
            {
                return false;
            }
            string[] arrs = seInfoMatch.Groups[1].Value.Trim().Split(',');
            BdComm.SeInfo = new SeInfo();
            foreach (string item in arrs)
            {
                string[] items = item.Split(':');
                if (items.Length != 2)
                    continue;
                switch (items[0].Trim().Trim('\''))
                {
                    case "fm": BdComm.SeInfo.fm = items[1].Trim().Trim('\''); break;
                    case "T": BdComm.SeInfo.T = items[1].Trim().Trim('\''); break;
                    case "y": BdComm.SeInfo.y = items[1].Trim().Trim('\''); break;
                    case "rsv_cache": BdComm.SeInfo.rsv_cache = "0"; break;
                }
            }
            MatchCollection matches = RegexUitl.baiduReg.Matches(Html);
            StringBuilder str=new StringBuilder();
            foreach (Match item in matches)
            {
                int id=int.Parse(item.Groups[1].Value);
                if (item.Value.IndexOf("general_image_pic") > 0)
                {
                    BdComm.SeInfo.rehDic.Add(id,109);
                    continue;
                }
                Match muMatch = RegexUitl.muReg.Match(item.Value);
                if (muMatch.Success)
                {
                    if (muMatch.Groups[1].Value.IndexOf("image.baidu.com")>0)
                    {
                        BdComm.SeInfo.rehDic.Add(id,152);
                        continue;
                    }
                    if (muMatch.Groups[1].Value.IndexOf("tieba.baidu.com") > 0)
                    {
                        BdComm.SeInfo.rehDic.Add(id,162);
                        continue;
                    }
                }
                BdComm.SeInfo.rehDic.Add(id, 85);
            }

            if (Cookies["BIDUPSID"] != null)
                BdComm.SeInfo.rsv_psid = Cookies["BIDUPSID"].Value;
            if (Cookies["PSTM"] != null)
                BdComm.SeInfo.rsv_pstm = Cookies["PSTM"].Value;
            return true;
        }

        public abstract void Process();

    }
}
