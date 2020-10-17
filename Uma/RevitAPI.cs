using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using System.Reflection;
using Autodesk.Revit.UI;
namespace Revit
{
    public class RevitAPI
    {
        private RevitAPI() { }

        public static dynamic RunStaticMethod(string ClassName, string Method, string[] Parameters)
        {
               try
                {
                    Type t = Type.GetType(ClassName);
                    object result = t.GetMethod(Method).Invoke(null, Parameters);
                    return result;
                }
                catch(Exception e)
                {
                    return e.ToString();
                }
            
        }
    }
}
