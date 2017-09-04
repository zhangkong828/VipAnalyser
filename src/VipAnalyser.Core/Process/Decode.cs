using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VipAnalyser.ClassCommon;
using VipAnalyser.ClassCommon.Models;

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
            _form._browser.Load("https://www.cnblogs.com/");
        }

        public void End()
        {
            //base._isCloseForm = true;
            base.End();
        }
    }
}
