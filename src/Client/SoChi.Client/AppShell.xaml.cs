using SoChi.Client.Views;

namespace SoChi.Client;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("transaction_form", typeof(TransactionFormPage));
    }
}