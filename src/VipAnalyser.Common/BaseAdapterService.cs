using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VipAnalyser.Common
{
    public class BaseAdapterService
    {
        /// <summary>
        /// 管道名称编号,用于记录日志
        /// </summary>
        protected string _pipeNameNo = string.Empty;

        /// <summary>
        /// 程序名称,也用于命令管道名称
        /// </summary>
        static string PipeName
        {
            get { return ConfigurationManager.AppSettings["ArtificialName"]; }
        }

        /// <summary>
        /// 最少进程数
        /// </summary>
        static int ProcessMinCount
        {
            get
            {
                return string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["ProcessMinCount"])
                    ? 5
                    : int.Parse(ConfigurationManager.AppSettings["ProcessMinCount"]);
            }
        }

        /// <summary>
        /// 最多进程数
        /// </summary>
        static int ProcessMaxCount
        {
            get
            {
                return string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["ProcessMaxCount"])
                    ? 50
                    : int.Parse(ConfigurationManager.AppSettings["ProcessMaxCount"]);
            }
        }

        /// <summary>
        /// 始终保持的空余进程数
        /// </summary>
        static int UnoccupiedProcessCount
        {
            get
            {
                return string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["UnoccupiedProcessCount"])
                    ? 3
                    : int.Parse(ConfigurationManager.AppSettings["UnoccupiedProcessCount"]);
            }
        }

        /// <summary>
        /// 检查进程是否开启时间间隔,秒
        /// </summary>
        static int ProcessCheckTime
        {
            get
            {
                return string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["ProcessCheckTime"])
                    ? 15
                    : int.Parse(ConfigurationManager.AppSettings["ProcessCheckTime"]);
            }
        }

        /// <summary>
        /// 当前进程数,初始化进程数
        /// </summary>
        static int ConcurrentProcessCount = ProcessMinCount;

        /// <summary>
        /// 当前并发数
        /// </summary>
        protected static int _concurrencyCount = 0;

        /// <summary>
        /// 进程ID集合
        /// </summary>
        static Dictionary<string, int> _processIdDic = new Dictionary<string, int>();

        /// <summary>
        /// 执行历史记录,用于移除进程
        /// </summary>
        static HashSet<RemoveModel> RemoveHs = new HashSet<RemoveModel>();

        /// <summary>
        /// 全局加锁
        /// </summary>
        static Object _objLock = new Object();

        class RemoveModel
        {
            public RemoveModel(int concurrency, DateTime time)
            {
                Concurrency = concurrency;
                Time = time;
            }

            public int Concurrency { get; set; }

            public DateTime Time { get; set; }
        }


        class ProcessModel
        {
            public ProcessModel(string value)
            {
                this.Value = value;
            }

            public string Value { get; set; }
        }

        /// <summary>
        /// 进程的路径
        /// </summary>
        static string ProcessPath()
        {
            var artificial = Environment.CurrentDirectory + "\\lib\\artificial\\" + PipeName + ".exe";
            return artificial;
        }

        /// <summary>
        /// 人工模拟程序启动和管理,这里可以不用加锁
        /// </summary>
        static BaseAdapterService()
        {
            var artificial = ProcessPath();

            //初始化管道名称列表,后面的Guid用于管道唯一标识,不显示
            for (int i = 0; i < ConcurrentProcessCount; i++)
            {
                string key = string.Format("{0}.{1}", PipeName, i);
                string value = string.Format("{0}~{1}", key, Guid.NewGuid());
                ProcessModel model = new ProcessModel(value);
                MemoryCache.Default.Add(key, model, null);

                _processIdDic.Add(value, 0);

                ManageProcess(artificial, key);
                Console.WriteLine("开启进程:{0}", key);
            }

            //启动移除进程的管理
            RemoveProcessStart();
        }

        /// <summary>
        /// 管理这个进程
        /// </summary>
        /// <param name="artificial">程序路径</param>
        /// <param name="key">缓存key</param>
        static void ManageProcess(string artificial, string key, bool isSleep = true)
        {
            Task.Factory.StartNew((k) =>
            {
                var cacheKey = k as string;
                if (isSleep)
                    Thread.Sleep(1000 * int.Parse(cacheKey.GetHashCode().ToString().Last().ToString()));
                //服务是否已经退出
                if (MemoryCache.Default.Get("BiHu.BaoXian.Service.stop") != null)
                    return;

                int processId = 0;
                processId = StartProcess(artificial, MemoryCache.Default.Get(cacheKey) as ProcessModel);

                while (true)
                {
                    Thread.Sleep(1000 * ProcessCheckTime);

                    try
                    {
                        //如果缓存中没有,说明已经被移除
                        if (MemoryCache.Default.Get(cacheKey) == null)
                            return;

                        var isHave = false;
                        var processList = Process.GetProcessesByName(PipeName);
                        foreach (var process in processList)
                        {
                            if (process.Id == processId)
                            {
                                isHave = true;
                                break;
                            }
                        }

                        foreach (var process in processList)
                            process.Dispose();

                        if (!isHave)
                            throw new Exception();
                    }
                    catch
                    {
                        //服务是否已经退出
                        if (MemoryCache.Default.Get("BiHu.BaoXian.Service.stop") == null)
                            processId = StartProcess(artificial, MemoryCache.Default.Get(cacheKey) as ProcessModel);
                        else
                            return;
                    }
                }

            }, key);
        }

        /// <summary>
        /// 开启一个进程
        /// </summary>
        /// <param name="artificial">程序路径</param>
        /// <param name="pipeName">管道名称参数</param>
        /// <returns>进程ID</returns>
        static int StartProcess(string artificial, ProcessModel model)
        {
            int processId = 0;
            try
            {
                if (File.Exists(artificial) && model != null)
                {
                    processId = ProcessHelp.Start(artificial, model.Value);

                    lock (_processIdDic)
                    {
                        _processIdDic[model.Value] = processId;
                    }
                }
            }
            catch { }
            return processId;
        }

        /// <summary>
        /// 检查是否新增进程
        /// </summary>
        static void StartProcessCheck(int concurrency)
        {
            try
            {
                //加入执行历史记录中
                lock (RemoveHs)
                {
                    RemoveHs.Add(new RemoveModel(_concurrencyCount, DateTime.Now));
                }
            }
            catch { }

            Task.Factory.StartNew((c) =>
            {
                int count = int.Parse(c.ToString());

                lock (_objLock)
                {
                    //始终保持的空余的进程 && 最大进程数
                    if (ConcurrentProcessCount - count == UnoccupiedProcessCount - 1 && ConcurrentProcessCount <= ProcessMaxCount)
                    {
                        string key = string.Format("{0}.{1}", PipeName, ConcurrentProcessCount);
                        ProcessModel model = MemoryCache.Default.Get(key) as ProcessModel;
                        if (model == null)
                        {
                            string value = string.Format("{0}~{1}", key, Guid.NewGuid());
                            model = new ProcessModel(value);
                            MemoryCache.Default.Add(key, model, null);

                            lock (_processIdDic)
                            {
                                _processIdDic.Add(value, 0);
                            }

                            ManageProcess(ProcessPath(), key, false);
                            Console.WriteLine("开启进程:{0}", key);

                            ConcurrentProcessCount += 1;
                        }
                    }
                }
            }, concurrency);
        }

        /// <summary>
        /// 移除进程的管理
        /// </summary>
        static void RemoveProcessStart()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000 * 60);

                    try
                    {
                        //当前时间应该有的进程数
                        int concurrency = 0;

                        lock (RemoveHs)
                        {
                            List<RemoveModel> removeModels = new List<RemoveModel>();

                            var enumerator = RemoveHs.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                //30分钟之内的
                                if (enumerator.Current.Time.AddMinutes(30) > DateTime.Now)
                                {
                                    if (enumerator.Current.Concurrency > concurrency)
                                        concurrency = enumerator.Current.Concurrency;
                                }
                                else
                                    removeModels.Add(enumerator.Current);
                            }

                            foreach (var model in removeModels)
                            {
                                RemoveHs.Remove(model);
                            }
                        }

                        //应该有的进程数 = 当前进程数 + UnoccupiedProcessCount
                        concurrency += UnoccupiedProcessCount;

                        //检查是否移除进程
                        RemoveProcessCheck(concurrency);
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                    }
                }
            });
        }

        /// <summary>
        /// 检查是否移除进程
        /// </summary>
        static void RemoveProcessCheck(int concurrency)
        {
            lock (_objLock)
            {
                if (ConcurrentProcessCount > ProcessMinCount && ConcurrentProcessCount > concurrency)
                {
                    if (concurrency < ProcessMinCount)
                        concurrency = ProcessMinCount;

                    int tempConcurrent = ConcurrentProcessCount;
                    ConcurrentProcessCount = concurrency;

                    for (int number = concurrency; number < tempConcurrent; number++)
                    {
                        string key = string.Format("{0}.{1}", PipeName, number);
                        ProcessModel model = MemoryCache.Default.Get(key) as ProcessModel;
                        if (model != null)
                        {
                            //移除缓存,此时启动管理线程将失效
                            MemoryCache.Default.Remove(key);

                            int processId = 0;
                            //得到进程ID,并从集合中移除
                            lock (_processIdDic)
                            {
                                processId = _processIdDic[model.Value];
                                _processIdDic.Remove(model.Value);
                            }

                            //关闭进程,尝试3次
                            Task.Factory.StartNew((id) =>
                            {
                                int pId = int.Parse(id.ToString());
                                for (int i = 0; i < 3; i++)
                                {
                                    try
                                    {
                                        var process = Process.GetProcessById(pId);
                                        process.Kill();
                                        Console.WriteLine("关闭进程:{0} ID:{1}", key, pId); //key的显示可能不正确
                                        process.Dispose();
                                    }
                                    catch
                                    { }
                                    Thread.Sleep(100);
                                }

                            }, processId);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 命令管道/多进程单线程
        /// </summary>
        /// <typeparam name="T1">参数类型</typeparam>
        /// <typeparam name="T2">返回类型</typeparam>
        /// <param name="method">调用方法名</param>
        /// <param name="param">参数</param>
        /// <param name="timeout">超时时间(秒)</param>
        /// <param name="stopkey">用于中止请求的key</param>
        /// <returns>T2</returns>
        protected T2 PPAccess<T1, T2>(string method, T1 param, int timeout, string stopkey)
        {

            lock (_objLock)
            {
                _concurrencyCount++;
                Console.WriteLine("当前并发:{0}", _concurrencyCount);

                //检查当前进程
                StartProcessCheck(_concurrencyCount);
            }

            try
            {
                string result = string.Empty;

                for (int i = 0; i < ConcurrentProcessCount; i++)
                {
                    string key = string.Format("{0}.{1}", PipeName, i);
                    ProcessModel model = MemoryCache.Default.Get(key) as ProcessModel;
                    if (model == null)
                        continue;

                    //在锁内处理
                    if (Monitor.TryEnter(model))
                    {
                        try
                        {
                            _pipeNameNo = model.Value.Split('~')[0];
                            return PipeAccess.Access<T1, T2>(model.Value, method, param, timeout, stopkey);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("算术运算导致溢出") || ex.Message.Contains("Arithmetic operation resulted in an overflow"))
                            {
                                throw new Exception(ArtificialCode.A_RequestAccidentBreak.ToString());
                            }
                            else if (ex.Message.Contains("操作已超时") || ex.Message.Contains("The operation has timed out"))
                            {
                                //在多进程的情况下,允许重试
                                result = ArtificialCode.A_ProgramNoStarted.ToString();
                            }
                            else if (ex.Message.Contains("信号灯超时时间已到"))
                            {
                                //在外部已经加锁的情况下,此异常不会发生
                                throw new Exception(ArtificialCode.A_SystemBusy.ToString());
                            }
                            else
                            {
                                throw ex;
                            }
                        }
                        finally
                        {
                            Monitor.Exit(model);
                        }
                    }
                }

                throw new Exception(result);
            }
            finally
            {
                lock (_objLock)
                {
                    _concurrencyCount--;
                    Console.WriteLine("当前并发:{0}", _concurrencyCount);
                }
            }
        }

        /// <summary>
        /// 中止请求-特殊处理的方法
        /// </summary>
        protected bool SetReqKeyVal(string stopkey)
        {
            if (!string.IsNullOrWhiteSpace(stopkey))
                return CommonCla.AddCacheAbsolute(stopkey, string.Empty, 60 * 10);
            else
                return false;
        }

        /// <summary>
        /// 重启人工模拟程序-特殊处理的方法
        /// </summary>
        /// <param name="times">尝试次数</param>
        /// <returns></returns>
        protected int Close(int times = 3)
        {
            var count = 0;
            for (int i = 0; i < times; i++)
            {
                lock (_processIdDic)
                {
                    foreach (var key in _processIdDic.Keys)
                    {
                        try
                        {
                            var processId = _processIdDic[key];
                            var process = Process.GetProcessById(processId);
                            process.Kill();
                            count++;
                            process.Dispose();
                        }
                        catch { }
                    }
                }
                Thread.Sleep(100);
            }
            return count;
        }
    }
}
