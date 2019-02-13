using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Xunit;
using SimplyFast.Reflection.Tests.TestData;

namespace SimplyFast.Reflection.Tests
{
    
    public class MethodInfoExTests
    {
        private delegate int SumDelegate(int a, int b, out int c, ref int d);

        private delegate object SumDelegateObj(object a, object b, out object c, ref object d);

        [Fact]
        public void FindOverloadReturnsNullOnMultipleGenericMatches()
        {
            var parameters = new[] { typeof(int), typeof(int) };
            var methodGeneric = typeof(SomeClass2).FindMethod("Max", parameters);
            Assert.Null(methodGeneric);
            var method = typeof(SomeClass2).FindMethod("Max", parameters);
            Assert.Null(method);
        }

        [Fact]
        public void FindOverloadWorks()
        {
            var intParameters = new[] { typeof(int), typeof(int) };
            var normalInt = typeof(SomeClass2).FindMethod("Max3", intParameters);
            Assert.NotNull(normalInt);
            Assert.Equal(7, normalInt.InvokerAs<Func<object, object, object>>()(3, 5));

            var doubleParameters = new[] { typeof(double), typeof(double) };
            var normalDouble = typeof(SomeClass2).FindMethod("Max3", doubleParameters);
            Assert.NotNull(normalDouble);
            Assert.Equal(6.5, normalDouble.InvokerAs<Func<double, double, double>>()(3.5, 5.5));

            var floatParameters = new[] { typeof(float), typeof(float) };
            var genericFloat = typeof(SomeClass2).FindMethod("Max3", floatParameters);
            Assert.NotNull(normalDouble);
            Assert.True(genericFloat.IsGenericMethod);
            Assert.Equal(5.5f, genericFloat.InvokerAs<Func<float, float, float>>()(3.5f, 5.5f));
        }

        [Fact]
        public void GenericWorks()
        {
            var genericMethod = typeof(SomeClass2).Method("Max", 1, Substitute.T[0], Substitute.T[0]);
            Assert.NotNull(genericMethod);
            var method = genericMethod.MakeGeneric(typeof(int));
            Assert.Equal(5, method.InvokerAs<Func<int, int, int>>()(3, 5));
            var method2 = genericMethod.MakeGeneric(typeof(double));
            Assert.Equal(3.7, method2.InvokerAs<Func<object, double, double>>()(3.7, 1.2));
        }

        [Fact]
        public void InvokesIntMethod()
        {
            var t2 = new SomeClass2();
            var t3 = new SomeClass3();
            var method = typeof(SomeClass2).Method("GetF1");
            var invoker1 = method.InvokerAs<Func<SomeClass2, int>>();
            var invoker2 = method.InvokerAs<Func<object, object>>();
            Assert.Equal(11, invoker1(t2));
            Assert.Equal(11, invoker2(t2));
            Assert.Equal(11, invoker1(t3));
            Assert.Equal(11, invoker2(t3));
        }

        [Fact]
        public void InvokesIntMethodInvoker()
        {
            var t2 = new SomeClass2();
            var t3 = new SomeClass3();
            var method = typeof(SomeClass2).Method("GetF1");
            var invoker = method.Invoker();
            Assert.Equal(11, invoker(t2));
            Assert.Equal(11, invoker(t3));
        }

        [Fact]
        public void InvokesIntMethodAs()
        {
            var t2 = new SomeClass2();
            var t3 = new SomeClass3();
            var method = typeof(SomeClass2).Method("GetF1");
            var invoker1 = method.InvokerAs<Func<SomeClass2, int>>();
            var invoker2 = method.InvokerAs<Func<object, object>>();
            Assert.Equal(11, invoker1(t2));
            Assert.Equal(11, invoker2(t2));
            Assert.Equal(11, invoker1(t3));
            Assert.Equal(11, invoker2(t3));
        }

        [Fact]
        public void InvokesNewMethod()
        {
            var t3 = new SomeClass3();
            var method = typeof(SomeClass3).Method("GetF1");
            Assert.Equal(12, method.InvokerAs<Func<SomeClass3, object>>()(t3));
            Assert.Equal(12, method.InvokerAs<Func<SomeClass3, int>>()(t3));
        }

        [Fact]
        public void InvokesNewMethodAs()
        {
            var t3 = new SomeClass3();
            var method = typeof(SomeClass3).Method("GetF1");
            Assert.Equal(12, method.InvokerAs<Func<SomeClass3, object>>()(t3));
            Assert.Equal(12, method.InvokerAs<Func<SomeClass3, int>>()(t3));
        }

