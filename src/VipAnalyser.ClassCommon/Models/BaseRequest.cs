using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VipAnalyser.ClassCommon.Models
{
    public class BaseRequest
    {
        public BaseRequest()
        {
            RequestKey = Guid.NewGuid().ToString();
            TimeOut = 30;
        }

        /// <summary>
        /// 请求唯一标识key
        /// </summary>
        public string RequestKey { get; set; }

        /// <summary>
        /// 超时时间(秒)
        /// </summary>
        public int TimeOut { get; set; }
    }
}
