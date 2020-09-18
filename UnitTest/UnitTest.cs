using System;
using System.Collections.Generic;
using System.Linq;
using FakerLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

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

            public object TestObject;
        }

        public class TestOpenClass2
        {
            public int TestInt;
            public int TestCustomInt;
        }

        public class TestClosedClass
        {
            public int TestInt { get; private set; }
            public TestClosedClass(int testInt)
            {
                TestInt = testInt;
            }
        }

        public class TestParentClass
        {
            public int TestInt;
            public TestOpenClass testOpenClass;
        }

        public class TestRecursiveClass1
        {
            public int TestInt;
            public TestRecursiveClass2 testRecursiveClass;
        }

        public class TestRecursiveClass2
        {
            public int TestInt;
            public TestRecursiveClass1 testRecursiveClass;
        }

        public class TestOpenClass3
        {
            public List<int> TestList;
            public int[] TestArray;
        }

        public object CustomIntGenerator(Type[] genericTypes)
        {
            return -1;
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

            Assert.IsNotNull(testClass.TestObject);
        }

        [TestMethod]
        public void TestInnerObjectFill()
        {
            TestParentClass testClass = FillClass<TestParentClass>();

            Assert.AreNotEqual(testClass.TestInt, 0);

            Assert.AreNotEqual(testClass.testOpenClass.TestInt, 0);
            Assert.AreNotEqual(testClass.testOpenClass.TestDouble, 0);
            Assert.AreNotEqual(testClass.testOpenClass.TestFloat, 0);
            Assert.AreNotEqual(testClass.testOpenClass.TestLong, 0);

            Assert.IsNotNull(testClass.testOpenClass.TestString);
            Assert.IsNotNull(testClass.testOpenClass.TestDateTime);
            Assert.IsNotNull(testClass.testOpenClass.TestSpan);
            Assert.IsNotNull(testClass.testOpenClass.TestUri);
        }

        [TestMethod]
        public void TestRecursionFill()
        {
            TestRecursiveClass1 testClass = FillClass<TestRecursiveClass1>();

            Assert.AreNotEqual(testClass.TestInt, 0);

            Assert.AreNotEqual(testClass.testRecursiveClass.TestInt, 0);

            Assert.IsNull(testClass.testRecursiveClass.testRecursiveClass);

        }

        [TestMethod]
        public void TestParamConstructor()
        {
            TestClosedClass testClass = FillClass<TestClosedClass>();

            Assert.AreNotEqual(testClass.TestInt, 0);
        }

        [TestMethod]
        public void TestCustomFaker()
        {
            FakerConfig config = new FakerConfig();
            config.Add<TestOpenClass2, Int32>(CustomIntGenerator, obj => obj.TestCustomInt);
            Faker faker = new Faker(config);

            TestOpenClass2 testClass = (TestOpenClass2)faker.Create(typeof(TestOpenClass2));

            Assert.AreNotEqual(testClass.TestInt, -1);
            Assert.AreEqual(testClass.TestCustomInt, -1);

        }

        [TestMethod]
        public void TestPluginListGen()
        {
            TestOpenClass3 testClass = FillClass<TestOpenClass3>();

            Assert.NotNull(testClass.TestList);
            Assert.NotNull(testClass.TestArray);

            Assert.That(testClass.TestList.Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(testClass.TestArray.Length, Is.GreaterThanOrEqualTo(2));

            Assert.AreNotEqual(testClass.TestList.First(), 0);
            Assert.AreNotEqual(testClass.TestArray[0], 0);
        }
    }
}
