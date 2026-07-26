using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PrintHub.Application.Features.Vouchers.Dtos;
using PrintHub.Desktop.Services;

namespace PrintHub.Desktop.Views;

public partial class VouchersView : UserControl
{
    public VouchersView()
    {
        InitializeComponent();
        ValidFrom.SelectedDate = DateTime.Today;
        ValidTo.SelectedDate = DateTime.Today.AddDays(30);
        Loaded += async (_, _) => await Load();
    }

    private async System.Threading.Tasks.Task Load()
    {
        var (ok, err, data) = await Api.Get<List<VoucherAdminDto>>("/api/admin/vouchers");
        if (!ok) { Say(err, true); return; }
        Grid.ItemsSource = data;
        Say($"{data?.Count ?? 0} voucher(s).");
    }

    private VoucherAdminDto? Selected => Grid.SelectedItem as VoucherAdminDto;

    private async void Refresh_Click(object sender, RoutedEventArgs e) => await Load();

    private async void Create_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Code.Text)) { Say("Code is required.", true); return; }
        if (!decimal.TryParse(Value.Text, out var value)) { Say("Discount value must be a number.", true); return; }
        decimal.TryParse(MinOrder.Text, out var min);
        decimal? max = decimal.TryParse(MaxDiscount.Text, out var m) ? m : null;
        int.TryParse(UsageLimit.Text, out var usage);
        int.TryParse(PerUser.Text, out var perUser);

        var body = new
        {
            code = Code.Text.Trim(),
            name = string.IsNullOrWhiteSpace(NameBox.Text) ? null : NameBox.Text.Trim(),
            discountType = TypeCombo.SelectedIndex,
            discountValue = value,
            minOrderAmount = min,
            maxDiscountAmount = max,
            usageLimit = usage,
            validFrom = ValidFrom.SelectedDate ?? DateTime.Today,
            validTo = ValidTo.SelectedDate ?? DateTime.Today.AddDays(30),
            perUserLimit = perUser <= 0 ? 1 : perUser,
            description = (string?)null
        };
        var (ok, err, _) = await Api.Post<object>("/api/admin/vouchers", body);
        if (!ok) { Say(err, true); return; }
        Say($"Created voucher {Code.Text.Trim()}.");
        Code.Text = NameBox.Text = Value.Text = "";
        await Load();
    }

    private async void Deactivate_Click(object sender, RoutedEventArgs e)
    {
        if (Selected is null) { Say("Select a voucher first.", true); return; }
        var (ok, err, _) = await Api.Delete($"/api/admin/vouchers/{Selected.Id}");
        if (!ok) { Say(err, true); return; }
        Say($"Deactivated {Selected.Code}.");
        await Load();
    }

    private void Say(string? msg, bool error = false)
    {
        Status.Text = msg;
        Status.Foreground = error ? (Brush)FindResource("Err") : (Brush)FindResource("Muted");
    }
}
