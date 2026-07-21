using Microsoft.EntityFrameworkCore;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Infrastructure.Persistence;

/// <summary>
/// Seeds demo data so every workflow state and report has something to show.
/// Idempotent: does nothing if users already exist. Runs at API startup.
///
/// Sample accounts (all password: <c>Password123!</c>):
///   admin@printhub.vn         Admin
///   owner.quickprint@printhub.vn / owner.campuscopy@printhub.vn / owner.makerlab@printhub.vn   ShopOwner
///   staff.quickprint@printhub.vn / staff.campuscopy@printhub.vn                                ShopStaff
///   customer1@printhub.vn / customer2@printhub.vn / customer3@printhub.vn                       Customer
/// </summary>
public static class DataSeeder
{
    private const string DefaultPassword = "Password123!";

    public static async Task SeedAsync(PrintHubDbContext db)
    {
        if (await db.Users.IgnoreQueryFilters().AnyAsync())
            return;

        var hash = BCrypt.Net.BCrypt.HashPassword(DefaultPassword);
        User NewUser(string name, string email, UserRole role, decimal wallet = 0, string? district = null) => new()
        {
            FullName = name,
            Email = email,
            PhoneNumber = "0900000000",
            PasswordHash = hash,
            Role = role,
            WalletBalance = wallet,
            DefaultAddress = district is null ? null : $"{district}, Hà Nội",
            EmailVerifiedAt = DateTime.UtcNow
        };

        // ---- Users ----
        var admin = NewUser("Platform Admin", "admin@printhub.vn", UserRole.Admin);
        var ownerQuick = NewUser("Quang Nguyen", "owner.quickprint@printhub.vn", UserRole.ShopOwner);
        var ownerCampus = NewUser("Huong Tran", "owner.campuscopy@printhub.vn", UserRole.ShopOwner);
        var ownerMaker = NewUser("Khoi Le", "owner.makerlab@printhub.vn", UserRole.ShopOwner);
        var staffQuick = NewUser("Lan Pham", "staff.quickprint@printhub.vn", UserRole.ShopStaff);
        var staffCampus = NewUser("Nam Vo", "staff.campuscopy@printhub.vn", UserRole.ShopStaff);
        var cust1 = NewUser("Minh Hoang", "customer1@printhub.vn", UserRole.Customer, 500_000, "Cầu Giấy");
        var cust2 = NewUser("An Bui", "customer2@printhub.vn", UserRole.Customer, 200_000, "Đống Đa");
        var cust3 = NewUser("Thao Do", "customer3@printhub.vn", UserRole.Customer, 1_000_000, "Hai Bà Trưng");

        db.Users.AddRange(admin, ownerQuick, ownerCampus, ownerMaker, staffQuick, staffCampus, cust1, cust2, cust3);
        await db.SaveChangesAsync();

        // ---- Service catalogue ----
        ServiceType St(string code, string name, ServiceGroup group, PricingModel model, string unit, bool requiresFile = true) => new()
        {
            Code = code, Name = name, ServiceGroup = group, PricingModel = model,
            UnitOfMeasure = unit, RequiresFile = requiresFile, IsActive = true
        };

        var docBwA4 = St("DOC_BW_A4", "A4 Black & White Printing", ServiceGroup.Document, PricingModel.PerPage, "page");
        var docColorA4 = St("DOC_COLOR_A4", "A4 Colour Printing", ServiceGroup.Document, PricingModel.PerPage, "page");
        var docBwA3 = St("DOC_BW_A3", "A3 Black & White Printing", ServiceGroup.Document, PricingModel.PerPage, "page");
        var photocopy = St("PHOTOCOPY_A4", "A4 Photocopy", ServiceGroup.Document, PricingModel.PerPage, "page");
        var plotA1 = St("PLOT_A1", "A1 Drawing Plot", ServiceGroup.Document, PricingModel.PerPage, "page");
        var bindSpiral = St("BIND_SPIRAL", "Spiral Binding", ServiceGroup.Finishing, PricingModel.PerUnit, "unit", requiresFile: false);
        var bindThermal = St("BIND_THERMAL", "Thermal Binding", ServiceGroup.Finishing, PricingModel.PerUnit, "unit", requiresFile: false);
        var laminate = St("LAMINATE_A4", "A4 Lamination", ServiceGroup.Finishing, PricingModel.PerUnit, "unit", requiresFile: false);
        var nameCard = St("NAMECARD", "Business Cards", ServiceGroup.Finishing, PricingModel.PerUnit, "unit");
        var decal = St("DECAL", "Waterproof Decal", ServiceGroup.Finishing, PricingModel.PerUnit, "unit");
        var print3d = St("PRINT_3D_FDM", "FDM 3D Printing", ServiceGroup.Fabrication, PricingModel.MaterialAndTime, "gram");
        var laserCut = St("LASER_CUT", "Laser Cutting & Engraving", ServiceGroup.Fabrication, PricingModel.MaterialAndTime, "gram");

        db.ServiceTypes.AddRange(docBwA4, docColorA4, docBwA3, photocopy, plotA1, bindSpiral,
            bindThermal, laminate, nameCard, decal, print3d, laserCut);
        await db.SaveChangesAsync();

        // ---- Shops with rate cards, machines, materials, staff ----
        var quickPrint = new Shop
        {
            Owner = ownerQuick, Name = "QuickPrint Cầu Giấy",
            Description = "Fast document printing and binding next to the university gate.",
            AddressLine = "144 Xuân Thủy", District = "Cầu Giấy", City = "Hà Nội",
            Latitude = 21.0362, Longitude = 105.7827, PhoneNumber = "0901111111",
            OpenTime = new TimeOnly(7, 30), CloseTime = new TimeOnly(21, 0),
            Status = ShopStatus.Active, RatingAverage = 4.6, RatingCount = 2,
            ApprovedAt = DateTime.UtcNow.AddMonths(-6), ApprovedBy = admin.Id
        };
        var campusCopy = new Shop
        {
            Owner = ownerCampus, Name = "Campus Copy Center",
            Description = "Colour printing, large-format plots, and finishing services.",
            AddressLine = "25 Tạ Quang Bửu", District = "Hai Bà Trưng", City = "Hà Nội",
            Latitude = 21.0045, Longitude = 105.8437, PhoneNumber = "0902222222",
            OpenTime = new TimeOnly(8, 0), CloseTime = new TimeOnly(20, 0),
            Status = ShopStatus.Active, RatingAverage = 4.2, RatingCount = 1,
            ApprovedAt = DateTime.UtcNow.AddMonths(-4), ApprovedBy = admin.Id
        };
        var makerLab = new Shop
        {
            Owner = ownerMaker, Name = "MakerLab Fabrication",
            Description = "3D printing and laser cutting for prototypes and student projects.",
            AddressLine = "1 Đại Cồ Việt", District = "Hai Bà Trưng", City = "Hà Nội",
            Latitude = 21.0070, Longitude = 105.8430, PhoneNumber = "0903333333",
            OpenTime = new TimeOnly(9, 0), CloseTime = new TimeOnly(18, 0),
            Status = ShopStatus.Active, RatingAverage = 0, RatingCount = 0,
            ApprovedAt = DateTime.UtcNow.AddMonths(-2), ApprovedBy = admin.Id
        };
        db.Shops.AddRange(quickPrint, campusCopy, makerLab);

        db.ShopStaff.AddRange(
            new ShopStaff { Shop = quickPrint, User = staffQuick, Position = "Counter", JoinedAt = DateTime.UtcNow.AddMonths(-3), IsActive = true },
            new ShopStaff { Shop = campusCopy, User = staffCampus, Position = "Counter", JoinedAt = DateTime.UtcNow.AddMonths(-2), IsActive = true });

        ShopService Rate(Shop shop, ServiceType type, decimal price, int leadMinutes, decimal setup = 0, int minQty = 1) => new()
        {
            Shop = shop, ServiceType = type, UnitPrice = price, SetupFee = setup,
            MinQuantity = minQty, LeadTimeMinutes = leadMinutes, IsActive = true
        };

        // QuickPrint: documents + basic finishing
        var qpBw = Rate(quickPrint, docBwA4, 500, 1);
        var qpColor = Rate(quickPrint, docColorA4, 3000, 1);
        var qpCopy = Rate(quickPrint, photocopy, 400, 1);
        var qpSpiral = Rate(quickPrint, bindSpiral, 15000, 5);
        var qpLaminate = Rate(quickPrint, laminate, 5000, 3);
        db.ShopServices.AddRange(qpBw, qpColor, qpCopy, qpSpiral, qpLaminate);
        db.PriceRules.AddRange(
            new PriceRule { ShopService = qpBw, RuleType = PriceRuleType.Sides, OptionKey = "Duplex", Multiplier = 1.8m },
            new PriceRule { ShopService = qpBw, RuleType = PriceRuleType.QuantityTier, OptionKey = "bulk", Multiplier = 0.9m, MinQuantity = 200 },
            new PriceRule { ShopService = qpColor, RuleType = PriceRuleType.Sides, OptionKey = "Duplex", Multiplier = 1.8m });

        // Campus Copy: colour, A3, plots, thermal binding, name cards
        var ccColor = Rate(campusCopy, docColorA4, 2500, 1);
        var ccA3 = Rate(campusCopy, docBwA3, 1500, 2);
        var ccPlot = Rate(campusCopy, plotA1, 25000, 10);
        var ccThermal = Rate(campusCopy, bindThermal, 20000, 8);
        var ccCard = Rate(campusCopy, nameCard, 250, 30, setup: 20000, minQty: 100);
        db.ShopServices.AddRange(ccColor, ccA3, ccPlot, ccThermal, ccCard);
        db.PriceRules.Add(new PriceRule { ShopService = ccColor, RuleType = PriceRuleType.PaperType, OptionKey = "A3", Multiplier = 2.0m });

        // MakerLab: fabrication
        var mlPrint3d = Rate(makerLab, print3d, 800, 2, setup: 20000);
        var mlLaser = Rate(makerLab, laserCut, 1200, 3, setup: 30000);
        db.ShopServices.AddRange(mlPrint3d, mlLaser);
        db.PriceRules.AddRange(
            new PriceRule { ShopService = mlPrint3d, RuleType = PriceRuleType.Material, OptionKey = "PETG", Multiplier = 1.3m },
            new PriceRule { ShopService = mlPrint3d, RuleType = PriceRuleType.QualityProfile, OptionKey = "Fine", Multiplier = 1.5m });

        db.Machines.AddRange(
            new Machine { Shop = quickPrint, Name = "Ricoh MP-1", MachineType = MachineType.Printer, ServiceGroup = ServiceGroup.Document, Status = MachineStatus.Idle },
            new Machine { Shop = quickPrint, Name = "Binder-1", MachineType = MachineType.Finishing, ServiceGroup = ServiceGroup.Finishing, Status = MachineStatus.Idle },
            new Machine { Shop = campusCopy, Name = "Xerox-Color-1", MachineType = MachineType.Printer, ServiceGroup = ServiceGroup.Document, Status = MachineStatus.Idle },
            new Machine { Shop = campusCopy, Name = "Plotter-A1", MachineType = MachineType.Plotter, ServiceGroup = ServiceGroup.Document, Status = MachineStatus.Idle },
            new Machine { Shop = makerLab, Name = "Prusa-MK4", MachineType = MachineType.Printer3D, ServiceGroup = ServiceGroup.Fabrication, Status = MachineStatus.Idle },
            new Machine { Shop = makerLab, Name = "Laser-60W", MachineType = MachineType.LaserCutter, ServiceGroup = ServiceGroup.Fabrication, Status = MachineStatus.Maintenance });

        db.Materials.AddRange(
            new Material { Shop = quickPrint, Name = "A4 80gsm", MaterialType = MaterialType.Paper, Unit = "sheet", StockQuantity = 5000, LowStockThreshold = 500, UnitCost = 150 },
            new Material { Shop = campusCopy, Name = "A4 Glossy 120gsm", MaterialType = MaterialType.Paper, Unit = "sheet", StockQuantity = 800, LowStockThreshold = 200, UnitCost = 500 },
            new Material { Shop = campusCopy, Name = "A1 Roll", MaterialType = MaterialType.Paper, Unit = "sheet", StockQuantity = 120, LowStockThreshold = 30, UnitCost = 8000 },
            new Material { Shop = makerLab, Name = "PLA White", MaterialType = MaterialType.Filament, Unit = "gram", StockQuantity = 4000, LowStockThreshold = 500, UnitCost = 400 },
            new Material { Shop = makerLab, Name = "PETG Black", MaterialType = MaterialType.Filament, Unit = "gram", StockQuantity = 300, LowStockThreshold = 400, UnitCost = 550 },
            new Material { Shop = makerLab, Name = "Plywood 3mm", MaterialType = MaterialType.Sheet, Unit = "gram", StockQuantity = 6000, LowStockThreshold = 1000, UnitCost = 120 });

        await db.SaveChangesAsync();

        // ---- Vouchers ----
        db.Vouchers.AddRange(
            new Voucher { Code = "WELCOME10", DiscountType = VoucherDiscountType.Percent, DiscountValue = 10, MinOrderAmount = 50_000, MaxDiscountAmount = 30_000, UsageLimit = 1000, UsedCount = 0, ValidFrom = DateTime.UtcNow.AddMonths(-1), ValidTo = DateTime.UtcNow.AddMonths(2), IsActive = true },
            new Voucher { Code = "STUDENT20K", DiscountType = VoucherDiscountType.FixedAmount, DiscountValue = 20_000, MinOrderAmount = 100_000, UsageLimit = 500, UsedCount = 0, ValidFrom = DateTime.UtcNow.AddDays(-7), ValidTo = DateTime.UtcNow.AddMonths(1), IsActive = true });
        await db.SaveChangesAsync();

        // ---- A few orders across statuses (gives reports something to aggregate) ----
        await SeedOrdersAsync(db, quickPrint, campusCopy, cust1, cust2, staffQuick, docBwA4, qpBw, docColorA4, ccColor);
    }

