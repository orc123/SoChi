using SoChi.Client.ViewModels;

namespace SoChi.Client.Views;

public partial class TransactionFormPage : ContentPage
{
	public TransactionFormPage(TransactionFormViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is TransactionFormViewModel viewModel)
        {
            await viewModel.LoadCategoriesAsync();
        }
    }
}