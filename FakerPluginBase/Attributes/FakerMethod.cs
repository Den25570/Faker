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
        public Type ParentType { get; private set; }

        public FakerMethod(Type returnType, Type parentType = null)
        {
            this.ReturnType = returnType;
            this.ParentType = parentType != null ? parentType : typeof(object);
        }
    }
}
