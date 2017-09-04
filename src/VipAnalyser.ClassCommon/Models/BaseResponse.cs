using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VipAnalyser.ClassCommon.Models
{
    public class BaseResponse
    {
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }
        public string Version { get; set; }
    }
}
