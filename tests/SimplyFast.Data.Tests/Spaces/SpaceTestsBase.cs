using System.Linq;
using NUnit.Framework;
using SimplyFast.Data.Spaces.Interface;

namespace SimplyFast.Data.Tests.Spaces
{
    public abstract class SpaceTestsBase
    {
        public abstract ISpace Space { get; }
        public abstract ISpaceProxy SpaceProxy { get; }

        private static readonly TupleType _testTupleType = new TupleType(0);

        private static readonly TestTuple _tuple00 = new TestTuple(0, 0);
        private static readonly TestTuple _tuple10 = new TestTuple(1, 0);
        private static readonly TestTuple _tuple11 = new TestTuple(1, 1);
        private static readonly TestTuple _tuple20 = new TestTuple(2, 0);


        private static readonly TestTupleQuery _tuple1Q = new TestTupleQuery(1, null);
        private static readonly TestTupleQuery _tuple2Q = new TestTupleQuery(2, null);
        private static readonly TestTupleQuery _tuplesq = new TestTupleQuery(null, null);

        [Test]
        public void CanWrite()
        {
            SpaceProxy.Add(_testTupleType, _tuple10);
            var result = SpaceProxy.TryRead(_tuple1Q);
            Assert.AreEqual(_tuple10, result);
        }

        [Test]
        public void CanTryRead()
        {
            SpaceProxy.Add(_testTupleType, _tuple10);
            SpaceProxy.Add(_testTupleType, _tuple20);
            var result = SpaceProxy.TryRead(_tuple1Q);
            Assert.AreEqual(_tuple10, result);
            result = SpaceProxy.TryRead(new TestTupleQuery(1, 0));
            Assert.AreEqual(_tuple10, result);
            result = SpaceProxy.TryRead(_tuple2Q);
            Assert.AreEqual(_tuple20, result);
            result = SpaceProxy.TryRead(new TestTupleQuery(3, null));
            Assert.IsNull(result);
        }

        [Test]
        public void CanScan()
        {
            SpaceProxy.Add(_testTupleType, _tuple10);
            SpaceProxy.Add(_testTupleType, _tuple11);
            SpaceProxy.Add(_testTupleType, _tuple20);
            var result = SpaceProxy.Scan(_tuple1Q);
            Assert.IsTrue(result.OrderBy(x => x.Y).SequenceEqual(new[] { _tuple10, _tuple11 }));
            result = SpaceProxy.Scan(_tuple2Q);
            Assert.IsTrue(result.SequenceEqual(new[] { _tuple20 }));
        }

        [Test]
        public void CanTryTake()
        {
            SpaceProxy.Add(_testTupleType, _tuple10);
            SpaceProxy.Add(_testTupleType, _tuple20);
            var result = SpaceProxy.TryTake(_tuple1Q);
            Assert.AreEqual(_tuple10, result);
            result = SpaceProxy.TryTake(_tuple1Q);
            Assert.IsNull(result);
            result = SpaceProxy.TryRead(_tuple1Q);
            Assert.IsNull(result);
            result = SpaceProxy.TryTake(_tuple2Q);
            Assert.AreEqual(_tuple20, result);
        }

        [Test]
        public void CanCount()
        {
            SpaceProxy.AddRange(_testTupleType, new[] { _tuple10, _tuple11, _tuple20 });
            Assert.AreEqual(0, SpaceProxy.Count(new TestTupleQuery(2, 1)));
            Assert.AreEqual(1, SpaceProxy.Count(_tuple2Q));
            Assert.AreEqual(2, SpaceProxy.Count(_tuple1Q));
            Assert.AreEqual(3, SpaceProxy.Count(_tuplesq));
            var obj = SpaceProxy.TryTake(_tuple1Q);
            Assert.IsNotNull(obj);
            Assert.AreEqual(0, SpaceProxy.Count(new TestTupleQuery(2, 1)));
            Assert.AreEqual(1, SpaceProxy.Count(_tuple2Q));
            Assert.AreEqual(1, SpaceProxy.Count(_tuple1Q));
            Assert.AreEqual(2, SpaceProxy.Count(_tuplesq));
        }

