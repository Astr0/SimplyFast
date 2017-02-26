using System;
using Xunit;
using SimplyFast.Tests.Stubs;

namespace SimplyFast.Tests
{
    
    public class MathExTests
    {
        [Fact]
        public void ClipIsMethodOfMathHelperAndItReturnsValueIfInRange()
        {
            const byte v1 = 1;
            const short v2 = 1;
            const ushort v3 = 1;
            const int v4 = 1;
            const uint v5 = 1;
            const long v6 = 1;
            const ulong v7 = 1;
            const sbyte v8 = 1;
            const float v9 = 1;
            const double v10 = 1;
            const decimal v11 = 1;
            var v12 = TimeSpan.FromDays(1);
            var v13 = new TestComparable(1);

            Assert.Equal(v1, v1.Clip((byte)0, (byte)2));
            Assert.Equal(v2, v2.Clip((short)0, (short)2));
            Assert.Equal(v3, v3.Clip((ushort)0, (ushort)2));
            Assert.Equal(v4, v4.Clip(0, 2));
            Assert.Equal(v5, v5.Clip(0U, 2U));
            Assert.Equal(v6, v6.Clip(0, 2));
            Assert.Equal(v7, v7.Clip(0UL, 2UL));
            Assert.Equal(v8, v8.Clip((sbyte)0, (sbyte)2));
            Assert.Equal(v9, v9.Clip(0, 2));
            Assert.Equal(v10, v10.Clip(0, 2));
            Assert.Equal(v11, v11.Clip(0, 2));
            Assert.Equal(v12, v12.Clip(TimeSpan.FromDays(0), TimeSpan.FromDays(2)));
            Assert.Equal(v13, v13.Clip(new TestComparable(0), new TestComparable(2)));
        }

        [Fact]
        public void ClipReturnsMaxIfValueGreater()
        {
            const byte v1 = 7;
            const short v2 = 7;
            const ushort v3 = 7;
            const int v4 = 7;
            const uint v5 = 7;
            const long v6 = 7;
            const ulong v7 = 7;
            const sbyte v8 = 7;
            const float v9 = 7;
            const double v10 = 7;
            const decimal v11 = 7;
            var v12 = TimeSpan.FromDays(7);
            var v13 = new TestComparable(7);

            Assert.Equal(5, v1.Clip((byte)3, (byte)5));
            Assert.Equal(5, v2.Clip((short)3, (short)5));
            Assert.Equal(5, v3.Clip((ushort)3, (ushort)5));
            Assert.Equal(5, v4.Clip(3, 5));
            Assert.Equal(5U, v5.Clip(3U, 5U));
            Assert.Equal(5, v6.Clip(3, 5));
            Assert.Equal(5UL, v7.Clip(3UL, 5UL));
            Assert.Equal(5, v8.Clip((sbyte)3, (sbyte)5));
            Assert.Equal(5, v9.Clip(3, 5));
            Assert.Equal(5, v10.Clip(3, 5));
            Assert.Equal(5, v11.Clip(3, 5));
            Assert.Equal(TimeSpan.FromDays(5), v12.Clip(TimeSpan.FromDays(3), TimeSpan.FromDays(5)));
            Assert.Equal(new TestComparable(5), v13.Clip(new TestComparable(3), new TestComparable(5)));
        }

        [Fact]
        public void ClipReturnsMinIfValueLess()
        {
            const byte v1 = 1;
            const short v2 = 1;
            const ushort v3 = 1;
            const int v4 = 1;
            const uint v5 = 1;
            const long v6 = 1;
            const ulong v7 = 1;
            const sbyte v8 = 1;
            const float v9 = 1;
            const double v10 = 1;
            const decimal v11 = 1;
            var v12 = TimeSpan.FromDays(1);
            var v13 = new TestComparable(1);

            Assert.Equal(3, v1.Clip((byte)3, (byte)5));
            Assert.Equal(3, v2.Clip((short)3, (short)5));
            Assert.Equal(3, v3.Clip((ushort)3, (ushort)5));
            Assert.Equal(3, v4.Clip(3, 5));
            Assert.Equal(3U, v5.Clip(3U, 5U));
            Assert.Equal(3, v6.Clip(3, 5));
            Assert.Equal(3UL, v7.Clip(3UL, 5UL));
            Assert.Equal(3, v8.Clip((sbyte)3, (sbyte)5));
            Assert.Equal(3, v9.Clip(3, 5));
            Assert.Equal(3, v10.Clip(3, 5));
            Assert.Equal(3, v11.Clip(3, 5));
            Assert.Equal(TimeSpan.FromDays(3), v12.Clip(TimeSpan.FromDays(3), TimeSpan.FromDays(5)));
            Assert.Equal(new TestComparable(3), v13.Clip(new TestComparable(3), new TestComparable(5)));
        }

