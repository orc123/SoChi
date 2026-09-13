using SoChi.Client.ViewModels;

namespace SoChi.Client.Views;

public partial class StatisticsPage : ContentPage
{
	public StatisticsPage(StatisticsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}
}