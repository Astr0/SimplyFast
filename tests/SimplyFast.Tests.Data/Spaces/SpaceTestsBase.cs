using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using SF.Data.Spaces;

namespace SF.Tests.Data.Spaces
{
    public abstract class SpaceTestsBase
    {
        public abstract ISyncSpace SyncSpace { get; }
        public abstract ISyncSpaceTable<TestTuple> SyncSpaceTable { get; }

        [Test]
        public void CanWrite()
        {
            var tuple = new TestTuple(1, 0);
            SyncSpaceTable.Add(tuple);
            var result = SyncSpaceTable.TryRead(new TestTupleQuery(1, null));
            Assert.AreEqual(tuple, result);
        }

        [Test]
        public void CanTryRead()
        {
            var tuple = new TestTuple(1, 0);
            var tuple2 = new TestTuple(2, 0);
            SyncSpaceTable.Add(tuple);
            SyncSpaceTable.Add(tuple2);
            var result = SyncSpaceTable.TryRead(new TestTupleQuery(1, null));
            Assert.AreEqual(tuple, result);
            result = SyncSpaceTable.TryRead(new TestTupleQuery(1, 0));
            Assert.AreEqual(tuple, result);
            result = SyncSpaceTable.TryRead(new TestTupleQuery(2, null));
            Assert.AreEqual(tuple2, result);
            result = SyncSpaceTable.TryRead(new TestTupleQuery(3, null));
            Assert.IsNull(result);
        }

        [Test]
        public void CanScan()
        {
            var tuple10 = new TestTuple(1, 0);
            var tuple11 = new TestTuple(1, 1);
            var tuple20 = new TestTuple(2, 0);
            SyncSpaceTable.Add(tuple10);
            SyncSpaceTable.Add(tuple11);
            SyncSpaceTable.Add(tuple20);
            var result = SyncSpaceTable.Scan(new TestTupleQuery(1, null));
            Assert.IsTrue(result.OrderBy(x => x.Y).SequenceEqual(new[] {tuple10, tuple11}));
            result = SyncSpaceTable.Scan(new TestTupleQuery(2, null));
            Assert.IsTrue(result.SequenceEqual(new[] { tuple20 }));
        }

        [Test]
        public void CanTryTake()
        {
            var tuple = new TestTuple(1, 0);
            var tuple2 = new TestTuple(2, 0);
            SyncSpaceTable.Add(tuple);
            SyncSpaceTable.Add(tuple2);
            var result = SyncSpaceTable.TryTake(new TestTupleQuery(1, null));
            Assert.AreEqual(tuple, result);
            result = SyncSpaceTable.TryTake(new TestTupleQuery(1, null));
            Assert.IsNull(result);
            result = SyncSpaceTable.TryRead(new TestTupleQuery(1, null));
            Assert.IsNull(result);
            result = SyncSpaceTable.TryTake(new TestTupleQuery(2, null));
            Assert.AreEqual(tuple2, result);
        }

        [Test]
        public void CanCount()
        {
            var tuple10 = new TestTuple(1, 0);
            var tuple11 = new TestTuple(1, 1);
            var tuple20 = new TestTuple(2, 0);
            SyncSpaceTable.AddRange(new []{tuple10, tuple11, tuple20});
            Assert.AreEqual(0, SyncSpaceTable.Count(new TestTupleQuery(2, 1)));
            Assert.AreEqual(1, SyncSpaceTable.Count(new TestTupleQuery(2, null)));
            Assert.AreEqual(2, SyncSpaceTable.Count(new TestTupleQuery(1, null)));
            Assert.AreEqual(3, SyncSpaceTable.Count(new TestTupleQuery(null, null)));
            var obj = SyncSpaceTable.TryTake(new TestTupleQuery(1, null));
            Assert.IsNotNull(obj);
            Assert.AreEqual(0, SyncSpaceTable.Count(new TestTupleQuery(2, 1)));
            Assert.AreEqual(1, SyncSpaceTable.Count(new TestTupleQuery(2, null)));
            Assert.AreEqual(1, SyncSpaceTable.Count(new TestTupleQuery(1, null)));
            Assert.AreEqual(2, SyncSpaceTable.Count(new TestTupleQuery(null, null)));
        }

