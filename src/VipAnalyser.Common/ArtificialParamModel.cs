using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VipAnalyser.Common
{
    public class ArtificialParamModel
    {
        public string Method { get; set; }

        public object Param { get; set; }

        /// <summary>
        /// 用于超时
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// ticks格式
        /// 用于超时
        /// </summary>
        public long StartTime { get; set; }

        /// <summary>
        /// 唯一标识的key
        /// 用于必要时的中止
        /// </summary>
        public string StopKey { get; set; }
    }
}
