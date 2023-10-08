using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
#pragma warning disable 0618
namespace 地面站
{
    class Expression
    {
        public string X_Expression;
        public string Y_Expression;
        public double Min;
        public double Max;
    
        public Expression(string X_expression,string Y_expression,double Min_,double Max_) 
        {
            X_Expression=X_expression;
            Y_Expression=Y_expression;
            Min=Min_;
            Max=Max_;

        }
        public void change(string X_expression, string Y_expression, double Min_, double Max_)
        {
            X_Expression = X_expression;
            Y_Expression = Y_expression;
            Min = Min_;
            Max = Max_;

        }
        public string Out_X_Expression(double w)
        {

            if (w > Min && w < Max)
            {
                return X_Expression;
            }
            return null;
            
        }
        public string Out_Y_Expression(double w)
        {
            if (w > Min && w < Max)
            {
                return Y_Expression;
            }
            return null;
            
        }

    }
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
