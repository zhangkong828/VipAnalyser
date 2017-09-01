using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VipAnalyser.Core
{
    public class Configure
    {
        private static Configure _instance = new Configure();

        /// <summary>
        /// 只读
        /// </summary>
        public static Configure Instance
        {
            get
            {
                return _instance;
            }
            private set { }
        }

        private string GetValue(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            return value;
        }

        /// <summary>
        /// 是否显示窗体,默认0不显示
        /// </summary>
        public bool IsShowForm
        {
            get
            {
                return GetValue("IsShowForm") == "1";
            }
            private set { }
        }


        /// <summary>
        /// 腾讯视频 用户中心（用于登录）
        /// </summary>
        public string Url_V_QQ_Com_U_History
        {
            get { return "https://v.qq.com/u/history/"; }
            private set { }
        }


        public string UserName
        {
            get { return GetValue("UserName"); }
            private set { }
        }


        public string Password
        {
            get { return GetValue("Password"); }
            private set { }
        }
    }
}