        [Fact]
        public void InvokesPrivateMethod()
        {
            var t2 = new SomeClass2();
            var method = typeof(SomeClass2).Method("SetP2P3Test", typeof(string), typeof(string));
            Assert.Null(method.InvokerAs<Func<object, object, object, object>>()(t2, "t1", "t2"));
            Assert.Equal("t1", t2.P2);
            Assert.Equal("t2", t2.P3);
            Assert.Null(method.InvokerAs<Func<object, string, string, object>>()(t2, "t1!", "t2!"));
            Assert.Equal("t1!", t2.P2);
            Assert.Equal("t2!", t2.P3);
        }

        [Fact]
        public void InvokesPrivateMethodInvoker()
        {
            var t2 = new SomeClass2();
            var method = typeof(SomeClass2).Method("SetP2P3Test", typeof(string), typeof(string));
            Assert.Null(method.Invoker()(t2, "t1", "t2"));
            Assert.Equal("t1", t2.P2);
            Assert.Equal("t2", t2.P3);
        }

        [Fact]
        public void InvokesPrivateMethodAs()
        {
            var t2 = new SomeClass2();
            var method = typeof(SomeClass2).Method("SetP2P3Test", typeof(string), typeof(string));
            Assert.Null(method.InvokerAs<Func<object, object, object, object>>()(t2, "t1", "t2"));
            Assert.Equal("t1", t2.P2);
            Assert.Equal("t2", t2.P3);
            Assert.Null(method.InvokerAs<Func<object, string, string, object>>()(t2, "t1!", "t2!"));
            Assert.Equal("t1!", t2.P2);
            Assert.Equal("t2!", t2.P3);
        }

        [Fact]
        public void InvokesStaticOptionalArgs()
        {
            var method = typeof(SomeClass2).Method("Sum", typeof(int), typeof(int), typeof(int[]));
            Assert.Equal(10, method.InvokerAs<Func<int, int, int[], int>>()(4, 6, new int[0]));
            Assert.Equal(25, method.InvokerAs<Func<object, object, int[], int>>()(4, 6, new[] { 1, 2, 3, 4, 5 }));
        }

        [Fact]
        public void InvokesStaticOptionalArgsAs()
        {
            var method = typeof(SomeClass2).Method("Sum", typeof(int), typeof(int), typeof(int[]));
            Assert.Equal(10, method.InvokerAs<Func<int, int, int[], int>>()(4, 6, new int[0]));
            Assert.Equal(25, method.InvokerAs<Func<object, object, int[], int>>()(4, 6, new[] { 1, 2, 3, 4, 5 }));
        }

        [Fact]
        public void InvokesStaticOptionalArgsInvoker()
        {
            var method = typeof(SomeClass2).Method("Sum", typeof(int), typeof(int), typeof(int[]));
            Assert.Equal(10, method.Invoker()(null, 4, 6, new int[0]));
            Assert.Equal(25, method.Invoker()(null, 4, 6, new[] { 1, 2, 3, 4, 5 }));
        }

        [Fact]
        public void InvokesVirtualMethod()
        {
            var t2 = new SomeClass2();
            var t3 = new SomeClass3();
            var method = typeof(SomeClass2).Method("SetP2P3", typeof(string), typeof(string));
            Assert.Null(method.InvokerAs<Func<SomeClass2, string, string, object>>()(t2, "t1", "t2"));
            Assert.Equal("t1", t2.P2);
            Assert.Equal("t2", t2.P3);
            Assert.Null(method.InvokerAs<Func<object, string, object, object>>()(t2, "t1!", "t2!"));
            Assert.Equal("t1!", t2.P2);
            Assert.Null(method.InvokerAs<Func<SomeClass2, string, string, object>>()(t3, "t1", "t2"));
            Assert.Equal("t1_", t3.P2);
            Assert.Equal("t2_", t3.P3);
            Assert.Null(method.InvokerAs<Func<SomeClass2, string, string, object>>()(t3, "t1!", "t2!"));
            Assert.Equal("t1!_", t3.P2);
            Assert.Equal("t2!_", t3.P3);
            var method2 = typeof(SomeClass3).Method("SetP2P3", typeof(string), typeof(string));
            Assert.Null(method2.InvokerAs<Func<SomeClass2, string, string, object>>()(t3, "t11", "t22"));
            Assert.Equal("t11_", t3.P2);
            Assert.Equal("t22_", t3.P3);
            Assert.Null(method2.InvokerAs<Func<SomeClass2, string, string, object>>()(t3, "t11!", "t22!"));
            Assert.Equal("t11!_", t3.P2);
            Assert.Equal("t22!_", t3.P3);
        }

