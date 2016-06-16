using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO.Compression;

namespace Sort
{
    /// <summary>
    /// 网络请求封装
    /// </summary>
    public class HttpHelper
    {
        private HttpWebRequest httpWebRequest;
        private HttpWebResponse httpWebResponse;

        public HttpHelper()
        {
            this.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E; InfoPath.2)";
            //this.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36";
            this.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            this.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            this.CookieContainer = new CookieContainer();
            this.Timeout = 15000;
            this.Headers = new WebHeaderCollection();
            this.Headers.Add("Accept-Language", "zh-cn,zh;q=0.5");
            this.Headers.Add("Accept-Charset", "GB2312,utf-8;q=0.7,*;q=0.7");
            this.IsGzip = true;
        }

        public HttpHelper(bool iskeeplive)
            : this()
        {
            this.IsKeepAlive = iskeeplive;
        }

        public string UserAgent { get; set; }
        public string Accept { get; set; }
        public string ContentType { get; set; }
        /// <summary>
        /// 返回是否已取消的状态
        /// </summary>
        public bool Cancel { get; private set; }
        /// <summary>
        /// 设置或获取提交的数据
        /// </summary>
        public string PostData { get; set; }

        public CookieContainer CookieContainer { get; set; }
        /// <summary>
        /// 设置通信的数据编码(默认自动识别)
        /// </summary>
        public Encoding Encoding;

        /// <summary>
        /// 设置或获取请求URI地址
        /// </summary>
        public Uri ResponseUri { get; set; }
        /// <summary>
        /// 设置或获取
        /// </summary>
        public string Referer { get; set; }

        public bool IsKeepAlive { get; set; }

        /// <summary>
        /// 是否开启Gzip压缩
        /// </summary>
        public bool IsGzip { get; set; }

        /// <summary>
        /// 超时时间（毫秒），默认15000
        /// </summary>
        public int Timeout { get; set; }

        public System.Net.WebHeaderCollection Headers { get; set; }

        public System.Net.WebHeaderCollection ResponseHeaders { get; set; }


        /// <summary>
        ///  压缩类型
        /// </summary>
        public string CompressType { get; set; }

        public string HttpHeader { get; private set; }

        public string RequestHeader { get; private set; }

        public long ResponseContentLength { get; private set; }

        public System.Net.CookieCollection Cookies { get; set; }

        #region 公共方法

        public static string GetUserAgent(int spidertype)
        {
            switch (spidertype)
            {
                case 0:
                    return "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                case 1:
                    return "Mozilla/5.0 (compatible; Baiduspider/2.0; +http://www.baidu.com/search/spider.html)";
                case 2:
                    return "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)";
                default:
                    return null;
            }
        }



        /// <summary>
        /// 将COOKIE字符串转成COOKIE集合
        /// </summary>
        /// <param name="strCookie">COOKIE字符串</param>
        /// <returns></returns>
        public static CookieCollection StringToCookieCollection(string strCookie, string domain = null)
        {
            CookieCollection cookies = new CookieCollection();
            string[] strArray = strCookie.Trim().TrimEnd(';').Split(';');
            foreach (string item in strArray)
            {
                string cookieItem = item.Trim();
                int pos = cookieItem.IndexOf('=');
                if (pos > 0)
                {
                    Cookie cookie = new Cookie(cookieItem.Substring(0, pos), cookieItem.Substring(pos + 1));
                    if (domain != null)
                    {
                        cookie.Domain = domain;
                    }
                    cookies.Add(cookie);
                }
            }
            return cookies;
        }

        /// <summary>
        /// 将COOKIE集合转成COOKIE字符串
        /// </summary>
        /// <param name="cookies">COOKIE集合</param>
        /// <returns></returns>
        public static string CookieCollectionToString(CookieCollection cookies)
        {
            string str = string.Empty;
            if ((cookies != null) && (cookies.Count > 0))
            {
                foreach (Cookie cookie in cookies)
                {
                    string name = cookie.Name;
                    string str3 = cookie.Value;
                    str = str + string.Format("{0}={1};", name, str3);
                }
            }
            return str;
        }

