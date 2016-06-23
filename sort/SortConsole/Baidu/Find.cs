using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace SortConsole
{
    public class Find:BaiduBase
    {
        public event Action<string> ErrorHandler;

        public event Action<string> PageHandler;

        public string LocationUrl { get; set; }

        public Find() { }

        public Find(string html, string keyword, string matchUrl, string locationUrl)
        {
            base.Html = html;
            base.Keyword = keyword;
            base.MatchUrl = matchUrl;
            this.LocationUrl = locationUrl;
        }

        public override void Process()
        {
            MatchCollection matches = RegexUitl.baiduReg.Matches(base.Html);
            foreach (Match item in matches)
            {
                Match urlMatch = RegexUitl.urlReg.Match(item.Value);
                if (urlMatch.Success)
                {
                    string url = urlMatch.Groups[1].Value.Trim();
                    url=RegexUitl.htmlReg.Replace(url, "");
                    url = RegexUitl.specialReg.Replace(url, "");
                    int pos = url.IndexOf("/");
                    if (pos == -1)//说明有省略号
                    {
                        pos = url.IndexOf("...");
                        if (pos == -1)
                        {
                            ThrowError("");
                            return;
                        }
                    }
                    else
                        url = url.Substring(0, pos);
                    if (string.Compare(url, MatchUrl, true) == 0)//匹配
                    {
                        Visit(item.Value);
                    }
                }
            }
        }

        void Visit(string html)
        {
            BdParam param = new BdParam();
            param.q = base.EncodeKeyword();
            param.rsv_xpath = "h3-a(title)";
            param.rsv_height = "110";
            param.rsv_width = "538";
            string divHtml = StringUtil.SubString(html, "<", ">");
            param.rsv_srcid=StringUtil.SubString(divHtml,@"srcid=""",@"""");
            param.rsv_tpl = StringUtil.SubString(divHtml, @"tpl=""", @"""");
            param.p1 = StringUtil.SubString(divHtml, @" id=""", @"""");
            string data_click = StringUtil.SubString(divHtml, @"data-click=""{", "}");
            Match match = RegexUitl.h3Reg.Match(html);
            if (match.Success==false)
            {
                ThrowError("");
                return ;
            }

            string h3ClickHtml = StringUtil.SubString(match.Groups[1].Value, @"data-click=""{", "}");
            h3ClickHtml = h3ClickHtml.Replace("\n", "").Replace("\t", "").Replace("\r", "").Trim('"');
            if (!string.IsNullOrEmpty(h3ClickHtml))
                param.h3Click = LitJson.JsonMapper.ToObject<h3Click>("{"+h3ClickHtml+"}");

            Match usburlMatch = RegexUitl.ubsurlReg.Match(base.Html);
            if (usburlMatch.Success == false)
            {
                ThrowError("");
                return;
            }
            param.usburl = usburlMatch.Groups[1].Value.Trim();

            param.url = StringUtil.SubString(match.Groups[1].Value, @"href=""",@"""");
            param.title = StringUtil.SubString(match.Groups[1].Value, @">", @"</a>");
            param.title = StringUtil.UrlEncode(RegexUitl.htmlReg.Replace(param.title, "")).Replace("+","%20");
            param.rsv_sid = Cookies["H_PS_PSSID"] != null ? Cookies["H_PS_PSSID"].Value : "";
            param.cid = "0";
            Match qidMatch = RegexUitl.qidReg.Match(base.Html);
            if (qidMatch.Success)
                param.qid = qidMatch.Groups[1].Value.Trim();
            param.t = StringUtil.GetUNIX_TIMESTAMP().ToString();
            param.rsv_isid = param.rsv_sid;
            param.rsv_cftime = "1";
            param.rsv_iorr = "0";
            param.rsv_ssl = "1";

            Match tnMatch = RegexUitl.tnReg.Match(base.Html);
            if (tnMatch.Success)
                param.rsv_tn = tnMatch.Groups[1].Value.Trim();

            param.path = this.LocationUrl;

            StringBuilder str = new StringBuilder(param.ToString());
            string[] arrs = data_click.Split(',');
            foreach (string item in arrs)
            {
                string[] temp = item.Split(':');
                if (temp.Length == 2)
                {
                    str.Append("&");
                    str.Append(temp[0].Trim().Trim('\''));
                    str.Append("=");
                    str.Append(temp[1].Trim().Trim('\''));
                }
            }

            var usbClick = new Action(delegate()
            {
                string url = string.Concat(param.usburl, "?", str.ToString());
                HttpUtil http= base.CreateHttp();
                http.GetHtml(url);
            });

            var nsClick = new Action(delegate()
            {
                string url = string.Concat(param.usburl, "?", str.ToString());
                HttpUtil http = base.CreateHttp();
                http.GetHtml(url);
            });
            nsClick.BeginInvoke(null, null);

            var urlRec = new Action(delegate()
            {
                HttpUtil http = base.CreateHttp();
                http.GetHtml(param.url);
            });
            urlRec.BeginInvoke(null,null);
            
        }

        void ThrowError(string msg)
        {
            if (ErrorHandler != null)
                ErrorHandler(msg);
        }
    }

    public class BdParam
    {
        public static Random ran = new Random();
        /// <summary>
        /// 关键词 
        /// </summary>
        public string q { get; set; }

        /// <summary>
        /// h3-a(title)
        /// </summary>
        public string rsv_xpath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 例https://www.baidu.com/link?url=mvtE_6Noxmo158w8a_8TmTJExWYSMg-6WpE9hAledMdCIZ-glHuVPbDdoOpPfD6bdecdTINHIyUupPRdnuVq14Qbid04PcRJ66uTg4mZQISrI9Vb65deXzHbjGujbwlE&wd=&eqid=ee8d2d5100050d710000000657344d6f
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 107
        /// </summary>
        public string rsv_height { get; set; }

        /// <summary>
        /// 538
        /// </summary>
        public string rsv_width { get; set; }

        /// <summary>
        /// se_com_default
        /// </summary>
        public string rsv_tpl { get; set; }

        /// <summary>
        /// 排名ID
        /// </summary>
        public string p1 { get; set; }

        public string rsv_srcid { get; set; }

        public h3Click h3Click { get; set; }


        public string rsv_sid { get; set; }

        /// <summary>
        /// 0
        /// </summary>
        public string cid { get; set; }

        public string qid { get; set; }

        public string t { get; set; }

        /// <summary>
        /// 1
        /// </summary>
        public string rsv_cftime { get; set; }

        /// <summary>
        /// 0
        /// </summary>
        public string rsv_iorr { get; set; }

        /// <summary>
        /// baidu
        /// </summary>
        public string rsv_tn { get; set; }

        /// <summary>
        /// 跟rsv_sid 一样
        /// </summary>
        public string rsv_isid { get; set; }

        /// <summary>
        /// 1
        /// </summary>
        public string rsv_ssl { get; set; }

        /// <summary>
        /// 当前页路径
        /// </summary>
        public string path { get; set; }

        public string usburl { get; set; }

        public string nsclickurl { get; set; }

        private string m_rsv_did;
        public string rsv_did
        {
            get {
                if (m_rsv_did == null)
                {
                    string b = "";
                    for (var d = 0; d < 32; d++)
                    {
                        int i = (int)Math.Floor(ran.NextDouble() * 16);
                        b += Convert.ToString(i, 16);
                    }
                    m_rsv_did = b;
                }
                return m_rsv_did;
            }
        }


        public override string ToString()
        {
            return string.Concat("q=" + q,
                "&",
                "rsv_xpath=" + rsv_xpath,
                "&",
                "title=" + title,
                "&",
                "url=" + url,
                "&",
                "rsv_height=" + rsv_height,
                "&",
                "rsv_width=" + rsv_width,
                "&",
                "rsv_tpl=" + rsv_tpl,
                "&",
                "p1=" + p1,
                "&",(h3Click!=null?
                "rsv_srcid=" + this.h3Click.F+
                "&"+
                "F1=" + this.h3Click.F1+
                "&"+
                "F2=" + this.h3Click.F2+
                "&"+
                "F3=" + this.h3Click.F3+
                "&"+
                "T=" + this.h3Click.T+
                "&"+
                "y=" + this.h3Click.y
                :""),
                "rsv_sid=" + rsv_sid,
                "&",
                "cid=" + cid,
                "&",
                "qid=" + qid,
                "&",
                "t=" + t,
                "&",
                "rsv_cftime=" + rsv_cftime,
                "&",
                "rsv_iorr=" + rsv_iorr,
                "&",
                "rsv_tn=" + rsv_tn,
                "&",
                "rsv_isid=" + rsv_isid,
                "&",
                "rsv_ssl=" + rsv_ssl,
                "&",
                "rsv_did="+rsv_did,
                "&",
                "path=" + path);
        }
    }

    public class h3Click
    {
        public string F { get; set; }

        public string F1 { get; set; }

        public string F2 { get; set; }

        public string F3 { get; set; }

        public string T { get; set; }

        public string y { get; set; }
    }
}
