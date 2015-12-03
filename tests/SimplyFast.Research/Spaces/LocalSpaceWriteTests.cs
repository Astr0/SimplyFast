using System.Collections.Generic;
using SF.Data.Spaces;

namespace SimplyFast.Research.Spaces
{
    public class LocalSpaceWriteTests: PerfTestBase
    {
        private ISpace _space;
        private ISpaceProxy _spaceProxy;
        private List<TestTuple> _list; 

        private readonly TupleType TestTupleType = new TupleType(0);
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
            _spaceProxy = _space.CreateProxy();
        }

        private void AddTupleNoTransaction()
        {
            _spaceProxy.Add(TestTupleType, new TestTuple(_counter, _counter));
            _counter++;
        }

        private void AddTupleToList()
        {
            _list.Add(new TestTuple(_counter, _counter));
            _counter++;
        }

        private void AddTupleInTransaction()
        {
            _spaceProxy.BeginTransaction();
            _spaceProxy.Add(TestTupleType, new TestTuple(_counter, _counter));
            _counter++;
            _spaceProxy.CommitTransaction();
        }

        private void AddTupleInTransactionAbort()
        {
            _spaceProxy.BeginTransaction();
            _spaceProxy.Add(TestTupleType, new TestTuple(_counter, _counter));
            _counter++;
            _spaceProxy.RollbackTransaction();
        }

        private void NothingInTransactionAbort()
        {
            _spaceProxy.BeginTransaction();
            _counter++;
            _spaceProxy.RollbackTransaction();
        }

        


        protected override void DoRun()
        {
            //TestPerformance(AddTupleInTransaction, Iterations, "Add tuples in transaction.", false);
            JitPrepare();

            ResetSpace();
            TestPerformance(AddTupleToList, Iterations, "Add tuples to list.", false);
            ResetSpace();
            TestPerformance(AddTupleNoTransaction, Iterations, "Add tuples, no transaction.", false);
            ResetSpace();
            TestPerformance(AddTupleInTransaction, Iterations, "Add tuples in transaction.", false);
            ResetSpace();
            TestPerformance(NothingInTransactionAbort, Iterations, "Just do transactions.", false);
            ResetSpace();
            TestPerformance(AddTupleInTransactionAbort, Iterations, "Add tuples in transaction abort.", false);
            ResetSpace();
            _spaceProxy.BeginTransaction();
            TestPerformance(AddTupleNoTransaction, Iterations, () => _spaceProxy.CommitTransaction(), "Add tuples in global transaction and commit.", false);
            ResetSpace();
            _spaceProxy.BeginTransaction();
            TestPerformance(AddTupleNoTransaction, Iterations, () => _spaceProxy.RollbackTransaction(), "Add tuples in global transaction and abort.", false);
        }

        private void JitPrepare()
        {
            ResetSpace();
            AddTupleToList();
            AddTupleNoTransaction();
            AddTupleInTransaction();
            AddTupleInTransactionAbort();
            NothingInTransactionAbort();
        }
    }
}