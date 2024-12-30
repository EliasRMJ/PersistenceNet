using Microsoft.EntityFrameworkCore.Storage;
using PersistenceNet.Enuns;
using PersistenceNet.Structs;

namespace PersistenceNet.Test.Domain.Managers
{
    public abstract class Manager
    {
        public ContextTest? _contextTest;

        protected IDbContextTransaction? _transaction;
        protected int _transationManager;

        private readonly OperationReturn _return = new() { ReturnType = ReturnTypeEnum.Success };

        protected OperationReturn IsValid { get { return _return; } }
        protected bool IsActiveTransaction { get { return (this._transaction != null); } }

        async protected Task InitNewTransaction()
        {
            try
            {
                if (!this.IsActiveTransaction)
                    this._transaction = await this._contextTest!.Database.BeginTransactionAsync();

                Interlocked.Increment(ref _transationManager);
            }
            catch (Exception ex)
            {
                _transationManager = 0;

                throw new Exception("Error transaction! "
                    , ex);
            }
        }

        protected void Commit()
        {
            if (_transationManager.Equals(1))
            {
                this._transaction?.Commit();

                this._transaction = null;
                this._transationManager = 0;
            }
            else
                Interlocked.Decrement(ref _transationManager);
        }

        protected void Rollback()
        {
            if (_transationManager.Equals(1))
            {
                this._transaction?.Rollback();

                this._transaction = null;
                this._transationManager = 0;
            }
            else
                Interlocked.Decrement(ref _transationManager);
        }

        protected void Dispose()
        {
            this._transaction?.Dispose();
        }
    }
}