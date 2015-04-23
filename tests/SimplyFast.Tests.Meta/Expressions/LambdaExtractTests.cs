using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SF.Expressions;
using SF.Reflection;
using SF.Tests.Reflection.TestData;

namespace SF.Tests.Expressions
{
    [TestFixture]
    public class LambdaExtractTests
    {
        /*[Test]
        public void CopyValuesWorksForFields()
        {
            var a = new TestClass1 {P1 = 321, P2 = "test2", P3 = "test3", P4 = "test4", P5 = "test5"};
            var b = new TestClass1();
            Assert.AreNotEqual(a, b);
            typeof (TestClass1).Simple().Fields.CopyValues(a, b);
            // Compiler creates hidden fields for auto-properties, that's why this thing works
            Assert.AreEqual(a, b);
        }

        [Test]
        public void CopyValuesWorksForProperties()
        {
            var a = new TestClass1 {P1 = 321, P2 = "test2", P3 = "test3", P4 = "test4", P5 = "test5"};
            var b = new TestClass1();
            Assert.AreNotEqual(a, b);
            typeof (TestClass1).Simple().Properties.CopyValues(a, b);
            Assert.AreEqual(a.P1, b.P1);
            Assert.AreEqual(a.P2, b.P2);
            Assert.AreEqual(a.P3, b.P3);
            Assert.AreEqual(a.P4, b.P4);
            Assert.AreEqual(a.P5, b.P5);
        }

        [Test]
        public void GetValuesAsDictionaryWorkForAnonymousTypes()
        {
            var a = new TestClass2 {P1 = 123, P2 = "test2", P3 = "test3", P4 = "test4", P5 = "test5"};
            var b = new {a.P1, a.P3, a.P4, Test5 = "5"};
            var values = SimpleReflection.TypeOf(b).Properties.Where(x => x.IsPublic).GetValuesAsDictionary(b);
            Assert.AreEqual(4, values.Count);
            Assert.AreEqual(123, values["P1"]);
            Assert.AreEqual("test3", values["P3"]);
            Assert.AreEqual("test4", values["P4"]);
            Assert.AreEqual("5", values["Test5"]);
        }

        [Test]
        public void GetValuesAsDictionaryWorkForNormalTypes()
        {
            var a = new TestClass2 {P1 = 231, P2 = "test2", P3 = "test3", P4 = "test4", P5 = "test5"};
            var values = SimpleReflection.TypeOf(a).Properties.Where(x => x.IsPublic).GetValuesAsDictionary(a);
            Assert.AreEqual(7, values.Count);
            Assert.AreEqual(231, values["P1"]);
            Assert.AreEqual("test2", values["P2"]);
            Assert.AreEqual("test3", values["P3"]);
            Assert.AreEqual("test4", values["P4"]);
            Assert.AreEqual("test5", values["P5"]);
            Assert.IsNull(values["P00"]);
            Assert.AreEqual("_f3t", values["F3"]);
        }

        [Test]
        public void GetValuesAsDictionaryWorkForTypesWithIndexProp()
        {
            var a = new TestClass3 {P1 = 321, P2 = "test2", P3 = "test3", P4 = "test4", P5 = "test5"};
            var values = SimpleReflection.TypeOf(a).Properties.Where(x => x.IsPublic).GetValuesAsDictionary(a);
            Assert.AreEqual(8, values.Count);
            Assert.IsNull(values["P00"]);
            Assert.AreEqual(321, values["P1"]);
            Assert.AreEqual("test2", values["P2"]);
            Assert.AreEqual("test3", values["P3"]);
            Assert.AreEqual("test4", values["P4"]);
            Assert.AreEqual("test5", values["P5"]);
            Assert.AreEqual("_f3t", values["F3"]);
            Assert.AreEqual(1, values["CanGet"]);
        }

        [Test]
        public void GetValuesWorkForAnonymousTypes()
        {
            var a = new TestClass2 {P1 = 123, P2 = "test2", P3 = "test3", P4 = "test4", P5 = "test5"};
            var b = new {a.P1, a.P3, a.P4, Test5 = "5"};
            var values = SimpleReflection.TypeOf(b).Properties.Where(x => x.IsPublic).GetValues(b);
            Assert.AreEqual(4, values.Count);
            Assert.IsTrue(values.Contains(123));
            Assert.IsTrue(values.Contains("test3"));
            Assert.IsTrue(values.Contains("test4"));
            Assert.IsTrue(values.Contains("5"));
        }

        [Test]
        public void GetValuesWorkForNormalTypes()
        {
            var a = new TestClass2 {P1 = 231, P2 = "test2", P3 = "test3", P4 = "test4", P5 = "test5"};
            var values = SimpleReflection.TypeOf(a).Properties.Where(x => x.IsPublic).GetValues(a);
            Assert.AreEqual(12, values.Count);
            var valuesExpected = new List<object> {1, 231, "test2", "test3", "test4", "test5", null, "_f3t"};
            foreach (var value in values)
            {
                if (!valuesExpected.Contains(value))
                    Assert.Fail();
            }
            foreach (var value in valuesExpected)
            {
                if (!values.Contains(value))
                    Assert.Fail();
            }
        }

        [Test]
        public void GetValuesWorkForTypesWithIndexProp()
        {
            var a = new TestClass3 {P1 = 321, P2 = "test2", P3 = "test3", P4 = "test4", P5 = "test5"};
            var values = SimpleReflection.TypeOf(a).Properties.Where(x => x.IsPublic).GetValues(a);
            Assert.AreEqual(13, values.Count);
            var valuesExpected = new List<object> {1, 321, "test2", "test3", "test4", "test5", null, "_f3t"};
            foreach (var value in values)
            {
                if (!valuesExpected.Contains(value))
                    Assert.Fail();
            }
            foreach (var value in valuesExpected)
            {
                if (!values.Contains(value))
                    Assert.Fail();
            }
        }*/

