using FakerPluginBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FakerLib
{
    public class PropertyFactory
    {
        private Random rand = new Random();
        public object GenerateInt(Type[] genericTypes)
        {
            return rand.Next();
        }

        public object GenerateDouble(Type[] genericTypes)
        {
            return rand.NextDouble();
        }

        public object GenerateString(Type[] genericTypes)
        {
            int length = rand.Next(1, 20);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[rand.Next(s.Length)]).ToArray());
        }

        public object GenerateChar(Type[] genericTypes)
        {
            return (char)rand.Next('0', 'Z'); ;
        }

        public object GenerateFloat(Type[] genericTypes)
        {
            return (float)rand.NextDouble();
        }

        public object GenerateLong(Type[] genericTypes)
        {
            return (long)rand.Next();
        }

        public object GenerateURI(Type[] genericTypes)
        {
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Host = (string)GenerateString(genericTypes) + "." + (string)GenerateString(genericTypes);
            uriBuilder.Path = (string)GenerateString(genericTypes);
            return uriBuilder.Uri;
        }

    }
}
