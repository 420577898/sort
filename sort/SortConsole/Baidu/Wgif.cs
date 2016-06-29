using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SortConsole
{
    public class Wgif:BaiduBase
    {

        public event Action<string> ErrorHandler;

        public Wgif(string keyword,BdComm bdcomm)
        {
            base.Keyword = keyword;
            base.BdComm = bdcomm;
        }

        public override void Process()
        {
            base.Url = FormatUrl();
            HttpUtil http = base.CreateHttp();
            http.Accept = "image/webp,image/*,*/*;q=0.8";
            http.Referer = BdComm.encodeurl;
            http.GetHtml(Url);
        }

        string FormatUrl()
        {
            StringBuilder str = new StringBuilder();
            str.Append(BdComm.usburl);
            str.Append("?");
            str.Append("q=");
            str.Append(EncodeKeyword());
            str.Append("&");

            str.Append("fm=");
            str.Append(BdComm.SeInfo.fm);
            str.Append("&");

            str.Append("T=");
            str.Append(BdComm.SeInfo.T);
            str.Append("&");

            str.Append("y=");
            str.Append(BdComm.SeInfo.y);
            str.Append("&");

            str.Append("rsv_cache=");
            str.Append(BdComm.SeInfo.rsv_cache);
            str.Append("&");

            str.Append("rsv_pre=");
            str.Append(BdComm.SeInfo.rsv_pre);
            str.Append("&");

            str.Append("rsv_reh=");
            str.Append(BdComm.SeInfo.rsv_reh);
            str.Append("&");

            str.Append("rsv_scr=");
            str.Append(BdComm.SeInfo.rsv_scr);
            str.Append("&");

            str.Append("rsv_psid=");
            str.Append(BdComm.SeInfo.rsv_psid);
            str.Append("&");

            str.Append("rsv_pstm=");
            str.Append(BdComm.SeInfo.rsv_pstm);
            str.Append("&");

            str.Append("rsv_idc=");
            str.Append(BdComm.SeInfo.rsv_idc);
            str.Append("&");

            str.Append("rsv_sid=");
            str.Append(BdComm.sid);
            str.Append("&");

            str.Append("cid=");
            str.Append(BdComm.cid);
            str.Append("&");

            str.Append("qid=");
            str.Append(BdComm.qid);
            str.Append("&");

            str.Append("rsv_iorr=");
            str.Append(BdComm.SeInfo.rsv_iorr);
            str.Append("&");

            str.Append("rsv_tn=");
            str.Append(BdComm.tn);
            str.Append("&");

            str.Append("rsv_isid=");
            str.Append(BdComm.sid);
            str.Append("&");

            str.Append("rsv_ssl=");
            str.Append(BdComm.SeInfo.rsv_ssl);
            str.Append("&");

            str.Append("path=");
            str.Append(BdComm.path);
            str.Append("&");

            str.Append("rsv_did=");
            str.Append(BdComm.rsv_did);

            return str.ToString();
        }

        void ThrowError(string msg)
        {
            if (ErrorHandler != null)
                ErrorHandler(msg);
        }

    }
}