        [Test]
        public void CanRead()
        {
            SpaceProxy.AddRange(_testTupleType, new[] { _tuple10, _tuple20 });
            TestTuple result = null;
            // instant read
            SpaceProxy.Read(_tuple1Q, r => result = r);
            Assert.AreEqual(_tuple10, result);
            result = null;
            SpaceProxy.Read(_tuple1Q, r => result = r);
            Assert.AreEqual(_tuple10, result);

            result = SpaceProxy.TryTake(_tuple1Q);
            Assert.AreEqual(_tuple10, result);
            result = null;
            SpaceProxy.Read(_tuple1Q, r => result = r);
            Assert.IsNull(result);
            SpaceProxy.Add(_testTupleType, _tuple10);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.AreEqual(_tuple10, result);
            SpaceProxy.Add(_testTupleType, _tuple11);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.AreEqual(_tuple10, result);


            Assert.AreEqual(_tuple11, SpaceProxy.TryTake(new TestTupleQuery(1, 1)));
            Assert.AreEqual(_tuple10, SpaceProxy.TryTake(new TestTupleQuery(1, 0)));

            result = _tuple00;
            var cancel = SpaceProxy.Read(new TestTupleQuery(1, null), r => result = r);
            Assert.AreEqual(_tuple00, result);
            // cancel read
            cancel.Dispose();
            Assert.IsNull(result);

            SpaceProxy.Add(_testTupleType, _tuple10);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.IsNull(result);
        }

        [Test]
        public void CanTake()
        {
            SpaceProxy.AddRange(_testTupleType, new[] { _tuple10, _tuple20 });
            TestTuple result = null;

            // instant take
            SpaceProxy.Take(_tuple1Q, r => result = r);
            Assert.AreEqual(_tuple10, result);

            // wait take
            result = null;
            SpaceProxy.Take(_tuple1Q, r => result = r);
            Assert.IsNull(result);
            SpaceProxy.Add(_testTupleType, _tuple10);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.AreEqual(_tuple10, result);
            // wait take only once
            SpaceProxy.Add(_testTupleType, _tuple11);


            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.AreEqual(_tuple10, result);
            SpaceProxy.Take(_tuple1Q, r => result = r);
            Assert.AreEqual(_tuple11, result);

            result = _tuple00;
            var cancel = SpaceProxy.Take(_tuple1Q, r => result = r);
            Assert.AreEqual(_tuple00, result);
            // cancel take
            cancel.Dispose();
            Assert.IsNull(result);

            SpaceProxy.Add(_testTupleType, _tuple10);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.IsNull(result);

            // check that tuple was not taken
            Assert.AreEqual(_tuple10, SpaceProxy.TryRead(_tuple1Q));
        }

