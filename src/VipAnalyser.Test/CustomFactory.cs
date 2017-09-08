using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VipAnalyser.Test
{
    public class CustomFactory
    {
        static List<Type> _allType = new List<Type>();

        static CustomFactory()
        {
            //得到ITest所有实现类
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var t in type.GetInterfaces())
                    {
                        if (t == typeof(ITest))
                        {
                            _allType.Add(type);
                            break;
                        }
                    }
                }
            }
        }

        static ITest Get(string attrbuteName)
        {
            foreach (var type in _allType)
            {
                var attrbutes = type.GetCustomAttributes(typeof(CustomAttribute), false);
                if (attrbutes.Length > 0)
                {
                    if (attrbutes[0] is CustomAttribute attrbute && attrbute.Type == attrbuteName)
                    {
                        return (ITest)Activator.CreateInstance(type);
                    }
                }
            }
            return null;

        }
    }
}
