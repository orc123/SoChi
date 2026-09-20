using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SoChi.Client.ViewModels;

public partial class OverviewViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Title { get; set; } = "Tổng Quan";

    [RelayCommand]
    private Task OpenTransactionFormAsync() => Shell.Current.GoToAsync("transaction_form");
}
