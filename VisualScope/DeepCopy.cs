using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace 地面站
{
    internal class DeepCopy
    {
        public static object deepcopy(object _object)
        {
            Type T = _object.GetType();
            object o = Activator.CreateInstance(T);
            PropertyInfo[] PI = T.GetProperties();
            for (int i = 0; i < PI.Length; i++)
            {
                PropertyInfo P = PI[i];
                P.SetValue(o, P.GetValue(_object));
            }
            return o;
        }
    }
}