        [Fact]
        public void ClipThrowsIfMaxLessThanMin()
        {
            const byte v1 = 7;
            const short v2 = 7;
            const ushort v3 = 7;
            const int v4 = 7;
            const uint v5 = 7;
            const long v6 = 7;
            const ulong v7 = 7;
            const sbyte v8 = 7;
            const float v9 = 7;
            const double v10 = 7;
            const decimal v11 = 7;
            var v12 = TimeSpan.FromDays(7);
            var v13 = new TestComparable(7);

            Assert.Throws(typeof(ArgumentException), () => v1.Clip((byte)5, (byte)3));
            Assert.Throws(typeof(ArgumentException), () => v2.Clip((short)5, (short)3));
            Assert.Throws(typeof(ArgumentException), () => v3.Clip((ushort)5, (ushort)3));
            Assert.Throws(typeof(ArgumentException), () => v4.Clip(5, 3));
            Assert.Throws(typeof(ArgumentException), () => v5.Clip(5U, 3U));
            Assert.Throws(typeof(ArgumentException), () => v6.Clip(5, 3));
            Assert.Throws(typeof(ArgumentException), () => v7.Clip(5UL, 3UL));
            Assert.Throws(typeof(ArgumentException), () => v8.Clip((sbyte)5, (sbyte)3));
            Assert.Throws(typeof(ArgumentException), () => v9.Clip(5, 3));
            Assert.Throws(typeof(ArgumentException), () => v10.Clip(5, 3));
            Assert.Throws(typeof(ArgumentException), () => v11.Clip(5, 3));
            Assert.Throws(typeof(ArgumentException), () => v12.Clip(TimeSpan.FromDays(5), TimeSpan.FromDays(3)));
            Assert.Throws(typeof(ArgumentException), () => v13.Clip(new TestComparable(5), new TestComparable(3)));
        }

        [Fact]
        public void ComparableMinMaxWorks()
        {
            var c1 = new TestComparable(1);
            var c2 = new TestComparable(2);

            Assert.Equal(c1, MathEx.Min(c1, c2));
            Assert.Equal(c1, MathEx.Min(c2, c1));
            Assert.Equal(c2, MathEx.Max(c1, c2));
            Assert.Equal(c2, MathEx.Max(c2, c1));
        }

        [Fact]
        public void DateTimeMinMaxWorks()
        {
            var dt1 = new DateTime(1999, 01, 1);
            var dt2 = new DateTime(1999, 01, 2);

            Assert.Equal(dt1, MathEx.Min(dt1, dt2));
            Assert.Equal(dt1, MathEx.Min(dt2, dt1));
            Assert.Equal(dt2, MathEx.Max(dt1, dt2));
            Assert.Equal(dt2, MathEx.Max(dt2, dt1));
        }

        [Fact]
        public void InRangeWorks()
        {
            Assert.True(5.InRange(1, 10));
            Assert.True(5d.InRange(5d, 10d));
            Assert.True(5.InRange(1, 5));
            Assert.True(0.InRange(int.MinValue, int.MaxValue));
            Assert.False(5U.InRange(6U, 10U));
            Assert.False(5.InRange(-1, 4));
            Assert.True(TimeSpan.FromDays(10).InRange(TimeSpan.FromDays(1), TimeSpan.FromDays(11)));
            Assert.True(TimeSpan.FromDays(10).InRange(TimeSpan.FromDays(10), TimeSpan.FromDays(11)));
            Assert.True(TimeSpan.FromDays(10).InRange(TimeSpan.FromDays(9), TimeSpan.FromDays(10)));
            Assert.Throws<ArgumentException>(() => 5.InRange(8, 3));
            Assert.True((-2f).InRange(-2f, -2f));
        }

    }
}