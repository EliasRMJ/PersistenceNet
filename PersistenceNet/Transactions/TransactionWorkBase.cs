using Flunt.Notifications;
using Microsoft.EntityFrameworkCore.Storage;
using PersistenceNet.Extensions;
using PersistenceNet.Interfaces;

namespace PersistenceNet.Transactions
{
    public abstract class TransactionWorkBase(PersistenceContext context, IMessagesProvider provider)
        : Notifiable<Notification>, ITransactionWork, IDisposable
    {
        private IDbContextTransaction? _transaction;
        private int _countTransaction = 0;

        protected bool IsActiveTransaction { get { return (this._transaction != null); } }

        public async Task BeginTransactionAsync()
        {
            try
            {
                if (!this.IsActiveTransaction)
                    this._transaction = await context.Database.BeginTransactionAsync();

                Interlocked.Increment(ref _countTransaction);
            }
            catch (Exception ex)
            {
                _countTransaction = 0;

                throw new Exception($"{provider.Current.TransactionError}"
                    , ex);
            }
        }

        public async Task CommitAsync()
        {
            try
            {
                if (this._transaction == null)
                    throw new Exception($"{provider.Current.TransactionNoStarting}");

                if (_countTransaction.Equals(1))
                {
                    this._transaction?.Commit();

                    this._transaction = null;
                    this._countTransaction = 0;

                    await Task.FromResult(this._transaction == null);
                }
                else
                    Interlocked.Decrement(ref _countTransaction);
            }
            catch (Exception ex)
            {
                this._countTransaction = 0;
                AddNotification("Commit", $"{provider.Current.TransactionErrorUnexpected}: {ex.AggregateMessage()}");
                throw;
            }
        }

        public async Task RollbackAsync()
        {
            if (_countTransaction.Equals(1))
            {
                this._transaction?.Rollback();

                this._transaction = null;
                this._countTransaction = 0;

                await Task.FromResult(this._transaction == null);
            }
            else
                Interlocked.Decrement(ref _countTransaction);
        }

#pragma warning disable CA1816 
        public void Dispose()
#pragma warning restore CA1816 
        {
            this._transaction?.Dispose();
        }
    }
}