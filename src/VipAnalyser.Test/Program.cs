using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VipAnalyser.ClassCommon;

namespace VipAnalyser.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test();
            // FiddlerCore.Start();


            PhantomJS();

            ////https://v.qq.com/x/cover/kds9l8b75jvb6y6.html


            //var browser = PhantomJSHelper.Instance;


            ////web.GoToUrl("https://www.baidu.com/");
            ////var mainHandle = web.GetCurrentWindowHandle();


            //var openJs = "window.open('https://v.qq.com/x/cover/kds9l8b75jvb6y6.html');";
            //browser.ExecuteScript(openJs);

            //Thread.Sleep(1000 * 10);
            //foreach (var item in browser.GetWindowHandles())
            //{
            //    var tab = browser.GoToWindow(item);
            //    Console.WriteLine(tab.Title);


            //}
            //browser.Close();
            Console.ReadKey();
        }

        static void ConsoleList(List<string> list)
        {
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }
        }


        static void PhantomJS()
        {
            var web = PhantomJSHelper.Instance;
            var pageSource = string.Empty;
            var url = "https://v.qq.com/u/history/";

            web.GoToUrl(url);
            //登录前cookie
            var cookie1 = web.GetAllCookies();
            Console.WriteLine("登录前cookie");
            ConsoleCookie(cookie1);
            //等待 登录弹窗
            if (!web.WaitForElementExists(By.Id("login_win_type"), 10))
            {
                return;
            }
            //选择qq登录
            web.FindElementBy(By.ClassName("btn_qq")).Click();
            //qq快速登录iframe
            if (!web.WaitForElementExists(By.Id("_login_frame_quick_"), 10))
            {
                //没有登录窗，说明已经登录了
                return;
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
            {
                //登录窗没有消失，要么账号密码错误，要么需要验证
                return;
            }
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("登录成功");
            Console.WriteLine("------------------------------------------");
            //用户名
            var name = web.FindElementBy(By.ClassName("__nickname")).Text;
            Console.WriteLine("用户名:{0}", name);
            //到期时间
            var vip_time = web.FindElementBy(By.ClassName("_vip_desc")).Text;
            Console.WriteLine("到期时间:{0}", vip_time);
            //登录后cookie
            var cookie2 = web.GetAllCookies();
            Console.WriteLine("登录后cookie");
            ConsoleCookie(cookie2);
            Console.WriteLine("------------------------------------------");

            var cookieStr = web.GetAllCookiesString();
            Console.WriteLine(cookieStr);
            Test(cookieStr);
        }

        static void ConsoleCookie(Dictionary<string, string> dic)
        {
            foreach (var item in dic)
            {
                Console.WriteLine("{0}:{1}", item.Key, item.Value);
            }
        }


        static void Test(string cookie)
        {
            var vid = "q0181hpdvo5";

            var info_api = $"http://vv.video.qq.com/getinfo?otype=json&appver=3.2.19.333&platform=11&defnpayver=1&vid={vid}";
            var info = HttpHelper.Get(info_api, cookie);
            //"QZOutputJson={\"em\":61,\"exem\":1,\"ip\":\"222.128.117.100\",\"msg\":\"vid is wrong\",\"s\":\"f\"};"
            //"QZOutputJson={\"em\":80,\"exem\":10,\"ip\":\"222.128.117.100\",\"msg\":\"ip-copy limit\",\"s\":\"f\",\"exinfo\":\"中国-北京市--联通\"};"

        }

    }
}
