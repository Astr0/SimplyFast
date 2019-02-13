using System;
using System.Linq;

// ReSharper disable All

namespace SimplyFast.Reflection.Tests.TestData
{
    public class SomeClass2 : SomeClass1
    {
        private static object _f3 = "_f3t";
        private long _f1;
        public static readonly object FStatic = new object();

        public SomeClass2() : this(string.Empty, 11)
        {
        }

        private SomeClass2(string p2, int f1)
        {
            P2 = p2;
            _f1 = f1;
        }

        public new int P1
        {
            get { return (int)_f1; }
            set { _f1 = value; }
        }

        [Some]
        public new string P2 { get; set; }

        [Some(Value = 35)]
        public new string P3 { get; set; }

        [Some(Value = 45)]
        public override string P4 { get; set; }

        public override string P5 { get; set; }

        public static object F3
        {
            get { return _f3; }
            set { _f3 = value; }
        }

        public int GetF1()
        {
            return (int)_f1;
        }

        public virtual void SetP2P3(string p2, string p3)
        {
            SetP2P3Test(p2, p3);
        }

        private void SetP2P3Test(string p2, string p3)
        {
            P2 = p2;
            P3 = p3;
        }

        public static int Sum(int a, int b, params int[] args)
        {
            return a + b + args.Sum();
        }

        public static int Sum(int a, int b, out int minus, ref int sum)
        {
            minus = a - b;
            sum = a + b + sum;
            return sum;
        }

        public static T Max<T>(T a, T b) where T : IComparable<T>
        {
            return a.CompareTo(b) > 0 ? a : b;
        }

        public static T Max2<T>(T a, T b) where T : IComparable<T>
        {
            return a.CompareTo(b) > 0 ? a : b;
        }

        public static TP Max<T, TP>(T a, TP b)
            where TP : IComparable
        {
            var pa = (TP) Convert.ChangeType(a, typeof (TP));
            return pa.CompareTo(b) > 0 ? pa : b;
        }

        public static T Max3<T>(T a, T b) where T : IComparable<T>
        {
            return a.CompareTo(b) > 0 ? a : b;
        }

        public static T Max3<T>(T a, T b, T c) where T : IComparable<T>
        {
            return Max3(Max3(a, b), c);
        }

        public static double Max3(double a, double b)
        {
            return 1 + (a.CompareTo(b) > 0 ? a : b);
        }

        public static int Max3(int a, int b)
        {
            return 2 + (a.CompareTo(b) > 0 ? a : b);
        }

        public static int Max3(int a, int b, int c)
        {
            return Max3(Max3(a, b), c);
        }
    }
}