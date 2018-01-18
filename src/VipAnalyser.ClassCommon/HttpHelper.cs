using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VipAnalyser.ClassCommon
{
    public class HttpHelper
    {
        public static string Get(string url, string cookie = null)
        {
            var html = "";
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                if (!string.IsNullOrEmpty(cookie))
                    request.Headers[HttpRequestHeader.Cookie] = cookie;
                request.Timeout = 1000 * 10;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.101 Safari/537.36";
                //请求数据
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var bytes = GetByte(response);
                    var encoding = GetEncoding(response, bytes);
                    html = encoding.GetString(bytes);
                }
            }
            catch (Exception ex)
            {
                //log
            }
            return html;
        }

        public static string Post(string url, string postData, string cookie = null, int timeout = 10, string encodingStr = "UTF-8")
        {
            var html = "";
            var encoding = Encoding.UTF8;
            try
            {
                encoding = Encoding.GetEncoding(encodingStr);
            }
            catch { }
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                if (!string.IsNullOrEmpty(cookie))
                    request.Headers[HttpRequestHeader.Cookie] = cookie;
                request.Timeout = 1000 * timeout;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.101 Safari/537.36";
                byte[] byteArray = encoding.GetBytes(postData);
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(byteArray, 0, byteArray.Length);
                }
                //请求数据
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var bytes = GetByte(response);
                    encoding = GetEncoding(response, bytes);
                    html = encoding.GetString(bytes);
                }

            }
            catch (Exception ex)
            {
                //log
            }
            return html;
        }

        private static byte[] GetByte(HttpWebResponse response)
        {
            byte[] ResponseByte = null;
            using (MemoryStream _stream = new MemoryStream())
            {
                //GZIIP处理
                if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                {
                    //开始读取流并设置编码方式
                    new GZipStream(response.GetResponseStream(), CompressionMode.Decompress).CopyTo(_stream, 10240);
                }
                else
                {
                    //开始读取流并设置编码方式
                    response.GetResponseStream().CopyTo(_stream, 10240);
                }
                //获取Byte
                ResponseByte = _stream.ToArray();
            }
            return ResponseByte;
        }


        private static Encoding GetEncoding(HttpWebResponse response, byte[] ResponseByte)
        {
            Encoding encoding = null;
            Match meta = Regex.Match(Encoding.Default.GetString(ResponseByte), "<meta[^<]*charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
            string c = string.Empty;
            if (meta != null && meta.Groups.Count > 0)
            {
                c = meta.Groups[1].Value.ToLower().Trim();
            }
            if (c.Length > 2)
            {
                try
                {
                    encoding = Encoding.GetEncoding(c.Replace("\"", string.Empty).Replace("'", "").Replace(";", "").Replace("iso-8859-1", "gbk").Trim());
                }
                catch
                {
                    if (string.IsNullOrEmpty(response.CharacterSet))
                    {
                        encoding = Encoding.UTF8;
                    }
                    else
                    {
                        encoding = Encoding.GetEncoding(response.CharacterSet);
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(response.CharacterSet))
                {
                    encoding = Encoding.UTF8;
                }
                else
                {
                    encoding = Encoding.GetEncoding(response.CharacterSet);
                }
            }
            return encoding;
        }


    }
}
