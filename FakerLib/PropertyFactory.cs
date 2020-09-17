using System;
using System.Collections.Generic;
using System.Linq;

namespace FakerLib
{
    public class PropertyFactory
    {
        public object GenerateInt(Random rand, List<Type> genericTypes)
        {
            return rand.Next();
        }

        public object GenerateDouble(Random rand, List<Type> genericTypes)
        {
            return rand.NextDouble();
        }

        public object GenerateString(Random rand, List<Type> genericTypes)
        {
            int length = rand.Next(1, 20);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[rand.Next(s.Length)]).ToArray());
        }

        public object GenerateChar(Random rand, List<Type> genericTypes)
        {
            return (char)rand.Next('0', 'Z'); ;
        }

        public object GenerateFloat(Random rand, List<Type> genericTypes)
        {
            return (float)rand.NextDouble();
        }

        public object GenerateLong(Random rand, List<Type> genericTypes)
        {
            return (long)rand.Next();
        }

        public object GenerateDate(Random rand, List<Type> genericTypes)
        {
            DateTime start = new DateTime(1970, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(rand.Next(range));
        }

        public object GenerateTime(Random rand, List<Type> genericTypes)
        {
            TimeSpan start = TimeSpan.FromHours(0);
            TimeSpan end = TimeSpan.FromHours(24);
            int maxMinutes = (int)((end - start).TotalMinutes);
            int minutes = rand.Next(maxMinutes);
            return start.Add(TimeSpan.FromMinutes(minutes));
        }

        public object GenerateURI(Random rand, List<Type> genericTypes)
        {
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Host = (string)GenerateString(rand, genericTypes) + "." + (string)GenerateString(rand, genericTypes);
            uriBuilder.Path = (string)GenerateString(rand, genericTypes);
            return uriBuilder.Uri;
        }

    }
}
