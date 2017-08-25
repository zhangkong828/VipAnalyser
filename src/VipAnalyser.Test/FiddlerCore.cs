using Fiddler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace VipAnalyser.Test
{
    public class FiddlerCore
    {
        public static void Start()
        {
            //Fiddler.FiddlerApplication.Shutdown();


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
            FiddlerApplication.Startup(8877, true, true);

            Console.WriteLine("监听已启动...");
        }

        private static void FiddlerApplication_BeforeResponse(Fiddler.Session oSession)
        {
           
            if (oSession.fullUrl.Contains("getvinfo"))
            {
                Console.WriteLine(oSession.fullUrl);
                //var body = oSession.ResponseBody;
                //Console.WriteLine(oSession.GetResponseBodyAsString());
            }

        }


    }
}
