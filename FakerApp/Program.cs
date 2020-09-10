using FakerLib;
using System;

namespace FakerApp
{
    class Program
    {
        public static object TestGen()
        {
            return 1;
        }

        static void Main(string[] args)
        {
            FakerConfig config = new FakerConfig();

          //  config.Add<Test, int>(TestGen, test => test.testInt);

            Faker faker = new Faker(config);

            Test test = (Test)faker.Create(typeof(Test));

            Console.WriteLine(test.testInt);
        }
    }
}
