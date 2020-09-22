using FakerLib;
using System;

namespace FakerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            B obj2 = new B();
            obj2.Foo();
            A obj3 = new B();
            obj2.Foo();
        }
    }
}
