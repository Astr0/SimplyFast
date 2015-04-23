using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SF.Reflection;
using SF.Tests.Reflection.TestData;

namespace SF.Tests.Reflection
{
    [TestFixture]
    public class TypeExTests
    {
        [Test]
        public void FieldsReturnValidCounts()
        {
            var a1 = typeof(TestClass1).Fields();
            Assert.AreEqual(7, a1.Length);
        }

        [Test]
        public void FieldsReturnValidFields()
        {
            var a2 = typeof(TestClass1).Fields();
            var fields = new[] {"_f1", "F2"};
            var other = from o in a2
                        where !fields.Contains(o.Name)
                        select o;
            // auto-property fields
            Assert.AreEqual(5, other.Count());
        }

        [Test]
        public void PropertiesReturnValidCounts()
        {
            var a1 = typeof(TestClass1).Properties();
            Assert.AreEqual(7, a1.Length);
        }

        [Test]
        public void PropertiesReturnValidProperties()
        {
            var a2 = typeof(TestClass1).Properties();
            var props = new[] {"P00", "P0", "P1", "P2", "P3", "P4", "P5", "F3"};
            var other = from o in a2
                        where !props.Contains(o.Name)
                        select o;
            Assert.AreEqual(0, other.Count());
        }

        [Test]
        public void StringClassWorks()
        {
            var type = typeof(string);
            var publicPropertiesCount = type.Properties().Count(x => x.IsPublic());
            Assert.AreEqual(2, publicPropertiesCount);
        }

        [Test]
        public void GetDeclaredTypeWorks()
        {
            IList<int> v1 = new List<int>();
            TestClass1 t = new TestClass2();

            Assert.AreEqual(typeof(IList<int>), TypeEx.TypeOf(v1));
            Assert.AreEqual(typeof(Dictionary<string, double>), TypeEx.TypeOf((Dictionary<string, double>)null));
            Assert.AreEqual(typeof(TestClass1), TypeEx.TypeOf(t));
        }

        [Test]
        public void SubstituteWorks()
        {
            var type = TypeEx.Substitute(typeof (Tuple<string, int, List<string>, double>),
                t => t == typeof (string) ? typeof (decimal) : t);
            Assert.AreEqual(typeof(Tuple<decimal, int, List<decimal>, double>), type);

            var type2 = TypeEx.Substitute(typeof (int), t => typeof (string));
            Assert.AreEqual(typeof(string), type2);
        }

        [Test]
        public void RemoveByRefWorks()
        {
            var refInt = typeof (int).MakeByRefType();
            Assert.AreEqual(typeof(int), refInt.RemoveByRef());
            Assert.AreEqual(typeof(int), typeof(int).RemoveByRef());
        }

        // ReSharper disable once PossibleInterfaceMemberAmbiguity
        public interface ITestEnumerable: IEnumerable<string>, IEnumerable<int>, IEnumerable<double>
        {
             
        }

        [Test]
        public void FindIEnumerableWorks()
        {
            Assert.IsTrue(new[] { typeof(string), typeof(int), typeof(double), typeof(object) }.SequenceEqual(TypeEx.FindIEnumerable(typeof(ITestEnumerable))));
            Assert.IsTrue(new[] { typeof(char), typeof(object) }.SequenceEqual(TypeEx.FindIEnumerable(typeof(string))));
            Assert.IsTrue(new[] { typeof(int), typeof(object) }.SequenceEqual(TypeEx.FindIEnumerable(typeof(List<int>))));
            Assert.IsTrue(new[] { typeof(object) }.SequenceEqual(TypeEx.FindIEnumerable(typeof(Hashtable))));
            Assert.IsTrue(new[] { typeof(char), typeof(object) }.SequenceEqual(TypeEx.FindIEnumerable(typeof(IEnumerable<char>))));
            Assert.IsTrue(new[] { typeof(object) }.SequenceEqual(TypeEx.FindIEnumerable(typeof(IEnumerable))));
            
            Assert.AreEqual(2, TypeEx.FindIEnumerable(typeof(IEnumerable<>)).Count());
            Assert.IsFalse(TypeEx.FindIEnumerable(typeof(int)).Any());
        }

        [Test]
        public void FindGenericTypeWorks()
        {
            Assert.AreEqual(typeof(IEnumerable<string>), TypeEx.FindGenericType(typeof(IEnumerable<>), typeof(ITestEnumerable)));
            Assert.AreEqual(typeof(ICollection<string>), TypeEx.FindGenericType(typeof(ICollection<>), typeof(List<string>)));
            Assert.AreEqual(null, TypeEx.FindGenericType(typeof(IEnumerable<>), typeof(int)));
        }

        [Test]
        public void MakeGenericWorks()
        {
            var generic = typeof(Tuple<,,>);
            var tint1 = generic.MakeGeneric(typeof(int), typeof(int), typeof(string));
            var tint2 = generic.MakeGeneric(typeof(int), typeof(int), typeof(string));
            var tstr = generic.MakeGeneric(typeof(int), typeof(string), typeof(string));
            Assert.IsNotNull(tint1);
            Assert.IsNotNull(tint2);
            Assert.IsNotNull(tstr);
            Assert.AreSame(tint1, tint2);
            Assert.AreNotEqual(tint1, tstr);
            Assert.Throws<ArgumentException>(() => generic.MakeGeneric());
            Assert.Throws<ArgumentException>(() => generic.MakeGeneric(typeof(int)));
            Assert.Throws<ArgumentException>(() => generic.MakeGeneric(typeof(int), typeof(string)));
            Assert.Throws<ArgumentException>(
                () => generic.MakeGeneric(typeof(int), typeof(string), typeof(int), typeof(string)));
        }

        [Test]
        public void ResolveTypeWorks()
        {
            Assert.AreEqual(typeof(string), TypeEx.ResolveType("System.String"));
            Assert.AreEqual(typeof(TypeExTests), TypeEx.ResolveType("SF.Tests.Reflection.TypeExTests"));
            Assert.IsNull(TypeEx.ResolveType("SF.Tests.Reflection.TypeNotExists"));
        }
    }
}