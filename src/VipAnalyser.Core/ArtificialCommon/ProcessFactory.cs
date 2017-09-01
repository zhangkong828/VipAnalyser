using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VipAnalyser.ClassCommon;

namespace VipAnalyser.Core
{
    public class ProcessFactory
    {
        static Dictionary<string, Type> _dicProcessActionType = new Dictionary<string, Type>();

        static ProcessFactory()
        {
            //得到IProcessBase所有实现类
            var types = CommonCla.FindAllClassByInterface<IProcessBase>();

            foreach (var type in types)
            {
                if (!string.IsNullOrWhiteSpace(type.Name))
                {
                    if (!_dicProcessActionType.ContainsKey(type.Name))
                        _dicProcessActionType.Add(type.Name, type);
                }
            }
        }

        public static IProcessBase GetProcessByMethod(DriverForm form, ArtificialParamModel paramModel)
        {
            if (!_dicProcessActionType.ContainsKey(paramModel.Method))
                throw new Exception(ArtificialCode.A_UnknownMethod.ToString());

            IProcessBase process = (IProcessBase)Activator.CreateInstance(_dicProcessActionType[paramModel.Method],
                form, paramModel);

            return process;
        }
    }
}