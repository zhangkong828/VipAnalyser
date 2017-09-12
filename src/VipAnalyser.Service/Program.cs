using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace VipAnalyser.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(c =>
            {
                c.SetServiceName("VipAnalyser");
                c.SetDisplayName("VipAnalyser视频解析服务");
                c.SetDescription("解析vip视频，提供视频解析后的真实地址源");

                c.RunAsLocalSystem();
                c.StartAutomatically();
                c.EnableShutdown();
                c.EnablePauseAndContinue();

                c.Service<CoreService>(s =>
                {
                    s.ConstructUsing(b => new CoreService());
                    s.WhenStarted(o => o.Start());
                    s.WhenStopped(o => o.Stop());
                });
            });
        }
    }
}
