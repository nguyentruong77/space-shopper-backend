using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SpaceShopper.Application.Interfaces.IRepositories.Common;
using SpaceShopper.Infrastructure.Data;

namespace SpaceShopper.Infrastructure.Repositories.Common
{
    public sealed class UnitOfWork(SpaceShopperDbContext context) : IUnitOfWork
    {
        private readonly DbContext _context = context;
        private IDbContextTransaction? _transaction;
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction is not null)
                throw new InvalidOperationException("Transaction has already been started.");
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }
        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction is null)
                throw new InvalidOperationException("No active transaction to commit.");
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                await _transaction.CommitAsync(cancellationToken);
            }
            finally
            {
                DisposeTransaction();
            }
        }
        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction is null)
                throw new InvalidOperationException("No active transaction to rollback.");
            await _transaction.RollbackAsync(cancellationToken);
            DisposeTransaction();
        }
        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
        private void DisposeTransaction()
        {
            _transaction?.Dispose();
            _transaction = null;
        }
        public void Dispose()
        {
            DisposeTransaction();
            _context.Dispose();
        }
    }
}
