using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace 地面站
{
    internal class DeepCopy
    {
        //public static object deepcopy(object _object)
        //{
        //    Type T = _object.GetType();
        //    object o = Activator.CreateInstance(T);
        //    PropertyInfo[] PI = T.GetProperties();
        //    for (int i = 0; i < PI.Length; i++)
        //    {
        //        PropertyInfo P = PI[i];
        //        P.SetValue(o, P.GetValue(_object));
        //    }
        //    return o;
        //}
        // 利用XML序列化和反序列化实现
        public static T deepcopy<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                xml.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = xml.Deserialize(ms);
                ms.Close();
            }


            return (T)retval;
        }
    }
}
