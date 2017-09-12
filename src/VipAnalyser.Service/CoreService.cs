using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using VipAnalyser.LoginManager;

namespace VipAnalyser.Service
{
    public class CoreService
    {
        private readonly HttpSelfHostServer server;
        private readonly HttpSelfHostConfiguration config;

        public CoreService()
        {

            var port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);

            config = new HttpSelfHostConfiguration("http://localhost:" + port);

            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            config.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{action}/{id}",
                new { id = RouteParameter.Optional });

            server = new HttpSelfHostServer(config);


        }
        public void Start()
        {
            //qq登录检测
            var qqUserName = ConfigurationManager.AppSettings["QQUserName"];
            var qqPassword = ConfigurationManager.AppSettings["QQPassword"];
            if (string.IsNullOrEmpty(qqUserName) || string.IsNullOrEmpty(qqPassword))
            {
                throw new Exception("账号密码不能为空");
            }
            LoginMonitor.QQ(qqUserName, qqPassword);

            server.OpenAsync();
        }

        public void Stop()
        {
            LoginMonitor.Quit();

            server.CloseAsync();
            server.Dispose();
        }
    }
}
