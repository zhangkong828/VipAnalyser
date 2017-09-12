using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VipAnalyser.LoginManager
{
    public class PhantomJSDriverHelper
    {
        public PhantomJSDriver _webDriver;

        public PhantomJSDriverHelper()
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
            _webDriver.Quit();
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
            cookies = string.Empty;
            var pageSource = string.Empty;
            var url = "https://v.qq.com/u/history/";
            _webDriver.Navigate().GoToUrl(url);
            //等待 qq登录frame
            if (!WaitForElementExists(By.Id("login_win_type"), 15))
            {
                //如果没有弹出qq登录窗，可能是已经登录了
                //判断到期时间
                if (_webDriver.FindElement(By.ClassName("_vip_desc")).Text.Contains("到期"))
                {
                    cookies = GetAllCookiesTexts();
                    return true;
                }
                return false;
            }
            //选择qq登录
            _webDriver.FindElement(By.ClassName("btn_qq")).Click();
            //qq快速登录iframe
            if (!WaitForElementExists(By.Id("_login_frame_quick_"), 15))
            {
                return false;
            }
            _webDriver.SwitchTo().Frame("_login_frame_quick_");
            //登录方式：账号密码登录
            _webDriver.FindElement(By.Id("switcher_plogin")).Click();
            //登录
            _webDriver.FindElement(By.Id("u")).SendKeys(username);
            _webDriver.FindElement(By.Id("p")).SendKeys(password);
            _webDriver.FindElement(By.Id("login_button")).Click();
            //回到 parent window
            _webDriver.SwitchTo().DefaultContent();
            //刷新页面
            //_webDriver.Navigate().Refresh();
            pageSource = _webDriver.PageSource;
            //根据到期时间判断 是否登录成功
            if (!WaitFor(new Func<IWebDriver, bool>(x => !x.FindElement(By.ClassName("_vip_desc")).Text.Contains("开通")), 15))
            {
                return false;
            }

            //用户名
            var name = _webDriver.FindElement(By.ClassName("__nickname")).Text;
            //到期时间
            var vip_time = _webDriver.FindElement(By.ClassName("_vip_desc")).Text;
            //登录后cookie
            cookies = GetAllCookiesTexts();

            return true;

        }

    }
}
