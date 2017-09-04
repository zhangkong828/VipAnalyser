using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VipAnalyser.ClassCommon;

namespace VipAnalyser.Core
{
    public partial class DriverForm : Form
    {
        /// <summary>
        /// 浏览器执行驱动
        /// </summary>
        //public MyWebDriver MyDriver { get; set; }

        public bool IsDispose { get; set; }

        public readonly bool _isShowForm;
        public readonly ChromiumWebBrowser _browser;
        public readonly Action<string> _excuteRecord;

        /// <summary>
        /// 执行事件完成后返回的结果,未完成时为null
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 处理程序,每次须重新创建
        /// </summary>
        IProcessBase _process;

        /// <summary>
        /// 需要辅助方法执行的代码
        /// </summary>
        Queue<Action> _assistActions = new Queue<Action>();

        /// <summary>
        /// 标记是否正在使用
        /// </summary>
        public bool IsWorking = false;

        /// <summary>
        /// 上一次使用完的时间(时间戳)
        /// </summary>
        public long LastUsedTime = 0;

        public DriverForm(bool isShow, Action<string> record)
        {
            InitializeComponent();

            _isShowForm = isShow;
            if (!isShow)
            {
                //启动时设置不显示窗口
                this.ShowInTaskbar = false;
                this.WindowState = FormWindowState.Minimized;

                //设置不在Alt+tab键中显示
                FormHelper.HideTabAltMenu(this.Handle);
            }

            _excuteRecord = record;
            _browser = new ChromiumWebBrowser("about:blank");

            this.Controls.Add(_browser);
            _browser.Dock = DockStyle.Fill;

            this.IsDispose = false;
        }

        private void DriverForm_Load(object sender, EventArgs e)
        {
            try
            {
                //MyDriver = new MyWebDriver(_browser);

                ////默认全局设置
                //MyDriver.SetPageLoadTimeout();
                //MyDriver.SetScriptTimeout();

                //if (!_isShowForm)
                //    MyDriver.driver.Manage().Window.Position = new Point(10000, 10000);

                _excuteRecord("已完成浏览器初始化");
            }
            catch (Exception ex)
            {
                _excuteRecord(ex.Message);
                _excuteRecord("浏览器初始化失败");

                this.IsDispose = true;
                this.Close();
            }
        }

        /// <summary>
        /// 辅助方法,当需要借助主线程进行某种操作时
        /// </summary>
        public void StartAssist()
        {
            var timer = new TimerHelp();
            timer.ExcuteCompleted += Assist_timer_ExcuteCompleted;
            timer.Start();
        }
        /// <summary>
        /// 增加方法到辅助方法中进行执行
        /// </summary>
        public void AddAssistAction(Action action)
        {
            lock (_assistActions)
            {
                _assistActions.Enqueue(action);
            }
        }
        void Assist_timer_ExcuteCompleted(TimerHelp timer, object param, bool isCancel)
        {
            lock (_assistActions)
            {
                while (_assistActions.Count > 0)
                {
                    try
                    {
                        var action = _assistActions.Dequeue();
                        action();
                    }
                    catch (Exception ex)
                    {
                        _excuteRecord("Assist:" + ex.Message);
                    }
                }
            }

            if (!this.IsDispose)
                StartAssist();
        }

        /// <summary>
        /// 设置操作类型,也即设置处理事件
        /// </summary>
        public void SetActionType(ArtificialParamModel paramModel)
        {
            //得到处理程序,若有异常直接抛出
            _process = ProcessFactory.GetProcessByMethod(this, paramModel);

            //赋值为null
            this.Result = null;

            //开始执行
            _process.Begin();
        }

        /// <summary>
        /// 执行完成之后的退出
        /// </summary>
        public void Quit()
        {
            //结束执行
            if (_process != null)
                _process.End();
        }

        private void DriverForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.IsDispose = true;
        }
    }
}