        private void CanAddInTransactionsAndNested(bool commit)
        {
            var cleanProxy = Space.CreateProxy();

            SpaceProxy.BeginTransaction();
            {
                // add tuple in transaction
                SpaceProxy.Add(_testTupleType, _tuple10);
                // space should not see it
                Assert.IsNull(cleanProxy.TryRead(_tuple1Q));
                // trans should see it
                Assert.AreEqual(_tuple10, SpaceProxy.TryRead(_tuple1Q));
                SpaceProxy.BeginTransaction();
                {
                    // nested should see it
                    Assert.AreEqual(_tuple10, SpaceProxy.TryRead(_tuple1Q));

                    // add tuple2 in nested transaction
                    SpaceProxy.Add(_testTupleType, _tuple20);

                    // space should not see it
                    Assert.IsNull(cleanProxy.TryRead(_tuple2Q));
                    // nested should see it
                    Assert.AreEqual(_tuple20, SpaceProxy.TryRead(_tuple2Q));

                    SpaceProxy.CommitTransaction();
                }
                // trans should see it now
                Assert.AreEqual(_tuple20, SpaceProxy.TryRead(_tuple2Q));
                // space still should not see it
                Assert.IsNull(cleanProxy.TryRead(_tuple2Q));

                if (commit)
                {
                    SpaceProxy.CommitTransaction();
                    // now space should see both
                    Assert.AreEqual(_tuple10, SpaceProxy.TryRead(_tuple1Q));
                    Assert.AreEqual(_tuple20, SpaceProxy.TryRead(_tuple2Q));
                    Assert.AreEqual(_tuple10, cleanProxy.TryRead(_tuple1Q));
                    Assert.AreEqual(_tuple20, cleanProxy.TryRead(_tuple2Q));

                }
                else
                {
                    SpaceProxy.RollbackTransaction();
                    // space should see nothing
                    Assert.IsNull(SpaceProxy.TryRead(_tuple1Q));
                    Assert.IsNull(SpaceProxy.TryRead(_tuple2Q));
                    Assert.IsNull(cleanProxy.TryRead(_tuple1Q));
                    Assert.IsNull(cleanProxy.TryRead(_tuple2Q));
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
            var cleanProxy = Space.CreateProxy();

            SpaceProxy.BeginTransaction();
            {
                TestTuple trans10Result = null;
                SpaceProxy.Read(_tuple1Q, x => trans10Result = x);

                TestTuple trans20Result = null;
                SpaceProxy.Read(_tuple2Q, x => trans20Result = x);
                SpaceProxy.BeginTransaction();
                {
                    SpaceProxy.Add(_testTupleType, _tuple10);
                    // space should not see it
                    Assert.IsNull(cleanProxy.TryRead(_tuple1Q));
                    // trans should not see it
                    Assert.IsNull(trans10Result);
                    // nested should see it
                    Assert.AreEqual(_tuple10, SpaceProxy.TryRead(_tuple1Q));

                    TestTuple nestedResult = null;
                    SpaceProxy.Read(_tuple2Q, x => nestedResult = x);
                    SpaceProxy.BeginTransaction();
                    {
                        // nested2 should see it
                        Assert.AreEqual(_tuple10, SpaceProxy.TryRead(_tuple1Q));

                        // add in nested 2
                        SpaceProxy.Add(_testTupleType, _tuple20);

                        // space should not see it
                        Assert.IsNull(cleanProxy.TryRead(_tuple2Q));
                        // trans should not see it
                        Assert.IsNull(trans20Result);
                        // nested should not see it
                        Assert.IsNull(nestedResult);
                        // nested2 should see it
                        Assert.AreEqual(_tuple20, SpaceProxy.TryRead(_tuple2Q));

                        SpaceProxy.CommitTransaction();
                    }
                    // space should not see it
                    Assert.IsNull(cleanProxy.TryRead(_tuple2Q));
                    // trans should not see it
                    Assert.IsNull(trans20Result);
                    // nested should see it
                    // ReSharper disable once ExpressionIsAlwaysNull
                    Assert.AreEqual(_tuple20, nestedResult);
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
                Assert.IsNull(cleanProxy.TryRead(_tuple1Q));
                Assert.IsNull(cleanProxy.TryRead(_tuple2Q));
                if (commitNested)
                {
                    // trans should see both
                    Assert.AreEqual(_tuple10, trans10Result);
                    Assert.AreEqual(_tuple20, trans20Result);
                }
                else
                {
                    // trans should not see both
                    Assert.IsNull(trans10Result);
                    Assert.IsNull(trans20Result);
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
                    Assert.AreEqual(_tuple10, cleanProxy.TryRead(_tuple1Q));
                    Assert.AreEqual(_tuple20, cleanProxy.TryRead(_tuple2Q));
                }
                else
                {
                    // space still should not see both
                    Assert.IsNull(cleanProxy.TryRead(_tuple1Q));
                    Assert.IsNull(cleanProxy.TryRead(_tuple2Q));
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
            using (var proxy = Space.CreateProxy())
            {
                proxy.BeginTransaction();
                proxy.Add(_testTupleType, _tuple10);
                // verify add
                Assert.AreEqual(_tuple10, proxy.TryRead(_tuple1Q));

                proxy.BeginTransaction();
                proxy.Add(_testTupleType, _tuple20);
                // verify add
                Assert.AreEqual(_tuple10, proxy.TryRead(_tuple1Q));
                Assert.AreEqual(_tuple20, proxy.TryRead(_tuple2Q));
            }
            // nothing in the space
            Assert.IsNull(SpaceProxy.TryRead(_tuple1Q));
            Assert.IsNull(SpaceProxy.TryRead(_tuple2Q));
        }

        [Test]
        public void DisposeAbortsWaitingActions()
        {
            var globalResult = _tuple00;
            var transactionResult = _tuple00;
            using (var proxy = Space.CreateProxy())
            {
                proxy.Read(_tuple1Q, x => globalResult = x);
                proxy.BeginTransaction();

                proxy.Add(_testTupleType, _tuple10);
                Assert.AreEqual(_tuple00, globalResult);
                Assert.AreEqual(_tuple00, transactionResult);

                proxy.Read(_tuple2Q, x => transactionResult = x);
                Assert.AreEqual(_tuple00, globalResult);
                Assert.AreEqual(_tuple00, transactionResult);
            }
            Assert.IsNull(globalResult);
            Assert.IsNull(transactionResult);
            SpaceProxy.Add(_testTupleType, _tuple10);
            SpaceProxy.Add(_testTupleType, _tuple20);
            Assert.IsNull(globalResult);
            Assert.IsNull(transactionResult);
        }

        [Test]
        public void AbortAndCommitAbortsWaitingAction()
        {
            var tresult = _tuple00;

            SpaceProxy.BeginTransaction();
            SpaceProxy.Read(_tuple1Q, x => tresult = x);
            Assert.AreEqual(_tuple00, tresult);
            SpaceProxy.RollbackTransaction();
            Assert.IsNull(tresult);
            // check that wa was deleted
            SpaceProxy.Add(_testTupleType, _tuple10);
            Assert.IsNull(tresult);

            tresult = _tuple00;
            SpaceProxy.BeginTransaction();
            SpaceProxy.Read(_tuple2Q, x => tresult = x);
            Assert.AreEqual(_tuple00, tresult);
            SpaceProxy.RollbackTransaction();
            Assert.IsNull(tresult);
            // check that wa was deleted
            SpaceProxy.Add(_testTupleType, _tuple20);
            Assert.IsNull(tresult);
        }

        [Test]
        public void OtherProxyTriggersGlobalWait()
        {
            var tresult = _tuple00;

            SpaceProxy.Read(_tuple1Q, x => tresult = x);
            Assert.AreEqual(_tuple00, tresult);
            using (var proxy = Space.CreateProxy())
            {
                proxy.Add(_testTupleType, _tuple10);
                Assert.AreEqual(_tuple10, tresult);
                Assert.AreEqual(1, proxy.Count(_tuple1Q));
            }

            SpaceProxy.Take(_tuple2Q, x => tresult = x);
            Assert.AreEqual(_tuple10, tresult);
            using (var proxy = Space.CreateProxy())
            {
                proxy.Add(_testTupleType, _tuple20);
                Assert.AreEqual(_tuple20, tresult);
                Assert.AreEqual(0, proxy.Count(_tuple2Q));
            }
        }

        [Test]
        public void TransactionCommitTriggersWait()
        {
            var gresult = _tuple00;
            var tresult = _tuple00;

            SpaceProxy.Read(_tuple1Q, x => gresult = x);

            SpaceProxy.BeginTransaction();
            SpaceProxy.Read(_tuple1Q, x => tresult = x);

            SpaceProxy.BeginTransaction();
            Assert.AreEqual(_tuple00, gresult);
            Assert.AreEqual(_tuple00, tresult);

            SpaceProxy.Add(_testTupleType, _tuple10);

            // nothing should be triggered for uncommited trans
            Assert.AreEqual(_tuple00, gresult);
            Assert.AreEqual(_tuple00, tresult);
            SpaceProxy.CommitTransaction();
            // now transactional read should be triggered
            Assert.AreEqual(_tuple00, gresult);
            Assert.AreEqual(_tuple10, tresult);
            SpaceProxy.CommitTransaction();
            // now boths reads should be triggered
            Assert.AreEqual(_tuple10, gresult);
            Assert.AreEqual(_tuple10, tresult);
        }

        [Test]
        public void GlobalsTryTakenInTransactionAreFine()
        {
            SpaceProxy.Add(_testTupleType, _tuple10);
            SpaceProxy.Add(_testTupleType, _tuple20);

            SpaceProxy.BeginTransaction();
            Assert.AreEqual(_tuple10, SpaceProxy.TryTake(_tuple1Q));
            SpaceProxy.CommitTransaction();
            Assert.IsNull(SpaceProxy.TryTake(_tuple1Q));

            SpaceProxy.BeginTransaction();
            Assert.AreEqual(_tuple20, SpaceProxy.TryTake(_tuple2Q));
            SpaceProxy.RollbackTransaction();
            Assert.AreEqual(_tuple20, SpaceProxy.TryTake(_tuple2Q));
            Assert.IsNull(SpaceProxy.TryTake(_tuple2Q));
        }

        [Test]
        public void GlobalsTakenInTransactionAreFine()
        {
            using (var proxy = Space.CreateProxy())
            {
                var tresult = _tuple00;

                SpaceProxy.BeginTransaction();

                SpaceProxy.Take(_tuple1Q, x => tresult = x);
                proxy.Add(_testTupleType, _tuple10);

                Assert.AreEqual(_tuple10, tresult);
                Assert.IsNull(SpaceProxy.TryTake(_tuple1Q));
                Assert.IsNull(proxy.TryTake(_tuple1Q));

                SpaceProxy.CommitTransaction();
                Assert.IsNull(SpaceProxy.TryTake(_tuple1Q));
                Assert.IsNull(proxy.TryTake(_tuple1Q));



                SpaceProxy.BeginTransaction();

                SpaceProxy.Take(_tuple2Q, x => tresult = x);
                proxy.Add(_testTupleType, _tuple20);
                Assert.AreEqual(_tuple20, tresult);

                Assert.IsNull(SpaceProxy.TryTake(_tuple2Q));
                Assert.IsNull(proxy.TryTake(_tuple2Q));

                SpaceProxy.RollbackTransaction();

                Assert.AreEqual(_tuple20, SpaceProxy.TryTake(_tuple2Q));
                Assert.IsNull(SpaceProxy.TryTake(_tuple2Q));
            }
        }

        [Test]
        public void GlobalTakesAndReadsTransactionCommits()
        {
            using (var proxy = Space.CreateProxy())
            {
                var tresult = _tuple00;
                var result = _tuple00;
                proxy.Read(_tuple1Q, x => result = x);
                proxy.Take(_tuple2Q, x => tresult = x);

                SpaceProxy.BeginTransaction();
                SpaceProxy.Add(_testTupleType, _tuple10);
                SpaceProxy.Add(_testTupleType, _tuple20);
                Assert.AreEqual(_tuple00, tresult);
                Assert.AreEqual(_tuple00, result);
                SpaceProxy.CommitTransaction();

                Assert.AreEqual(_tuple10, result);
                Assert.AreEqual(_tuple20, tresult);
            }
        }

        [Test]
        public void GlobalTakesAndReadsTransactionCommitsEvenWhileTransWasRunning()
        {
            using (var proxy = Space.CreateProxy())
            {
                var tresult = _tuple00;
                var result = _tuple00;


                SpaceProxy.BeginTransaction();
                SpaceProxy.Add(_testTupleType, _tuple10);
                SpaceProxy.Add(_testTupleType, _tuple20);

                proxy.Read(_tuple1Q, x => result = x);
                proxy.Take(_tuple2Q, x => tresult = x);
                Assert.AreEqual(_tuple00, tresult);
                Assert.AreEqual(_tuple00, result);
                SpaceProxy.CommitTransaction();

                Assert.AreEqual(_tuple10, result);
                Assert.AreEqual(_tuple20, tresult);
            }
        }

        [Test]
        public void AllReadersReadTuple()
        {
            var results = Enumerable.Range(0, 10).Select(x => _tuple00).ToArray();
            for (var i = 0; i < results.Length; i++)
            {
                var i1 = i;
                SpaceProxy.Read(_tuple1Q, x => results[i1] = x);
            }
            Assert.IsTrue(results.All(x => x.Equals(_tuple00)));
            SpaceProxy.Add(_testTupleType, _tuple10);
            Assert.IsTrue(results.All(x => x.Equals(_tuple10)));
        }

        [Test]
        public void TransactionTakesByTransactionAreFine()
        {
            SpaceProxy.BeginTransaction();
            SpaceProxy.Add(_testTupleType, _tuple10);
            SpaceProxy.BeginTransaction();
            Assert.AreEqual(_tuple10, SpaceProxy.TryTake(_tuple1Q));
            Assert.IsNull(SpaceProxy.TryTake(_tuple1Q));
            SpaceProxy.RollbackTransaction();
            Assert.AreEqual(_tuple10, SpaceProxy.TryRead(_tuple1Q));

            SpaceProxy.BeginTransaction();
            Assert.AreEqual(_tuple10, SpaceProxy.TryTake(_tuple1Q));
            Assert.IsNull(SpaceProxy.TryTake(_tuple1Q));
            SpaceProxy.CommitTransaction();
            Assert.IsNull(SpaceProxy.TryRead(_tuple1Q));
            SpaceProxy.CommitTransaction();
            Assert.IsNull(SpaceProxy.TryRead(_tuple1Q));
        }

        [Test]
        public void TransactionTakesByMoreNestedTransactionAreFine()
        {
            SpaceProxy.BeginTransaction();
            SpaceProxy.Add(_testTupleType, _tuple10);

            SpaceProxy.BeginTransaction();

            SpaceProxy.BeginTransaction();

            Assert.AreEqual(_tuple10, SpaceProxy.TryTake(_tuple1Q));
            Assert.IsNull(SpaceProxy.TryTake(_tuple1Q));

            SpaceProxy.RollbackTransaction();

            Assert.AreEqual(_tuple10, SpaceProxy.TryRead(_tuple1Q));

            SpaceProxy.CommitTransaction();

            Assert.AreEqual(_tuple10, SpaceProxy.TryRead(_tuple1Q));

            SpaceProxy.CommitTransaction();

            Assert.AreEqual(_tuple10, SpaceProxy.TryRead(_tuple1Q));
        }


        [Test]
        public void TransTakesAndReadsNestedTransactionCommits()
        {
            var tresult = _tuple00;
            var result = _tuple00;

            SpaceProxy.BeginTransaction();

            SpaceProxy.Read(_tuple1Q, x => result = x);
            SpaceProxy.Take(_tuple2Q, x => tresult = x);

            SpaceProxy.BeginTransaction();

            SpaceProxy.Add(_testTupleType, _tuple10);
            Assert.AreEqual(_tuple00, tresult);
            Assert.AreEqual(_tuple00, result);


            SpaceProxy.BeginTransaction();
            SpaceProxy.Add(_testTupleType, _tuple20);
            Assert.AreEqual(_tuple00, tresult);
            Assert.AreEqual(_tuple00, result);
            SpaceProxy.CommitTransaction();
            Assert.AreEqual(_tuple00, tresult);
            Assert.AreEqual(_tuple00, result);

            SpaceProxy.CommitTransaction();
            Assert.AreEqual(_tuple10, result);
            Assert.AreEqual(_tuple20, tresult);
            SpaceProxy.CommitTransaction();

            Assert.AreEqual(_tuple10, result);
            Assert.AreEqual(_tuple20, tresult);

            Assert.AreEqual(_tuple10, SpaceProxy.TryRead(_tuple1Q));
            Assert.IsNull(SpaceProxy.TryRead(_tuple2Q));
        }

        private void TestScanCountInTransaction(int current, int count)
        {
            if (current >= count)
                return;

            SpaceProxy.BeginTransaction();

            Assert.AreEqual(current, SpaceProxy.Count(_tuple1Q));
            var scan = SpaceProxy.Scan(_tuple1Q);
            Assert.AreEqual(current, scan.Count);
            Assert.IsTrue(scan.All(_tuple10.Equals));

            // after adding, all stuff should be  +1
            SpaceProxy.Add(_testTupleType, _tuple10);
            current++;

            Assert.AreEqual(current, SpaceProxy.Count(_tuple1Q));
            scan = SpaceProxy.Scan(_tuple1Q);
            Assert.AreEqual(current, scan.Count);
            Assert.IsTrue(scan.All(_tuple10.Equals));

            TestScanCountInTransaction(current, count);

            // now we should have count stuff

            Assert.AreEqual(count, SpaceProxy.Count(_tuple1Q));
            scan = SpaceProxy.Scan(_tuple1Q);
            Assert.AreEqual(count, scan.Count);
            Assert.IsTrue(scan.All(_tuple10.Equals));

            SpaceProxy.CommitTransaction();
        }

        [Test]
        public void ScanAndCountTakeTransItems()
        {
            TestScanCountInTransaction(0, 10);
        }
    }
}