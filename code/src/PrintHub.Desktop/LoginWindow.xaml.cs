using System.Windows;
using PrintHub.Application.Features.Auth.Dtos;
using PrintHub.Desktop.Services;

namespace PrintHub.Desktop;

public partial class LoginWindow : Window
{
    public LoginWindow() => InitializeComponent();

    private async void SignIn_Click(object sender, RoutedEventArgs e)
    {
        ErrorText.Text = "";
        var (ok, err, data) = await Api.Post<AuthResponse>("/api/auth/login",
            new { email = Email.Text.Trim(), password = Pwd.Password });

        if (!ok || data is null) { ErrorText.Text = err ?? "Sign in failed."; return; }
        if (data.User.Role != "Admin") { ErrorText.Text = "This console is for administrators only."; return; }

        Api.Token = data.AccessToken;
        Api.Role = data.User.Role;
        Api.UserName = data.User.FullName;

        new MainWindow().Show();
        Close();
    }
}
