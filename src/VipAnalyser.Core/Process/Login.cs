using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VipAnalyser.ClassCommon;

namespace VipAnalyser.Core.Process
{
    public class Login : ProcessBase, IProcessBase
    {
        public Login(DriverForm form, ArtificialParamModel paramModel)
           : base(form, paramModel)
        { }

        public void Begin()
        {
            _form._browser.Load("http://v.qq.com/u/history/");
           
        }

        public void End()
        {
            base.End();
        }
    }
}
