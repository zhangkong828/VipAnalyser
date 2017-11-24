using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VipAnalyser.ClassCommon
{
    public class SocketHelper
    {
        public static Socket GetSocket(out IPEndPoint ipe, int port, string ip = null)
        {
            if (string.IsNullOrEmpty(ip))
            {
                ipe = new IPEndPoint(IPAddress.Any, port);
            }
            else
            {
                ipe = new IPEndPoint(IPAddress.Parse(ip), port);
            }
            uint num = 0u;
            byte[] array = new byte[Marshal.SizeOf(num) * 3];
            BitConverter.GetBytes(1u).CopyTo(array, 0);
            BitConverter.GetBytes(15000u).CopyTo(array, Marshal.SizeOf(num));
            BitConverter.GetBytes(15000u).CopyTo(array, Marshal.SizeOf(num) * 2);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.IOControl(IOControlCode.KeepAliveValues, array, null);
            return socket;
        }

        public static bool Connect(Socket socket, IPEndPoint ipe, int timeout = 0)
        {
            if (timeout == 0)
            {
                while (!socket.Connected)
                {
                    try
                    {
                        socket.Connect(ipe);
                    }
                    catch
                    {
                    }
                }
                return true;
            }
            IAsyncResult asyncResult = socket.BeginConnect(ipe, null, null);
            asyncResult.AsyncWaitHandle.WaitOne(timeout * 1000);
            return socket.Connected;
        }

        public static string Receive(Socket socket, int timeout = 0, int receiveTimeout = 60)
        {
            long ticks = DateTime.Now.Ticks;
            socket.ReceiveTimeout = timeout * 1000;
            List<byte> list = new List<byte>();
            byte[] array = new byte[1024];
            while (true)
            {
                int num;
                if ((num = socket.Receive(array)) > 0)
                {
                    for (int i = 0; i < num; i++)
                    {
                        list.Add(array[i]);
                    }
                    if (num >= array.Length)
                    {
                        continue;
                    }
                }
                if (IsComplete(list))
                {
                    break;
                }
                if (timeout > 0)
                {
                    int timeout2 = GetTimeout(ticks, timeout);
                    if (timeout2 <= 0)
                    {
                        goto Block_4;
                    }
                    socket.ReceiveTimeout = timeout2 * 1000;
                }
                else
                {
                    timeout = receiveTimeout;
                    socket.ReceiveTimeout = timeout * 1000;
                }
            }
            return Encoding.Default.GetString(list.ToArray(), 0, list.Count);
            Block_4:
            throw new Exception("数据未能完整获取");
        }

        public static int Send(Socket socket, string msg, int timeout = 0)
        {
            socket.SendTimeout = timeout * 1000;
            byte[] bytes = Encoding.Default.GetBytes(msg + "#IZK");
            return socket.Send(bytes, bytes.Length, SocketFlags.None);
        }

        public static bool IsComplete(List<byte> data)
        {
            if (data.Count >= 4 && data[data.Count - 4] == 35 && data[data.Count - 3] == 81 && data[data.Count - 2] == 112 && data[data.Count - 1] == 112)
            {
                for (int i = 0; i < 4; i++)
                {
                    data.RemoveAt(data.Count - 1);
                }
                return true;
            }
            return false;
        }

        public static int GetTimeout(long starttime, int timeout)
        {
            return (int)((long)timeout - (DateTime.Now.Ticks - starttime) / 10000L / 1000L);
        }
    }
}
