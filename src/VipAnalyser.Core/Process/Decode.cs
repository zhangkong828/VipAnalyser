using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VipAnalyser.ClassCommon;
using VipAnalyser.ClassCommon.Models;
using VipAnalyser.Core.Handler;

namespace VipAnalyser.Core.Process
{
    public class Decode : ProcessBase, IProcessBase
    {
        VideoDecodeRequest request;

        public Decode(DriverForm form, ArtificialParamModel paramModel)
           : base(form, paramModel)
        {
            request = JsonConvert.DeserializeObject<VideoDecodeRequest>(_paramModel.Param.ToString());
        }

        public void Begin()
        {
            _form._browser.RequestHandler = new CustomRequestHandler("vv.video.qq.com/getvinfo", new Action<byte[]>(x =>
            {
                ProcessResult(x);
            }));
            //https://v.qq.com/x/cover/kds9l8b75jvb6y6.html
            _form._browser.Load(request.Url);

        }

        private void ProcessResult(byte[] bytes)
        {
            var xml = Encoding.UTF8.GetString(bytes);
            VQQXmlModel qqXmlModel = null;
            try
            {
                qqXmlModel = XmlHelper.XmlDeserialize<VQQXmlModel>(xml, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                base.RecordLog(ex.Message);
            }
            var result = JsonConvert.SerializeObject(qqXmlModel);
            base.SetResult(result);
        }

        public void End()
        {
            //base._isCloseForm = true;
            base.End();
        }
    }
}
