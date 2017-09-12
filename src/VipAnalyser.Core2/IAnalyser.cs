using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VipAnalyser.ClassCommon.Models;

namespace VipAnalyser.Core2
{
    interface IAnalyser
    {
        VideoAnalyseResponse Analyse(string url, string cookie);
    }
}
