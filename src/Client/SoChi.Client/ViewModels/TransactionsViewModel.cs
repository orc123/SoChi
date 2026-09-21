using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SoChi.Client.Services;
using SoChi.Client.Dtos;
using SoChi.Client.Data.Enums;

namespace SoChi.Client.ViewModels;

public partial class TransactionsViewModel(ITransactionRepository transactionRepository) : ObservableObject
{
    private readonly ITransactionRepository _transactionRepository = transactionRepository;
    public ObservableCollection<TransactionDto> Transactions { get; } = new();

    [RelayCommand]
    public async Task LoadTransactionsAsync()
    {
        Transactions.Clear();
        var items = await _transactionRepository.GetTransactionsAsync();
        foreach (var item in items)
        {
            Transactions.Add(item);
        }
    }
}