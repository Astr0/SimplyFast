//using System;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using Blink.Common.Meta;
//using NClone.MetadataProviders;
//using NUnit.Framework;

//namespace Blink.Tests.Common.Meta
//{
//    [TestFixture]
//    public class CloneExTests
//    {
//        [Test]
//        public void EverythingWorksOk()
//        {
//            var testInner = new TestNormal(1, null, new TestCopy(), new TestIgnore());
//            var testOuter = new TestNormal(2, testInner, new TestCopy(), new TestIgnore());

//            var outerClone = CloneEx.DeepClone(testOuter);
//            var innerClone = outerClone.Test;
            
//            Assert.NotNull(innerClone);
//            Assert.IsFalse(ReferenceEquals(innerClone, testInner));
//            Assert.AreEqual(1, innerClone.Prop);
//            Assert.IsNull(innerClone.Test);
//            Assert.IsNull(innerClone.ReadonlyTest);
            
//            Assert.IsTrue(ReferenceEquals(testInner.Copy, innerClone.Copy));
//            Assert.IsNull(innerClone.CopyProp);

//            Assert.IsNull(innerClone.Ignore);
//            Assert.IsNull(innerClone.IgnoreProp);
//            Assert.IsNull(innerClone.IgnoreCopy);

//            Assert.AreEqual(2, outerClone.Prop);
//            Assert.IsNotNull(outerClone.ReadonlyTest);
//            Assert.IsTrue(ReferenceEquals(outerClone.Test, outerClone.ReadonlyTest));
//            //Assert.AreEqual(1, outerClone.ReadonlyTest.Prop);
//            Assert.IsTrue(ReferenceEquals(testOuter.Copy, outerClone.Copy));
//            Assert.IsTrue(ReferenceEquals(testOuter.CopyProp, outerClone.CopyProp));

//            Assert.IsNull(outerClone.Ignore);
//            Assert.IsNull(outerClone.IgnoreProp);
//            Assert.IsNull(outerClone.IgnoreCopy);
//        }

//        [Test]
//        public void ThrowsForCircular()
//        {
//            var test1 = new TestNormal(1, null,new TestCopy(), new TestIgnore());
//            var test2 = new TestNormal(2, test1, new TestCopy(), new TestIgnore());
//            test1.Test = test2;
            
//            Assert.That(() => CloneEx.DeepClone(test1), Throws.Exception);
//            Assert.That(() => CloneEx.DeepClone(test2), Throws.Exception);
//        }

//        [Test]
//        public void WorksForStructs()
//        {
//            var a = new {test = new KeyValuePair<object, int>(Tuple.Create("test", 1), 1)};
//            var b = CloneEx.DeepClone(a);
//            Assert.AreEqual(Tuple.Create("test", 1), b.test.Key);
//            //Assert.IsFalse(ReferenceEquals(a.test.Key, b.test.Key));
//            Assert.AreEqual(1, b.test.Value);
//        }

//        [Test]
//        public void WorksForArrays()
//        {
//            var a = new
//            {
//                list = new[]
//                {
//                    Tuple.Create("test1", 1),
//                    Tuple.Create("test2", 2)
//                }
//            };
//            var b = CloneEx.DeepClone(a);
//            Assert.IsFalse(ReferenceEquals(a.list, b.list));
//            Assert.AreEqual(2, b.list.Length);
//            Assert.AreEqual(Tuple.Create("test1", 1), b.list[0]);
//            Assert.AreEqual(Tuple.Create("test2", 2), b.list[1]);
//            Assert.IsFalse(ReferenceEquals(a.list[0], b.list[0]));
//            Assert.IsFalse(ReferenceEquals(a.list[1], b.list[1]));
//        }

//        [Test]
//        public void WorksForArrayCopy()
//        {
//            var a = new[] {new TestCopy(), new TestCopy()};
//            var b = CloneEx.DeepClone(a);
//            Assert.AreEqual(2, b.Length);
//            Assert.IsTrue(ReferenceEquals(a[0], b[0]));
//            Assert.IsTrue(ReferenceEquals(a[1], b[1]));
//        }

//        [Test]
//        public void WorksForArrayIgnore()
//        {
//            var a = new[] {new TestIgnore(), new TestIgnore()};
//            var b = CloneEx.DeepClone(a);
//            Assert.IsNull(b);
//        }

        
//        [Test]
//        public void WorksForArrayMixed()
//        {
//            var a = new object[] {new TestNormal(2, null, null, null), new TestCopy(), new TestIgnore()};
//            var b = CloneEx.DeepClone(a);
//            Assert.AreEqual(3, b.Length);
//            Assert.IsFalse(ReferenceEquals(a[0], b[0]));
//            Assert.AreEqual(2, ((TestNormal)b[0]).Prop);
//            Assert.IsTrue(ReferenceEquals(a[1], b[1]));
//            Assert.IsNull(b[2]);
//        }


//        private class TestNormal
//        {
//            private int _int;
//            private readonly int _readonlyInt;
            
//            public TestNormal Test { get; set; }
//            public TestNormal ReadonlyTest { get; }
            
//            public TestCopy Copy { get; }
//            [CustomReplicationBehavior(ReplicationBehavior.Copy)]
//            public TestNormal CopyProp { get; }

//            public TestIgnore Ignore { get; }
//            [CustomReplicationBehavior(ReplicationBehavior.Ignore)]
//            public TestNormal IgnoreProp { get; }
//            [CustomReplicationBehavior(ReplicationBehavior.Ignore)]
//            public TestCopy IgnoreCopy { get; }

//            [SuppressMessage("ReSharper", "UnusedMember.Local")]
//            public int Prop
//            {
//                get
//                {
//                    Assert.AreEqual(_int, _readonlyInt);
//                    return _readonlyInt;
//                }
//                set
//                {
//                    _int = value;
//                    Assert.Fail("Shouldn't be here");
//                }
//            }

//            public TestNormal(int i, TestNormal test, TestCopy copy, TestIgnore ignore)
//            {
//                _int = i;
//                _readonlyInt = i;
                
//                Test = test;
//                ReadonlyTest = test;
                
//                Copy = copy;
//                CopyProp = test;
                
//                Ignore = ignore;
//                IgnoreProp = test;
//                IgnoreCopy = copy;
//            }
//        }

//        [CustomReplicationBehavior(ReplicationBehavior.Copy)]
//        private class TestCopy
//        {

//        }

//        [CustomReplicationBehavior(ReplicationBehavior.Ignore)]
//        private class TestIgnore
//        {

//        }
//    }
//}