        public static string DecodeData(string contentType, Stream respStream, int timeout, ref Encoding encoding, long ResponseContentLength = -1)
        {
            string name = null;
            if (contentType != null)
            {
                contentType = contentType.ToLower();
                int index = contentType.IndexOf("charset=");
                if (index != -1)
                {
                    int nextIndex = contentType.IndexOf(',');
                    if (nextIndex > index + 8)
                    {
                        name = contentType.Substring(index + 8, nextIndex - index - 8).TrimEnd(';').Replace("\"", "");
                    }
                    else
                        name = contentType.Substring(index + 8).TrimEnd(';').Replace("\"", "");
                    if (name == "none")
                    {
                        name = null;
                    }
                }
            }

            using (MemoryStream stream = new MemoryStream())
            {
                using (Stream responseStream = respStream)
                {
                    if (responseStream.CanTimeout)
                    {
                        responseStream.ReadTimeout = timeout;
                    }
                    int nnn = 0;
                    long readcount = 0;
                    while (nnn < 1000)
                    {
                        byte[] buffer = new byte[100 * 1024];
                        int i = responseStream.Read(buffer, 0, buffer.Length);
                        if (i == 0)
                        {
                            System.Threading.Thread.Sleep(200);
                            i = responseStream.Read(buffer, 0, buffer.Length);
                            if (i == 0)
                                break;
                        }
                        stream.Write(buffer, 0, i);
                        readcount += i;
                        if (ResponseContentLength > 0 && readcount == ResponseContentLength)
                        {
                            break;
                        }
                        nnn++;
                    }
                }

                Encoding encode = null;
                if (name == null)
                {
                    stream.Seek((long)0, SeekOrigin.Begin);
                    string temphtml = new StreamReader(stream, Encoding.ASCII).ReadToEnd();
                    Match match = Regex.Match(temphtml, @"<meta[^>]+charset\s*=\s*[""']?([-\w]+)[""'>\s]?", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    if (match.Success)
                    {
                        name = match.Groups[1].Value;
                    }
                }
                try
                {
                    if (name != null && name.ToLower() == "utf8")
                    {
                        name = "utf-8";
                    }
                    encode = Encoding.GetEncoding(name);
                }
                catch
                {
                    encode = Encoding.GetEncoding("GBK");
                }
                encoding = encode;
                stream.Seek((long)0, SeekOrigin.Begin);
                using (StreamReader reader2 = new StreamReader(stream, encode))
                {
                    return reader2.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// 获取网络流数据（自动识别编码）
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public string DecodeData(string contentType, Stream respStream)
        {
            return DecodeData(contentType, respStream, this.Timeout, ref Encoding, ResponseContentLength);
        }

        /// <summary>
        /// 取消请求
        /// </summary>
        public void CancelRequest()
        {
            this.Cancel = true;
            httpWebRequest.Abort();
        }

        #region===提交Post请求，并返回结果字符串
        /// <summary>
        /// 提交Post请求，并返回结果字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="Content_Type"></param>
        /// <returns></returns>
        public string PostRequest(string url, string param, string Content_Type, string encodingString)
        {
            Stream outstream = null;
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.GetEncoding(encodingString);
            byte[] data = encoding.GetBytes(param);
            string result = string.Empty;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Accept = Accept;
                request.ContentType = Content_Type;
                request.Headers.Add("Accept-Language", "zh-cn,zh;q=0.5");
                request.Headers.Add("Accept-Charset", "GB2312,utf-8;q=0.7,*;q=0.7");
                request.KeepAlive = false;
                request.Method = "POST";
                request.UserAgent = UserAgent;
                request.ContentLength = data.Length;
                outstream = request.GetRequestStream();
                outstream.Write(data, 0, data.Length);
                outstream.Close();
                response = (HttpWebResponse)request.GetResponse();
                instream = response.GetResponseStream();
                sr = new StreamReader(instream, encoding);
                result = sr.ReadToEnd();
                sr.Close();
            }
            catch //(Exception ex)
            {
                return null;

            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    request.Abort();
                }
            }
            return result;
        }

        public string PostRequest(string url, string param, string Content_Type)
        {
            return PostRequest(url, param, Content_Type, "UTF-8");
        }
        public string PostRequest(string url, string param)
        {
            return PostRequest(url, param, "application/x-www-form-urlencoded; charset=UTF-8");
        }
        #endregion

        public string GetHtml(string url, string ip = null, bool isCountTime = false)
        {
            return GetHtml(url, null, false, ip, isCountTime);
        }

        public string GetHtml(string url, string postData, bool isPost, string ip = null, bool isCountTime = false)
        {
            httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            httpWebRequest.Headers = this.Headers;
            httpWebRequest.CookieContainer = CookieContainer;
            httpWebRequest.Referer = Referer;
            httpWebRequest.Accept = Accept;
            httpWebRequest.AllowAutoRedirect = true;
            if (this.IsGzip)
            {
                httpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
            }
            httpWebRequest.UserAgent = UserAgent;

            httpWebRequest.KeepAlive = this.IsKeepAlive;

            if (!string.IsNullOrEmpty(PostData))
            {
                httpWebRequest.Method = "Post";
                httpWebRequest.ContentType = ContentType;
                byte[] byteRequest = this.Encoding.GetBytes(PostData);
                httpWebRequest.ContentLength = byteRequest.Length;
                using (Stream SendStream = httpWebRequest.GetRequestStream())
                {
                    SendStream.Write(byteRequest, 0, byteRequest.Length);
                }
            }
            else
                httpWebRequest.Method = "GET";
            try
            {
                IAsyncResult result = httpWebRequest.BeginGetResponse(null, null);
                bool bl = result.AsyncWaitHandle.WaitOne(this.Timeout, true);
                if (!bl)
                {
                    throw new WebException("主机连接超时", WebExceptionStatus.Timeout);
                }
                httpWebResponse = (HttpWebResponse)httpWebRequest.EndGetResponse(result);

                this.ResponseContentLength = httpWebResponse.ContentLength;
                this.RequestHeader = "GET " + httpWebRequest.RequestUri.PathAndQuery + " HTTP/" + httpWebRequest.ProtocolVersion + "\r\n" + httpWebRequest.Headers.ToString();
                this.HttpHeader = "HTTP/" + httpWebResponse.ProtocolVersion + " " + Convert.ToInt32(httpWebResponse.StatusCode) + " " + httpWebResponse.StatusCode + "\r\n" + httpWebResponse.Headers.ToString();
                this.RequestHeader = this.RequestHeader.Trim('\r', '\n');
                this.HttpHeader = this.HttpHeader.Trim('\r', '\n');
                this.ResponseHeaders = httpWebResponse.Headers;
                this.Cookies = httpWebResponse.Cookies;

                CompressType = httpWebResponse.ContentEncoding.ToLower();

                Stream stream = null;
                if (CompressType == "gzip")
                {
                    this.IsGzip = true;
                    stream = new System.IO.Compression.GZipStream(httpWebResponse.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                }
                else if (CompressType == "deflate")
                {
                    this.IsGzip = true;
                    stream = new System.IO.Compression.DeflateStream(httpWebResponse.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                }
                else
                {
                    this.IsGzip = false;
                    stream = httpWebResponse.GetResponseStream();
                }
                if (Encoding != null)
                {
                    using (System.IO.StreamReader sr = new StreamReader(stream, Encoding))
                    {
                        return sr.ReadToEnd();
                    }
                }
                else
                {
                    string html = DecodeData(httpWebResponse.Headers["content-type"], stream);
                    httpWebResponse.Close();
                    return html;
                }
            }
            catch 
            {
                return null;
            }
            finally
            {
                if (httpWebResponse != null)
                    httpWebResponse.Close();
                httpWebRequest.Abort();
            }
        }

       

        public static bool CheckDomainDns(string url)
        {
            try
            {
                string domain = new Uri("http://" + url).DnsSafeHost;
                IPAddress ip = Dns.GetHostAddresses(domain)[0];
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

    }
}
