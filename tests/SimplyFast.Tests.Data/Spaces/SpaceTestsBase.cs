using System.Linq;
using NUnit.Framework;
using SF.Data.Spaces;

namespace SF.Tests.Data.Spaces
{
    public abstract class SpaceTestsBase
    {
        public abstract ISpace Space { get; }
        public abstract ISpaceProxy SpaceProxy { get; }

        private static readonly TupleType _testTupleType = new TupleType(0);

        [Test]
        public void CanWrite()
        {
            var tuple = new TestTuple(1, 0);
            SpaceProxy.Add(_testTupleType, tuple);
            var result = SpaceProxy.TryRead(new TestTupleQuery(1, null));
            Assert.AreEqual(tuple, result);
        }

        [Test]
        public void CanTryRead()
        {
            var tuple = new TestTuple(1, 0);
            var tuple2 = new TestTuple(2, 0);
            SpaceProxy.Add(_testTupleType, tuple);
            SpaceProxy.Add(_testTupleType, tuple2);
            var result = SpaceProxy.TryRead(new TestTupleQuery(1, null));
            Assert.AreEqual(tuple, result);
            result = SpaceProxy.TryRead(new TestTupleQuery(1, 0));
            Assert.AreEqual(tuple, result);
            result = SpaceProxy.TryRead(new TestTupleQuery(2, null));
            Assert.AreEqual(tuple2, result);
            result = SpaceProxy.TryRead(new TestTupleQuery(3, null));
            Assert.IsNull(result);
        }

        [Test]
        public void CanScan()
        {
            var tuple10 = new TestTuple(1, 0);
            var tuple11 = new TestTuple(1, 1);
            var tuple20 = new TestTuple(2, 0);
            SpaceProxy.Add(_testTupleType, tuple10);
            SpaceProxy.Add(_testTupleType, tuple11);
            SpaceProxy.Add(_testTupleType, tuple20);
            var result = SpaceProxy.Scan(new TestTupleQuery(1, null));
            Assert.IsTrue(result.OrderBy(x => x.Y).SequenceEqual(new[] {tuple10, tuple11}));
            result = SpaceProxy.Scan(new TestTupleQuery(2, null));
            Assert.IsTrue(result.SequenceEqual(new[] { tuple20 }));
        }

        [Test]
        public void CanTryTake()
        {
            var tuple = new TestTuple(1, 0);
            var tuple2 = new TestTuple(2, 0);
            SpaceProxy.Add(_testTupleType, tuple);
            SpaceProxy.Add(_testTupleType, tuple2);
            var result = SpaceProxy.TryTake(new TestTupleQuery(1, null));
            Assert.AreEqual(tuple, result);
            result = SpaceProxy.TryTake(new TestTupleQuery(1, null));
            Assert.IsNull(result);
            result = SpaceProxy.TryRead(new TestTupleQuery(1, null));
            Assert.IsNull(result);
            result = SpaceProxy.TryTake(new TestTupleQuery(2, null));
            Assert.AreEqual(tuple2, result);
        }

        [Test]
        public void CanCount()
        {
            var tuple10 = new TestTuple(1, 0);
            var tuple11 = new TestTuple(1, 1);
            var tuple20 = new TestTuple(2, 0);
            SpaceProxy.AddRange(_testTupleType, new[]{tuple10, tuple11, tuple20});
            Assert.AreEqual(0, SpaceProxy.Count(new TestTupleQuery(2, 1)));
            Assert.AreEqual(1, SpaceProxy.Count(new TestTupleQuery(2, null)));
            Assert.AreEqual(2, SpaceProxy.Count(new TestTupleQuery(1, null)));
            Assert.AreEqual(3, SpaceProxy.Count(new TestTupleQuery(null, null)));
            var obj = SpaceProxy.TryTake(new TestTupleQuery(1, null));
            Assert.IsNotNull(obj);
            Assert.AreEqual(0, SpaceProxy.Count(new TestTupleQuery(2, 1)));
            Assert.AreEqual(1, SpaceProxy.Count(new TestTupleQuery(2, null)));
            Assert.AreEqual(1, SpaceProxy.Count(new TestTupleQuery(1, null)));
            Assert.AreEqual(2, SpaceProxy.Count(new TestTupleQuery(null, null)));
        }

