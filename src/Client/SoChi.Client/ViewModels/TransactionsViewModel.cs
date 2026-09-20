using CommunityToolkit.Mvvm.ComponentModel;

namespace SoChi.Client.ViewModels;

public partial class TransactionsViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Title { get; set; } = "Giao dịch";
}
