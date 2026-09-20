using SoChi.Client.Data;
using SoChi.Client.Data.Enums;
using SoChi.Dtos.Data;
using SoChi.Shared.Extensions;

using SQLite;

namespace SoChi.Client.Services;

public class TransactionRepository : ITransactionRepository
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;
    private bool _isInitialized;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    public TransactionRepository()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "sochi_v1.db3");
    }
    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        var database = await GetDatabaseAsync();
        var categories = database.Table<Category>()
            .Where(c => !c.IsDeleted);
        return FastMapper.MapList<Category, CategoryDto>(await categories.ToListAsync());
    }

    public async Task<TransactionDto?> GetTransactionAsync(Guid id)
    {
        var database = await GetDatabaseAsync();
        var transaction = await database.Table<Transaction>()
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        return transaction != null ? FastMapper.Map<Transaction, TransactionDto>(transaction) : null;
    }

    public async Task<List<TransactionDto>> GetTransactionsAsync()
    {
        var database = await GetDatabaseAsync();
        var transactions = database.Table<Transaction>()
            .Where(t => !t.IsDeleted);
        return FastMapper.MapList<Transaction, TransactionDto>(await transactions.ToListAsync());
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }
        await _initLock.WaitAsync();
        try
        {
            if (_isInitialized)
            {
                return;
            }

            _database = new SQLiteAsyncConnection(_dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
            await _database.CreateTableAsync<Category>();
            await _database.CreateTableAsync<Transaction>();

            _isInitialized = true;

        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task<int> SaveCategoryAsync(CategoryDto category)
    {
        var database = await GetDatabaseAsync();
        var categoryEntity = FastMapper.Map<CategoryDto, Category>(category);

        categoryEntity.UpdatedAt = DateTime.UtcNow;
        categoryEntity.SyncState = SyncState.Local;

        return await database.Table<Category>().FirstOrDefaultAsync(c => c.Id == categoryEntity.Id) != null ?
               await database.UpdateAsync(categoryEntity)
            : await database.InsertAsync(categoryEntity);
    }

    public async Task<int> SaveTransactionAsync(TransactionDto transaction)
    {
        var database = await GetDatabaseAsync();
        var transactionEntity = FastMapper.Map<TransactionDto, Transaction>(transaction);

        transactionEntity.UpdatedAt = DateTime.UtcNow;
        transactionEntity.SyncState = SyncState.Local;

        return await database.Table<Transaction>().FirstOrDefaultAsync(t => t.Id == transactionEntity.Id) != null ?
               await database.UpdateAsync(transactionEntity)
            : await database.InsertAsync(transactionEntity);
    }

    public async Task<int> SoftDeleteTransactionAsync(Guid id)
    {
        var db = await GetDatabaseAsync();
        var existing = await db.Table<Transaction>().FirstOrDefaultAsync(t => t.Id == id);
        if (existing != null)
        {
            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.SyncState = SyncState.Local;
            return await db.UpdateAsync(existing);
        }
        return 0;
    }

    private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
    {
        if (!_isInitialized)
        {
            await InitializeAsync();
        }
        return _database!;
    }
}
