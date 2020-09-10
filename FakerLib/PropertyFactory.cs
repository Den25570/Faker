using System;
using System.Linq;

namespace FakerLib
{
    public static class PropertyFactory
    {
        public static object GenerateInt(Random rand)
        {
            return rand.Next();
        }

        public static object GenerateDouble(Random rand)
        {
            return rand.NextDouble();
        }

        public static object GenerateString(Random rand)
        {
            int length = rand.Next(1, 20);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[rand.Next(s.Length)]).ToArray());
        }

        public static object GenerateChar(Random rand)
        {
            return (char)rand.Next('0', 'Z'); ;
        }

        public static object GenerateFloat(Random rand)
        {
            return (float)rand.NextDouble();
        }

        public static object GenerateLong(Random rand)
        {
            return (long)rand.Next();
        }

        public static object GenerateDate(Random rand)
        {
            DateTime start = new DateTime(1970, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(rand.Next(range));
        }

        public static object GenerateTime(Random rand)
        {
            TimeSpan start = TimeSpan.FromHours(0);
            TimeSpan end = TimeSpan.FromHours(24);
            int maxMinutes = (int)((end - start).TotalMinutes);
            int minutes = rand.Next(maxMinutes);
            return start.Add(TimeSpan.FromMinutes(minutes));
        }

        public static object GenerateURI(Random rand)
        {
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Host = (string)GenerateString(rand) + "." + (string)GenerateString(rand);
            uriBuilder.Path = (string)GenerateString(rand);
            return uriBuilder.Uri;
        }

    }
}
