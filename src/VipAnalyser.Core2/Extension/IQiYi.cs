using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VipAnalyser.ClassCommon.Models;

namespace VipAnalyser.Core2.Extension
{
    public class IQiYi : IAnalyser
    {
        public VideoAnalyseResponse Analyse(string url)
        {
            return new VideoAnalyseResponse() { ErrMsg = "iqiyi" };
        }

    }
}
