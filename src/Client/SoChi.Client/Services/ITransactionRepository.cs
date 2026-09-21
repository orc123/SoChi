using SoChi.Client.Dtos;

namespace SoChi.Client.Services;

public interface ITransactionRepository
{
    Task<List<TransactionDto>> GetTransactionsAsync();
    Task<TransactionDto?> GetTransactionAsync(Guid id);
    Task<int> SaveTransactionAsync(TransactionDto transaction);
    Task<int> SoftDeleteTransactionAsync(Guid id);

    Task<List<CategoryDto>> GetCategoriesAsync();
    Task<int> SaveCategoryAsync(CategoryDto category);
    Task InitializeAsync();
}
