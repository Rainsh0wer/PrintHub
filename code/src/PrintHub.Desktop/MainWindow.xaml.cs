using System.Windows;
using System.Windows.Controls;
using PrintHub.Desktop.Services;
using PrintHub.Desktop.Views;

namespace PrintHub.Desktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        WhoName.Text = Api.UserName ?? "Admin";
        Host.Content = new ApprovalsView();
    }

    private void Nav_Click(object sender, RoutedEventArgs e)
    {
        var tag = (sender as Button)?.Tag as string;
        Host.Content = tag switch
        {
            "approvals" => new ApprovalsView(),
            "users" => new UsersView(),
            "catalog" => new ServiceTypesView(),
            "vouchers" => new VouchersView(),
            "complaints" => new ComplaintsView(),
            "report" => new ReportView(),
            _ => Host.Content
        };
    }

    private void SignOut_Click(object sender, RoutedEventArgs e)
    {
        Api.Token = null;
        Api.Role = null;
        Api.UserName = null;
        new LoginWindow().Show();
        Close();
    }
}
