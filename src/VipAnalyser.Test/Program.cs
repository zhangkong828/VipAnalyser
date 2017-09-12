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


            var url = "http://mp.weixin.qq.com/s/IuJfF7zidy9MU6OsHveu7w";
            var result = AnalysisFactory.GetResponse(url, cookie);

            var resultJson = JsonConvert.SerializeObject(result);

            Console.WriteLine(resultJson);

            //var cookie = PhantomJS();


            //ConsoleWrite("开始解析");
            //ConsoleWrite("https://v.qq.com/x/cover/kds9l8b75jvb6y6.html");
            //Test("x0012ezj2z6", cookie);

            Console.ReadKey();
        }

        static void ConsoleWrite(string msg)
        {
            Console.WriteLine(msg);
            Logger.Info(msg);
        }


        static string PhantomJS()
        {
            var web = PhantomJSHelper.Instance;
            var pageSource = string.Empty;
            var url = "https://v.qq.com/u/history/";

            web.GoToUrl(url);
            //登录前cookie
            var cookie1 = web.GetAllCookies();
            ConsoleWrite("登录前cookie");
            ConsoleCookie(cookie1);
            //等待 登录弹窗
            if (!web.WaitForElementExists(By.Id("login_win_type"), 10))
            {
                return null;
            }
            //选择qq登录
            web.FindElementBy(By.ClassName("btn_qq")).Click();
            //qq快速登录iframe
            if (!web.WaitForElementExists(By.Id("_login_frame_quick_"), 10))
            {
                //没有登录窗，说明已经登录了
                return null;
            }
            var quick_frame = web.Frame("_login_frame_quick_");
            //登录方式：账号密码登录
            quick_frame.FindElement(By.Id("switcher_plogin")).Click();
            //登录
            quick_frame.FindElement(By.Id("u")).SendKeys("");
            quick_frame.FindElement(By.Id("p")).SendKeys("");
            quick_frame.FindElement(By.Id("login_button")).Click();
            //回到 parent window
            var main = quick_frame.SwitchTo().DefaultContent();
            pageSource = main.PageSource;

            //等待页面刷新
            if (!web.WaitForInvisibilityOfElementLocated(By.ClassName("login_win_type"), 10))
            //if (main.WaitFor(new Func<IWebDriver, bool>(x => x.FindElement(By.ClassName("__nickname")).Text.Length > 0), 10))
            {
                //登录窗没有消失，要么账号密码错误，要么需要验证
                return null;
            }
            ConsoleWrite("------------------------------------------");
            ConsoleWrite("登录成功");
            ConsoleWrite("------------------------------------------");
            //用户名
            var name = web.FindElementBy(By.ClassName("__nickname")).Text;
            ConsoleWrite($"用户名:{name}");
            //到期时间
            var vip_time = web.FindElementBy(By.ClassName("_vip_desc")).Text;
            ConsoleWrite($"到期时间:{vip_time}");
            //登录后cookie
            var cookie2 = web.GetAllCookies();
            ConsoleWrite("登录后cookie");
            ConsoleCookie(cookie2);
            ConsoleWrite("------------------------------------------");

            var cookieStr = web.GetAllCookiesString();
            ConsoleWrite(cookieStr);

            return cookieStr;

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
