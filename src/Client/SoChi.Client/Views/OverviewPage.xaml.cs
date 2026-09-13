using SoChi.Client.ViewModels;

namespace SoChi.Client.Views;

public partial class OverviewPage : ContentPage
{
	public OverviewPage(OverviewViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}
}