using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using NUnit.Framework;
using SF.Reflection.Emit;

namespace SF.Tests.Reflection.Emit
{
    [TestFixture]
    public class EmitControlTests
    {
        public class TestClass
        {
            public int TestField;
            public string TestProp
            {
                get { return TestField.ToString(CultureInfo.InvariantCulture); }
                set { TestField = int.Parse(value); }
            }

            public bool IsOk()
            {
                return true;
            }

            public double TestMethod(float param)
            {
                return TestField * param;
            }

            public Action<bool> TestAction;

            public int this[int index]
            {
                get { return TestField * index; }
                set { TestField = value / index; }
            }

            public static explicit operator int(TestClass a)
            {
                return a.TestField;
            }

            public static explicit operator TestClass(int a)
            {
                return new TestClass { TestField = a };
            }
        }

        [Test]
        public void CastFromOpTest()
        {
            var method = EmitEx.CreateMethod<Func<int, TestClass>>();
            var il = method.GetILGenerator();
            il.EmitLdarg(0);
            il.EmitCast(typeof(int), typeof(TestClass));
            il.Emit(OpCodes.Ret);
            var del = method.CreateDelegate<Func<int, TestClass>>();
            Assert.AreEqual(1, del(1).TestField);
            Assert.AreEqual(5, del(5).TestField);
        }

        [Test]
        public void CastToOpTest()
        {
            var method = EmitEx.CreateMethod<Func<TestClass, int>>();
            var il = method.GetILGenerator();
            il.EmitLdarg(0);
            il.EmitCast(typeof(TestClass), typeof(int));
            il.Emit(OpCodes.Ret);
            var del = method.CreateDelegate<Func<TestClass, int>>();
            Assert.AreEqual(1, del(new TestClass{TestField = 1}));
            Assert.AreEqual(5, del(new TestClass { TestField = 5 }));
        }

        [Test]
        public void CastToBoxTest()
        {
            var method = EmitEx.CreateMethod<Func<int, object>>();
            var il = method.GetILGenerator();
            il.EmitLdarg(0);
            il.EmitCast(typeof(int), typeof(object));
            il.Emit(OpCodes.Ret);
            var del = method.CreateDelegate<Func<int, object>>();
            Assert.AreEqual(1, del(1));
            Assert.AreEqual(5, del(5));
        }

        [Test]
        public void CastToUnBoxTest()
        {
            var method = EmitEx.CreateMethod<Func<object, int>>();
            var il = method.GetILGenerator();
            il.EmitLdarg(0);
            il.EmitCast(typeof(object), typeof(int));
            il.Emit(OpCodes.Ret);
            var del = method.CreateDelegate<Func<object, int>>();
            Assert.AreEqual(1, del(1));
            Assert.AreEqual(5, del(5));
        }

        [Test]
        public void CastToCastClassToObjectTest()
        {
            var method = EmitEx.CreateMethod<Func<string, object>>();
            var il = method.GetILGenerator();
            il.EmitLdarg(0);
            il.EmitCast(typeof(string), typeof(object));
            il.Emit(OpCodes.Ret);
            var del = method.CreateDelegate<Func<string, object>>();
            Assert.AreEqual("1", del("1"));
            Assert.AreEqual("5", del("5"));
        }

        [Test]
        public void CastToCastClassFromObjectTest()
        {
            var method = EmitEx.CreateMethod<Func<object, string>>();
            var il = method.GetILGenerator();
            il.EmitLdarg(0);
            il.EmitCast(typeof(object), typeof(string));
            il.Emit(OpCodes.Ret);
            var del = method.CreateDelegate<Func<object, string>>();
            Assert.AreEqual("1", del("1"));
            Assert.AreEqual("5", del("5"));
        }

        [Test]
        public void TestFor()
        {
            var method = EmitEx.CreateMethod<Func<int, int>>();
            var il = method.GetILGenerator();
            var s = il.DeclareLocal(typeof (int));
            var i = il.DeclareLocal(typeof (int));
            il.EmitLdcI4(0);
            il.EmitStloc(s);
            il.EmitLdarg(0);
            il.EmitStloc(i);
            il.EmitFor(
                b =>
                {
                    il.EmitLdloc(i);
                    il.EmitLdcI4(0);
                    il.Emit(OpCodes.Bgt_S, b);
                },
                () =>
                {
                    il.EmitLdloc(i);
                    il.EmitLdcI4(1);
                    il.Emit(OpCodes.Sub);
                    il.EmitStloc(i);
                },
                c =>
                {
                    il.EmitLdloc(s);
                    il.EmitLdloc(i);
                    il.Emit(OpCodes.Add);
                    il.EmitStloc(s);
                }
                );
            il.EmitLdloc(s);
            il.Emit(OpCodes.Ret);
            var del = method.CreateDelegate<Func<int, int>>();
            Assert.AreEqual(Enumerable.Range(0, 6).Sum(), del(5));
            Assert.AreEqual(Enumerable.Range(0, 4).Sum(), del(3));
        }

