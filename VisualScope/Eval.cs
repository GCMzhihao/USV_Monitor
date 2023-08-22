using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable 0618
namespace 地面站
{
    class Eval
    {
        static Microsoft.JScript.Vsa.VsaEngine engine = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t">参数w</param>
        /// <param name="expression">参数方程因变量的表达式</param>
        /// <returns>表达式计算结果</returns>
        public static double Calculate(double t, string expression)//t为参数w，expression为参数方程因变量的表达式
        {
            double result = 0;
            try
            {
                expression = "var w=" + t.ToString() + ";" + expression;
                result = Convert.ToDouble(Microsoft.JScript.Eval.JScriptEvaluate(expression, engine));
                
            }
            catch
            {
            }
            return result;
        }
    }
}
