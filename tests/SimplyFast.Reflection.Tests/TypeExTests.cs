using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SimplyFast.Reflection.Tests.TestData;

namespace SimplyFast.Reflection.Tests
{
    
    public class TypeExTests
    {
        [Fact]
        public void FieldsReturnValidCounts()
        {
            var a1 = typeof(SomeClass1).Fields();
            Assert.Equal(7, a1.Length);
        }

        [Fact]
        public void FieldsReturnValidFields()
        {
            var a2 = typeof(SomeClass1).Fields();
            var fields = new[] { "_f1", "F2" };
            var other = from o in a2
                        where !fields.Contains(o.Name)
                        select o;
            // auto-property fields
            Assert.Equal(5, other.Count());
        }

        [Fact]
        public void PropertiesReturnValidCounts()
        {
            var a1 = typeof(SomeClass1).Properties();
            Assert.Equal(7, a1.Length);
        }

        [Fact]
        public void PropertiesReturnValidProperties()
        {
            var a2 = typeof(SomeClass1).Properties();
            var props = new[] { "P00", "P0", "P1", "P2", "P3", "P4", "P5", "F3" };
            var other = from o in a2
                        where !props.Contains(o.Name)
                        select o;
            Assert.Equal(0, other.Count());
        }

        [Fact]
        public void StringClassWorks()
        {
            var type = typeof(string);
            var publicPropertiesCount = type.Properties().Count(x => x.IsPublic());
            Assert.Equal(2, publicPropertiesCount);
        }

        [Fact]
        public void GetDeclaredTypeWorks()
        {
            IList<int> v1 = new List<int>();
            SomeClass1 t = new TestClass2();

            Assert.Equal(typeof(IList<int>), TypeEx.TypeOf(v1));
            Assert.Equal(typeof(Dictionary<string, double>), TypeEx.TypeOf((Dictionary<string, double>)null));
            Assert.Equal(typeof(SomeClass1), TypeEx.TypeOf(t));
        }

        [Fact]
        public void SubstituteWorks()
        {
            var type = TypeEx.Substitute(typeof(Tuple<string, int, List<string>, double>),
                t => t == typeof(string) ? typeof(decimal) : t);
            Assert.Equal(typeof(Tuple<decimal, int, List<decimal>, double>), type);

            var type2 = TypeEx.Substitute(typeof(int), t => typeof(string));
            Assert.Equal(typeof(string), type2);
        }

        [Fact]
        public void RemoveByRefWorks()
        {
            var refInt = typeof(int).MakeByRefType();
            Assert.Equal(typeof(int), refInt.RemoveByRef());
            Assert.Equal(typeof(int), typeof(int).RemoveByRef());
        }

        // ReSharper disable once PossibleInterfaceMemberAmbiguity
        private interface ITestEnumerable : IEnumerable<string>, IEnumerable<int>, IEnumerable<double>
        {

        }

        private class SomeEnumerable : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                throw new NotSupportedException();
            }
        }

        [Fact]
        public void FindIEnumerableWorks()
        {
            Assert.True(new[] { typeof(string), typeof(int), typeof(double), typeof(object) }.SequenceEqual(TypeEx.FindIEnumerable(typeof(ITestEnumerable))));
            Assert.True(new[] { typeof(char), typeof(object) }.SequenceEqual(TypeEx.FindIEnumerable(typeof(string))));
            Assert.True(new[] { typeof(int), typeof(object) }.SequenceEqual(TypeEx.FindIEnumerable(typeof(List<int>))));
            Assert.True(new[] { typeof(object) }.SequenceEqual(TypeEx.FindIEnumerable(typeof(SomeEnumerable))));
            Assert.True(new[] { typeof(char), typeof(object) }.SequenceEqual(TypeEx.FindIEnumerable(typeof(IEnumerable<char>))));
            Assert.True(new[] { typeof(object) }.SequenceEqual(TypeEx.FindIEnumerable(typeof(IEnumerable))));

            Assert.Equal(2, TypeEx.FindIEnumerable(typeof(IEnumerable<>)).Count());
            Assert.False(TypeEx.FindIEnumerable(typeof(int)).Any());
        }

        [Fact]
        public void FindGenericTypeWorks()
        {
            Assert.Equal(typeof(IEnumerable<string>), TypeEx.FindGenericType(typeof(IEnumerable<>), typeof(ITestEnumerable)));
            Assert.Equal(typeof(ICollection<string>), TypeEx.FindGenericType(typeof(ICollection<>), typeof(List<string>)));
            Assert.Equal(null, TypeEx.FindGenericType(typeof(IEnumerable<>), typeof(int)));
        }

        [Fact]
        public void MakeGenericWorks()
        {
            var generic = typeof(Tuple<,,>);
            var tint1 = generic.MakeGeneric(typeof(int), typeof(int), typeof(string));
            var tint2 = generic.MakeGeneric(typeof(int), typeof(int), typeof(string));
            var tstr = generic.MakeGeneric(typeof(int), typeof(string), typeof(string));
            Assert.NotNull(tint1);
            Assert.NotNull(tint2);
            Assert.NotNull(tstr);
            Assert.Equal(tint1, tint2);
            Assert.NotEqual(tint1, tstr);
            Assert.Throws<ArgumentException>(() => generic.MakeGeneric());
            Assert.Throws<ArgumentException>(() => generic.MakeGeneric(typeof(int)));
            Assert.Throws<ArgumentException>(() => generic.MakeGeneric(typeof(int), typeof(string)));
            Assert.Throws<ArgumentException>(
                () => generic.MakeGeneric(typeof(int), typeof(string), typeof(int), typeof(string)));
        }

        [Fact]
        public void GenericArgumentsWork()
        {
            Assert.Equal(0, typeof(string).GenericArguments().Length);
            Assert.Equal(1, typeof(IEnumerable<>).GenericArguments().Length);
            Assert.Equal(typeof(string), typeof(IEnumerable<string>).GenericArguments()[0]);
            Assert.Equal(0, typeof(string).TypeInfo().GenericArguments().Length);
            Assert.Equal(1, typeof(IEnumerable<>).TypeInfo().GenericArguments().Length);
            Assert.Equal(typeof(string), typeof(IEnumerable<string>).TypeInfo().GenericArguments()[0]);
        }

        [Fact]
        public void ResolveTypeWorks()
        {
            Assert.Equal(typeof(string), TypeEx.ResolveType(typeof(string).FullName));
            Assert.Equal(typeof(TypeExTests), TypeEx.ResolveType(typeof(TypeExTests).FullName));
            Assert.Null(TypeEx.ResolveType("SF.Reflection.Tests.TypeNotExists"));
        }
    }
}