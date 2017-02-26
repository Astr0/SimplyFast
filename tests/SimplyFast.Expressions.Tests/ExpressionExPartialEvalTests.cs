using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Xunit;

namespace SimplyFast.Expressions.Tests
{
    
    public sealed class ExpressionExPartialEvalTests
    {
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static int TestFuncDontEval(int x)
        {
            throw new InvalidOperationException("Don't eval me!");
        }

        private static int TestFuncEval(int x)
        {
            return x;
        }

        [Fact]
        public void TestPartialEvalEvalsAll()
        {
            Expression<Func<int>> test = () => 2 + TestFuncEval(1);
            var expr = test.Body;
            Assert.Equal(ExpressionType.Add, expr.NodeType);
            var evaled = ExpressionEx.PartialEval(expr);
            Assert.Equal(ExpressionType.Constant, evaled.NodeType);
        }

        [Fact]
        public void TestPartialEvalDontEvalsWhatThrows()
        {
            Expression<Func<int>> test = () => 2 + TestFuncDontEval(1);
            var expr = test.Body;
            Assert.Equal(ExpressionType.Add, expr.NodeType);
            var evaled = ExpressionEx.PartialEval(expr);
            Assert.Equal(ExpressionType.Add, evaled.NodeType);
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

        [Fact]
        public void TestPartialEvalEvalsVoid()
        {
            var c = new ClassWithVoid();
            Expression<Action> test = () => c.SetValue(12);
            var expr = test.Body;
            Assert.Equal(ExpressionType.Call, expr.NodeType);
            var evaled = ExpressionEx.PartialEval(expr);
            Assert.Equal(ExpressionType.Default, evaled.NodeType);
            Assert.Equal(12, c.Value);
        }

        [Fact]
        public void TestPartialDontEvalsVoidThatThrows()
        {
            var c = new ClassWithVoid();
            Expression<Action> test = () => c.InvokeWith42(10);
            var expr = test.Body;
            Assert.Equal(ExpressionType.Call, expr.NodeType);
            var evaled = ExpressionEx.PartialEval(expr);
            Assert.Equal(ExpressionType.Call, evaled.NodeType);
            test = () => c.InvokeWith42(42);

            expr = test.Body;
            Assert.Equal(ExpressionType.Call, expr.NodeType);
            evaled = ExpressionEx.PartialEval(expr);
            Assert.Equal(ExpressionType.Default, evaled.NodeType);

            Assert.Equal(42, c.Value);
        }

        [Fact]
        public void PartialEvalListInit()
        {
            Expression<Func<int, List<int>>> expr = x => new List<int> { x, x, x, 2 };
            Assert.Equal(ExpressionType.ListInit, expr.Body.NodeType);
            var evaled = (LambdaExpression)ExpressionEx.PartialEval(expr);
            Assert.Equal(ExpressionType.ListInit, evaled.Body.NodeType);

            expr = x => new List<int> { 1, 2, 3, 4 };
            Assert.Equal(ExpressionType.ListInit, expr.Body.NodeType);
            evaled = (LambdaExpression)ExpressionEx.PartialEval(expr);
            Assert.Equal(ExpressionType.Constant, evaled.Body.NodeType);

            expr = x => new List<int> { TestFuncEval(1), x };
            Assert.Equal(ExpressionType.ListInit, expr.Body.NodeType);
            var listInit = (ListInitExpression)expr.Body;
            Assert.Equal(ExpressionType.Call, listInit.Initializers[0].Arguments[0].NodeType);
            evaled = (LambdaExpression)ExpressionEx.PartialEval(expr);
            Assert.Equal(ExpressionType.ListInit, evaled.Body.NodeType);
            listInit = (ListInitExpression)evaled.Body;
            Assert.Equal(ExpressionType.Constant, listInit.Initializers[0].Arguments[0].NodeType);
        }

        [SuppressMessage("ReSharper", "CollectionNeverQueried.Global")]
        [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
        public class Test
        {
            public Test()
            {
                Next = new Test(5);
            }

            [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
            public Test(int count)
            {
                if (count > 0)
                    Next = new Test(count - 1);
            }

            public List<int> List = new List<int>();
            public Test Next;
        }

        [Fact]
        public void PartialEvalMemberInit()
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
            Assert.Equal(ExpressionType.MemberInit, evaled.Body.NodeType);
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
            Assert.Equal(ExpressionType.Constant, evaled.Body.NodeType);
        }
    }
}