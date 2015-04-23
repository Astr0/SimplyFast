using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SF.Reflection;
using SF.Tests.Reflection.TestData;

namespace SF.Tests.Reflection
{
    [TestFixture]
    public class MethodInfoExTests
    {
        private delegate int SumDelegate(int a, int b, out int c, ref int d);

        private delegate object SumDelegateObj(object a, object b, out object c, ref object d);

        [Test]
        public void FindOverloadReturnsNullOnMultipleGenericMatches()
        {
            var parameters = new[] {typeof (int), typeof (int)};
            var methodGeneric = typeof(TestClass2).FindMethod("Max", parameters);
            Assert.IsNull(methodGeneric);
            var method = typeof(TestClass2).FindMethod("Max", parameters);
            Assert.IsNull(method);
        }

        [Test]
        public void FindOverloadWorks()
        {
            var intParameters = new[] {typeof (int), typeof (int)};
            var normalInt = typeof(TestClass2).FindMethod("Max3", intParameters);
            Assert.IsNotNull(normalInt);
            Assert.AreEqual(7, normalInt.InvokerAs<Func<object, object, object>>()(3, 5));

            var doubleParameters = new[] {typeof (double), typeof (double)};
            var normalDouble = typeof(TestClass2).FindMethod("Max3", doubleParameters);
            Assert.IsNotNull(normalDouble);
            Assert.AreEqual(6.5, normalDouble.InvokerAs<Func<double, double, double>>()(3.5, 5.5));

            var floatParameters = new[] {typeof (float), typeof (float)};
            var genericFloat = typeof(TestClass2).FindMethod("Max3", floatParameters);
            Assert.IsNotNull(normalDouble);
            Assert.IsTrue(genericFloat.IsGenericMethod);
            Assert.AreEqual(5.5f, genericFloat.InvokerAs<Func<float, float, float>>()(3.5f, 5.5f));
        }

        [Test]
        public void GenericWorks()
        {
            var genericMethod = typeof(TestClass2).Method("Max", 1, Substitute.T[0], Substitute.T[0]);
            Assert.IsNotNull(genericMethod);
            var method = genericMethod.MakeGeneric(new[] {typeof (int)});
            Assert.AreEqual(5, method.InvokerAs<Func<int, int, int>>()(3, 5));
            var method2 = genericMethod.MakeGeneric(new[] {typeof (double)});
            Assert.AreEqual(3.7, method2.InvokerAs<Func<object, double, double>>()(3.7, 1.2));
        }

        [Test]
        public void InvokesIntMethod()
        {
            var t2 = new TestClass2();
            var t3 = new TestClass3();
            var method = typeof(TestClass2).Method("GetF1");
            var invoker1 = method.InvokerAs<Func<TestClass2, int>>();
            var invoker2 = method.InvokerAs<Func<object, object>>();
            Assert.AreEqual(11, invoker1(t2));
            Assert.AreEqual(11, invoker2(t2));
            Assert.AreEqual(11, invoker1(t3));
            Assert.AreEqual(11, invoker2(t3));
        }

        [Test]
        public void InvokesIntMethodAs()
        {
            var t2 = new TestClass2();
            var t3 = new TestClass3();
            var method = typeof(TestClass2).Method("GetF1");
            var invoker1 = method.InvokerAs<Func<TestClass2, int>>();
            var invoker2 = method.InvokerAs<Func<object, object>>();
            Assert.AreEqual(11, invoker1(t2));
            Assert.AreEqual(11, invoker2(t2));
            Assert.AreEqual(11, invoker1(t3));
            Assert.AreEqual(11, invoker2(t3));
        }

        [Test]
        public void InvokesNewMethod()
        {
            var t3 = new TestClass3();
            var method = typeof(TestClass3).Method("GetF1");
            Assert.AreEqual(12, method.InvokerAs<Func<TestClass3, object>>()(t3));
            Assert.AreEqual(12, method.InvokerAs<Func<TestClass3, int>>()(t3));
        }

        [Test]
        public void InvokesNewMethodAs()
        {
            var t3 = new TestClass3();
            var method = typeof(TestClass3).Method("GetF1");
            Assert.AreEqual(12, method.InvokerAs<Func<TestClass3, object>>()(t3));
            Assert.AreEqual(12, method.InvokerAs<Func<TestClass3, int>>()(t3));
        }

