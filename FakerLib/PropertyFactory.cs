using System;
using System.Collections.Generic;
using System.Text;

namespace FakerLib
{
    public static class PropertyFactory
    {
        public static int GenerateInt()
        {
            Random rand = new Random();
            return rand.Next();
        }

        public static double GenerateDouble()
        {
            Random rand = new Random();
            return rand.NextDouble();
        }
    }
}
