using System.Collections.Generic;
using SF.Data.Spaces;

namespace SimplyFast.Research
{
    public class LocalSpaceWriteTests: PerfTestBase
    {
        private ISyncSpace _space;
        private ISyncSpaceTable<TestTuple> _table;
        private List<TestTuple> _list; 

        private class TestTuple
        {
            public TestTuple(int x, int y)
            {
                X = x;
                Y = y;
            }

            public readonly int X;
            public readonly int Y;
        }

        public LocalSpaceWriteTests()
        {
            ResetSpace();
        }

        private int _counter;
        private void ResetSpace()
        {
            _counter = 0;
            _list = new List<TestTuple>();
            _space = SpaceFactory.UnsafeLocal();
            _table = _space.GetTable<TestTuple>(0);
        }

        private void AddTupleNoTransaction()
        {
            _table.Add(new TestTuple(_counter, _counter));
            _counter++;
        }

        private void AddTupleToList()
        {
            _list.Add(new TestTuple(_counter, _counter));
            _counter++;
        }

        private void AddTupleInTransaction()
        {
            using (var trans = _space.BeginTransaction())
            {
                _table.Add(new TestTuple(_counter, _counter), trans);
                _counter++;
                trans.Commit();
            }
        }

        private void AddTupleInTransactionAbort()
        {
            using (var trans = _space.BeginTransaction())
            {
                _table.Add(new TestTuple(_counter, _counter), trans);
                _counter++;
                trans.Abort();
            }
        }

        private void NothingInTransactionAbort()
        {
            using (var trans = _space.BeginTransaction())
            {
                _counter++;
                trans.Abort();
            }
        }

        private ISyncTransaction _globalTransaction;
        private void AddTupleInGlobalTransaction()
        {
            _table.Add(new TestTuple(_counter, _counter), _globalTransaction);
            _counter++;
        }


        protected override void DoRun()
        {
            TestPerformance(AddTupleInTransaction, Iterations, "Add tuples in transaction.", false);
            //JitPrepare();

            //ResetSpace();
            //TestPerformance(AddTupleToList, Iterations, "Add tuples to list.", false);
            ///*ResetSpace();
            //TestPerformance(AddTupleNoTransaction, Iterations, "Add tuples, no transaction.", false);*/
            //ResetSpace();
            //TestPerformance(NothingInTransactionAbort, Iterations, "Just do transactions.", false);
            //ResetSpace();
            //TestPerformance(AddTupleInTransaction, Iterations, "Add tuples in transaction.", false);
            //ResetSpace();
            //TestPerformance(AddTupleInTransactionAbort, Iterations, "Add tuples in transaction abort.", false);
            //ResetSpace();
            //using (_globalTransaction = _space.BeginTransaction())
            //{
            //    TestPerformance(AddTupleInGlobalTransaction, Iterations, () => _globalTransaction.Commit(), "Add tuples in global transaction and commit.", false);
            //}
            //ResetSpace();
            //using (_globalTransaction = _space.BeginTransaction())
            //{
            //    TestPerformance(AddTupleInGlobalTransaction, Iterations, () => _globalTransaction.Abort(), "Add tuples in global transaction and abort.", false);
            //}
        }

        private void JitPrepare()
        {
            ResetSpace();
            AddTupleToList();
            AddTupleNoTransaction();
            AddTupleInTransaction();
            AddTupleInTransactionAbort();
            NothingInTransactionAbort();
            using (_globalTransaction = _space.BeginTransaction())
            {
                AddTupleInGlobalTransaction();
            }
        }
    }
}