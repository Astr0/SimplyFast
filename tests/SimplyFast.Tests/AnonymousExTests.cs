using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace SimplyFast.Tests
{
    
    public class AnonymousExTests
    {
        [Fact]
        public void ActionWorks()
        {
            var a = new { a = 1 };
            var val = 0;
            var act = AnonymousEx.Action(a, x => val = x.a);
            Assert.Equal(0, val);
            act(a);
            Assert.Equal(1, val);
            act(new { a = 2 });
            Assert.Equal(2, val);
        }

        [Fact]
        public void FuncWorks()
        {
            var a = new { a = 1 };
            var func = AnonymousEx.Func(a, x => x.a + 1);
            Assert.Equal(2, func(a)); 
            Assert.Equal(3, func(new { a = 2 }));
        }

        [Fact]
        public void ExprActionWorks()
        {
            var a = new { a = 1 };
            var list = AnonymousEx.List(a);
            var act = AnonymousEx.ExpressionAction(a, x => list.Add(x)).Compile();
            Assert.False(list.Contains(a));
            act(a);
            Assert.True(list.Contains(a));
            var b = new { a = 2 };
            act(b);
            Assert.True(list.Contains(b));
        }

        [Fact]
        public void ExpressionFuncWorks()
        {
            var a = new { a = 1 };
            var func = AnonymousEx.ExpressionFunc(a, x => x.a + 1).Compile();
            Assert.Equal(2, func(a)); 
            Assert.Equal(3, func(new { a = 2 }));
        }

        [Fact]
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public void EnumerableWorks()
        {
            var a = new { a = 1 };
            var enumerable = AnonymousEx.Enumerable(a);
            Assert.False(enumerable.Contains(a));
            Assert.False(enumerable.Any());
        }

        [Fact]
        public void ArrayWorks()
        {
            var a = new { a = 1 };
            var arr = AnonymousEx.Array(a, 1);
            Assert.Equal(1, arr.Length);
            Assert.Null(arr[0]);
            arr[0] = a;
            Assert.Equal(a, arr[0]);
        }

        [Fact]
        public void ListWorks()
        {
            var a = new { a = 1 };
            var list1 = AnonymousEx.List(a);
            Assert.Equal(0, list1.Count);
            list1.Add(a);
            Assert.Equal(a, list1[0]);

            var list2 = AnonymousEx.List(a, 10);
            Assert.Equal(0, list2.Count);
            Assert.Equal(10, list2.Capacity);
            list2.Add(a);
            Assert.Equal(a, list2[0]);
        }

        [Fact]
        public void StackWorks()
        {
            var a = new { a = 1 };
            var stack1 = AnonymousEx.Stack(a);
            Assert.Equal(0, stack1.Count);
            stack1.Push(a);
            Assert.Equal(a, stack1.Pop());

            var stack2 = AnonymousEx.Stack(a, 10);
            Assert.Equal(0, stack2.Count);
            stack2.Push(a);
            Assert.Equal(a, stack2.Pop());
        }

        [Fact]
        public void QueueWorks()
        {
            var a = new { a = 1 };
            var queue1 = AnonymousEx.Queue(a);
            Assert.Equal(0, queue1.Count);
            queue1.Enqueue(a);
            Assert.Equal(a, queue1.Dequeue());

            var queue2 = AnonymousEx.Queue(a, 10);
            Assert.Equal(0, queue2.Count);
            queue2.Enqueue(a);
            Assert.Equal(a, queue2.Dequeue());
        }

        [Fact]
        public void HashSetWorks()
        {
            var a = new { a = 1 };
            var set = AnonymousEx.HashSet(a);
            Assert.Equal(0, set.Count);
            set.Add(a);
            Assert.True(set.Contains(a));
            Assert.Equal(1, set.Count);
            set.Add(a);
            Assert.True(set.Contains(a));
            Assert.Equal(1, set.Count);

            Assert.True(set.SetEquals(new[] { a }));
        }
    }
}