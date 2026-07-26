using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Desktop.Services;

namespace PrintHub.Desktop.Views;

public partial class ApprovalsView : UserControl
{
    public ApprovalsView()
    {
        InitializeComponent();
        Loaded += async (_, _) => await Load();
    }

    private async System.Threading.Tasks.Task Load()
    {
        var (ok, err, data) = await Api.Get<List<ShopAdminListItemDto>>("/api/admin/shops/applications");
        if (!ok) { Say(err, true); return; }
        Grid.ItemsSource = data;
        Say(data is { Count: > 0 } ? $"{data.Count} application(s) pending." : "No pending applications.");
    }

    private ShopAdminListItemDto? Selected => Grid.SelectedItem as ShopAdminListItemDto;

    private async void Refresh_Click(object sender, RoutedEventArgs e) => await Load();

    private async void Approve_Click(object sender, RoutedEventArgs e)
    {
        if (Selected is null) { Say("Select a shop first.", true); return; }
        var (ok, err, _) = await Api.Put<object>($"/api/admin/shops/{Selected.Id}/approve");
        if (!ok) { Say(err, true); return; }
        Say($"Approved “{Selected.Name}”.");
        await Load();
    }

    private async void Reject_Click(object sender, RoutedEventArgs e)
    {
        if (Selected is null) { Say("Select a shop first.", true); return; }
        if (string.IsNullOrWhiteSpace(Reason.Text)) { Say("A reason is required to reject.", true); return; }
        var (ok, err, _) = await Api.Put<object>($"/api/admin/shops/{Selected.Id}/reject", new { reason = Reason.Text.Trim() });
        if (!ok) { Say(err, true); return; }
        Say($"Rejected “{Selected.Name}”.");
        Reason.Text = "";
        await Load();
    }

    private void Say(string? msg, bool error = false)
    {
        Status.Text = msg;
        Status.Foreground = error ? (Brush)FindResource("Err") : (Brush)FindResource("Muted");
    }
}
