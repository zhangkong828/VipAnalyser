using System.Configuration;
using System.IO;
using System.Security.Permissions;

namespace VipAnalyser.Core
{
    /// <summary>
    /// 监测文件变化
    /// </summary>
    public class MonitorConfig
    {
        /// <summary>
        /// 监测config配置文件变化
        /// </summary>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Monitor(string path, string[] settings)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.config";

            watcher.Changed += new FileSystemEventHandler((source, e) =>
            {
                foreach (var setting in settings)
                {
                    ConfigurationManager.RefreshSection(setting);
                }
            });

            watcher.EnableRaisingEvents = true;
        }
    }
}