using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace VipAnalyser.Core
{
    public class CommonWb
    {
        #region 删除IE临时文件
        /// <summary> 
        /// 删除临时文件
        /// </summary>
        /// <param name="name">完整或缺省的文件名称</param>
        public static void CleanTempFiles(string name = "")
        {
            FolderClear(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache), name);

            //命令行清除
            //RunCmd("RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 8");
        }
        static void RunCmd(string cmd)
        {
            ProcessStartInfo process = new ProcessStartInfo();
            process.CreateNoWindow = false;
            process.UseShellExecute = false;
            process.WindowStyle = ProcessWindowStyle.Hidden;
            process.FileName = "cmd.exe";
            process.Arguments = "/c " + cmd;
            Process.Start(process);
        }
        static void FolderClear(string path, string name = "")
        {
            DirectoryInfo diPath = new DirectoryInfo(path);
            foreach (FileInfo fiCurrFile in diPath.GetFiles())
            {
                FileDelete(fiCurrFile.FullName, name);
            }
            foreach (DirectoryInfo diSubFolder in diPath.GetDirectories())
            {
                FolderClear(diSubFolder.FullName, name);
            }
        }
        static bool FileDelete(string path, string name = "")
        {
            FileInfo file = new FileInfo(path);
            FileAttributes att = 0;
            bool attModified = false;

            try
            {
                if (string.IsNullOrWhiteSpace(name) || file.Name.Contains(name))
                {
                    att = file.Attributes;
                    file.Attributes &= (~FileAttributes.ReadOnly);
                    attModified = true;
                    file.Delete();
                }
            }
            catch (Exception e)
            {
                if (attModified)
                    file.Attributes = att;
                return false;
            }
            return true;
        }
        #endregion

        #region 获取完整cookie
        private const int INTERNET_COOKIE_HTTPONLY = 0x00002000;

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetGetCookieEx(
            string url,
            string cookieName,
            StringBuilder cookieData,
            ref int size,
            int flags,
            IntPtr pReserved);

        /// <summary>
        /// 获取完整cookie
        /// </summary>
        /// <param name="url">当前处于登录状态</param>
        /// <returns></returns>
        public static string GetCookie(string url)
        {
            int size = 512;
            StringBuilder sb = new StringBuilder(size);
            if (!InternetGetCookieEx(url, null, sb, ref size, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero))
            {
                if (size < 0)
                {
                    return null;
                }
                sb = new StringBuilder(size);
                if (!InternetGetCookieEx(url, null, sb, ref size, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero))
                {
                    return null;
                }
            }
            return sb.ToString();
        }
        #endregion

        /// <summary>
        /// 下载图片
        /// 超时间单位:秒
        /// </summary>
        public static Bitmap DownloadBitmap(string url, int timeout = 15, string cookie = "")
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 1000 * timeout;
                request.Headers["Cookie"] = cookie;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream requestStream = response.GetResponseStream();
                if (requestStream != null)
                {
                    Bitmap sourceBm = new Bitmap(requestStream);
                    requestStream.Dispose();
                    response.Dispose();
                    return sourceBm;
                }
            }
            catch { }
            return null;
        }
    }
}