        [Test]
        public void CanRead()
        {
            var tuple10 = new TestTuple(1, 0);
            var tuple2 = new TestTuple(2, 0);
            SpaceProxy.AddRange(_testTupleType, new[]{tuple10, tuple2});
            TestTuple result = null;
            // instant read
            SpaceProxy.Read(new TestTupleQuery(1, null), r => result = r);
            Assert.AreEqual(tuple10, result);
            result = null;
            SpaceProxy.Read(new TestTupleQuery(1, null), r => result = r);
            Assert.AreEqual(tuple10, result);

            result = SpaceProxy.TryTake(new TestTupleQuery(1, null));
            Assert.AreEqual(tuple10, result);
            result = null;
            SpaceProxy.Read(new TestTupleQuery(1, null), r => result = r);
            Assert.IsNull(result);
            SpaceProxy.Add(_testTupleType, tuple10);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.AreEqual(tuple10, result);
            var tuple11 = new TestTuple(1, 1);
            SpaceProxy.Add(_testTupleType, tuple11);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.AreEqual(tuple10, result);


            Assert.AreEqual(tuple11, SpaceProxy.TryTake(new TestTupleQuery(1, 1)));
            Assert.AreEqual(tuple10, SpaceProxy.TryTake(new TestTupleQuery(1, 0)));

            var other = new TestTuple(0, 0);
            result = other;
            var cancel = SpaceProxy.Read(new TestTupleQuery(1, null), r => result = r);
            Assert.AreEqual(other, result);
            // cancel read
            cancel.Dispose();
            SpaceProxy.Add(_testTupleType, tuple10);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.IsNull(result);
        }

        [Test]
        public void CanTake()
        {
            var tuple10 = new TestTuple(1, 0);
            var tuple2 = new TestTuple(2, 0);
            SpaceProxy.AddRange(_testTupleType, new[] { tuple10, tuple2 });
            TestTuple result = null;

            // instant take
            SpaceProxy.Take(new TestTupleQuery(1, null), r => result = r);
            Assert.AreEqual(tuple10, result);

            // wait take
            result = null;
            SpaceProxy.Take(new TestTupleQuery(1, null), r => result = r);
            Assert.IsNull(result);
            SpaceProxy.Add(_testTupleType, tuple10);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.AreEqual(tuple10, result);
            // wait take only once
            var tuple11 = new TestTuple(1, 1);
            SpaceProxy.Add(_testTupleType, tuple11);


            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.AreEqual(tuple10, result);
            SpaceProxy.Take(new TestTupleQuery(1, null), r => result = r);
            Assert.AreEqual(tuple11, result);

            var other = new TestTuple(0, 0);
            result = other;
            var cancel = SpaceProxy.Take(new TestTupleQuery(1, null), r => result = r);
            Assert.AreEqual(other, result);
            // cancel take
            cancel.Dispose();
            SpaceProxy.Add(_testTupleType, tuple10);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.IsNull(result);

            // check that tuple was not taken
            Assert.AreEqual(tuple10, SpaceProxy.TryRead(new TestTupleQuery(1, null)));
        }

        private void CanAddInTransactionsAndNested(bool commit)
        {
            var tuple10 = new TestTuple(1, 0);
            var tuple20 = new TestTuple(2, 0);
            var cleanProxy = Space.CreateProxy();

            SpaceProxy.BeginTransaction();
            {
                // add tuple in transaction
                SpaceProxy.Add(_testTupleType, tuple10);
                // space should not see it
                Assert.IsNull(cleanProxy.TryRead(new TestTupleQuery(1, null)));
                // trans should see it
                Assert.AreEqual(tuple10, SpaceProxy.TryRead(new TestTupleQuery(1, null)));
                SpaceProxy.BeginTransaction();
                {
                    // nested should see it
                    Assert.AreEqual(tuple10, SpaceProxy.TryRead(new TestTupleQuery(1, null)));

                    // add tuple2 in nested transaction
                    SpaceProxy.Add(_testTupleType, tuple20);

                    // space should not see it
                    Assert.IsNull(cleanProxy.TryRead(new TestTupleQuery(2, null)));
                    // nested should see it
                    Assert.AreEqual(tuple20, SpaceProxy.TryRead(new TestTupleQuery(2, null)));

                    SpaceProxy.CommitTransaction();
                }
                // trans should see it now
                Assert.AreEqual(tuple20, SpaceProxy.TryRead(new TestTupleQuery(2, null)));
                // space still should not see it
                Assert.IsNull(cleanProxy.TryRead(new TestTupleQuery(2, null)));

                if (commit)
                {
                    SpaceProxy.CommitTransaction();
                    // now space should see both
                    Assert.AreEqual(tuple10, SpaceProxy.TryRead(new TestTupleQuery(1, null)));
                    Assert.AreEqual(tuple20, SpaceProxy.TryRead(new TestTupleQuery(2, null)));
                    Assert.AreEqual(tuple10, cleanProxy.TryRead(new TestTupleQuery(1, null)));
                    Assert.AreEqual(tuple20, cleanProxy.TryRead(new TestTupleQuery(2, null)));

                }
                else
                {
                    SpaceProxy.RollbackTransaction();
                    // space should see nothing
                    Assert.IsNull(SpaceProxy.TryRead(new TestTupleQuery(1, null)));
                    Assert.IsNull(SpaceProxy.TryRead(new TestTupleQuery(2, null)));
                    Assert.IsNull(cleanProxy.TryRead(new TestTupleQuery(1, null)));
                    Assert.IsNull(cleanProxy.TryRead(new TestTupleQuery(2, null)));
                }

            }
        }

