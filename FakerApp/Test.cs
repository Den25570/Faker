using System;
using System.Collections.Generic;
using System.Text;

namespace FakerApp
{
    class A
    {
        public virtual void Foo()
        {
            Console.Write("a");
        }
    }

    class B : A
    {
        public new void Foo()
        {
            Console.Write("b");
        }
    }
}
