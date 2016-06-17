using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sort
{
    public class RegexUitl
    {
        public static Regex formReg = new Regex(@"<form[^>]+id=""form""[^>]*>(.*?)</form>",RegexOptions.IgnoreCase);

        public static Regex hideInputReg = new Regex(@"<input[^>]+type=""hidden""[^>]+name=""(.*?)""[^>]+value=""(.*?)""[^>]*>", RegexOptions.IgnoreCase);

        public static Regex rsvtReg = new Regex(@"<input[^>]+type=""hidden""[^>]+name=""rsv_t""[^>]+value=""(.*?)""[^>]*>",RegexOptions.IgnoreCase);

        public static Regex baiduReg = new Regex(@"<div[^>]+class=""[^""]+c-container[^""]*""[^>]+id=""(\d{1,2})""[^>]*>(?><div[^>]*>(?<Open>)|</div>(?<-Open>)|(?:(?!</?div\b).)*)*(?(Open)(?!))</div>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static Regex urlReg = new Regex(@"<(?:span|a)[^>]+class=""c-showurl""[^>]*>(.*?)</(?:span|a)[^>]*>",RegexOptions.IgnoreCase);

        public static Regex htmlReg = new Regex(@"<[^>]+>", RegexOptions.IgnoreCase);

        public static Regex specialReg = new Regex(@"&[a-z0-9]+;", RegexOptions.IgnoreCase);

        public static Regex h3Reg = new Regex(@"<h3[^>]+class=""t[^""]*""[^>]*>(.*?)</h3>", RegexOptions.IgnoreCase|RegexOptions.Singleline);
    }
}
