using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Data.UoW
{
    public class UnitOfWork(DbContext context, IDbContextTransaction transaction) : IUnitOfWork
    {
        private readonly DbContext _context = context;
        private IDbContextTransaction _transaction = transaction;
        private bool _disposed;

        public void ChangeDbConnectionString(string connectionString)
        {
            _context.Database.SetConnectionString(connectionString);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _transaction = _context.Database.BeginTransaction(isolationLevel);
        }

        public void CommitTrans()
        {
            _transaction.Commit();
        }

        public void RollBackTrans()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _context.Dispose();
                _transaction.Dispose();

            }

            _disposed = true;
        }
    }

}