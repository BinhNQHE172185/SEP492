using Microsoft.EntityFrameworkCore.Storage;

namespace LMCM_BE.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Begins a new transaction asynchronously.
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commits the transaction asynchronously.
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
        Task CommitAsync();

        /// <summary>
        /// Rolls back the transaction asynchronously.
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
        Task RollbackAsync();
    }

}