        [Test]
        public void CanAddInTransactionsAndNested()
        {
            CanAddInTransactionsAndNested(false);
            CanAddInTransactionsAndNested(true);
        }


        private void CanAddInNestedTransactions(bool commitNested, bool commitTrans)
        {
            var tuple10 = new TestTuple(1, 0);
            var tuple20 = new TestTuple(2, 0);
            var tuple10q = new TestTupleQuery(1, null);
            var tuple20q = new TestTupleQuery(2, null);
            var cleanProxy = Space.CreateProxy();

            SpaceProxy.BeginTransaction();
            {
                TestTuple trans10result = null;
                SpaceProxy.Read(tuple10q, x => trans10result = x);

                TestTuple trans20result = null;
                SpaceProxy.Read(tuple20q, x => trans20result = x);
                SpaceProxy.BeginTransaction();
                {
                    SpaceProxy.Add(_testTupleType, tuple10);
                    // space should not see it
                    Assert.IsNull(cleanProxy.TryRead(tuple10q));
                    // trans should not see it
                    Assert.IsNull(trans10result);
                    // nested should see it
                    Assert.AreEqual(tuple10, SpaceProxy.TryRead(tuple10q));

                    TestTuple nestedResult = null;
                    SpaceProxy.Read(tuple20q, x => nestedResult = x);
                    SpaceProxy.BeginTransaction();
                    {
                        // nested2 should see it
                        Assert.AreEqual(tuple10, SpaceProxy.TryRead(tuple10q));

                        // add in nested 2
                        SpaceProxy.Add(_testTupleType, tuple20);

                        // space should not see it
                        Assert.IsNull(cleanProxy.TryRead(tuple20q));
                        // trans should not see it
                        Assert.IsNull(trans20result);
                        // nested should not see it
                        Assert.IsNull(nestedResult);
                        // nested2 should see it
                        Assert.AreEqual(tuple20, SpaceProxy.TryRead(tuple20q));

                        SpaceProxy.CommitTransaction();
                    }
                    // space should not see it
                    Assert.IsNull(cleanProxy.TryRead(tuple20q));
                    // trans should not see it
                    Assert.IsNull(trans20result);
                    // nested should see it
                    Assert.AreEqual(tuple20, nestedResult);
                    if (commitNested)
                    {
                        SpaceProxy.CommitTransaction();
                    }
                    else
                    {
                        SpaceProxy.RollbackTransaction();
                    }
                }
                // space should not see both
                Assert.IsNull(cleanProxy.TryRead(tuple10q));
                Assert.IsNull(cleanProxy.TryRead(tuple20q));
                if (commitNested)
                {
                    // trans should see both
                    Assert.AreEqual(tuple10, trans10result);
                    Assert.AreEqual(tuple20, trans20result);
                }
                else
                {
                    // trans should not see both
                    Assert.IsNull(trans10result);
                    Assert.IsNull(trans20result);
                }

                if (commitTrans)
                {
                    SpaceProxy.CommitTransaction();
                }
                else
                {
                    SpaceProxy.RollbackTransaction();
                }

                if (commitTrans && commitNested)
                {
                    // now space should see both
                    Assert.AreEqual(tuple10, cleanProxy.TryRead(tuple10q));
                    Assert.AreEqual(tuple20, cleanProxy.TryRead(tuple20q));
                }
                else
                {
                    // space still should not see both
                    Assert.IsNull(cleanProxy.TryRead(tuple10q));
                    Assert.IsNull(cleanProxy.TryRead(tuple20q));
                }
            }
        }

        [Test]
        public void CanAddInNestedTransactions()
        {
            CanAddInNestedTransactions(false, false);
            CanAddInNestedTransactions(false, true);
            CanAddInNestedTransactions(true, false);
            CanAddInNestedTransactions(true, true);
        }

        

        [Test]
        public void DisposeAbortsTransaction()
        {
            var tuple10 = new TestTuple(1, 0);
            var tuple20 = new TestTuple(2, 0);
            var tuple10q = new TestTupleQuery(1, null);
            var tuple20q = new TestTupleQuery(2, null);
            using (var proxy = Space.CreateProxy())
            {
                proxy.BeginTransaction();
                proxy.Add(_testTupleType, tuple10);
                // verify add
                Assert.AreEqual(tuple10, proxy.TryRead(tuple10q));

                proxy.BeginTransaction();
                proxy.Add(_testTupleType, tuple20);
                // verify add
                Assert.AreEqual(tuple10, proxy.TryRead(tuple10q));
                Assert.AreEqual(tuple20, proxy.TryRead(tuple20q));
            }
            // nothing in the space
            Assert.IsNull(SpaceProxy.TryRead(tuple10q));
            Assert.IsNull(SpaceProxy.TryRead(tuple20q));
        }
    }
}