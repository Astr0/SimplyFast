using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;

namespace SimplyFast.Tests
{
    [TestFixture]
    public class AnonymousExTests
    {
        [Test]
        public void ActionWorks()
        {
            var a = new { a = 1 };
            var val = 0;
            var act = AnonymousEx.Action(a, x => val = x.a);
            Assert.AreEqual(0, val);
            act(a);
            Assert.AreEqual(1, val);
            act(new { a = 2 });
            Assert.AreEqual(2, val);
        }

        [Test]
        public void FuncWorks()
        {
            var a = new { a = 1 };
            var func = AnonymousEx.Func(a, x => x.a + 1);
            Assert.AreEqual(2, func(a)); 
            Assert.AreEqual(3, func(new { a = 2 }));
        }

        [Test]
        public void ExprActionWorks()
        {
            var a = new { a = 1 };
            var list = AnonymousEx.List(a);
            var act = AnonymousEx.ExpressionAction(a, x => list.Add(x)).Compile();
            Assert.IsFalse(list.Contains(a));
            act(a);
            Assert.IsTrue(list.Contains(a));
            var b = new { a = 2 };
            act(b);
            Assert.IsTrue(list.Contains(b));
        }

        [Test]
        public void ExpressionFuncWorks()
        {
            var a = new { a = 1 };
            var func = AnonymousEx.ExpressionFunc(a, x => x.a + 1).Compile();
            Assert.AreEqual(2, func(a)); 
            Assert.AreEqual(3, func(new { a = 2 }));
        }

        [Test]
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public void EnumerableWorks()
        {
            var a = new { a = 1 };
            var enumerable = AnonymousEx.Enumerable(a);
            Assert.IsFalse(enumerable.Contains(a));
            Assert.IsFalse(enumerable.Any());
        }

        [Test]
        public void ArrayWorks()
        {
            var a = new { a = 1 };
            var arr = AnonymousEx.Array(a, 1);
            Assert.AreEqual(1, arr.Length);
            Assert.IsNull(arr[0]);
            arr[0] = a;
            Assert.AreEqual(a, arr[0]);
        }

        [Test]
        public void ListWorks()
        {
            var a = new { a = 1 };
            var list1 = AnonymousEx.List(a);
            Assert.AreEqual(0, list1.Count);
            list1.Add(a);
            Assert.AreEqual(a, list1[0]);

            var list2 = AnonymousEx.List(a, 10);
            Assert.AreEqual(0, list2.Count);
            Assert.AreEqual(10, list2.Capacity);
            list2.Add(a);
            Assert.AreEqual(a, list2[0]);
        }

        [Test]
        public void StackWorks()
        {
            var a = new { a = 1 };
            var stack1 = AnonymousEx.Stack(a);
            Assert.AreEqual(0, stack1.Count);
            stack1.Push(a);
            Assert.AreEqual(a, stack1.Pop());

            var stack2 = AnonymousEx.Stack(a, 10);
            Assert.AreEqual(0, stack2.Count);
            stack2.Push(a);
            Assert.AreEqual(a, stack2.Pop());
        }

        [Test]
        public void QueueWorks()
        {
            var a = new { a = 1 };
            var queue1 = AnonymousEx.Queue(a);
            Assert.AreEqual(0, queue1.Count);
            queue1.Enqueue(a);
            Assert.AreEqual(a, queue1.Dequeue());

            var queue2 = AnonymousEx.Queue(a, 10);
            Assert.AreEqual(0, queue2.Count);
            queue2.Enqueue(a);
            Assert.AreEqual(a, queue2.Dequeue());
        }

        [Test]
        public void HashSetWorks()
        {
            var a = new { a = 1 };
            var set = AnonymousEx.HashSet(a);
            Assert.AreEqual(0, set.Count);
            set.Add(a);
            Assert.IsTrue(set.Contains(a));
            Assert.AreEqual(1, set.Count);
            set.Add(a);
            Assert.IsTrue(set.Contains(a));
            Assert.AreEqual(1, set.Count);

            Assert.IsTrue(set.SetEquals(new[] { a }));
        }
    }
}