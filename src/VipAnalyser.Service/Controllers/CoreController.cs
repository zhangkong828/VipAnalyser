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

namespace VipAnalyser.Service.Controllers
{
    public class CoreController : ApiController
    {
        string Version
        {
            get { return ConfigurationManager.AppSettings["ClientVersion"]; }
        }


        [Route("/api/core/decode")]
        [HttpGet]
        public VideoAnalyseResponse Decode(VideoAnalyseRequest request)
        {
            var response = new VideoAnalyseResponse();
            try
            {
                //校验url正确性
                //判断来源


                var result = SocketAccess.Access<string, string>(
                            "Decode",
                            request.Url,
                            request.TimeOut,
                            Guid.NewGuid().ToString(),
                            6666);

            }
            catch (Exception ex)
            {
                response.ErrCode = -1;
                response.ErrMsg = JsonConvert.SerializeObject(ex);
            }
            response.Version = this.Version;
            return response;
        }

        [Route("/api/proxy/push")]
        [HttpPost]
        public bool Push([FromBody]string param)
        {
            if (string.IsNullOrEmpty(param))
            {
                return false;
            }

            return true;
        }
    }
}
