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
                c.SetServiceName("服务名称");
                c.SetDisplayName("显示名称");
                c.SetDescription("描述");

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
