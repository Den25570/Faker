using System;
using System.Collections.Generic;
using System.Text;

namespace FakerLib
{
    public static class PropertyFactory
    {
        public static object GenerateInt()
        {
            Random rand = new Random();
            return rand.Next();
        }

        public static object GenerateDouble()
        {
            Random rand = new Random();
            return rand.NextDouble();
        }
    }
}
