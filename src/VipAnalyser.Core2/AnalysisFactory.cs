using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using VipAnalyser.ClassCommon;
using VipAnalyser.ClassCommon.Models;

namespace VipAnalyser.Core2
{
    public class AnalysisFactory
    {
        static List<Type> _allType = new List<Type>();

        static AnalysisFactory()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var t in type.GetInterfaces())
                    {
                        if (t == typeof(IAnalyser))
                        {
                            _allType.Add(type);
                            break;
                        }
                    }
                }
            }
        }

        public static VideoAnalyseResponse GetResponse(string url, string cookie = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new VideoAnalyseResponse()
                {
                    ErrCode = -1,
                    ErrMsg = "url错误"
                };
            }
            var analyser = Create(url);
            if (analyser == null)
            {
                return new VideoAnalyseResponse()
                {
                    ErrCode = -1,
                    ErrMsg = "没有找到对应的解析器，无法解析"
                };
            }
            return analyser.Analyse(url, cookie);
        }


        private static IAnalyser Create(string url)
        {
            foreach (var type in _allType)
            {
                var attrbutes = type.GetCustomAttributes(typeof(AnalysisTypeAttribute), false);
                if (attrbutes.Length > 0)
                {
                    if (attrbutes[0] is AnalysisTypeAttribute attrbute && Regex.IsMatch(url, attrbute.Type.GetDescription()))
                    {
                        return (IAnalyser)Activator.CreateInstance(type);
                    }
                }
            }
            return null;

        }
    }
}
