using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Complaints.Dtos;
using PrintHub.Desktop.Services;

namespace PrintHub.Desktop.Views;

public partial class ComplaintsView : UserControl
{
    public ComplaintsView()
    {
        InitializeComponent();
        Loaded += async (_, _) => await Load();
    }

    private async System.Threading.Tasks.Task Load()
    {
        var (ok, err, data) = await Api.Get<PagedResult<ComplaintDto>>("/api/admin/complaints?pageSize=100");
        if (!ok) { Say(err, true); return; }
        Grid.ItemsSource = data?.Items;
        Say(data is { TotalCount: > 0 } ? $"{data.TotalCount} escalated complaint(s)." : "No escalated complaints.");
    }

    private ComplaintDto? Selected => Grid.SelectedItem as ComplaintDto;

    private async void Refresh_Click(object sender, RoutedEventArgs e) => await Load();

    private async void Adjudicate_Click(object sender, RoutedEventArgs e)
    {
        if (Selected is null) { Say("Select a complaint first.", true); return; }
        var uphold = Uphold.IsChecked == true;
        decimal? refund = decimal.TryParse(RefundAmount.Text, out var r) ? r : null;
        if (uphold && refund is null or <= 0) { Say("Enter a refund amount to uphold.", true); return; }

        var body = new
        {
            upholdRefund = uphold,
            refundAmount = uphold ? refund : null,
            adminRuling = string.IsNullOrWhiteSpace(Ruling.Text) ? null : Ruling.Text.Trim()
        };
        var (ok, err, _) = await Api.Put<object>($"/api/admin/complaints/{Selected.Id}/adjudicate", body);
        if (!ok) { Say(err, true); return; }
        Say(uphold ? $"Refund upheld on order {Selected.OrderCode}." : $"Complaint on {Selected.OrderCode} rejected.");
        RefundAmount.Text = Ruling.Text = "";
        await Load();
    }

    private void Say(string? msg, bool error = false)
    {
        Status.Text = msg;
        Status.Foreground = error ? (Brush)FindResource("Err") : (Brush)FindResource("Muted");
    }
}
