using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using VipAnalyser.ClassCommon;
using VipAnalyser.ClassCommon.Models;
using VipAnalyser.Core2;
using VipAnalyser.LoginManager;

namespace VipAnalyser.Service.Controllers
{
    public class CoreController : ApiController
    {
        string Version
        {
            get { return ConfigurationManager.AppSettings["ClientVersion"]; }
        }


        [Route("/api/core/analyse")]
        [HttpPost]
        public VideoAnalyseResponse Analyse([FromBody]VideoAnalyseRequest request)
        {
            var response = new VideoAnalyseResponse();
            try
            {
                if (string.IsNullOrEmpty(request.Url))
                {
                    response.ErrCode = -1;
                    response.ErrMsg = "url不存在";
                }
                else
                {
                    response = AnalysisFactory.GetResponse(request.Url, LoginMonitor.QQCookies);
                }
            }
            catch (Exception ex)
            {
                response.ErrCode = -1;
                response.ErrMsg = "出现异常，请求失败";
                Logger.Fatal($"{ex.Message}\r\n{ex.StackTrace}");
            }
            response.Version = this.Version;
            Logger.Info($"--->request\r\n{JsonConvert.SerializeObject(request)}\r\n--->response\r\n{JsonConvert.SerializeObject(response)}");
            return response;
        }


    }
}
