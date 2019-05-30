using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using VipAnalyser.Web.Models;

namespace VipAnalyser.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AnalyseFrame(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return HttpNotFound();
            }
            ViewData["rk"] = FormatHelper.EncodeBase64(url);
            return View();
        }

        [HttpGet]
        public ActionResult Core(string rk)
        {
            var url = FormatHelper.DecodeBase64(rk);
            var ckplayerJson = new CKPlayerJsonViewModel() { autoplay = true };
            var response = this.Analyse(url);

            //处理数据
            var result = new List<string>();
            var name = string.Empty;
            if (response == null || response.ErrCode != 0)
            {
                //result.Add("http://movie.ks.js.cn/flv/other/1_0.mp4");
                //解析失败   
                //应该返回错误 这里返回null
            }
            else
            {
                //可能会有多个视频  这里只取第一个
                var video = response.Data.FirstOrDefault();
                if (video != null)
                {
                    var ckvideo = new CKVideo()
                    {
                        type = "mp4",
                        weight = 0,
                        definition = video.Definition
                    };

                    foreach (var item in video.Part)
                    {
                        if (!string.IsNullOrEmpty(item.Url))
                            ckvideo.video.Add(new CKVideoInfo()
                            {
                                file = item.Url,
                                duration = item.Duration
                            });
                    }
                    ckplayerJson.video.Add(ckvideo);
                }
            }

            return Json(ckplayerJson,JsonRequestBehavior.AllowGet);
        }


        private VideoAnalyseResponse Analyse(string url)
        {
            var address = "http://127.0.0.1:11234/api/core/analyse";
            var postData = $"url={url}";
            try
            {
                var data = Post(address, postData);
                return JsonConvert.DeserializeObject<VideoAnalyseResponse>(data);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static string Post(string url, string postData, string cookie = null, string encodingStr = "UTF-8")
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
                request.Timeout = 1000 * 10;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.101 Safari/537.36";
                byte[] byteArray = encoding.GetBytes(postData);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(byteArray, 0, byteArray.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream(), encoding))
                {
                    html = sr.ReadToEnd();
                }
                html = GetResponseBody(response, encoding);
            }
            catch (Exception ex)
            {
                //log
            }
            return html;
        }


        private static string GetResponseBody(HttpWebResponse response, Encoding encoding)
        {
            string responseBody = string.Empty;
            if (response.ContentEncoding.ToLower().Contains("gzip"))
            {
                using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(stream, encoding))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }
            }
            else if (response.ContentEncoding.ToLower().Contains("deflate"))
            {
                using (DeflateStream stream = new DeflateStream(
                    response.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (StreamReader reader =
                        new StreamReader(stream, encoding))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }
            }
            else
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, encoding))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }
            }
            return responseBody;
        }
    }

}