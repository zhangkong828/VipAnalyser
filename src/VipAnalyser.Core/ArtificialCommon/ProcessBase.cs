using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VipAnalyser.ClassCommon;

namespace VipAnalyser.Core
{
    /// <summary>
    /// 每个操作流程会使用一个新的
    /// </summary>
    public class ProcessBase
    {
        /// <summary>
        /// 需要交互的页面
        /// </summary>
        protected DriverForm _form;

        /// <summary>
        /// 请求参数
        /// </summary>
        protected ArtificialParamModel _paramModel;

        /// <summary>
        /// 标识是否已经退出此操作
        /// </summary>
        protected bool _isQuit = false;

        /// <summary>
        /// 标记是否在结束时关闭此窗口
        /// </summary>
        protected bool _isCloseForm = false;

        /// <summary>
        /// 标记是否在结束时关闭程序
        /// </summary>
        protected bool _isShutdown = false;

        /// <summary>
        /// 尝试重新进入的默认次数
        /// </summary>
        protected int _tryTimesDefault = 3;

        /// <summary>
        /// 用于记录操作日志
        /// </summary>
        StringBuilder _sb = new StringBuilder();

        public ProcessBase(DriverForm form, ArtificialParamModel paramModel)
        {
            _form = form;
            _paramModel = paramModel;

            _sb.AppendLine(JsonConvert.SerializeObject(paramModel.Param));
        }

        /// <summary>
        /// 设置返回结果,即执行结束
        /// </summary>
        protected void SetResult(string result)
        {
            _isQuit = true;
            if (_form != null)
                _form.Result = result;
        }

        /// <summary>
        /// 显示并记录操作信息
        /// </summary>
        protected void RecordLog(string format, params object[] args)
        {
            RecordLog(string.Format(format, args));
        }
        protected void RecordLog(string txt)
        {
            if (_form != null)
            {
                _form._excuteRecord(txt);

                var log = string.Format(" --> T[{0}] {1} {2}", Thread.CurrentThread.ManagedThreadId, DateTime.Now.ToLongTimeString(), txt);
                _sb.AppendLine(log);
            }
        }

        /// <summary>
        /// 结束时必须调用
        /// </summary>
        protected void End(bool isWorkGoOn = false)
        {
            if (!isWorkGoOn)
            {
                _isQuit = true;
                RecordLog("end 耗时:{0}", CommonCla.GetMilliseconds(_paramModel.StartTime));
                _sb.Append(_form.Result);
                Logger.Info(_sb.ToString());

                _form.LastUsedTime = DateTime.Now.Ticks;
                if (_isCloseForm)
                    _form.Close();
                else
                    _form.IsWorking = false;

                if (_isShutdown)
                    MainForm.IsShutdown = true;

                _form = null;
            }
        }


    }
}