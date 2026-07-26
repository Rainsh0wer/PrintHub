using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PrintHub.Application.Features.Reports.Dtos;
using PrintHub.Desktop.Services;

namespace PrintHub.Desktop.Views;

public partial class ReportView : UserControl
{
    public ReportView()
    {
        InitializeComponent();
        Loaded += async (_, _) => await Load();
    }

    private async System.Threading.Tasks.Task Load()
    {
        var path = "/api/reports/platform";
        var qs = new System.Collections.Generic.List<string>();
        if (From.SelectedDate is { } f) qs.Add($"from={f:yyyy-MM-dd}");
        if (To.SelectedDate is { } t) qs.Add($"to={t:yyyy-MM-dd}");
        if (qs.Count > 0) path += "?" + string.Join("&", qs);

        var (ok, err, data) = await Api.GetRaw<PlatformReportDto>(path);
        if (!ok || data is null) { Say(err ?? "No data.", true); return; }

        KpiShops.Text = data.TotalShops.ToString("#,##0");
        KpiActive.Text = data.ActiveShops.ToString("#,##0");
        KpiOrders.Text = data.CompletedOrders.ToString("#,##0");
        KpiGmv.Text = $"{data.Gmv:#,##0} đ";
        KpiCommission.Text = $"{data.CommissionEarned:#,##0} đ";
        KpiTop.Text = string.IsNullOrWhiteSpace(data.TopShopName)
            ? "—"
            : $"{data.TopShopName} · {data.TopShopRevenue:#,##0} đ";
        Say("Report generated.");
    }

    private async void Run_Click(object sender, RoutedEventArgs e) => await Load();

    private void Say(string? msg, bool error = false)
    {
        Status.Text = msg;
        Status.Foreground = error ? (Brush)FindResource("Err") : (Brush)FindResource("Muted");
    }
}
