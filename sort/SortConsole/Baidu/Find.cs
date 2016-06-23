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
            //BdParam param = new BdParam();
            //param.q = base.EncodeKeyword();
            //param.rsv_xpath = "h3-a(title)";
            //param.rsv_height = "110";
            //param.rsv_width = "538";
            //string divHtml = StringUtil.SubString(html, "<", ">");
            //param.rsv_srcid=StringUtil.SubString(divHtml,@"srcid=""",@"""");
            //param.rsv_tpl = StringUtil.SubString(divHtml, @"tpl=""", @"""");
            //param.p1 = StringUtil.SubString(divHtml, @" id=""", @"""");
            //string data_click = StringUtil.SubString(divHtml, @"data-click=""{", "}");
            //Match match = RegexUitl.h3Reg.Match(html);
            //if (match.Success==false)
            //{
            //    ThrowError("");
            //    return ;
            //}

            //string h3ClickHtml = StringUtil.SubString(match.Groups[1].Value, @"data-click=""{", "}");
            //h3ClickHtml = h3ClickHtml.Replace("\n", "").Replace("\t", "").Replace("\r", "").Trim('"');
            //if (!string.IsNullOrEmpty(h3ClickHtml))
            //    param.h3Click = LitJson.JsonMapper.ToObject<h3Click>("{"+h3ClickHtml+"}");

            

            //Match urlMatch = RegexUitl.hrefReg.Match(match.Groups[1].Value);
            //if (urlMatch.Success == false)
            //{
            //    ThrowError("");
            //    return;
            //}
            //param.url = urlMatch.Groups[1].Value.Trim();

            //param.title = StringUtil.SubString(match.Groups[1].Value, @">", @"</a>");
            //param.title = StringUtil.UrlEncode(RegexUitl.htmlReg.Replace(param.title, "")).Replace("+","%20");
            //param.rsv_sid = Cookies["H_PS_PSSID"] != null ? Cookies["H_PS_PSSID"].Value : "";
            //param.cid = "0";
            //Match qidMatch = RegexUitl.qidReg.Match(base.Html);
            //if (qidMatch.Success)
            //    param.qid = qidMatch.Groups[1].Value.Trim();
            //param.t = StringUtil.GetUNIX_TIMESTAMP().ToString();
            //param.rsv_isid = param.rsv_sid;
            //param.rsv_cftime = "1";
            //param.rsv_iorr = "0";
            //param.rsv_ssl = "1";

            //Match tnMatch = RegexUitl.tnReg.Match(base.Html);
            //if (tnMatch.Success)
            //    param.rsv_tn = tnMatch.Groups[1].Value.Trim();

            //param.path = this.LocationUrl;

            //StringBuilder str = new StringBuilder(param.ToString());
            //string[] arrs = data_click.Split(',');
            //foreach (string item in arrs)
            //{
            //    string[] temp = item.Split(':');
            //    if (temp.Length == 2)
            //    {
            //        str.Append("&");
            //        str.Append(temp[0].Trim().Trim('\''));
            //        str.Append("=");
            //        str.Append(temp[1].Trim().Trim('\''));
            //    }
            //}

            //var usbClick = new Action(delegate()
            //{
            //    string url = string.Concat(param.usburl, "?", str.ToString());
            //    HttpUtil http= base.CreateHttp();
            //    http.GetHtml(url);
            //});

            //var nsClick = new Action(delegate()
            //{
            //    string url = string.Concat(param.usburl, "?", str.ToString());
            //    HttpUtil http = base.CreateHttp();
            //    http.GetHtml(url);
            //});
            //nsClick.BeginInvoke(null, null);

            //var urlRec = new Action(delegate()
            //{
            //    HttpUtil http = base.CreateHttp();
            //    http.GetHtml(param.url);
            //});
            //urlRec.BeginInvoke(null,null);
            
        }

        void ThrowError(string msg)
        {
            if (ErrorHandler != null)
                ErrorHandler(msg);
        }
    }
}
