using System.Collections.Generic;
using Xunit;
using SimplyFast.Expressions.Tests.TestData;
using SimplyFast.Reflection;

namespace SimplyFast.Expressions.Tests
{
    
    public class LambdaExtractTests
    {
        /*[Fact]
        public void CopyValuesWorksForFields()
        {
            var a = new TestClass1 {P1 = 321, P2 = "test2", P3 = "test3", P4 = "test4", P5 = "test5"};
            var b = new TestClass1();
            Assert.NotEqual(a, b);
            typeof (TestClass1).Simple().Fields.CopyValues(a, b);
            // Compiler creates hidden fields for auto-properties, that's why this thing works
            Assert.Equal(a, b);
        }

        [Fact]
        public void CopyValuesWorksForProperties()
        {
            var a = new TestClass1 {P1 = 321, P2 = "test2", P3 = "test3", P4 = "test4", P5 = "test5"};
            var b = new TestClass1();
            Assert.NotEqual(a, b);
            typeof (TestClass1).Simple().Properties.CopyValues(a, b);
            Assert.Equal(a.P1, b.P1);
            Assert.Equal(a.P2, b.P2);
            Assert.Equal(a.P3, b.P3);
            Assert.Equal(a.P4, b.P4);
            Assert.Equal(a.P5, b.P5);
        }

        [Fact]
        public void GetValuesAsDictionaryWorkForAnonymousTypes()
        {
            var a = new TestClass2 {P1 = 123, P2 = "test2", P3 = "test3", P4 = "test4", P5 = "test5"};
            var b = new {a.P1, a.P3, a.P4, Test5 = "5"};
            var values = SimpleReflection.TypeOf(b).Properties.Where(x => x.IsPublic).GetValuesAsDictionary(b);
            Assert.Equal(4, values.Count);
            Assert.Equal(123, values["P1"]);
            Assert.Equal("test3", values["P3"]);
            Assert.Equal("test4", values["P4"]);
            Assert.Equal("5", values["Test5"]);
        }

        [Fact]
        public void GetValuesAsDictionaryWorkForNormalTypes()
        {
            var a = new TestClass2 {P1 = 231, P2 = "test2", P3 = "test3", P4 = "test4", P5 = "test5"};
            var values = SimpleReflection.TypeOf(a).Properties.Where(x => x.IsPublic).GetValuesAsDictionary(a);
            Assert.Equal(7, values.Count);
            Assert.Equal(231, values["P1"]);
            Assert.Equal("test2", values["P2"]);
            Assert.Equal("test3", values["P3"]);
            Assert.Equal("test4", values["P4"]);
            Assert.Equal("test5", values["P5"]);
            Assert.Null(values["P00"]);
            Assert.Equal("_f3t", values["F3"]);
        }

        [Fact]
        public void GetValuesAsDictionaryWorkForTypesWithIndexProp()
        {
            var a = new TestClass3 {P1 = 321, P2 = "test2", P3 = "test3", P4 = "test4", P5 = "test5"};
            var values = SimpleReflection.TypeOf(a).Properties.Where(x => x.IsPublic).GetValuesAsDictionary(a);
            Assert.Equal(8, values.Count);
            Assert.Null(values["P00"]);
            Assert.Equal(321, values["P1"]);
            Assert.Equal("test2", values["P2"]);
            Assert.Equal("test3", values["P3"]);
            Assert.Equal("test4", values["P4"]);
            Assert.Equal("test5", values["P5"]);
            Assert.Equal("_f3t", values["F3"]);
            Assert.Equal(1, values["CanGet"]);
        }

        [Fact]
        public void GetValuesWorkForAnonymousTypes()
        {
            var a = new TestClass2 {P1 = 123, P2 = "test2", P3 = "test3", P4 = "test4", P5 = "test5"};
            var b = new {a.P1, a.P3, a.P4, Test5 = "5"};
            var values = SimpleReflection.TypeOf(b).Properties.Where(x => x.IsPublic).GetValues(b);
            Assert.Equal(4, values.Count);
            Assert.True(values.Contains(123));
            Assert.True(values.Contains("test3"));
            Assert.True(values.Contains("test4"));
            Assert.True(values.Contains("5"));
        }

        [Fact]
        public void GetValuesWorkForNormalTypes()
        {
            var a = new TestClass2 {P1 = 231, P2 = "test2", P3 = "test3", P4 = "test4", P5 = "test5"};
            var values = SimpleReflection.TypeOf(a).Properties.Where(x => x.IsPublic).GetValues(a);
            Assert.Equal(12, values.Count);
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

        [Fact]
        public void GetValuesWorkForTypesWithIndexProp()
        {
            var a = new TestClass3 {P1 = 321, P2 = "test2", P3 = "test3", P4 = "test4", P5 = "test5"};
            var values = SimpleReflection.TypeOf(a).Properties.Where(x => x.IsPublic).GetValues(a);
            Assert.Equal(13, values.Count);
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

        [Fact]
        public void GetMemberWorks()
        {
            Assert.Equal(typeof(List<int>).Property("Count"), LambdaExtract.Member((List<int> l) => l.Count));
            Assert.Equal(typeof(TestClass1).Constructor(), LambdaExtract.Member(() => new TestClass1()));
            Assert.Equal(typeof(string).Constructor(typeof(char), typeof(int)), LambdaExtract.Member(() => new string('c', 10)));
            Assert.Equal(typeof(TestClass1).Property("P00"), LambdaExtract.Member((TestClass1 tc) => tc.P00));
            Assert.Equal(typeof(TestClass1).Field("F2"), LambdaExtract.Member((TestClass1 tc) => tc.F2));
            Assert.Equal(typeof(object).Method("GetHashCode"), LambdaExtract.Member((TestClass1 tc) => tc.GetHashCode()));
            Assert.Equal(typeof(Dictionary<string, double>).Property("Keys"),
                            LambdaExtract.Member((Dictionary<string, double> d) => d.Keys));
            Assert.Equal(typeof(TestClass3).Property("Item"),
                            LambdaExtract.Member((TestClass3 d) => d[0]));
        }

        [Fact]
        public void GetMemberWorksTyped()
        {
            Assert.Equal(typeof(List<int>).Property("Count"), LambdaExtract.Property((List<int> l) => l.Count));
            Assert.Equal(typeof(TestClass1).Constructor(), LambdaExtract.Constructor(() => new TestClass1()));
            Assert.Equal(typeof(string).Constructor(typeof(char), typeof(int)), LambdaExtract.Constructor(() => new string('c', 10)));
            Assert.Equal(typeof(TestClass1).Property("P00"), LambdaExtract.Property((TestClass1 tc) => tc.P00));
            Assert.Equal(typeof(TestClass1).Field("F2"), LambdaExtract.Field((TestClass1 tc) => tc.F2));
            Assert.Equal(typeof(object).Method("GetHashCode"), LambdaExtract.Method((TestClass1 tc) => tc.GetHashCode()));
            Assert.Equal(typeof(Dictionary<string, double>).Property("Keys"),
                            LambdaExtract.Property((Dictionary<string, double> d) => d.Keys));
            Assert.Equal(typeof(TestClass3).Property("Item"),
                LambdaExtract.Property((TestClass3 d) => d[0]));
            Assert.Equal(typeof(TestClass2).Property("F3"),
                LambdaExtract.Property(() => TestClass2.F3));
            Assert.Equal(typeof(TestClass2).Field("FStatic"),
                LambdaExtract.Field(() => TestClass2.FStatic));
        }

        [Fact]
        public void ArrayLengthPropertyWorks()
        {
            var prop = LambdaExtract.Property((string[] x) => x.Length);
            Assert.NotNull(prop);
            Assert.Equal("Length", prop.Name);
        }
    }
}