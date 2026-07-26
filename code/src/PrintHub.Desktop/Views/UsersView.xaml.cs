using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Users.Dtos;
using PrintHub.Desktop.Services;

namespace PrintHub.Desktop.Views;

public partial class UsersView : UserControl
{
    public UsersView()
    {
        InitializeComponent();
        Loaded += async (_, _) => await Load();
    }

    private async System.Threading.Tasks.Task Load()
    {
        var q = Query.Text.Trim();
        var role = RoleFilter.SelectedIndex; // 0 = all, else 1..4 → enum+ shift
        var path = "/api/admin/users?pageSize=100";
        if (!string.IsNullOrWhiteSpace(q)) path += $"&q={System.Uri.EscapeDataString(q)}";
        if (role > 0) path += $"&role={role - 1}";

        var (ok, err, data) = await Api.Get<PagedResult<UserListItemDto>>(path);
        if (!ok) { Say(err, true); return; }
        Grid.ItemsSource = data?.Items;
        Say($"{data?.TotalCount ?? 0} account(s).");
    }

    private UserListItemDto? Selected => Grid.SelectedItem as UserListItemDto;

    private async void Search_Click(object sender, RoutedEventArgs e) => await Load();

    private async void Lock_Click(object sender, RoutedEventArgs e)
    {
        if (Selected is null) { Say("Select an account first.", true); return; }
        var (ok, err, _) = await Api.Put<object>($"/api/admin/users/{Selected.Id}/lock");
        if (!ok) { Say(err, true); return; }
        Say($"Locked {Selected.Email}.");
        await Load();
    }

    private async void Unlock_Click(object sender, RoutedEventArgs e)
    {
        if (Selected is null) { Say("Select an account first.", true); return; }
        var (ok, err, _) = await Api.Put<object>($"/api/admin/users/{Selected.Id}/unlock");
        if (!ok) { Say(err, true); return; }
        Say($"Unlocked {Selected.Email}.");
        await Load();
    }

    private void Say(string? msg, bool error = false)
    {
        Status.Text = msg;
        Status.Foreground = error ? (Brush)FindResource("Err") : (Brush)FindResource("Muted");
    }
}
