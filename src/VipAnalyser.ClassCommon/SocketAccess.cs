using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;

namespace VipAnalyser.ClassCommon
{
    public class SocketAccess
    {
        public static T2 Access<T1, T2>(string method, T1 param, int timeout, string stopkey, int port)
        {
            Socket socket = null;
            try
            {
                IPEndPoint ipe;
                socket = SocketHelper.GetSocket(out ipe, port, "127.0.0.1");
                SocketHelper.Connect(socket, ipe, 3); //这里是内网
                if (!socket.Connected)
                    throw new Exception("未启动/已关闭 无法连接");

                ArtificialParamModel paramModel = new ArtificialParamModel();
                paramModel.Method = method;
                paramModel.Param = param;
                paramModel.Timeout = timeout;
                paramModel.StartTime = DateTime.Now.Ticks;
                paramModel.StopKey = !string.IsNullOrWhiteSpace(stopkey) ? stopkey : Guid.NewGuid().ToString();
                string dataParam = JsonConvert.SerializeObject(paramModel);

                string result = string.Empty;
                try
                {
                    SocketHelper.Send(socket, dataParam, 3); //这里是内网
                    result = SocketHelper.Receive(socket, 0, timeout - 1); //让返回优先
                }
                catch
                {
                    throw new Exception("请求被意外中断");
                }

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
            finally
            {
                if (socket != null)
                    socket.Close();
            }
        }
    }
}
