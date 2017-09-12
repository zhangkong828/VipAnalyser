using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using VipAnalyser.ClassCommon;
using VipAnalyser.Core2;
using VipAnalyser.LoginManager;

namespace VipAnalyser.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            LoginMonitor.QQ("", "");

            Console.WriteLine("等待登录...");
            while (string.IsNullOrEmpty(LoginMonitor.QQCookies))
            {
                Thread.Sleep(500);
            }
            var cookie = LoginMonitor.QQCookies;
            Console.WriteLine(cookie);

            //https://v.qq.com/x/cover/kds9l8b75jvb6y6.html
            var url = "http://mp.weixin.qq.com/s/IuJfF7zidy9MU6OsHveu7w";
            var result = AnalysisFactory.GetResponse(url, cookie);

            var resultJson = JsonConvert.SerializeObject(result);

            Console.WriteLine(resultJson);

          
            Console.ReadKey();
        }

        static void ConsoleWrite(string msg)
        {
            Console.WriteLine(msg);
            Logger.Info(msg);
        }

        static void ConsoleCookie(Dictionary<string, string> dic)
        {
            foreach (var item in dic)
            {
                ConsoleWrite($"{item.Key}:{item.Value}");
            }
        }


        static void Test(string vid, string cookie)
        {
            //http://vv.video.qq.com/getinfo?otype=json&appver=3.2.19.333&platform=11&defnpayver=1&vid=x0012ezj2z6
            var info_api = $"http://vv.video.qq.com/getinfo?otype=json&appver=3.2.19.333&platform=11&defnpayver=1&vid={vid}";
            var info = HttpHelper.Get(info_api);
            var infoText = Regex.Match(info, "QZOutputJson=(.*)").Groups[1].Value.TrimEnd(';');
            var infoJson = JsonConvert.DeserializeObject(infoText) as JObject;

            if ((int)infoJson["exem"] != 0)
            {
                ConsoleWrite((string)infoJson["msg"]);
                return;
            }

            var fn_pre = (string)infoJson["vl"]["vi"][0]["lnk"];
            var title = (string)infoJson["vl"]["vi"][0]["ti"];
            var host = (string)infoJson["vl"]["vi"][0]["ul"]["ui"][0]["url"];

            var streams = infoJson["fl"]["fi"];

            var seg_cnt = (int)infoJson["vl"]["vi"][0]["cl"]["fc"];
            if (seg_cnt == 0)
                seg_cnt = 1;

            var best_quality = (string)streams.Last["name"];
            var part_format_id = (int)streams.Last["id"];

            var part_urls = new List<KeyValuePair<string, string>>();

            for (int i = 1; i < seg_cnt + 1; i++)
            {
                var filename = $"{fn_pre}.p{part_format_id % 10000}.{i}.mp4";
                var key_api = $"http://vv.video.qq.com/getkey?otype=json&platform=11&format={part_format_id}&vid={vid}&filename={filename}&appver=3.2.19.333";
                var keyInfo = HttpHelper.Get(key_api, cookie);
                var keyText = Regex.Match(keyInfo, "QZOutputJson=(.*)").Groups[1].Value.TrimEnd(';');
                var keyJson = JsonConvert.DeserializeObject(keyText) as JObject;

                if (string.IsNullOrEmpty((string)keyJson["key"]))
                {
                    ConsoleWrite((string)keyJson["msg"]);
                    break;
                }

                var vkey = (string)keyJson["key"];
                var url = $"{host}{filename}?vkey={vkey}";
                part_urls.Add(new KeyValuePair<string, string>(filename, url));
            }


            foreach (var part in part_urls)
            {
                ConsoleWrite("--------------------------------");
                ConsoleWrite(part.Key);
                ConsoleWrite(part.Value);
            }
        }

    }
}
