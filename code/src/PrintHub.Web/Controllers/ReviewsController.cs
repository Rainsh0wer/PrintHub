using Microsoft.AspNetCore.Mvc;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class ReviewsController : Controller
{
    private readonly PrintHubApiClient _api;
    public ReviewsController(PrintHubApiClient api) => _api = api;

    [HttpPost]
    public async Task<IActionResult> Create(int orderId, int rating, string? comment)
    {
        var res = await _api.PostAsync<object>($"/api/orders/{orderId}/review", new { rating, comment });
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Thanks for your review!" : res.Error;
        return RedirectToAction("Details", "Orders", new { id = orderId });
    }
}
