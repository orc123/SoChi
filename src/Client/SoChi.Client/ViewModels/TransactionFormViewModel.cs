using System;
using System.Collections.Generic;
using System.Text;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SoChi.Client.ViewModels;

public partial class TransactionFormViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Title { get; set; } = "Modal Thêm sửa";

    [RelayCommand]
    private Task CloseAsync() => Shell.Current.GoToAsync("..");
}
