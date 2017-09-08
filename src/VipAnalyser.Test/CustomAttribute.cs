using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VipAnalyser.Test
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class CustomAttribute : Attribute
    {
        public string Type { get; set; }
    }
}
