using System;
using System.Runtime.InteropServices;
using Cjwdev.WindowsApi;

namespace VipAnalyser.Common
{
    public class ProcessHelp
    {
        #region 用于释放浏览器占用的内存
        [DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetProcessWorkingSetSize(IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetCurrentProcess", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetCurrentProcess();

        public static void Collect()
        {
            //释放内存
            IntPtr pHandle = GetCurrentProcess();
            SetProcessWorkingSetSize(pHandle, -1, -1);

            GC.Collect();
        }
        #endregion

        //通过Windows Service启动外部程序
        public static int Start(string path, string arguments = "")
        {
            int _currentAquariusProcessID;

            string appStartpath = path;
            IntPtr userTokenHandle = IntPtr.Zero;
            ApiDefinitions.WTSQueryUserToken(ApiDefinitions.WTSGetActiveConsoleSessionId(), ref userTokenHandle);
            ApiDefinitions.PROCESS_INFORMATION procinfo = new ApiDefinitions.PROCESS_INFORMATION();
            ApiDefinitions.STARTUPINFO startinfo = new ApiDefinitions.STARTUPINFO();
            startinfo.cb = (uint)Marshal.SizeOf(startinfo);

            ApiDefinitions.CreateProcessAsUser(userTokenHandle,
                appStartpath,
                arguments,
                IntPtr.Zero,
                IntPtr.Zero,
                false,
                0,
                IntPtr.Zero,
                null,
                ref startinfo,
                out procinfo
                );

            if (userTokenHandle != IntPtr.Zero)
                ApiDefinitions.CloseHandle(userTokenHandle);

            _currentAquariusProcessID = (int)procinfo.dwProcessId;
            return _currentAquariusProcessID;
        }
    }
}
