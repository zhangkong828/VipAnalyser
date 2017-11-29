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
            response.Version = this.Version;
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
                response.ErrMsg = JsonConvert.SerializeObject(ex);
            }
            Logger.Info($"{JsonConvert.SerializeObject(request)} \r\n--->\r\n {JsonConvert.SerializeObject(response)}");
            return response;
        }


    }
}
