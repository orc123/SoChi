using SoChi.Client.ViewModels;

namespace SoChi.Client.Views;

public partial class CategoriesPage : ContentPage
{
	public CategoriesPage(CategoriesViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}
}