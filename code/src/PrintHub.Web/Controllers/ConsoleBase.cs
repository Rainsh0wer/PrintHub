using Microsoft.AspNetCore.Mvc;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public abstract class ConsoleBase : Controller
{
    protected bool IsShop() => HttpContext.Session.GetString(SessionKeys.UserRole) is "ShopOwner" or "ShopStaff";
    protected bool IsAdmin() => HttpContext.Session.GetString(SessionKeys.UserRole) == "Admin";

    protected int? CurrentShopId()
    {
        var csv = HttpContext.Session.GetString(SessionKeys.ShopIds);
        var first = csv?.Split(',', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return int.TryParse(first, out var v) ? v : null;
    }
}
