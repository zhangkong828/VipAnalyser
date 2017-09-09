using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VipAnalyser.Core2
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class AnalysisTypeAttribute : Attribute
    {
        public SiteCode Type { get; set; }
    }
}
