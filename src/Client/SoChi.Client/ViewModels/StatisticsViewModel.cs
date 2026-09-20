using CommunityToolkit.Mvvm.ComponentModel;

namespace SoChi.Client.ViewModels;

public partial class StatisticsViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Title { get; set; } = "Thống kê";
}
