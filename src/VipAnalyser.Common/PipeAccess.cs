using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace VipAnalyser.Common
{
    /// <summary>
    /// 命令管道通信
    /// </summary>
    public class PipeAccess
    {
        /// <summary>
        /// 访问人工模拟程序
        /// </summary>
        /// <typeparam name="T1">参数类型</typeparam>
        /// <typeparam name="T2">返回类型</typeparam>
        /// <typeparam name="pipeName">命令管道名称</typeparam>
        /// <param name="method">调用方法名</param>
        /// <param name="param">参数</param>
        /// <param name="timeout">超时时间(秒)</param>
        /// <param name="stopkey">用于中止请求的key</param>
        /// <returns></returns>
        public static T2 Access<T1, T2>(string pipeName, string method, T1 param, int timeout, string stopkey)
        {
            if (string.IsNullOrWhiteSpace(method))
                throw new Exception(ArtificialCode.A_UnknownMethod.ToString());
            if (timeout <= 0)
                throw new Exception(ArtificialCode.A_TimeOutZero.ToString());

            ArtificialParamModel paramModel = new ArtificialParamModel();
            paramModel.Method = method;
            paramModel.Param = param;
            paramModel.Timeout = timeout;
            paramModel.StartTime = DateTime.Now.Ticks;
            paramModel.StopKey = !string.IsNullOrWhiteSpace(stopkey) ? stopkey : Guid.NewGuid().ToString();
            string dataParam = JsonConvert.SerializeObject(paramModel);

            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut))
            {
                //此处极为耗CPU
                pipeClient.Connect(1000);

                StreamString pipeSS = new StreamString(pipeClient);
                pipeSS.WriteString(dataParam);

                string result = string.Empty;
                while (true)
                {
                    result = pipeSS.ReadString();
                    if (result == "qpp")
                    {
                        if (CommonCla.CacheIsHave(paramModel.StopKey))
                        {
                            CommonCla.RemoveCache(paramModel.StopKey);
                            throw new Exception(ArtificialCode.A_RequestNormalBreak.ToString());
                        }
                    }
                    else
                        break;
                }

                //能到这里说明是正常返回的
                var resultModel = JsonConvert.DeserializeObject<ArtificialResultModel>(result);
                if (!resultModel.IsSuccess)
                    throw new Exception(resultModel.Result);

                try
                {
                    //如果返回的不是对应返回类型,则可能是想抛出此异常
                    return JsonConvert.DeserializeObject<T2>(resultModel.Result);
                }
                catch
                {
                    throw new Exception(resultModel.Result);
                }
            }
        }
    }
}