        [Test]
        public void InvokesPrivateMethod()
        {
            var t2 = new TestClass2();
            var method = typeof(TestClass2).Method("SetP2P3Test", typeof(string), typeof(string));
            Assert.IsNull(method.InvokerAs<Func<object, object, object, object>>()(t2, "t1", "t2"));
            Assert.AreEqual("t1", t2.P2);
            Assert.AreEqual("t2", t2.P3);
            Assert.IsNull(method.InvokerAs<Func<object, string, string, object>>()(t2, "t1!", "t2!"));
            Assert.AreEqual("t1!", t2.P2);
            Assert.AreEqual("t2!", t2.P3);
        }

        [Test]
        public void InvokesPrivateMethodAs()
        {
            var t2 = new TestClass2();
            var method = typeof(TestClass2).Method("SetP2P3Test", typeof(string), typeof(string));
            Assert.IsNull(method.InvokerAs<Func<object, object, object, object>>()(t2, "t1", "t2"));
            Assert.AreEqual("t1", t2.P2);
            Assert.AreEqual("t2", t2.P3);
            Assert.IsNull(method.InvokerAs<Func<object, string, string, object>>()(t2, "t1!", "t2!"));
            Assert.AreEqual("t1!", t2.P2);
            Assert.AreEqual("t2!", t2.P3);
        }

        [Test]
        public void InvokesStaticOptionalArgs()
        {
            var method = typeof(TestClass2).Method("Sum", typeof(int), typeof(int), typeof(int[]));
            Assert.AreEqual(10, method.InvokerAs<Func<int, int, int[], int>>()(4, 6, new int[0]));
            Assert.AreEqual(25, method.InvokerAs<Func<object, object, int[], int>>()(4, 6, new[] { 1, 2, 3, 4, 5 }));
        }

        [Test]
        public void InvokesStaticOptionalArgsAs()
        {
            var method = typeof(TestClass2).Method("Sum", typeof(int), typeof(int), typeof(int[]));
            Assert.AreEqual(10, method.InvokerAs<Func<int, int, int[], int>>()(4, 6, new int[0]));
            Assert.AreEqual(25, method.InvokerAs<Func<object, object, int[], int>>()(4, 6, new[] { 1, 2, 3, 4, 5 }));
        }

        [Test]
        public void InvokesVirtualMethod()
        {
            var t2 = new TestClass2();
            var t3 = new TestClass3();
            var method = typeof(TestClass2).Method("SetP2P3", typeof(string), typeof(string));
            Assert.IsNull(method.InvokerAs<Func<TestClass2, string, string, object>>()(t2, "t1", "t2"));
            Assert.AreEqual("t1", t2.P2);
            Assert.AreEqual("t2", t2.P3);
            Assert.IsNull(method.InvokerAs<Func<object, string, object, object>>()(t2, "t1!", "t2!"));
            Assert.AreEqual("t1!", t2.P2);
            Assert.IsNull(method.InvokerAs<Func<TestClass2, string, string, object>>()(t3, "t1", "t2"));
            Assert.AreEqual("t1_", t3.P2);
            Assert.AreEqual("t2_", t3.P3);
            Assert.IsNull(method.InvokerAs<Func<TestClass2, string, string, object>>()(t3, "t1!", "t2!"));
            Assert.AreEqual("t1!_", t3.P2);
            Assert.AreEqual("t2!_", t3.P3);
            var method2 = typeof(TestClass3).Method("SetP2P3", typeof(string), typeof(string));
            Assert.IsNull(method2.InvokerAs<Func<TestClass2, string, string, object>>()(t3, "t11", "t22"));
            Assert.AreEqual("t11_", t3.P2);
            Assert.AreEqual("t22_", t3.P3);
            Assert.IsNull(method2.InvokerAs<Func<TestClass2, string, string, object>>()(t3, "t11!", "t22!"));
            Assert.AreEqual("t11!_", t3.P2);
            Assert.AreEqual("t22!_", t3.P3);
        }

