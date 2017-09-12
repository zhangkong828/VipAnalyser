using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VipAnalyser.ClassCommon;
using VipAnalyser.ClassCommon.Models;

namespace VipAnalyser.Core2.Extension
{
    [AnalysisType(Type = SiteCode.QQ)]
    public class QQ : IAnalyser
    {
        public VideoAnalyseResponse Analyse(string url, string cookie = null)
        {
            var response = new VideoAnalyseResponse();
            var vids = GetVid(url);
            if (vids.Count > 0)
            {
                var info = new VideoInfo();
                var errorMsg = string.Empty;
                foreach (var vid in vids)
                {
                    if (GetInfo(vid, cookie, out info, out errorMsg))
                    {
                        response.Data.Add(info);
                    }
                }
                return response;
            }
            else
            {
                return new VideoAnalyseResponse() { ErrCode = -1, ErrMsg = $"[{url}]没有找到vid" };
            }

        }

        private List<string> GetVid(string url)
        {
            var result = new List<string>();

            //http://mp.weixin.qq.com/s/IuJfF7zidy9MU6OsHveu7w
            if (url.Contains("mp.weixin.qq.com/s"))
            {
                var content = HttpHelper.Get(url);
                var matchs = Regex.Matches(content, "\\?vid=(\\w+)");
                foreach (Match item in matchs)
                {
                    var vid = item.Groups[1].Value;
                    if (!string.IsNullOrEmpty(vid))
                        result.Add(vid);
                }
            }
            else if (url.Contains("v.qq.com"))
            {
                var content = HttpHelper.Get(url);
                var vid = Regex.Match(content, "&vid=(.+?)&").Groups[1].Value;
                if (!string.IsNullOrEmpty(vid))
                    result.Add(vid);
            }

            return result;
        }


        private bool GetInfo(string vid, string cookie, out VideoInfo videoInfo, out string errorMsg)
        {
            videoInfo = new VideoInfo();
            errorMsg = string.Empty;

            try
            {
                var info_api = $"http://vv.video.qq.com/getinfo?otype=json&appver=3.2.19.333&platform=11&defnpayver=1&vid={vid}";
                var info = HttpHelper.Get(info_api);
                var infoText = Regex.Match(info, "QZOutputJson=(.*)").Groups[1].Value.TrimEnd(';');
                var infoJson = JsonConvert.DeserializeObject(infoText) as JObject;

                if ((int)infoJson["exem"] != 0)
                {
                    errorMsg = (string)infoJson["msg"];
                    return false;
                }

                var fn_pre = (string)infoJson["vl"]["vi"][0]["lnk"];

                var title = (string)infoJson["vl"]["vi"][0]["ti"];
                videoInfo.Name = title;

                var host = (string)infoJson["vl"]["vi"][0]["ul"]["ui"][0]["url"];

                var streams = infoJson["fl"]["fi"];

                var seg_cnt = (int)infoJson["vl"]["vi"][0]["cl"]["fc"];
                if (seg_cnt == 0)
                    seg_cnt = 1;

                var best_quality = (string)streams.Last["name"];
                var part_format_id = (int)streams.Last["id"];


                var partInfos = new List<PartInfo>();

                for (int i = 1; i < seg_cnt + 1; i++)
                {
                    var part = new PartInfo();
                    var filename = $"{fn_pre}.p{part_format_id % 10000}.{i}.mp4";
                    part.Name = filename;
                    var key_api = $"http://vv.video.qq.com/getkey?otype=json&platform=11&format={part_format_id}&vid={vid}&filename={filename}&appver=3.2.19.333";
                    var keyInfo = HttpHelper.Get(key_api, cookie);
                    var keyText = Regex.Match(keyInfo, "QZOutputJson=(.*)").Groups[1].Value.TrimEnd(';');
                    var keyJson = JsonConvert.DeserializeObject(keyText) as JObject;

                    if (string.IsNullOrEmpty((string)keyJson["key"]))
                    {
                        part.Remark = (string)keyJson["msg"];
                        partInfos.Add(part);
                        continue;
                    }

                    var vkey = (string)keyJson["key"];
                    var url = $"{host}{filename}?vkey={vkey}";
                    part.Url = url;
                    partInfos.Add(part);
                }
                videoInfo.Part = partInfos;
                videoInfo.PartCount = partInfos.Count;
                return true;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }

        }
    }
}