    private static async Task SeedOrdersAsync(
        PrintHubDbContext db, Shop quickPrint, Shop campusCopy, User cust1, User cust2, User staffQuick,
        ServiceType docBwA4, ShopService qpBw, ServiceType docColorA4, ShopService ccColor)
    {
        var seq = 1;
        Order NewOrder(User customer, Shop shop, OrderStatus status, decimal total, DateTime placed) => new()
        {
            OrderCode = $"PH-{placed:yyMMdd}-{seq++:D4}",
            Customer = customer, Shop = shop, Status = status,
            FulfilmentMethod = FulfilmentMethod.Pickup,
            SubTotal = total, TotalAmount = total,
            CommissionRate = status == OrderStatus.Completed ? 0.08m : 0,
            CommissionAmount = status == OrderStatus.Completed ? Math.Round(total * 0.08m, 2) : 0,
            ProgressPercent = status == OrderStatus.Completed ? 100 : (status == OrderStatus.InProduction ? 40 : 0),
            PlacedAt = placed,
            CompletedAt = status == OrderStatus.Completed ? placed.AddHours(2) : null
        };

        // Completed order 1 — 150 pages B/W duplex at QuickPrint
        var o1 = NewOrder(cust1, quickPrint, OrderStatus.Completed, 135_000, DateTime.UtcNow.AddDays(-10));
        o1.Items.Add(new OrderItem { ServiceType = docBwA4, Quantity = 1, PageCount = 150, ColorMode = ColorMode.BlackWhite, Sides = Sides.Duplex, UnitPrice = 900, LineTotal = 135_000 });
        o1.StatusHistory.Add(new OrderStatusHistory { FromStatus = OrderStatus.ReadyForPickup, ToStatus = OrderStatus.Completed, ActorUserId = cust1.Id, ActorRole = UserRole.Customer, Reason = "Picked up", CreatedAt = o1.CompletedAt!.Value });
        o1.WalletTransactions.Add(new WalletTransaction { User = cust1, Type = WalletTransactionType.Payment, Amount = -135_000, BalanceAfter = 365_000, RefCode = "PAY-0001", Status = WalletTransactionStatus.Completed, CreatedAt = o1.PlacedAt!.Value });

        // Completed order 2 — colour printing at Campus Copy
        var o2 = NewOrder(cust2, campusCopy, OrderStatus.Completed, 100_000, DateTime.UtcNow.AddDays(-5));
        o2.Items.Add(new OrderItem { ServiceType = docColorA4, Quantity = 40, PageCount = 40, ColorMode = ColorMode.Color, Sides = Sides.Simplex, UnitPrice = 2500, LineTotal = 100_000 });
        o2.WalletTransactions.Add(new WalletTransaction { User = cust2, Type = WalletTransactionType.Payment, Amount = -100_000, BalanceAfter = 100_000, RefCode = "PAY-0002", Status = WalletTransactionStatus.Completed, CreatedAt = o2.PlacedAt!.Value });

        // In-production order at QuickPrint
        var o3 = NewOrder(cust1, quickPrint, OrderStatus.InProduction, 60_000, DateTime.UtcNow.AddHours(-3));
        o3.Items.Add(new OrderItem { ServiceType = docBwA4, Quantity = 1, PageCount = 120, ColorMode = ColorMode.BlackWhite, Sides = Sides.Simplex, UnitPrice = 500, LineTotal = 60_000 });

        // Awaiting acceptance at Campus Copy
        var o4 = NewOrder(cust2, campusCopy, OrderStatus.AwaitingAcceptance, 50_000, DateTime.UtcNow.AddMinutes(-30));
        o4.Items.Add(new OrderItem { ServiceType = docColorA4, Quantity = 20, PageCount = 20, ColorMode = ColorMode.Color, Sides = Sides.Simplex, UnitPrice = 2500, LineTotal = 50_000 });

        db.Orders.AddRange(o1, o2, o3, o4);
        await db.SaveChangesAsync();

        // Reviews for completed orders
        db.Reviews.AddRange(
            new Review { Order = o1, Customer = cust1, Shop = quickPrint, Rating = 5, Comment = "Fast and cheap, right by the gate." },
            new Review { Order = o2, Customer = cust2, Shop = campusCopy, Rating = 4, Comment = "Good colour quality." });
        await db.SaveChangesAsync();
    }
}
