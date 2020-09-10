using System;
using FakerLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class UnitTest
    {
        public class TestOpenClass
        {
            public int TestInt;
            public double TestDouble;
            public string TestString;
            public float TestFloat;
            public char TestChar;
            public long TestLong;
            public DateTime TestDateTime;
            public TimeSpan TestSpan;
            public Uri TestUri;
        }

        private T FillClass<T>()
        {
            FakerConfig config = new FakerConfig();
            Faker faker = new Faker(config);

            T testClass = (T)faker.Create(typeof(T));
            return testClass;
        }

        [TestMethod]
        public void TestFieldsFullfill()
        {
            TestOpenClass testClass = FillClass<TestOpenClass>();

            Assert.AreNotEqual(testClass.TestInt, 0);
            Assert.AreNotEqual(testClass.TestDouble, 0);
            Assert.AreNotEqual(testClass.TestFloat, 0);
            Assert.AreNotEqual(testClass.TestLong, 0);

            Assert.IsNotNull(testClass.TestString);
            Assert.IsNotNull(testClass.TestDateTime);
            Assert.IsNotNull(testClass.TestSpan);
            Assert.IsNotNull(testClass.TestUri);
        }
    }
}