        [Test]
        public void TestForEach()
        {
            var method = EmitEx.CreateMethod<Func<IEnumerable<int>, int>>();
            var il = method.GetILGenerator();
            var s = il.DeclareLocal(typeof(int));
            il.EmitLdcI4(0);
            il.EmitStloc(s);
            il.EmitLdarg(0);
            il.EmitForEach(typeof(IEnumerable<int>),
                c =>
                {
                    il.EmitLdloc(s);
                    il.Emit(OpCodes.Add);
                    il.EmitStloc(s);
                });
            il.EmitLdloc(s);
            il.Emit(OpCodes.Ret);
            var del = method.CreateDelegate<Func<IEnumerable<int>, int>>();
            Assert.AreEqual(Enumerable.Range(0, 6).Sum(), del(Enumerable.Range(0, 6)));
            Assert.AreEqual(Enumerable.Range(0, 4).Sum(), del(Enumerable.Range(0, 4)));
        }

        [Test]
        public void TestForEachEnumerable()
        {
            var method = EmitEx.CreateMethod<Func<IEnumerable, int>>();
            var il = method.GetILGenerator();
            var s = il.DeclareLocal(typeof(int));
            il.EmitLdcI4(0);
            il.EmitStloc(s);
            il.EmitLdarg(0);
            il.EmitForEach(typeof(IEnumerable),
                c =>
                {
                    il.EmitUnBoxAnyOrCastClass(typeof(int));
                    il.EmitLdloc(s);
                    il.Emit(OpCodes.Add);
                    il.EmitStloc(s);
                });
            il.EmitLdloc(s);
            il.Emit(OpCodes.Ret);
            var del = method.CreateDelegate<Func<IEnumerable, int>>();
            var list = new List<object>(Enumerable.Range(0, 6).Cast<object>());
            /*list.Insert(2, "test");
            list.Insert(4, 2.3);
            list.Add(null);*/
            Assert.AreEqual(Enumerable.Range(0, 6).Sum(), del(list));
            Assert.AreEqual(Enumerable.Range(0, 4).Sum(), del(Enumerable.Range(0, 4)));
        }

        [Test]
        public void TestUsing()
        {
            var method = EmitEx.CreateMethod<Func<IDisposable, int>>();
            var il = method.GetILGenerator();
            il.EmitLdarg(0);
            var disp = il.DeclareLocal(typeof(IDisposable));
            il.EmitStloc(disp);
            il.EmitUsing(disp, () =>
            {
                
            });
            il.EmitLdcI4(42);
            il.Emit(OpCodes.Ret);

            var compiled = method.CreateDelegate<Func<IDisposable, int>>(); 
            var disposed = false;
            var dis = DisposableEx.Action(() => disposed = true);
            Assert.AreEqual(42, compiled(dis));
            Assert.IsTrue(disposed);
        }

        [Test]
        public void TestWhile()
        {
            var method = EmitEx.CreateMethod<Func<int, int>>();
            var il = method.GetILGenerator();
            var i = il.DeclareLocal(typeof (int));
            var s = il.DeclareLocal(typeof (int));
            il.EmitLdcI4(0);
            il.EmitStloc(s);
            il.EmitLdarg(0);
            il.EmitStloc(i);
            il.EmitWhile(
                b =>
                {
                    il.EmitLdloc(i);
                    il.EmitLdcI4(0);
                    il.Emit(OpCodes.Bgt_S, b);
                },
                c =>
                {
                    il.EmitLdloc(i);
                    il.EmitLdloc(s);
                    il.Emit(OpCodes.Add);
                    il.EmitStloc(s);
                    
                    il.EmitLdloc(i);
                    il.EmitLdcI4(1);
                    il.Emit(OpCodes.Sub);
                    il.EmitStloc(i);
                });
            
            il.EmitLdloc(s);
            il.Emit(OpCodes.Ret);

            var compiled = method.CreateDelegate<Func<int, int>>(); 
            Assert.AreEqual(Enumerable.Range(0, 6).Sum(), compiled(5));
        }
    }
}