        [Fact]
        public void InvokesVirtualMethodAs()
        {
            var t2 = new SomeClass2();
            var t3 = new SomeClass3();
            var method = typeof(SomeClass2).Method("SetP2P3", typeof(string), typeof(string));
            Assert.Null(method.InvokerAs<Func<SomeClass2, string, string, object>>()(t2, "t1", "t2"));
            Assert.Equal("t1", t2.P2);
            Assert.Equal("t2", t2.P3);
            Assert.Null(method.InvokerAs<Func<object, string, object, object>>()(t2, "t1!", "t2!"));
            Assert.Equal("t1!", t2.P2);
            Assert.Null(method.InvokerAs<Func<SomeClass2, string, string, object>>()(t3, "t1", "t2"));
            Assert.Equal("t1_", t3.P2);
            Assert.Equal("t2_", t3.P3);
            Assert.Null(method.InvokerAs<Func<SomeClass2, string, string, object>>()(t3, "t1!", "t2!"));
            Assert.Equal("t1!_", t3.P2);
            Assert.Equal("t2!_", t3.P3);
            var method2 = typeof(SomeClass3).Method("SetP2P3", typeof(string), typeof(string));
            Assert.Null(method2.InvokerAs<Func<SomeClass2, string, string, object>>()(t3, "t11", "t22"));
            Assert.Equal("t11_", t3.P2);
            Assert.Equal("t22_", t3.P3);
            Assert.Null(method2.InvokerAs<Func<SomeClass2, string, string, object>>()(t3, "t11!", "t22!"));
            Assert.Equal("t11!_", t3.P2);
            Assert.Equal("t22!_", t3.P3);
        }

        [Fact]
        public void ModifiersWorks()
        {
            var method = typeof(SomeClass2).Method("Sum", typeof(int), typeof(int), typeof(int).MakeByRefType(), typeof(int).MakeByRefType());
            Assert.NotNull(method);
            int c;
            var d = 2;
            Assert.Equal(12, method.InvokerAs<SumDelegate>()(7, 3, out c, ref d));
            Assert.Equal(4, c);
            Assert.Equal(12, d);
            object cobj;
            object dobj = 2;
            var invoker = method.InvokerAs<SumDelegateObj>();
            Assert.Equal(12, invoker(7, 3, out cobj, ref dobj));
            Assert.Equal(4, cobj);
            Assert.Equal(12, dobj);
        }

        [Fact]
        public void ModifiersWorksAs()
        {
            var method = typeof(SomeClass2).Method("Sum", typeof(int), typeof(int), typeof(int).MakeByRefType(), typeof(int).MakeByRefType());
            Assert.NotNull(method);
            int c;
            var d = 2;
            Assert.Equal(12, method.InvokerAs<SumDelegate>()(7, 3, out c, ref d));
            Assert.Equal(4, c);
            Assert.Equal(12, d);
            object cobj;
            object dobj = 2;
            var invoker = method.InvokerAs<SumDelegateObj>();
            Assert.Equal(12, invoker(7, 3, out cobj, ref dobj));
            Assert.Equal(4, cobj);
            Assert.Equal(12, dobj);
        }

        [Fact]
        public void IsOperatorWorks()
        {
            var methods =
                typeof(decimal).GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                            BindingFlags.NonPublic);
            var count = methods.Count(x => x.IsOperator());
            Assert.Equal(37, count);
        }

        private class ClassWithInvoke
        {
            [SuppressMessage("ReSharper", "UnusedMember.Local")]
            public void Invoke() { }
        }

