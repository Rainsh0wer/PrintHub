using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PrintHub.Application.Features.Catalog.Dtos;
using PrintHub.Desktop.Services;

namespace PrintHub.Desktop.Views;

public partial class ServiceTypesView : UserControl
{
    public ServiceTypesView()
    {
        InitializeComponent();
        Loaded += async (_, _) => await Load();
    }

    private async System.Threading.Tasks.Task Load()
    {
        var (ok, err, data) = await Api.Get<List<ServiceTypeAdminDto>>("/api/admin/service-types");
        if (!ok) { Say(err, true); return; }
        Grid.ItemsSource = data;
        Say($"{data?.Count ?? 0} service type(s).");
    }

    private ServiceTypeAdminDto? Selected => Grid.SelectedItem as ServiceTypeAdminDto;

    private async void Refresh_Click(object sender, RoutedEventArgs e) => await Load();

    private async void Create_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Code.Text) || string.IsNullOrWhiteSpace(NameBox.Text))
        { Say("Code and name are required.", true); return; }
        int.TryParse(Order.Text, out var order);

        var body = new
        {
            code = Code.Text.Trim(),
            name = NameBox.Text.Trim(),
            serviceGroup = GroupCombo.SelectedIndex,
            pricingModel = PricingCombo.SelectedIndex,
            unitOfMeasure = string.IsNullOrWhiteSpace(Unit.Text) ? "unit" : Unit.Text.Trim(),
            requiresFile = RequiresFile.IsChecked == true,
            description = (string?)null,
            displayOrder = order,
            iconUrl = (string?)null
        };
        var (ok, err, _) = await Api.Post<object>("/api/admin/service-types", body);
        if (!ok) { Say(err, true); return; }
        Say($"Created “{NameBox.Text.Trim()}”.");
        Code.Text = NameBox.Text = "";
        await Load();
    }

    private async void Deactivate_Click(object sender, RoutedEventArgs e)
    {
        if (Selected is null) { Say("Select a service type first.", true); return; }
        var (ok, err, _) = await Api.Delete($"/api/admin/service-types/{Selected.Id}");
        if (!ok) { Say(err, true); return; }
        Say($"Deactivated “{Selected.Name}”.");
        await Load();
    }

    private void Say(string? msg, bool error = false)
    {
        Status.Text = msg;
        Status.Foreground = error ? (Brush)FindResource("Err") : (Brush)FindResource("Muted");
    }
}
