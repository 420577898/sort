using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sort
{
    public class ParamModel
    {
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

        public divClick divClick { get; set; }

        /// <summary>
        /// as
        /// </summary>
        public string fm { get; set; }

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

        public string cookie { get; set; }

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
                "&",
                "rsv_srcid=" + this.h3Click.F,
                "&",
                "F1=" + this.h3Click.F1,
                "&",
                "F2=" + this.h3Click.F2,
                "&",
                "F3=" + this.h3Click.F3,
                "&",
                "T=" + this.h3Click.T,
                "&",
                "y=" + this.h3Click.y,
                "&",
                "rsv_bdr=" + this.divClick.rsv_bdr.ToString(),
                "&",
                "p5=" + this.divClick.p5.ToString(),
                "&",
                "fm=" + fm,
                "&",
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
                "path=" + path);
        }
    }

    public class divClick
    {
        public object rsv_bdr { get; set; }

        public object p5 { get; set; }
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