        [Test]
        public void CanRead()
        {
            var tuple10 = new TestTuple(1, 0);
            var tuple2 = new TestTuple(2, 0);
            SyncSpaceTable.AddRange(new []{tuple10, tuple2});
            TestTuple result = null;
            // instant read
            SyncSpaceTable.Read(new TestTupleQuery(1, null), r => result = r, TimeSpan.FromHours(1));
            Assert.AreEqual(tuple10, result);
            result = null;
            SyncSpaceTable.Read(new TestTupleQuery(1, null), r => result = r, TimeSpan.Zero);
            Assert.AreEqual(tuple10, result);

            result = SyncSpaceTable.TryTake(new TestTupleQuery(1, null));
            Assert.AreEqual(tuple10, result);
            result = null;
            SyncSpaceTable.Read(new TestTupleQuery(1, null), r => result = r, TimeSpan.FromHours(1));
            Assert.IsNull(result);
            SyncSpaceTable.Add(tuple10);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.AreEqual(tuple10, result);
            var tuple11 = new TestTuple(1, 1);
            SyncSpaceTable.Add(tuple11);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.AreEqual(tuple10, result);


            Assert.AreEqual(tuple11, SyncSpaceTable.TryTake(new TestTupleQuery(1, 1)));
            Assert.AreEqual(tuple10, SyncSpaceTable.TryTake(new TestTupleQuery(1, 0)));

            var other = new TestTuple(0, 0);
            result = other;
            SyncSpaceTable.Read(new TestTupleQuery(1, null), r => result = r, TimeSpan.Zero);
            Assert.AreEqual(other, result);
            // wait some time so timeout would be met
            Thread.Sleep(1);
            SyncSpaceTable.Add(tuple10);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.IsNull(result);
        }

        [Test]
        public void CanTake()
        {
            var tuple10 = new TestTuple(1, 0);
            var tuple2 = new TestTuple(2, 0);
            SyncSpaceTable.AddRange(new[] { tuple10, tuple2 });
            TestTuple result = null;

            // instant take
            SyncSpaceTable.Take(new TestTupleQuery(1, null), r => result = r, TimeSpan.FromHours(1));
            Assert.AreEqual(tuple10, result);

            // wait take
            result = null;
            SyncSpaceTable.Take(new TestTupleQuery(1, null), r => result = r, TimeSpan.FromHours(1));
            Assert.IsNull(result);
            SyncSpaceTable.Add(tuple10);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.AreEqual(tuple10, result);
            // wait take only once
            var tuple11 = new TestTuple(1, 1);
            SyncSpaceTable.Add(tuple11);


            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.AreEqual(tuple10, result);
            SyncSpaceTable.Take(new TestTupleQuery(1, null), r => result = r, TimeSpan.FromHours(1));
            Assert.AreEqual(tuple11, result);

            var other = new TestTuple(0, 0);
            result = other;
            SyncSpaceTable.Take(new TestTupleQuery(1, null), r => result = r, TimeSpan.Zero);
            Assert.AreEqual(other, result);
            // wait some time so timeout would be met
            Thread.Sleep(1);
            SyncSpaceTable.Add(tuple10);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.IsNull(result);

            // check that tuple was not taken
            Assert.AreEqual(tuple10, SyncSpaceTable.TryRead(new TestTupleQuery(1, null)));
        }

        private void CanAddInTransactions(bool commit)
        {
            var tuple10 = new TestTuple(1, 0);
            var tuple20 = new TestTuple(2, 0);

            using (var trans = SyncSpace.BeginTransaction())
            {
                // add tuple in transaction
                SyncSpaceTable.Add(tuple10, trans);
                // space should not see it
                Assert.IsNull(SyncSpaceTable.TryRead(new TestTupleQuery(1, null)));
                // trans should see it
                Assert.AreEqual(tuple10, SyncSpaceTable.TryRead(new TestTupleQuery(1, null), trans));
                using (var nested = trans.BeginTransaction())
                {
                    // nested should see it
                    Assert.AreEqual(tuple10, SyncSpaceTable.TryRead(new TestTupleQuery(1, null), nested));

                    // add tuple2 in nested transaction
                    SyncSpaceTable.Add(tuple20, nested);

                    // space should not see it
                    Assert.IsNull(SyncSpaceTable.TryRead(new TestTupleQuery(2, null)));
                    // trans should not see it yet
                    Assert.IsNull(SyncSpaceTable.TryRead(new TestTupleQuery(2, null), trans));
                    // nested should see it
                    Assert.AreEqual(tuple20, SyncSpaceTable.TryRead(new TestTupleQuery(2, null), nested));

                    nested.Commit();
                }
                // trans should see it now
                Assert.AreEqual(tuple20, SyncSpaceTable.TryRead(new TestTupleQuery(2, null), trans));
                // space still should not see it
                Assert.IsNull(SyncSpaceTable.TryRead(new TestTupleQuery(2, null)));

                if (commit)
                {
                    trans.Commit();
                    // now space should see both
                    Assert.AreEqual(tuple10, SyncSpaceTable.TryRead(new TestTupleQuery(1, null)));
                    Assert.AreEqual(tuple20, SyncSpaceTable.TryRead(new TestTupleQuery(2, null)));

                }
                else
                {
                    trans.Abort();
                    // space should see nothing
                    Assert.IsNull(SyncSpaceTable.TryRead(new TestTupleQuery(1, null)));
                    Assert.IsNull(SyncSpaceTable.TryRead(new TestTupleQuery(2, null)));
                }

            }
        }

        [Test]
        public void CanAddInTransactions()
        {
            CanAddInTransactions(false);
            CanAddInTransactions(true);
        }

    }
}