        [Fact]
        public void GetInvokeMethodWorks()
        {
            Assert.Equal(typeof(string), MethodInfoEx.GetInvokeMethod(typeof(Func<string>)).ReturnType);
            Assert.Equal(0, MethodInfoEx.GetInvokeMethod(typeof(Func<string>)).GetParameters().Length);
            Assert.Equal(typeof(void), MethodInfoEx.GetInvokeMethod(typeof(Action<string, string>)).ReturnType);
            Assert.Equal(2, MethodInfoEx.GetInvokeMethod(typeof(Action<string, string>)).GetParameters().Length);
            Assert.Throws<ArgumentException>(() => MethodInfoEx.GetInvokeMethod(typeof(string)));
            Assert.Throws<ArgumentException>(() => MethodInfoEx.GetInvokeMethod(typeof(ClassWithInvoke)));
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public class Cast1
        {
            public static explicit operator int(Cast1 a)
            {
                return 0;
            }

            public static implicit operator double(Cast1 a)
            {
                return 0;
            }

            public static explicit operator Cast2(Cast1 a)
            {
                return new Cast2();
            }
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public class Cast2
        {
            public static explicit operator Cast2(int a)
            {
                return new Cast2();
            }

            public static implicit operator Cast2(double a)
            {
                return new Cast2();
            }

            public static implicit operator Cast2(Cast1 a)
            {
                return new Cast2();
            }

            public static explicit operator Cast1(Cast2 a)
            {
                return new Cast1();
            }
        }

        [Fact]
        public void FindCastToWorks()
        {
            var t = typeof(Cast1);
            Assert.Equal(typeof(int), MethodInfoEx.FindCastToOperator(t, typeof(int)).ReturnType);
            Assert.Equal(typeof(double), MethodInfoEx.FindCastToOperator(t, typeof(double)).ReturnType);
            Assert.Equal(typeof(Cast2), MethodInfoEx.FindCastToOperator(t, typeof(Cast2)).ReturnType);
            Assert.Null(MethodInfoEx.FindCastToOperator(t, typeof(string)));
            Assert.Null(MethodInfoEx.FindCastToOperator(t, typeof(decimal)));
        }

        [Fact]
        public void FindCastFromWorks()
        {
            var t = typeof(Cast2);
            Assert.Equal(typeof(int), MethodInfoEx.FindCastFromOperator(typeof(int), t).GetParameterTypes()[0]);
            Assert.Equal(typeof(double), MethodInfoEx.FindCastFromOperator(typeof(double), t).GetParameterTypes()[0]);
            Assert.Equal(typeof(Cast1), MethodInfoEx.FindCastFromOperator(typeof(Cast1), t).GetParameterTypes()[0]);
            Assert.Null(MethodInfoEx.FindCastFromOperator(typeof(string), t));
            Assert.Null(MethodInfoEx.FindCastFromOperator(typeof(decimal), t));
        }

        [Fact]
        public void FindCastWorks()
        {
            var t1 = typeof(Cast1);
            var t2 = typeof(Cast2);
            Assert.Equal(typeof(int), MethodInfoEx.FindCastOperator(t1, typeof(int)).ReturnType);
            Assert.Equal(typeof(double), MethodInfoEx.FindCastOperator(t1, typeof(double)).ReturnType);
            Assert.Null(MethodInfoEx.FindCastToOperator(t1, typeof(string)));
            Assert.Null(MethodInfoEx.FindCastToOperator(t1, typeof(decimal)));

            Assert.Equal(typeof(int), MethodInfoEx.FindCastOperator(typeof(int), t2).GetParameterTypes()[0]);
            Assert.Equal(typeof(double), MethodInfoEx.FindCastOperator(typeof(double), t2).GetParameterTypes()[0]);
            Assert.Equal(t2, MethodInfoEx.FindCastOperator(t2, t1).GetParameterTypes()[0]);
            Assert.Null(MethodInfoEx.FindCastOperator(typeof(string), t2));
            Assert.Null(MethodInfoEx.FindCastOperator(typeof(decimal), t2));

            Assert.Throws<AmbiguousMatchException>(() => MethodInfoEx.FindCastOperator(t1, t2));
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private class ClassWith2Methods
        {
            public void SomeInstance()
            {
            }

            [SuppressMessage("ReSharper", "UnusedParameter.Local")]
            public static void SomeStatic<T>(T v, int x)
            {
            }
        }

        [Fact]
        public void MethodsOk()
        {
            var objMethods =
                typeof(object).GetMethods(BindingFlags.Instance | BindingFlags.Public |
                                          (MemberInfoEx.PrivateAccess ? BindingFlags.NonPublic : 0))
                                          .Where(x => (x.Attributes & MethodAttributes.Family) != 0)
                                          .ToArray();

            var methods = typeof(ClassWith2Methods).Methods();
            Assert.Equal(2 + objMethods.Length, methods.Length);
            Assert.Equal(2, methods.Count(x => x.DeclaringType == typeof(ClassWith2Methods)));
            Assert.Equal(1, typeof(ClassWith2Methods).Methods("ToString").Length);
            Assert.NotNull(typeof(ClassWith2Methods).Method("SomeStatic", new[] { typeof(string) }, new[] { typeof(string), typeof(int) }));
        }


        private interface ITest
        {
            T GetDefault<T>();
        }

        private class SomeInterface : ITest
        {
            public T GetDefault<T>()
            {
                return default(T);
            }
        }

        [Fact]
        public void InvokesGenericInterfaceMethod()
        {
            var test = new SomeInterface();
            var method = typeof(ITest).Method("GetDefault").MakeGeneric(typeof(int));
            var exactInvoker = method.InvokerAs<Func<ITest, int>>();
            Assert.Equal(test.GetDefault<int>(), exactInvoker(test));
            var returnInvoker = method.InvokerAs<Func<ITest, object>>();
            Assert.Equal(test.GetDefault<int>(), returnInvoker(test));
            var convertInvoker = method.InvokerAs<Func<object, object>>();
            Assert.Equal(test.GetDefault<int>(), convertInvoker(test));
        }

    }
}