using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using SF.Expressions;

namespace SF.Tests.Expressions
{
    [TestFixture]
    public class ExpressionExPartialEvalTests
    {
        public static int TestFuncDontEval(int x)
        {
            throw new InvalidOperationException("Don't eval me!");
        }

        public static int TestFuncEval(int x)
        {
            return x;
        }

        [Test]
        public void TestPartialEvalEvalsAll()
        {
            Expression<Func<int>> test = () => 2 + TestFuncEval(1);
            var expr = test.Body;
            Assert.AreEqual(ExpressionType.Add, expr.NodeType);
            var evaled = ExpressionEx.PartialEval(expr);
            Assert.AreEqual(ExpressionType.Constant, evaled.NodeType);
        }

        [Test]
        public void TestPartialEvalDontEvalsWhatThrows()
        {
            Expression<Func<int>> test = () => 2 + TestFuncDontEval(1);
            var expr = test.Body;
            Assert.AreEqual(ExpressionType.Add, expr.NodeType);
            var evaled = ExpressionEx.PartialEval(expr);
            Assert.AreEqual(ExpressionType.Add, evaled.NodeType);
        }

        private class ClassWithVoid
        {
            public int Value;

            public void SetValue(int value)
            {
                Value = value;
            }

            public void InvokeWith42(int value)
            {
                if (value != 42)
                    throw new InvalidOperationException("Invoke me with 42!");
                Value = value;
            }
        }

        [Test]
        public void TestPartialEvalEvalsVoid()
        {
            var c = new ClassWithVoid();
            Expression<Action> test = () => c.SetValue(12);
            var expr = test.Body;
            Assert.AreEqual(ExpressionType.Call, expr.NodeType);
            var evaled = ExpressionEx.PartialEval(expr);
            Assert.AreEqual(ExpressionType.Default, evaled.NodeType);
            Assert.AreEqual(12, c.Value);
        }

        [Test]
        public void TestPartialDontEvalsVoidThatThrows()
        {
            var c = new ClassWithVoid();
            Expression<Action> test = () => c.InvokeWith42(10);
            var expr = test.Body;
            Assert.AreEqual(ExpressionType.Call, expr.NodeType);
            var evaled = ExpressionEx.PartialEval(expr);
            Assert.AreEqual(ExpressionType.Call, evaled.NodeType);
            test = () => c.InvokeWith42(42);

            expr = test.Body;
            Assert.AreEqual(ExpressionType.Call, expr.NodeType);
            evaled = ExpressionEx.PartialEval(expr);
            Assert.AreEqual(ExpressionType.Default, evaled.NodeType);

            Assert.AreEqual(42, c.Value);
        }

        [Test]
        public void PartialEvalListInit()
        {
            Expression<Func<int, List<int>>> expr = x => new List<int> { x, x, x, 2 };
            Assert.AreEqual(ExpressionType.ListInit, expr.Body.NodeType);
            var evaled = (LambdaExpression)ExpressionEx.PartialEval(expr);
            Assert.AreEqual(ExpressionType.ListInit, evaled.Body.NodeType);

            expr = x => new List<int> { 1, 2, 3, 4 };
            Assert.AreEqual(ExpressionType.ListInit, expr.Body.NodeType);
            evaled = (LambdaExpression)ExpressionEx.PartialEval(expr);
            Assert.AreEqual(ExpressionType.Constant, evaled.Body.NodeType);

            expr = x => new List<int> { TestFuncEval(1), x };
            Assert.AreEqual(ExpressionType.ListInit, expr.Body.NodeType);
            var listInit = (ListInitExpression)expr.Body;
            Assert.AreEqual(ExpressionType.Call, listInit.Initializers[0].Arguments[0].NodeType);
            evaled = (LambdaExpression)ExpressionEx.PartialEval(expr);
            Assert.AreEqual(ExpressionType.ListInit, evaled.Body.NodeType);
            listInit = (ListInitExpression)evaled.Body;
            Assert.AreEqual(ExpressionType.Constant, listInit.Initializers[0].Arguments[0].NodeType);
        }

        public class Test
        {
            public Test()
            {
                Next = new Test(5);
            }

            public Test(int count)
            {
                if (count > 0)
                    Next = new Test(count - 1);
            }

            public List<int> List = new List<int>();
            public Test Next;
        }

        [Test]
        public virtual void PartialEvalMemberInit()
        {
            Expression<Func<int, Test>> expr = x => new Test
            {
                List = new List<int>(x),
                Next =
                {
                    Next =
                    {
                        List = { 1, 2, 3, x }
                    },
                    List = { 1, 2, 3 }
                }
            };
            var evaled = (LambdaExpression)ExpressionEx.PartialEval(expr);
            Assert.AreEqual(ExpressionType.MemberInit, evaled.Body.NodeType);
            expr = x => new Test
            {
                List = new List<int>(3),
                Next =
                {
                    Next =
                    {
                        List = {1, 2, 3}
                    },
                    List = {1, 2, 3}
                }
            };
            evaled = (LambdaExpression)ExpressionEx.PartialEval(expr);
            Assert.AreEqual(ExpressionType.Constant, evaled.Body.NodeType);
        }
    }
}