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
            var url = "https://v.qq.com/u/history/";

            PhantomJSHelper.Instance.GoToUrl(url);

            if (PhantomJSHelper.Instance.WaitForElementExists(By.Id("web_qr_login"), 10))
            {

            }



            Console.ReadKey();
        }
    }
}
