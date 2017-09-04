using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VipAnalyser.ClassCommon;

namespace VipAnalyser.Core
{
    public partial class MainForm : Form
    {

        /// <summary>
        /// 显示还是隐藏窗口
        /// </summary>
        private bool IsShowForm { get; set; }
        /// <summary>
        /// 每个实例维持登录的时间间隔
        /// </summary>
        private int TryLoginTime { get; set; }
        /// <summary>
        /// 初始化启动实例数
        /// </summary>
        private int LoadCount { get; set; }
        /// <summary>
        /// 最大实例数
        /// </summary>
        private int MaxCount { get; set; }

        private ChromiumWebBrowser _browser;

        /// <summary>
        /// 端口号
        /// </summary>
        private int Port { get; set; }

        private List<DriverForm> DriverForms = new List<DriverForm>();

        static int _processId = System.Diagnostics.Process.GetCurrentProcess().Id;

        /// <summary>
        /// 标记是否关闭程序
        /// </summary>
        public static bool IsShutdown = false;

        public MainForm(bool isShow = false, int tryLoginTime = 10 * 60, int loadCount = 5, int maxCount = 50)
        {
            InitializeComponent();

            InitializeChromium();

            IsShowForm = isShow;
            TryLoginTime = tryLoginTime;
            LoadCount = loadCount;
            MaxCount = maxCount;

            if (!isShow)
            {
                //启动时设置不显示窗口
                this.ShowInTaskbar = false;
                this.WindowState = FormWindowState.Minimized;

                //设置不在Alt+tab键中显示
                FormHelper.HideTabAltMenu(this.Handle);
            }

            //允许控件跨线程访问
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        public void InitializeChromium()
        {
            CefSettings settings = new CefSettings();
            settings.Locale = "zh-CN";
            settings.CachePath = @"Chromium\Cache";
            settings.CefCommandLineArgs.Add("ppapi-flash-path", @"Plugins\pepflashplayer32_26_0_0_151.dll");
            Cef.Initialize(settings);


            _browser = new ChromiumWebBrowser("about:blank");

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                //使用这个避免程序崩溃无法关闭
                this.Text = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

                //得到端口号
                string[] CmdArgs = Environment.GetCommandLineArgs();
                if (CmdArgs[0].StartsWith("port:"))
                    Port = int.Parse(CmdArgs[0].Split(':')[1]);
                else
                    Port = 6666;

                //开启登录监测
                StartLoginMonitor();

                //开启监听调用
                StartListen();

                //初始化执行程序
                for (int i = 0; i < LoadCount; i++)
                    DriverForms.Add(CreateExcuteForm());


                ExcuteRecord("程序正常启动");
            }
            catch (Exception ex)
            {
                ExcuteRecord("初始化发生异常:" + ex.Message);
                ExcuteRecord("3秒后自动关闭...");

                Task.Run(() =>
                {
                    Thread.Sleep(1000 * 3);
                    Environment.Exit(0);
                });
            }
        }