        [Test]
        public void GetMemberWorks()
        {
            Assert.AreEqual(typeof(List<int>).Property("Count"), LambdaExtract.Member((List<int> l) => l.Count));
            Assert.AreEqual(typeof(TestClass1).Constructor(), LambdaExtract.Member(() => new TestClass1()));
            Assert.AreEqual(typeof(string).Constructor(typeof(char), typeof(int)), LambdaExtract.Member(() => new string('c', 10)));
            Assert.AreEqual(typeof(TestClass1).Property("P00"), LambdaExtract.Member((TestClass1 tc) => tc.P00));
            Assert.AreEqual(typeof(TestClass1).Field("F2"), LambdaExtract.Member((TestClass1 tc) => tc.F2));
            Assert.AreEqual(typeof(object).Method("GetHashCode"), LambdaExtract.Member((TestClass1 tc) => tc.GetHashCode()));
            Assert.AreEqual(typeof(Dictionary<string, double>).Property("Keys"),
                            LambdaExtract.Member((Dictionary<string, double> d) => d.Keys));
            Assert.AreEqual(typeof(TestClass3).Property("Item"),
                            LambdaExtract.Member((TestClass3 d) => d[0]));
        }

        [Test]
        public void GetMemberWorksTyped()
        {
            Assert.AreEqual(typeof(List<int>).Property("Count"), LambdaExtract.Property((List<int> l) => l.Count));
            Assert.AreEqual(typeof(TestClass1).Constructor(), LambdaExtract.Constructor(() => new TestClass1()));
            Assert.AreEqual(typeof(string).Constructor(typeof(char), typeof(int)), LambdaExtract.Constructor(() => new string('c', 10)));
            Assert.AreEqual(typeof(TestClass1).Property("P00"), LambdaExtract.Property((TestClass1 tc) => tc.P00));
            Assert.AreEqual(typeof(TestClass1).Field("F2"), LambdaExtract.Field((TestClass1 tc) => tc.F2));
            Assert.AreEqual(typeof(object).Method("GetHashCode"), LambdaExtract.Method((TestClass1 tc) => tc.GetHashCode()));
            Assert.AreEqual(typeof(Dictionary<string, double>).Property("Keys"),
                            LambdaExtract.Property((Dictionary<string, double> d) => d.Keys));
            Assert.AreEqual(typeof(TestClass3).Property("Item"),
                LambdaExtract.Property((TestClass3 d) => d[0]));
        }

        [Test]
        public void ArrayLengthPropertyWorks()
        {
            var prop = LambdaExtract.Property((string[] x) => x.Length);
            Assert.IsNotNull(prop);
            Assert.AreEqual("Length", prop.Name);
        }
    }
}