using OpenQA.Selenium;
using OpenQA.Selenium.Html5;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VipAnalyser.Test
{
    public class PhantomJSHelper
    {
        private static PhantomJSDriver _webDriver = null;
        private static PhantomJSHelper _instance = null;
        private static readonly object obj = new object();

        public static PhantomJSHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (obj)
                    {
                        if (_instance == null)
                        {
                            _instance = new PhantomJSHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        private PhantomJSHelper()
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
            //日志文件路径，为空则 console window
            _service.LogFile = "";

            _webDriver = new PhantomJSDriver(_service, _option);
        }



        /// <summary>
        /// 获取页面源码
        /// </summary>
        public string GetPageSource()
        {
            return _webDriver.PageSource;
        }

        /// <summary>
        /// 打开页面
        /// </summary>
        public void GoToUrl(string url)
        {
            _webDriver.Navigate().GoToUrl(url);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            _webDriver.Navigate().Refresh();
        }

        /// <summary>
        /// 后退
        /// </summary>
        public void Back()
        {
            _webDriver.Navigate().Back();
        }

        /// <summary>
        /// 前进
        /// </summary>
        public void Forward()
        {
            _webDriver.Navigate().Forward();
        }

        /// <summary>
        /// 获取当前url
        /// </summary>
        public string GetCurrentUrl()
        {
            return _webDriver.Url;
        }

        /// <summary>
        /// 获取当前页面title
        /// </summary>
        public string GetPageTitle()
        {
            return _webDriver.Title;
        }

        public string GetCurrentWindowHandle()
        {
            return _webDriver.CurrentWindowHandle;
        }

        public List<string> GetWindowHandles()
        {
            return _webDriver.WindowHandles.ToList();
        }

        /// <summary>
        /// 将焦点设置为具有指定标题的浏览器窗口
        /// </summary>
        /// <param name="title"></param>
        /// <param name="exactMatch">true:完全匹配  false:包含</param>
        public void GoToWindow(string title, bool exactMatch = false)
        {
            string theCurrent = _webDriver.CurrentWindowHandle;
            IList<string> windows = _webDriver.WindowHandles;
            if (exactMatch)
            {
                foreach (var window in windows)
                {
                    _webDriver.SwitchTo().Window(window);
                    if (_webDriver.Title.ToLower() == title.ToLower())
                    {
                        return;
                    }
                }
            }
            else
            {
                foreach (var window in windows)
                {
                    _webDriver.SwitchTo().Window(window);
                    if (_webDriver.Title.ToLower().Contains(title.ToLower()))
                    {
                        return;
                    }
                }
            }

            _webDriver.SwitchTo().Window(theCurrent);
        }

        public PhantomJSDriver GoToWindow(string handle)
        {
            return _webDriver.SwitchTo().Window(handle) as PhantomJSDriver;
        }

        public IWebDriver Frame(string name)
        {
            return _webDriver.SwitchTo().Frame(name);
        }

        public IWebDriver Frame(int index)
        {
            return _webDriver.SwitchTo().Frame(index);
        }

        public IWebDriver Frame(IWebElement frameElement)
        {
            return _webDriver.SwitchTo().Frame(frameElement);
        }

        public IWebDriver ParentFrame()
        {
            return _webDriver.SwitchTo().ParentFrame();
        }

        public IWebDriver DefaultContent()
        {
            return _webDriver.SwitchTo().DefaultContent();
        }

        public void ExecuteScript(string js)
        {
            _webDriver.ExecuteScript(js, null);
        }
        public void ExecuteScript(string js, params object[] args)
        {
            _webDriver.ExecuteScript(js, args);
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

        /// <summary>
        /// 检查一个元素在页面的DOM上可见的，可见性意味着该元素不仅显示，而且具有大于0的高度和宽度。
        /// </summary>
        public bool WaitForElementIsVisible(By by, int timeOut)
        {
            try
            {
                new WebDriverWait(_webDriver, TimeSpan.FromSeconds(timeOut)).Until(ExpectedConditions.ElementIsVisible(by));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查某个元素是不可见的或不存在的
        /// </summary>
        public bool WaitForInvisibilityOfElementLocated(By by, int timeOut)
        {
            try
            {
                new WebDriverWait(_webDriver, TimeSpan.FromSeconds(timeOut)).Until(ExpectedConditions.InvisibilityOfElementLocated(by));
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

        #region 查找元素


        public IWebElement FindElementBy(By by)
        {
            return _webDriver.FindElement(by);
        }

        public IWebElement FindElementById(string id)
        {
            return _webDriver.FindElementById(id);
        }

        public IWebElement FindElementByClassName(string className)
        {
            return _webDriver.FindElementByClassName(className);
        }

        public IWebElement FindElementByCssSelector(string cssSelector)
        {
            return _webDriver.FindElementByCssSelector(cssSelector);
        }

        public IWebElement FindElementByName(string name)
        {
            return _webDriver.FindElementByName(name);
        }

        public IWebElement FindElementByTagName(string tagName)
        {
            return _webDriver.FindElementByTagName(tagName);
        }

        public IWebElement FindElementByXPath(string xpath)
        {
            return _webDriver.FindElementByXPath(xpath);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return _webDriver.FindElements(by);
        }
        public ReadOnlyCollection<IWebElement> FindElementsById(string id)
        {
            return _webDriver.FindElementsById(id);
        }

        public ReadOnlyCollection<IWebElement> FindElementsByClassName(string className)
        {
            return _webDriver.FindElementsByClassName(className);
        }

        public ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector)
        {
            return _webDriver.FindElementsByCssSelector(cssSelector);
        }

        public ReadOnlyCollection<IWebElement> FindElementsByName(string name)
        {
            return _webDriver.FindElementsByName(name);
        }

        public ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName)
        {
            return _webDriver.FindElementsByTagName(tagName);
        }

        public ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath)
        {
            return _webDriver.FindElementsByXPath(xpath);
        }


        #endregion

        #region Screenshot截图

        public void ScreenshotBySave(string savePath, ScreenshotImageFormat imageFormat = ScreenshotImageFormat.Png)
        {
            _webDriver.GetScreenshot().SaveAsFile(savePath, imageFormat);
        }

        public string ScreenshotAsBase64EncodedString()
        {
            return _webDriver.GetScreenshot().AsBase64EncodedString;
        }

        public byte[] ScreenshotAsByteArray()
        {
            return _webDriver.GetScreenshot().AsByteArray;
        }


        #endregion

        #region Cookie

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
        public string GetCookieValue(string name)
        {
            var cookie = _webDriver.Manage().Cookies.GetCookieNamed(name);
            if (cookie == null)
            {
                return string.Empty;
            }
            return cookie.Value;
        }
        public void DeleteAllCookies()
        {
            _webDriver.Manage().Cookies.DeleteAllCookies();
        }
        public void DeleteCookieNamed(string name)
        {
            _webDriver.Manage().Cookies.DeleteCookieNamed(name);
        }
        public void AddCookie(string name, string value)
        {
            _webDriver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie(name, value));
        }
        public void AddCookie(string name, string value, string path)
        {
            _webDriver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie(name, value, path));
        }
        public void AddCookie(string name, string value, string path, DateTime? expiry)
        {
            _webDriver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie(name, value, path, expiry));
        }
        public void AddCookie(string name, string value, string domain, string path, DateTime? expiry)
        {
            _webDriver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie(name, value, domain, path, expiry));
        }


        #endregion

        #region 弹窗

        /// <summary>
        /// Alert,Confirm，Prompt，AuthenticationCredentials都实现了IAlert接口
        /// </summary>
        public IAlert SwitchToAlert()
        {
            return _webDriver.SwitchTo().Alert();
        }

        public string GetAlertString()
        {
            string theString = string.Empty;
            var alert = _webDriver.SwitchTo().Alert();
            if (alert != null)
            {
                theString = alert.Text;
            }
            return theString;
        }

        public void AlertAccept()
        {
            var alert = _webDriver.SwitchTo().Alert();
            if (alert != null)
            {
                alert.Accept();
            }
        }

        public void AlertDismiss()
        {
            var alert = _webDriver.SwitchTo().Alert();
            if (alert != null)
            {
                alert.Dismiss();
            }
        }

        #endregion

        public void ResetInputState()
        {
            _webDriver.ResetInputState();
        }

        /// <summary>
        /// 按键相关操作
        /// </summary>
        public IKeyboard Keyboard { get; }
        /// <summary>
        /// 鼠标相关操作
        /// </summary>
        public IMouse Mouse { get; }
        /// <summary>
        /// WebStorage相关操作
        /// </summary>
        public IWebStorage WebStorage { get; }

        public IOptions Manage
        {
            get
            {
                return _webDriver.Manage();
            }
        }

        /// <summary>
        /// 关闭浏览器
        /// </summary>
        public void Close()
        {
            _webDriver.Close();
        }


        /// <summary>
        /// 关闭浏览器和释放WebDriver
        /// </summary>
        public void Quit()
        {
            _webDriver.Quit();
        }

    }
}