        [Test]
        public void InvokesVirtualMethodAs()
        {
            var t2 = new TestClass2();
            var t3 = new TestClass3();
            var method = typeof(TestClass2).Method("SetP2P3", typeof(string), typeof(string));
            Assert.IsNull(method.InvokerAs<Func<TestClass2, string, string, object>>()(t2, "t1", "t2"));
            Assert.AreEqual("t1", t2.P2);
            Assert.AreEqual("t2", t2.P3);
            Assert.IsNull(method.InvokerAs<Func<object, string, object, object>>()(t2, "t1!", "t2!"));
            Assert.AreEqual("t1!", t2.P2);
            Assert.IsNull(method.InvokerAs<Func<TestClass2, string, string, object>>()(t3, "t1", "t2"));
            Assert.AreEqual("t1_", t3.P2);
            Assert.AreEqual("t2_", t3.P3);
            Assert.IsNull(method.InvokerAs<Func<TestClass2, string, string, object>>()(t3, "t1!", "t2!"));
            Assert.AreEqual("t1!_", t3.P2);
            Assert.AreEqual("t2!_", t3.P3);
            var method2 = typeof(TestClass3).Method("SetP2P3", typeof(string), typeof(string));
            Assert.IsNull(method2.InvokerAs<Func<TestClass2, string, string, object>>()(t3, "t11", "t22"));
            Assert.AreEqual("t11_", t3.P2);
            Assert.AreEqual("t22_", t3.P3);
            Assert.IsNull(method2.InvokerAs<Func<TestClass2, string, string, object>>()(t3, "t11!", "t22!"));
            Assert.AreEqual("t11!_", t3.P2);
            Assert.AreEqual("t22!_", t3.P3);
        }

        [Test]
        public void ModifiersWorks()
        {
            var method = typeof (TestClass2).Method("Sum", typeof (int), typeof (int), typeof (int).MakeByRefType(), typeof (int).MakeByRefType());
            Assert.IsNotNull(method);
            int c;
            var d = 2;
            Assert.AreEqual(12, method.InvokerAs<SumDelegate>()(7, 3, out c, ref d));
            Assert.AreEqual(4, c);
            Assert.AreEqual(12, d);
            object cobj;
            object dobj = 2;
            var invoker = method.InvokerAs<SumDelegateObj>();
            Assert.AreEqual(12, invoker(7, 3, out cobj, ref dobj));
            Assert.AreEqual(4, cobj);
            Assert.AreEqual(12, dobj);
        }

        [Test]
        public void ModifiersWorksAs()
        {
            var method = typeof(TestClass2).Method("Sum", typeof(int), typeof(int), typeof(int).MakeByRefType(), typeof(int).MakeByRefType());
            Assert.IsNotNull(method);
            int c;
            var d = 2;
            Assert.AreEqual(12, method.InvokerAs<SumDelegate>()(7, 3, out c, ref d));
            Assert.AreEqual(4, c);
            Assert.AreEqual(12, d);
            object cobj;
            object dobj = 2;
            var invoker = method.InvokerAs<SumDelegateObj>();
            Assert.AreEqual(12, invoker(7, 3, out cobj, ref dobj));
            Assert.AreEqual(4, cobj);
            Assert.AreEqual(12, dobj);
        }

        [Test]
        public void IsOperatorWorks()
        {
            var methods =
                typeof(decimal).GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                            BindingFlags.NonPublic);
            var count = methods.Count(x => x.IsOperator());
            Assert.AreEqual(37, count);
        }

        public class ClassWithInvoke
        {
            public void Invoke() { }
        }

        [Test]
        public void GetInvokeMethodWorks()
        {
            Assert.AreEqual(typeof(string), MethodInfoEx.GetInvokeMethod(typeof(Func<string>)).ReturnType);
            Assert.AreEqual(0, MethodInfoEx.GetInvokeMethod(typeof(Func<string>)).GetParameters().Length);
            Assert.AreEqual(typeof(void), MethodInfoEx.GetInvokeMethod(typeof(Action<string, string>)).ReturnType);
            Assert.AreEqual(2, MethodInfoEx.GetInvokeMethod(typeof(Action<string, string>)).GetParameters().Length);
            Assert.Throws<ArgumentException>(() => MethodInfoEx.GetInvokeMethod(typeof (string)));
            Assert.Throws<ArgumentException>(() => MethodInfoEx.GetInvokeMethod(typeof(ClassWithInvoke)));
        }
    }
}