using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VipAnalyser.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var web = PhantomJSHelper.Instance;

            var url = "https://v.qq.com/u/history/";
            var PageSource = string.Empty;
            web.GoToUrl(url);
            PageSource= web.GetPageSource();
            //检查是否有登录弹窗
            if (web.WaitForElementExists(By.Id("login_win"), 30))
            {
                //iframe   _login_frame_quick_
                web.Frame("_login_frame_quick_");
                if (web.WaitForElementExists(By.Id("web_qr_login"), 10))
                {
                    web.FindElementById("u").SendKeys("");
                    web.FindElementById("p").SendKeys("");
                    web.FindElementById("login_button").Click();

                    web.DefaultContent();

                    var s = web.GetPageSource();
                    Console.WriteLine(s);
                }
            }





            Console.ReadKey();
        }
    }
}
