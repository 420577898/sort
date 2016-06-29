using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortConsole
{
    public class BdComm
    {
        public static Random ran = new Random();

        public string encodeurl { get; set; }

        public string path { get; set; }


        /// <summary>
        /// bds.comm.cid = "0";
        /// </summary>
        public string cid { get; set; }

        /// <summary>
        /// bds.comm.qid = "ecdfb8730007a892";
        /// </summary>
        public string qid { get; set; }

        /// <summary>
        /// bds.comm.ubsurl = "https://sp0.baidu.com/5bU_dTmfKgQFm2e88IuM_a/w.gif";
        /// </summary>
        public string usburl { get; set; }

        /// <summary>
        /// "nsclick.baidu.com":"https://sp1.baidu.com/8qUJcD3n0sgCo2Kml5_Y_D3"
        /// </summary>
        public string nsclickurl { get; set; }

        /// <summary>
        /// bds.comm.cgif = "https://sp0.baidu.com/9foIbT3kAMgDnd_/c.gif?t=0&q=df&p=0&pn=1";
        /// </summary>
        public string cgif { get; set; }

        /// <summary>
        /// bds.comm.tn = "baidu";
        /// </summary>
        public string tn { get; set; }

        /// <summary>
        /// bds.comm.sid = "1431_17710_20516_18282_17945_20389_20416_20455_18559_15874_11470";
        /// </summary>
        public string sid { get; set; }


        private string m_rsv_did;

        public string rsv_did
        {
            get
            {
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
        public SeInfo SeInfo { get; set; }
    }

    public class SeInfo
    {
        public string fm { get; set; }

        public string T { get; set; }

        public string y { get; set; }

        public string rsv_cache { get; set; }


        public string rsv_pre = "0";

        /// <summary>
        ///  var bw = []
        ///   , bu = [];
        /// try {
        ///    $("#content_left").children(".result,.result-op").each(function(bx, by) {
        ///        bw.push($(by).height())
        ///    })
        ///  } catch (bv) {}
        /// try {
        ///     $("#con-ar").children(".result,.result-op").each(function(bx, by) {
        ///        bu.push($(by).height())
        ///     })
        /// } catch (bv) {}
        /// return bw.join("_") + "|" + bu.join("_")
        /// </summary>
        /// 
        private string m_rsv_reh;
        public string rsv_reh
        {
            get {
                if (m_rsv_reh == null)
                {
                    StringBuilder str = new StringBuilder();
                    foreach (KeyValuePair<int, int> item in rehDic)
                    {
                        str.Append(item.Value);
                        str.Append("_");
                    }
                    m_rsv_reh = str.ToString().Trim('_') + "|440";
                }
                return m_rsv_reh;
            }
        }

        /// <summary>
        /// var bu = "";
        ///try {
        ///    bu += [document.body.clientWidth 1903, document.body.clientHeight +567, window.screenTop 0, window.screenLeft 0, window.screen.height 1080, window.screen.width 1920].join("_")
        ///} catch (bv) {}
        ///return bu
        /// </summary>
        private string m_rsv_scr;
        public string rsv_scr
        {
            get {
                if (m_rsv_scr == null)
                {
                    var clientHeight = rehDic.Sum(i => i.Value) + 567;
                    m_rsv_scr = string.Format("1903_{0}_0_0_1080_1920", clientHeight);
                }
                return m_rsv_scr;
            }
        }

        /// <summary>
        /// $.getCookie("BIDUPSID");
        /// </summary>
        public string rsv_psid { get; set; }

        /// <summary>
        ///  $.getCookie("PSTM");
        /// </summary>
        public string rsv_pstm { get; set; }

        public string rsv_iorr = "1";

        public string rsv_ssl_sample = "ssl_b1";

        public string rsv_ssl = "1";

        public string rsv_idc = "5";

        public Dictionary<int, int> rehDic = new Dictionary<int, int>();
    }
}
