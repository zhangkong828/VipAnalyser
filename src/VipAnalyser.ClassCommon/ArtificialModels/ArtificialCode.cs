using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VipAnalyser.ClassCommon
{
    public enum ArtificialCode
    {
        [Description("未知异常")]
        A_UnknownException = -99999,

        [Description("未知方法名")]
        A_UnknownMethod = -99998,

        [Description("超时时间为零")]
        A_TimeOutZero = -99997,

        [Description("人工模拟超时未能得到执行结果,请稍后再试")]
        A_TimeOutResult = -99996,

        [Description("系统繁忙,请稍后再试")]
        A_SystemBusy = -99995,

        [Description("请求被中断(意外/正常)")]
        A_RequestBreak = -99994,

        [Description("请求被意外中断")]
        A_RequestAccidentBreak = -99993,

        [Description("请求被正常调用中断")]
        A_RequestNormalBreak = -99992,

        [Description("人工模拟未启动/已关闭,请稍后再试")]
        A_ProgramNoStarted = -99991,

        [Description("非工作时段")]
        A_NotWorkTime = -99990
    }
}
