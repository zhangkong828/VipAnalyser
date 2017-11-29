using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VipAnalyser.ClassCommon;

namespace VipAnalyser.LoginManager
{
    public class LoginMonitor
    {
        public static string QQCookies = string.Empty;

        private static PhantomJSDriverHelper driver;
        private static bool IsQuit = false;

        static LoginMonitor()
        {
            driver = new PhantomJSDriverHelper();
        }

        public static void QQ(string username, string password, int tryLoginTime = 10 * 60)
        {
            Task.Run(() =>
            {
                Thread.Sleep(1000 * 2);
                while (!IsQuit)
                {
                    try
                    {
                        int tryCount = 2;
                        var cookies = string.Empty;
                        QQLogin:
                        if (!driver.QQLogin(username, password, out cookies))
                        {
                            if (tryCount > 0)
                            {
                                tryCount--;
                                goto QQLogin;
                            }
                            Logger.Trace("QQ登录检测：登录失败");
                        }
                        else
                        {
                            QQCookies = cookies;
                            Logger.Trace($"QQ登录检测：登录成功, Cookies:{QQCookies}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Fatal("QQ登录检测未知异常", ex);
                    }
                    finally
                    {
                        try
                        {
                            driver.Close();
                        }
                        catch { }
                    }

                    Thread.Sleep(1000 * tryLoginTime);
                }
            });
        }

        public static void Quit()
        {
            IsQuit = true;
            try
            {
                driver.Quit();
            }
            catch { }
        }

    }
}
