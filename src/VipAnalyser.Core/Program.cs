using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VipAnalyser.Core
{
    static class Program
    {
        static Program()
        {
            int maxWorkerThreads, maxIoThreads;
            ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxIoThreads);
            ThreadPool.SetMinThreads(maxWorkerThreads, maxIoThreads);
        }
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //监测config配置文件变化
            MonitorConfig.Monitor(Application.StartupPath, new[] { "appSettings" });

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(Configure.Instance.IsShowForm, 10 * 60, 1));
        }
    }
}
