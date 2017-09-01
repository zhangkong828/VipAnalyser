using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VipAnalyser.Core
{
    class HiddenForm
    {
        static int _processId = Process.GetCurrentProcess().Id;

        [DllImport("user32.dll")]
        public static extern int EnumWindows(EnumWindowsProc ewp, int lParam);
        public delegate bool EnumWindowsProc(int hWnd, int lParam);
        [DllImport("user32.dll")]
        private static extern int GetWindowText(int hWnd, StringBuilder title, int size);
        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(int hWnd);
        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(int hWnd);
        [DllImport("USER32.DLL")]
        private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        public static bool Callback(int hWnd, int lParam)
        {
            if (IsWindowVisible(hWnd))
            {
                var cTxtLen = GetWindowTextLength(hWnd) + 1;
                StringBuilder text = new StringBuilder(cTxtLen);
                GetWindowText(hWnd, text, cTxtLen);
                var cTitle = text.ToString();

                if (cTitle.Contains("壁虎车险-人工模拟-" + _processId) || cTitle.Contains(Application.StartupPath))
                {
                    ShowWindow((IntPtr)hWnd, 0);
                }
            }
            return true;
        }
    }
}