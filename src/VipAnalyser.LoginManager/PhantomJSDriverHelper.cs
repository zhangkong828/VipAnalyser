using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VipAnalyser.LoginManager
{
    public class PhantomJSDriverHelper
    {
        public PhantomJSDriver _webDriver;


        public PhantomJSDriverHelper(string cookies, string domain)
        {
            var _option = new PhantomJSOptions();
            _option.AddAdditionalCapability(@"phantomjs.page.settings.userAgent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36");

            var _service = PhantomJSDriverService.CreateDefaultService();
            //代理
            _service.ProxyType = "none";//'http', 'socks5' or 'none'
            _service.Proxy = "";//{address}:{port}
            //不加载图片
            _service.LoadImages = false;
            //忽略SSL证书错误
            _service.IgnoreSslErrors = true;
            //隐藏命令提示窗口
            _service.HideCommandPromptWindow = true;
            //日志文件路径
            _service.LogFile = "";
            //Cookie文件路径
            //_service.CookiesFile = "PhantomJSCookie.json";
            //_service.DiskCache = true;
            //_service.MaxDiskCacheSize = 10240;
            //_service.LocalStoragePath = "LocalStoragePath";
            _webDriver = new PhantomJSDriver(_service, _option);


            if (!string.IsNullOrEmpty(cookies))
                AddCookie(cookies, domain);
        }

        public void AddCookie(string cookies, string domain)
        {
            string[] cookielist = cookies.Split(';');
            for (int i = 0; i < cookielist.Length; i++)
            {
                string cookie = cookielist[i];
                string[] cookieVal = cookie.Split('=');
                Cookie ck = new Cookie(cookieVal[0], cookieVal[1], domain, "/", null);
                _webDriver.Manage().Cookies.AddCookie(ck);
            }
        }

        public void AddCookie(Dictionary<string, string> cookies)
        {
            foreach (var item in cookies)
            {
                _webDriver.Manage().Cookies.AddCookie(new Cookie(item.Key, item.Value));
            }
        }
        public Dictionary<string, string> GetAllCookies()
        {
            Dictionary<string, string> cookies = new Dictionary<string, string>();
            var allCookies = _webDriver.Manage().Cookies.AllCookies;
            foreach (var cookie in allCookies)
            {
                cookies[cookie.Name] = cookie.Value;
            }
            return cookies;
        }
        public string GetAllCookiesTexts()
        {
            var list = new List<string>();
            var allCookies = _webDriver.Manage().Cookies.AllCookies;
            foreach (var cookie in allCookies)
            {
                list.Add(string.Format("{0}={1}", cookie.Name, cookie.Value));
            }
            return string.Join(";", list);
        }
        public string GetCookieValue(string name)
        {
            var cookie = _webDriver.Manage().Cookies.GetCookieNamed(name);
            if (cookie == null)
            {
                return string.Empty;
            }
            return cookie.Value;
        }

        public void Close()
        {
            _webDriver.Close();
        }

        public void Quit()
        {
            try
            {
                _webDriver.Quit();
            }
            catch { }

        }

        public void CloseWindow()
        {
            var windows = _webDriver.WindowHandles.ToList();
            for (int i = windows.Count - 1; i >= 0; i--)
            {
                try
                {
                    _webDriver.SwitchTo().Window(windows[i]);
                    _webDriver.Close();
                    try
                    {
                        var alert = _webDriver.SwitchTo().Alert();
                        if (alert != null)
                        {
                            Console.WriteLine(alert.Text);
                            alert.Accept();
                        }
                    }
                    catch { }
                }
                catch (Exception)
                {
                }
            }
        }


        /// <summary>
        /// 检查某个元素是否存在于页面的DOM上,这并不一定意味着元素是可见的。
        /// </summary>
        public bool WaitForElementExists(By by, int timeOut)
        {
            try
            {
                new WebDriverWait(_webDriver, TimeSpan.FromSeconds(timeOut)).Until(ExpectedConditions.ElementExists(by));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WaitFor(Func<IWebDriver, bool> condition, int timeOut)
        {
            try
            {
                new WebDriverWait(_webDriver, TimeSpan.FromSeconds(timeOut)).Until(condition);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public bool QQLogin(string username, string password, out string cookies)
        {
            Console.WriteLine("开始检测QQ登录");
            cookies = string.Empty;
            var pageSource = string.Empty;
            var url = $"https://v.qq.com/u/history/?r={DateTime.Now.Ticks}";
            Console.WriteLine($"跳转url:{url}");
            _webDriver.Navigate().GoToUrl(url);
            Thread.Sleep(2000);
            //等待 qq登录frame
            if (!WaitForElementExists(By.Id("login_win_type"), 15))
            {
                pageSource = _webDriver.PageSource;
                Console.WriteLine($"没有弹出登录,可能已登录");
                //如果没有弹出qq登录窗，可能是已经登录了
                if (!WaitForElementExists(By.ClassName("_vip_desc"), 15))
                {
                    return false;
                }
                var text = _webDriver.FindElement(By.ClassName("_vip_desc")).Text;
                Console.WriteLine(text);
                if (text.Contains("到期"))
                {
                    cookies = GetAllCookiesTexts();
                    return true;
                }
                return false;
            }
            pageSource = _webDriver.PageSource;
            //选择qq登录
            _webDriver.FindElement(By.ClassName("btn_qq")).Click();
            //qq快速登录iframe
            if (!WaitForElementExists(By.Id("_login_frame_quick_"), 15))
            {
                return false;
            }
            _webDriver.SwitchTo().Frame("_login_frame_quick_");
            //快速安全登录iframe
            if (!WaitForElementExists(By.Id("ptlogin_iframe"), 15))
            {
                return false;
            }
            _webDriver.SwitchTo().Frame("ptlogin_iframe");
            //pageSource = _webDriver.PageSource;
            //选择账号密码登录
            _webDriver.FindElement(By.Id("switcher_plogin")).Click();
            Console.WriteLine("选择账号密码登录");
            Thread.Sleep(2000);
            //登录
            _webDriver.FindElement(By.Id("u")).SendKeys(username);
            _webDriver.FindElement(By.Id("p")).SendKeys(password);
            Thread.Sleep(2000);
            _webDriver.FindElement(By.Id("login_button")).Click();
            Console.WriteLine("点击登录");
            Thread.Sleep(2000);
            //pageSource = _webDriver.PageSource;
            //Console.WriteLine(pageSource);
            //回到 parent window
            _webDriver.SwitchTo().DefaultContent();
            //刷新页面
            _webDriver.Navigate().Refresh();
            //根据到期时间判断 是否登录成功
            if (!WaitFor(new Func<IWebDriver, bool>(x => !x.FindElement(By.ClassName("_vip_desc")).Text.Contains("开通")), 15))
            {
                return false;
            }
            Console.WriteLine("登录成功");
            //用户名
            var name = _webDriver.FindElement(By.ClassName("__nickname")).Text;
            Console.WriteLine($"用户名：{name}");
            //到期时间
            var vip_time = _webDriver.FindElement(By.ClassName("_vip_desc")).Text;
            Console.WriteLine($"到期时间：{vip_time}");
            //登录后cookie
            cookies = GetAllCookiesTexts();
            Console.WriteLine($"cookie:{cookies}");
            return true;

        }

    }
}
