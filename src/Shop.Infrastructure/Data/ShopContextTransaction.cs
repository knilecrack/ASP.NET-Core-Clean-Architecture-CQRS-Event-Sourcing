using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop.Core.Interfaces;

namespace Shop.Infrastructure.Data;

public class ShopContextTransaction : IDataBaseTransaction
{
    private readonly ShopContext _dbContext;
    private readonly ILogger<ShopContextTransaction> _logger;

    public ShopContextTransaction(ShopContext dbContext, ILogger<ShopContextTransaction> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task ExecuteAsync(Func<Task> action)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            _logger.LogInformation("----- Begin transaction {TransactionId}", transaction.TransactionId);

            await action();

            try
            {
                var rowsAffected = await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("----- Commit transaction {TransactionId}, row(s) affected {RowsAffected}", transaction.TransactionId, rowsAffected);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occurred while committing the transaction {TransactionId}", transaction.TransactionId);

                await transaction.RollbackAsync();

                throw;
            }
            finally
            {
                transaction.Dispose();
            }
        });
    }
}
