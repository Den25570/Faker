using FakerLib;
using System;

namespace FakerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Faker faker = new Faker(new FakerConfig());

            Test test = faker.Create<Test>();

            Console.WriteLine(test.testInt);
        }
    }
}