        /// <summary>
        /// 开启登录监测
        /// </summary>
        void StartLoginMonitor()
        {
            ExcuteRecord("开启登录监测");
            Task.Run(() =>
            {
                Thread.Sleep(1000 * 5);
                while (true)
                {
                    try
                    {
                        foreach (var form in DriverForms.ToArray())
                        {
                            if (!form.IsDispose)
                            {
                                Monitor.Enter(form);
                                {
                                    try
                                    {
                                        if (!form.IsWorking)
                                            form.IsWorking = true;
                                        else
                                            continue;

                                        ArtificialParamModel paramModel = new ArtificialParamModel();
                                        paramModel.Method = "Login";
                                        paramModel.Param = string.Empty;
                                        paramModel.Timeout = 60;
                                        paramModel.StartTime = DateTime.Now.Ticks;
                                        paramModel.StopKey = Guid.NewGuid().ToString();
                                        string dataParam = JsonConvert.SerializeObject(paramModel);

                                        ExcuteAndResult(new ArtificialResultModel(), form, dataParam, paramModel);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error("尝试登录时未知异常", ex);
                                    }
                                    finally
                                    {
                                        Monitor.Exit(form);
                                    }

                                    if (IsShutdown)
                                    {
                                        Environment.Exit(0);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("监测登录状态时未知异常", ex);
                    }

                    Thread.Sleep(1000 * TryLoginTime);
                }
            });
        }

        /// <summary>
        /// 开启监听调用
        /// </summary>
        void StartListen()
        {
            IPEndPoint ipe;
            Socket serverSocket = SocketHelper.GetSocket(out ipe, Port);
            serverSocket.Bind(ipe);
            serverSocket.Listen(MaxCount);

            Task.Run(() =>
            {
                Thread.Sleep(1000 * 10);
                while (true)
                {
                    try
                    {
                        if (IsShutdown)
                        {
                            serverSocket.Dispose();
                            return;
                        }

                        Socket cSocket = serverSocket.Accept();
                        Task.Factory.StartNew((c) =>
                        {
                            var socket = c as Socket;
                            try
                            {
                                string recvStr = SocketHelper.Receive(socket, 15);
                                ArtificialResultModel resultModel = Excute(recvStr);
                                SocketHelper.Send(socket, JsonConvert.SerializeObject(resultModel), 15);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error("StartListen-Excute:" + ex.Message);
                            }
                            finally
                            {
                                socket.Dispose();
                            }

                            if (IsShutdown)
                                Environment.Exit(0);

                        }, cSocket);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("StartListen:" + ex.Message);
                        Thread.Sleep(1000);
                    }
                }
            });

            ExcuteRecord("建立监听完成!端口:" + Port);
        }

        ArtificialResultModel Excute(string dataParam)
        {
            var resultModel = new ArtificialResultModel();
            resultModel.IsSuccess = false;
            resultModel.Result = null;

            DriverForm form = null;
            DriverForm newform = null;
            try
            {
                lock (DriverForms)
                {
                    if (!IsShutdown)
                    {
                        foreach (var f in DriverForms.ToArray().OrderBy(e => e.LastUsedTime))
                        {
                            if (!f.IsDispose)
                            {
                                if (Monitor.TryEnter(f))
                                {
                                    if (!f.IsWorking)
                                    {
                                        f.IsWorking = true;
                                        form = f;
                                        break;
                                    }
                                    else
                                        Monitor.Exit(f);
                                }
                            }
                        }

                        if (form == null)
                        {
                            if (DriverForms.Count < MaxCount)
                            {
                                Task.WaitAll(Task.Run(() =>
                                {
                                    newform = CreateExcuteForm();
                                }));
                            }
                        }
                    }
                    else
                        resultModel.Result = ArtificialCode.A_ProgramNoStarted.ToString();
                }

                if (newform != null)
                {
                    int times = 10;
                    while (times > 0)
                    {
                        if (newform._browser != null)
                            break;

                        Thread.Sleep(1000);
                        times--;
                    }

                    if (!newform.IsDispose)
                    {
                        form = newform;
                        Monitor.Enter(form);
                        form.IsWorking = true;

                        DriverForms.Add(newform);
                    }
                }

                if (form != null)
                {
                    ArtificialParamModel paramModel = JsonConvert.DeserializeObject<ArtificialParamModel>(dataParam);
                    ExcuteAndResult(resultModel, form, dataParam, paramModel);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Excute:" + ex.Message);
                resultModel.Result = ArtificialCode.A_UnknownException.ToString();
            }
            finally
            {
                if (form != null)
                    Monitor.Exit(form);
            }

            return resultModel;
        }

        /// <summary>
        /// 设置执行并返回结果
        /// </summary>
        void ExcuteAndResult(ArtificialResultModel resultModel, DriverForm form, string dataParam, ArtificialParamModel paramModel)
        {
            try
            {
                //设置并执行相应操作
                form.SetActionType(paramModel);

                try
                {
                    //得到执行结果
                    while (!CommonCla.IsTimeout(paramModel.StartTime, paramModel.Timeout - 1))
                    {
                        if (form.Result == null)
                        {
                            if (!form.IsDispose)
                            {
                                MonitorStopExcute(paramModel.StopKey);
                                Thread.Sleep(100);
                            }
                            else
                                throw new Exception(ArtificialCode.A_RequestAccidentBreak.ToString());
                        }
                        else
                        {
                            resultModel.Result = form.Result;
                            break;
                        }
                    }

                    if (resultModel.Result != null)
                        resultModel.IsSuccess = true;
                    else
                        throw new Exception(ArtificialCode.A_TimeOutResult.ToString());
                }
                catch (Exception ex)
                {
                    resultModel.Result = ex.Message;
                    Logger.Error(string.Format("{0}\r\n{1}", dataParam, ex.Message));
                }
                finally
                {
                    try
                    {
                        form.Quit();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(string.Format("End()\r\n{0}\r\n{1}", dataParam, ex.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                //如果Start发生异常
                form.IsWorking = false;

                resultModel.Result = ex.Message;
                Logger.Error(string.Format("Start()\r\n{0}\r\n{1}", dataParam, ex.Message));
            }
        }

        /// <summary>
        /// 中断请求的监视
        /// </summary>
        void MonitorStopExcute(string guidkey)
        {
            if (CommonCla.CacheIsHave(guidkey))
            {
                CommonCla.RemoveCache(guidkey);
                throw new Exception(ArtificialCode.A_RequestNormalBreak.ToString());
            }
        }

        /// <summary>
        /// 打开调试窗口,一定是显示状态下
        /// </summary>
        private void btn_test_Click(object sender, EventArgs e)
        {
            CreateExcuteForm();
        }

        /// <summary>
        /// 得到一个在线程中的处理窗口
        /// </summary>
        DriverForm CreateExcuteForm()
        {
            DriverForm form = null;
            Thread newThread = new Thread((() =>
            {
                form = new DriverForm(IsShowForm, ExcuteRecord);

                try
                {
                    form.StartAssist();
                    form.ShowDialog();
                }
                catch (Exception ex)
                {
                    form.Close();
                    ExcuteRecord(ex.Message);
                }
                finally
                {
                    lock (DriverForms)
                        DriverForms.Remove(form);
                }
            }));
            newThread.SetApartmentState(ApartmentState.STA);
            newThread.IsBackground = true; //随主线程一同退出
            newThread.Start();

            while (form == null)
                Thread.Sleep(100);

            return form;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsShutdown = true;

            lock (DriverForms)
            {
                foreach (var form in DriverForms)
                    form.Close();
            }
            Cef.Shutdown();
        }

        /// <summary>
        /// 显示执行时会造成程序无法响应,隐藏时没有问题
        /// </summary>
        private void ExcuteRecord(string msg)
        {
            try
            {
                lock (rtb_record)
                {
                    if (rtb_record.Text.Length > 1000)
                        rtb_record.Clear();

                    rtb_record.AppendText(string.Format("T[{0}] {1}\r\n", Thread.CurrentThread.ManagedThreadId, msg));
                    rtb_record.ScrollToCaret();
                }
            }
            catch { }
        }
    }
}
