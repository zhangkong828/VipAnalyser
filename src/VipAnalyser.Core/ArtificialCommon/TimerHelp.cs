using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VipAnalyser.Core
{
    public class TimerHelp
    {
        /// <summary>
        /// 创建一个定时器对象。
        /// </summary>
        /// <param name="times">指示定时时间，以毫秒为单位。</param>
        /// <param name="intervalTimes">指示定时之中的间歇时间，用于检查是否取消执行。</param>
        public TimerHelp(int times = 100, int intervalTimes = 100)
        {
            this.Times = times;
            this.IntervalTimes = intervalTimes;
        }

        /// <summary>
        /// 指示定时时间，以毫秒为单位。
        /// </summary>
        public int Times
        {
            get
            {
                return _times;
            }
            set
            {
                _times = value;
            }
        }
        private int _times;

        /// <summary>
        /// 指示定时之中的间歇时间，用于检查是否取消执行。
        /// </summary>
        public int IntervalTimes
        {
            get
            {
                return _intervalTimes;
            }
            set
            {
                _intervalTimes = value;
            }
        }
        private int _intervalTimes;

        private BackgroundWorker BackgroundWork
        {
            get
            {
                return _backgroundWork;
            }
            set
            {
                _backgroundWork = value;
            }
        }
        private BackgroundWorker _backgroundWork;

        private object Param
        {
            get
            {
                return _param;
            }
            set
            {
                _param = value;
            }
        }
        private object _param;

        /// <summary>
        /// 指示定时器是否处于运行状态
        /// </summary>
        public bool Excuting
        {
            get
            {
                return _excuting;
            }
        }
        private bool _excuting;

        /// <summary>
        /// 启动定时器，如果定时器已经启动，则引发异常。
        /// </summary>
        /// <param name="param">在定时完成时可能被使用到的传递对象。</param>
        public void Start(object param = null)
        {
            if (Excuting) throw new Exception("定时器已启动!");
            _excuting = true;
            this.Param = param;
            BackgroundWork = new BackgroundWorker();
            BackgroundWork.WorkerSupportsCancellation = true;
            BackgroundWork.DoWork += new DoWorkEventHandler(b_DoWork);
            BackgroundWork.RunWorkerCompleted += new RunWorkerCompletedEventHandler(b_RunWorkerCompleted);
            BackgroundWork.RunWorkerAsync(this);
        }

        /// <summary>
        /// 请求中止执行，如果定时器尚未启动，则引发异常。
        /// </summary>
        public void Stop(bool isCancle)
        {
            if (!Excuting) throw new Exception("定时器尚未启动!");
            this.IsCancle = isCancle;
            BackgroundWork.CancelAsync();
        }

        /// <summary>
        /// 达到定时事件代理
        /// </summary>
        public delegate void ExcuteCompletedAgency(TimerHelp timer, object param, bool isCancel);

        private bool IsCancle
        {
            get
            {
                return _isCancle;
            }
            set
            {
                _isCancle = value;
            }
        }
        private bool _isCancle;

        /// <summary>
        /// 达到定时事件
        /// </summary>
        public event ExcuteCompletedAgency ExcuteCompleted;

        void b_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null) throw e.Error;
            if (!IsCancle && ExcuteCompleted != null)
            {
                var o = e.Result as TimerHelp;
                ExcuteCompleted(o, o.Param, e.Cancelled);
            }
            _excuting = false;
            BackgroundWork.Dispose();
        }

        void b_DoWork(object sender, DoWorkEventArgs e)
        {
            var o = e.Argument as TimerHelp;
            e.Result = o;
            int x = 0;
            while (true)
            {
                if (x >= o.Times || (sender as BackgroundWorker).CancellationPending) break;
                Thread.Sleep(o.IntervalTimes);
                x += o.IntervalTimes;
            }
        }

        public static void Start(ExcuteCompletedAgency excuteCompleted, object param = null, int times = 100, int intervalTimes = 100)
        {
            var timer = new TimerHelp(times, intervalTimes);
            timer.ExcuteCompleted += excuteCompleted;
            timer.Start(param);
        }
    }
}