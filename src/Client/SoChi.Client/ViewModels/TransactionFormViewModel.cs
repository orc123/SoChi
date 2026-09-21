using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SoChi.Client.Data.Enums;
using SoChi.Client.Services;
using SoChi.Client.Dtos;

namespace SoChi.Client.ViewModels;

public partial class TransactionFormViewModel(ITransactionRepository transactionRepository) : ObservableObject
{
    private readonly ITransactionRepository _transactionRepository = transactionRepository;

    [ObservableProperty]
    public partial string Title { get; set; } = "Thêm giao dịch";

    [ObservableProperty]
    public partial string AmountText { get; set; } = String.Empty;
    [ObservableProperty]
    public partial string Note { get; set; } = String.Empty;
    [ObservableProperty]
    public partial DateTime OccurredOn { get; set; } = DateTime.Today;
    [ObservableProperty]
    public partial CategoryDto? SelectedCategory { get; set; }
    [ObservableProperty]
    public partial TransactionKind SelectedKind { get; set; } = TransactionKind.Expense;

    public ObservableCollection<CategoryDto> Categories { get; } = new();

    public async Task LoadCategoriesAsync()
    {
        Categories.Clear();
        var list = await _transactionRepository.GetCategoriesAsync();

        foreach (var category in list.Where(x => x.Kind == SelectedKind))
        {
            Categories.Add(category);
        }
        SelectedCategory = Categories.FirstOrDefault();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var amount = ParsedAmount;
        if (amount <= 0)
        {
            await Shell.Current.DisplayAlertAsync("Lỗi", "Vui lòng nhập số tiền hợp lệ.", "Đóng");
            return;
        }

        if (SelectedCategory == null)
        {
            await Shell.Current.DisplayAlertAsync("Lỗi", "Vui lòng chọn một danh mục", "Đóng");
            return;
        }

        var transaction = new TransactionDto
        {
            Amount = amount,
            Note = Note,
            OccurredOn = OccurredOn,
            CategoryId = SelectedCategory.Id,
            SyncState = SyncState.Local
        };

        await _transactionRepository.SaveTransactionAsync(transaction);
        await Shell.Current.GoToAsync("..");
    }

    partial void OnSelectedKindChanged(TransactionKind value)
    {
        _ = LoadCategoriesAsync();
    }

    private long ParsedAmount => long.TryParse(new string(AmountText.Where(char.IsDigit).ToArray()), out var amount)
        ? amount
        : 0;
}
