using Google;
using LMCM_BE.DbContext;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace LMCM_BE.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LMCM_DBContext _context;
        private IDbContextTransaction _transaction;

        public UnitOfWork(LMCM_DBContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            // Begin a new transaction asynchronously
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                // Commit all changes made during the transaction
                await _context.SaveChangesAsync();

                // Commit the transaction
                await _transaction.CommitAsync();
            }
            catch
            {
                // In case of an error, roll back the transaction
                await RollbackAsync();
                throw;
            }
        }

        public async Task RollbackAsync()
        {
            // Rollback the transaction in case of an error
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
        }

        public void Dispose()
        {
            // Dispose of the transaction object if needed
            _transaction?.Dispose();
        }
    }

}
