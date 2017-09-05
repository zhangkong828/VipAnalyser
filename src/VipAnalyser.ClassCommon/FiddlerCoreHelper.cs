using Fiddler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VipAnalyser.ClassCommon
{
    public class FiddlerCoreHelper
    {
        public static int _port = 0;
        public static string _urlRegexText = string.Empty;
        public static Action<string, string> _action;

        public static void Start(int port, string[] urlTexts, Action<string, string> action)
        {
            _port = port;
            _urlRegexText = string.Join("|", urlTexts);
            _action = action;

            //创建证书
            CONFIG.bCaptureCONNECT = true;
            CONFIG.IgnoreServerCertErrors = false;
            if (!CertMaker.rootCertExists())
            {
                if (!CertMaker.createRootCert())
                {
                    throw new Exception("Unable to create cert for FiddlerCore.");
                }
                X509Store certStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                certStore.Open(OpenFlags.ReadWrite);
                try
                {
                    certStore.Add(CertMaker.GetRootCertificate());
                }
                finally
                {
                    certStore.Close();
                }
            }

            FiddlerApplication.BeforeResponse += FiddlerApplication_BeforeResponse;
            FiddlerApplication.Startup(_port, true, true);

            Console.WriteLine("端口{0} 监听已启动...", _port);
        }

        public static void Close()
        {
            FiddlerApplication.Shutdown();
        }

        private static void FiddlerApplication_BeforeResponse(Session oSession)
        {
            if (Regex.IsMatch(oSession.fullUrl, _urlRegexText))
            {
                _action(oSession.fullUrl, oSession.GetResponseBodyAsString());
            }
        }

    }
}
