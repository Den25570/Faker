using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerPluginBase
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class FakerMethod : Attribute
    {
        public Type ReturnType { get; private set; }

        public FakerMethod(Type returnType)
        {
            this.ReturnType = returnType;
        }
    }
}
