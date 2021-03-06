﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SortConsole
{
    public class Index:BaiduBase
    {
        public event Action<string> ErrorHandler;

        public event Action<string,string,string,string> NextHandler;

        public override void Process()
        {
            if(string.IsNullOrEmpty(Keyword)||
                string.IsNullOrEmpty(MatchUrl))
            {
                ThrowError("请设置关键词跟地址");
                return;
            }

            base.Url = "https://www.baidu.com";
            base.GetResult();
            if (string.IsNullOrEmpty(base.Html))
            {
                ThrowError("首页获取HTML为空");
                return;
            }
            base.Url = FormatGetUrl();
            if (string.IsNullOrEmpty(base.Url))
            {
                ThrowError("获取首页跳转URL为空");
                return;
            }

            base.InitHttp();
            base.GetResult();
            if (string.IsNullOrEmpty(base.Html))
            {
                ThrowError("首页跳转获取HTML为空");
                return;
            }

            if (InitBdComm()==false)//初始化bds.comm参数
            {
                ThrowError("初始化bds.comm参数失败");
                return;
            }

            var wgifAction = new Action(delegate()
            {
                Wgif wgif = new Wgif(base.Keyword,base.BdComm);
                wgif.Process();

            });
            wgifAction.BeginInvoke(null, null);

            var vgifAction = new Action(delegate()
            {
                HttpUtil http = base.CreateHttp();
                http.Accept = "image/webp,image/*,*/*;q=0.8";
                http.Referer = BdComm.encodeurl;
                http.GetHtml(BdComm.cgif);

            });
            vgifAction.BeginInvoke(null, null);

            if (NextHandler != null)
                NextHandler(base.Html, base.Keyword, base.MatchUrl, base.Url);

        }

        void ThrowError(string msg)
        {
            if (ErrorHandler != null)
                ErrorHandler(msg);
        }

        string FormatGetUrl()
        {
            Match match=RegexUitl.formReg.Match(base.Html);
            if (match.Success == false)
                return null;
            string matchHtml = match.Groups[1].Value;
            MatchCollection matches = RegexUitl.hideInputReg.Matches(matchHtml);
            StringBuilder str = new StringBuilder("https://www.baidu.com/s?wd=");
            str.Append(Keyword);
            foreach (Match item in matches)
            { 
                string key=item.Groups[1].Value.Trim();
                string value=item.Groups[2].Value.Trim();
                str.Append("&");
                str.Append(key);
                str.Append("=");
                str.Append(value);
            }
            return str.ToString().Trim('&');
            
        }
    }
}
