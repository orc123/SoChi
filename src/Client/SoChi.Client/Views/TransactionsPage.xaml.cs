using SoChi.Client.ViewModels;

namespace SoChi.Client.Views;

public partial class TransactionsPage : ContentPage
{
	public TransactionsPage(TransactionsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}
}