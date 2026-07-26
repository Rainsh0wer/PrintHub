using Microsoft.EntityFrameworkCore;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Infrastructure.Persistence;

/// <summary>
/// Seeds a large demo dataset so every workflow state and report has plenty to
/// show. Idempotent: does nothing if users already exist. Runs at API startup.
///
/// Sample accounts (all password: <c>Password123!</c>):
///   admin@printhub.vn                                                        Admin
///   owner.quickprint@ / owner.campuscopy@ / owner.makerlab@printhub.vn       ShopOwner
///   owner.printcorner@ / owner.sinhvien@ / owner.colorzone@                  ShopOwner
///   owner.binderpro@ / owner.rainbow@ / owner.photoexpress@printhub.vn       ShopOwner
///   staff.quickprint@ / staff.campuscopy@ / staff.sinhvien@                  ShopStaff
///   staff.colorzone@ / staff.binderpro@printhub.vn                          ShopStaff
///   customer1@ .. customer12@printhub.vn                                     Customer
/// </summary>
public static class DataSeeder
{
    private const string DefaultPassword = "Password123!";

    public static async Task SeedAsync(PrintHubDbContext db)
    {
        if (await db.Users.IgnoreQueryFilters().AnyAsync())
            return;

        var hash = BCrypt.Net.BCrypt.HashPassword(DefaultPassword);
        var rng = new Random(42);

        // Deterministic placeholder imagery (no account needed): DiceBear for
        // avatars/logos, Lorem Picsum for photos. Seeded so results are stable.
        static string Avatar(string s) => $"https://api.dicebear.com/9.x/initials/svg?seed={Uri.EscapeDataString(s)}&fontWeight=600&backgroundType=gradientLinear";
        static string Logo(string s) => $"https://api.dicebear.com/9.x/initials/svg?seed={Uri.EscapeDataString(s)}&radius=16&fontWeight=700&backgroundColor=0091c7,0ea5e9,7c3aed,f97316,16a34a";
        static string Photo(string seed, int w, int h) => $"https://picsum.photos/seed/{Uri.EscapeDataString(seed)}/{w}/{h}";

        User NewUser(string name, string email, UserRole role, decimal wallet = 0, string? district = null) => new()
        {
            FullName = name,
            Email = email,
            PhoneNumber = "09" + rng.Next(10000000, 99999999),
            PasswordHash = hash,
            Role = role,
            WalletBalance = wallet,
            DefaultAddress = district is null ? null : $"{district}, Hà Nội",
            EmailVerifiedAt = DateTime.UtcNow,
            AvatarUrl = Avatar(name)
        };

        // ---- Users ----
        var admin = NewUser("Platform Admin", "admin@printhub.vn", UserRole.Admin);

        var ownerQuick = NewUser("Quang Nguyen", "owner.quickprint@printhub.vn", UserRole.ShopOwner);
        var ownerCampus = NewUser("Huong Tran", "owner.campuscopy@printhub.vn", UserRole.ShopOwner);
        var ownerMaker = NewUser("Khoi Le", "owner.makerlab@printhub.vn", UserRole.ShopOwner);
        var ownerPrintCorner = NewUser("Duc Pham", "owner.printcorner@printhub.vn", UserRole.ShopOwner);
        var ownerSinhVien = NewUser("Trang Nguyen", "owner.sinhvien@printhub.vn", UserRole.ShopOwner);
        var ownerColorZone = NewUser("Bao Tran", "owner.colorzone@printhub.vn", UserRole.ShopOwner);
        var ownerBinderPro = NewUser("Hai Vu", "owner.binderpro@printhub.vn", UserRole.ShopOwner);
        var ownerRainbow = NewUser("Linh Dang", "owner.rainbow@printhub.vn", UserRole.ShopOwner);
        var ownerPhotoExpress = NewUser("Son Hoang", "owner.photoexpress@printhub.vn", UserRole.ShopOwner);

        var staffQuick = NewUser("Lan Pham", "staff.quickprint@printhub.vn", UserRole.ShopStaff);
        var staffCampus = NewUser("Nam Vo", "staff.campuscopy@printhub.vn", UserRole.ShopStaff);
        var staffSinhVien = NewUser("Tuan Dinh", "staff.sinhvien@printhub.vn", UserRole.ShopStaff);
        var staffColorZone = NewUser("Ngoc Ly", "staff.colorzone@printhub.vn", UserRole.ShopStaff);
        var staffBinderPro = NewUser("Vy Phan", "staff.binderpro@printhub.vn", UserRole.ShopStaff);

        var cust1 = NewUser("Minh Hoang", "customer1@printhub.vn", UserRole.Customer, 900_000, "Cầu Giấy");
        var cust2 = NewUser("An Bui", "customer2@printhub.vn", UserRole.Customer, 450_000, "Đống Đa");
        var cust3 = NewUser("Thao Do", "customer3@printhub.vn", UserRole.Customer, 1_000_000, "Hai Bà Trưng");
        var cust4 = NewUser("Ha Le", "customer4@printhub.vn", UserRole.Customer, 800_000, "Ba Đình");
        var cust5 = NewUser("Duy Tran", "customer5@printhub.vn", UserRole.Customer, 1_500_000, "Tây Hồ");
        var cust6 = NewUser("Linh Pham", "customer6@printhub.vn", UserRole.Customer, 600_000, "Thanh Xuân");
        var cust7 = NewUser("Quan Vu", "customer7@printhub.vn", UserRole.Customer, 2_000_000, "Long Biên");
        var cust8 = NewUser("Mai Nguyen", "customer8@printhub.vn", UserRole.Customer, 600_000, "Nam Từ Liêm");
        var cust9 = NewUser("Phong Do", "customer9@printhub.vn", UserRole.Customer, 1_200_000, "Cầu Giấy");
        var cust10 = NewUser("Yen Bui", "customer10@printhub.vn", UserRole.Customer, 450_000, "Đống Đa");
        var cust11 = NewUser("Kien Hoang", "customer11@printhub.vn", UserRole.Customer, 900_000, "Hai Bà Trưng");
        var cust12 = NewUser("Chi Vo", "customer12@printhub.vn", UserRole.Customer, 1_800_000, "Hoàng Mai");

        db.Users.AddRange(admin,
            ownerQuick, ownerCampus, ownerMaker, ownerPrintCorner, ownerSinhVien, ownerColorZone, ownerBinderPro, ownerRainbow, ownerPhotoExpress,
            staffQuick, staffCampus, staffSinhVien, staffColorZone, staffBinderPro,
            cust1, cust2, cust3, cust4, cust5, cust6, cust7, cust8, cust9, cust10, cust11, cust12);
        await db.SaveChangesAsync();

        var customers = new[] { cust1, cust2, cust3, cust4, cust5, cust6, cust7, cust8, cust9, cust10, cust11, cust12 };

        // ---- Service catalogue ----
        ServiceType St(string code, string name, ServiceGroup group, PricingModel model, string unit, bool requiresFile = true) => new()
        {
            Code = code, Name = name, ServiceGroup = group, PricingModel = model,
            UnitOfMeasure = unit, RequiresFile = requiresFile, IsActive = true,
            IconUrl = $"https://api.dicebear.com/9.x/icons/svg?seed={Uri.EscapeDataString(code)}&backgroundColor=eef2f6&radius=20"
        };

        var docBwA4 = St("DOC_BW_A4", "A4 Black & White Printing", ServiceGroup.Document, PricingModel.PerPage, "page");
        var docColorA4 = St("DOC_COLOR_A4", "A4 Colour Printing", ServiceGroup.Document, PricingModel.PerPage, "page");
        var docBwA3 = St("DOC_BW_A3", "A3 Black & White Printing", ServiceGroup.Document, PricingModel.PerPage, "page");
        var photocopy = St("PHOTOCOPY_A4", "A4 Photocopy", ServiceGroup.Document, PricingModel.PerPage, "page");
        var plotA1 = St("PLOT_A1", "A1 Drawing Plot", ServiceGroup.Document, PricingModel.PerPage, "page");
        var posterA2 = St("POSTER_A2", "A2 Poster Printing", ServiceGroup.Document, PricingModel.PerPage, "page");
        var bindSpiral = St("BIND_SPIRAL", "Spiral Binding", ServiceGroup.Finishing, PricingModel.PerUnit, "unit", requiresFile: false);
        var bindThermal = St("BIND_THERMAL", "Thermal Binding", ServiceGroup.Finishing, PricingModel.PerUnit, "unit", requiresFile: false);
        var bindHardcover = St("BIND_HARDCOVER", "Hardcover Binding", ServiceGroup.Finishing, PricingModel.PerUnit, "unit", requiresFile: false);
        var laminate = St("LAMINATE_A4", "A4 Lamination", ServiceGroup.Finishing, PricingModel.PerUnit, "unit", requiresFile: false);
        var nameCard = St("NAMECARD", "Business Cards", ServiceGroup.Finishing, PricingModel.PerUnit, "unit");
        var decal = St("DECAL", "Waterproof Decal", ServiceGroup.Finishing, PricingModel.PerUnit, "unit");
        var print3d = St("PRINT_3D_FDM", "FDM 3D Printing", ServiceGroup.Fabrication, PricingModel.MaterialAndTime, "gram");
        var laserCut = St("LASER_CUT", "Laser Cutting & Engraving", ServiceGroup.Fabrication, PricingModel.MaterialAndTime, "gram");

        db.ServiceTypes.AddRange(docBwA4, docColorA4, docBwA3, photocopy, plotA1, posterA2, bindSpiral,
            bindThermal, bindHardcover, laminate, nameCard, decal, print3d, laserCut);
        await db.SaveChangesAsync();

        // ---- Shops with rate cards, machines, materials, staff, gallery ----
        var quickPrint = new Shop
        {
            Owner = ownerQuick, Name = "QuickPrint Cầu Giấy",
            Description = "Fast document printing and binding next to the university gate.",
            AddressLine = "144 Xuân Thủy", District = "Cầu Giấy", City = "Hà Nội",
            Latitude = 21.0362, Longitude = 105.7827, PhoneNumber = "0901111111",
            OpenTime = new TimeOnly(7, 30), CloseTime = new TimeOnly(21, 0),
            Status = ShopStatus.Active, RatingAverage = 0, RatingCount = 0,
            LogoUrl = Logo("QuickPrint"), CoverImageUrl = Photo("quickprint-cover", 1200, 480),
            ApprovedAt = DateTime.UtcNow.AddMonths(-8), ApprovedBy = admin.Id
        };
        var campusCopy = new Shop
        {
            Owner = ownerCampus, Name = "Campus Copy Center",
            Description = "Colour printing, large-format plots, and finishing services.",
            AddressLine = "25 Tạ Quang Bửu", District = "Hai Bà Trưng", City = "Hà Nội",
            Latitude = 21.0045, Longitude = 105.8437, PhoneNumber = "0902222222",
            OpenTime = new TimeOnly(8, 0), CloseTime = new TimeOnly(20, 0),
            Status = ShopStatus.Active, RatingAverage = 0, RatingCount = 0,
            LogoUrl = Logo("Campus Copy"), CoverImageUrl = Photo("campuscopy-cover", 1200, 480),
            ApprovedAt = DateTime.UtcNow.AddMonths(-7), ApprovedBy = admin.Id
        };
        var makerLab = new Shop
        {
            Owner = ownerMaker, Name = "MakerLab Fabrication",
            Description = "3D printing and laser cutting for prototypes and student projects.",
            AddressLine = "1 Đại Cồ Việt", District = "Hai Bà Trưng", City = "Hà Nội",
            Latitude = 21.0070, Longitude = 105.8430, PhoneNumber = "0903333333",
            OpenTime = new TimeOnly(9, 0), CloseTime = new TimeOnly(18, 0),
            Status = ShopStatus.Active, RatingAverage = 0, RatingCount = 0,
            LogoUrl = Logo("MakerLab"), CoverImageUrl = Photo("makerlab-cover", 1200, 480),
            ApprovedAt = DateTime.UtcNow.AddMonths(-5), ApprovedBy = admin.Id
        };
        var printCorner = new Shop
        {
            Owner = ownerPrintCorner, Name = "Print Corner Thanh Xuân",
            Description = "Neighbourhood print shop for everyday document jobs.",
            AddressLine = "68 Nguyễn Trãi", District = "Thanh Xuân", City = "Hà Nội",
            Latitude = 20.9970, Longitude = 105.8060, PhoneNumber = "0904444444",
            OpenTime = new TimeOnly(7, 0), CloseTime = new TimeOnly(21, 30),
            Status = ShopStatus.Active, RatingAverage = 0, RatingCount = 0,
            LogoUrl = Logo("Print Corner"), CoverImageUrl = Photo("printcorner-cover", 1200, 480),
            ApprovedAt = DateTime.UtcNow.AddMonths(-3), ApprovedBy = admin.Id
        };
        var sinhVien = new Shop
        {
            Owner = ownerSinhVien, Name = "Sinh Viên Print",
            Description = "Budget printing for students, right by the dormitories.",
            AddressLine = "Lô 12 Mễ Trì Hạ", District = "Nam Từ Liêm", City = "Hà Nội",
            Latitude = 21.0075, Longitude = 105.7820, PhoneNumber = "0905555555",
            OpenTime = new TimeOnly(7, 0), CloseTime = new TimeOnly(22, 0),
            Status = ShopStatus.Active, RatingAverage = 0, RatingCount = 0,
            LogoUrl = Logo("Sinh Vien Print"), CoverImageUrl = Photo("sinhvien-cover", 1200, 480),
            ApprovedAt = DateTime.UtcNow.AddMonths(-4), ApprovedBy = admin.Id
        };
        var colorZone = new Shop
        {
            Owner = ownerColorZone, Name = "ColorZone Studio",
            Description = "Colour and large-format printing for design students and studios.",
            AddressLine = "12 Ngọc Lâm", District = "Long Biên", City = "Hà Nội",
            Latitude = 21.0450, Longitude = 105.8850, PhoneNumber = "0906666666",
            OpenTime = new TimeOnly(8, 30), CloseTime = new TimeOnly(19, 30),
            Status = ShopStatus.Active, RatingAverage = 0, RatingCount = 0,
            LogoUrl = Logo("ColorZone"), CoverImageUrl = Photo("colorzone-cover", 1200, 480),
            ApprovedAt = DateTime.UtcNow.AddMonths(-2), ApprovedBy = admin.Id
        };
        var binderPro = new Shop
        {
            Owner = ownerBinderPro, Name = "BinderPro",
            Description = "Binding, lamination, and finishing specialists.",
            AddressLine = "9 Đội Cấn", District = "Ba Đình", City = "Hà Nội",
            Latitude = 21.0340, Longitude = 105.8210, PhoneNumber = "0907777777",
            OpenTime = new TimeOnly(8, 0), CloseTime = new TimeOnly(19, 0),
            Status = ShopStatus.Active, RatingAverage = 0, RatingCount = 0,
            LogoUrl = Logo("BinderPro"), CoverImageUrl = Photo("binderpro-cover", 1200, 480),
            ApprovedAt = DateTime.UtcNow.AddMonths(-3), ApprovedBy = admin.Id
        };
        var rainbowPrints = new Shop
        {
            Owner = ownerRainbow, Name = "Rainbow Prints & Fabrication",
            Description = "3D printing, laser cutting, and document printing under one roof.",
            AddressLine = "3 Xuân Diệu", District = "Tây Hồ", City = "Hà Nội",
            Latitude = 21.0580, Longitude = 105.8210, PhoneNumber = "0908888888",
            OpenTime = new TimeOnly(9, 0), CloseTime = new TimeOnly(18, 0),
            Status = ShopStatus.Suspended, RatingAverage = 0, RatingCount = 0,
            ReviewNote = "Repeated late deliveries reported across several orders; suspended pending review.",
            LogoUrl = Logo("Rainbow Prints"), CoverImageUrl = Photo("rainbow-cover", 1200, 480),
            ApprovedAt = DateTime.UtcNow.AddMonths(-6), ApprovedBy = admin.Id
        };
        var photoExpress = new Shop
        {
            Owner = ownerPhotoExpress, Name = "PhotoPrint Express",
            Description = "Photo and document printing — new application awaiting review.",
            AddressLine = "77 Giải Phóng", District = "Hoàng Mai", City = "Hà Nội",
            Latitude = 20.9950, Longitude = 105.8410, PhoneNumber = "0909999999",
            OpenTime = new TimeOnly(8, 0), CloseTime = new TimeOnly(20, 0),
            Status = ShopStatus.PendingReview, RatingAverage = 0, RatingCount = 0,
            LogoUrl = Logo("PhotoPrint Express"), CoverImageUrl = Photo("photoexpress-cover", 1200, 480)
        };

        db.Shops.AddRange(quickPrint, campusCopy, makerLab, printCorner, sinhVien, colorZone, binderPro, rainbowPrints, photoExpress);

        db.ShopGalleryImages.AddRange(
            new ShopGalleryImage { Shop = quickPrint, Url = Photo("qp-g1", 700, 500), Caption = "Bound thesis copies", DisplayOrder = 1 },
            new ShopGalleryImage { Shop = quickPrint, Url = Photo("qp-g2", 700, 500), Caption = "Counter service", DisplayOrder = 2 },
            new ShopGalleryImage { Shop = quickPrint, Url = Photo("qp-g3", 700, 500), Caption = "Colour proofs", DisplayOrder = 3 },
            new ShopGalleryImage { Shop = campusCopy, Url = Photo("cc-g1", 700, 500), Caption = "A1 plotting", DisplayOrder = 1 },
            new ShopGalleryImage { Shop = campusCopy, Url = Photo("cc-g2", 700, 500), Caption = "Business cards", DisplayOrder = 2 },
            new ShopGalleryImage { Shop = campusCopy, Url = Photo("cc-g3", 700, 500), Caption = "Lamination", DisplayOrder = 3 },
            new ShopGalleryImage { Shop = makerLab, Url = Photo("ml-g1", 700, 500), Caption = "FDM prints", DisplayOrder = 1 },
            new ShopGalleryImage { Shop = makerLab, Url = Photo("ml-g2", 700, 500), Caption = "Laser-cut acrylic", DisplayOrder = 2 },
            new ShopGalleryImage { Shop = makerLab, Url = Photo("ml-g3", 700, 500), Caption = "Prototype parts", DisplayOrder = 3 },
            new ShopGalleryImage { Shop = printCorner, Url = Photo("pc-g1", 700, 500), Caption = "Front counter", DisplayOrder = 1 },
            new ShopGalleryImage { Shop = printCorner, Url = Photo("pc-g2", 700, 500), Caption = "Bulk print run", DisplayOrder = 2 },
            new ShopGalleryImage { Shop = sinhVien, Url = Photo("sv-g1", 700, 500), Caption = "Dorm-side counter", DisplayOrder = 1 },
            new ShopGalleryImage { Shop = sinhVien, Url = Photo("sv-g2", 700, 500), Caption = "Student rush hour", DisplayOrder = 2 },
            new ShopGalleryImage { Shop = colorZone, Url = Photo("cz-g1", 700, 500), Caption = "Poster wall", DisplayOrder = 1 },
            new ShopGalleryImage { Shop = colorZone, Url = Photo("cz-g2", 700, 500), Caption = "Colour calibration", DisplayOrder = 2 },
            new ShopGalleryImage { Shop = binderPro, Url = Photo("bp-g1", 700, 500), Caption = "Hardcover binding", DisplayOrder = 1 },
            new ShopGalleryImage { Shop = binderPro, Url = Photo("bp-g2", 700, 500), Caption = "Finishing bench", DisplayOrder = 2 },
            new ShopGalleryImage { Shop = rainbowPrints, Url = Photo("rp-g1", 700, 500), Caption = "3D print farm", DisplayOrder = 1 },
            new ShopGalleryImage { Shop = rainbowPrints, Url = Photo("rp-g2", 700, 500), Caption = "Laser cutting bay", DisplayOrder = 2 });

        db.ShopStaff.AddRange(
            new ShopStaff { Shop = quickPrint, User = staffQuick, Position = "Counter", JoinedAt = DateTime.UtcNow.AddMonths(-3), IsActive = true },
            new ShopStaff { Shop = campusCopy, User = staffCampus, Position = "Counter", JoinedAt = DateTime.UtcNow.AddMonths(-2), IsActive = true },
            new ShopStaff { Shop = sinhVien, User = staffSinhVien, Position = "Counter", JoinedAt = DateTime.UtcNow.AddMonths(-2), IsActive = true },
            new ShopStaff { Shop = colorZone, User = staffColorZone, Position = "Print operator", JoinedAt = DateTime.UtcNow.AddMonths(-1), IsActive = true },
            new ShopStaff { Shop = binderPro, User = staffBinderPro, Position = "Finishing", JoinedAt = DateTime.UtcNow.AddMonths(-1), IsActive = true });

        ShopService Rate(Shop shop, ServiceType type, decimal price, int leadMinutes, decimal setup = 0, int minQty = 1) => new()
        {
            Shop = shop, ServiceType = type, UnitPrice = price, SetupFee = setup,
            MinQuantity = minQty, LeadTimeMinutes = leadMinutes, IsActive = true
        };

        // QuickPrint: documents + basic finishing
        var qpBw = Rate(quickPrint, docBwA4, 800, 1);
        var qpColor = Rate(quickPrint, docColorA4, 3000, 1);
        var qpCopy = Rate(quickPrint, photocopy, 700, 1);
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
        var ccCard = Rate(campusCopy, nameCard, 600, 30, setup: 20000, minQty: 100);
        db.ShopServices.AddRange(ccColor, ccA3, ccPlot, ccThermal, ccCard);
        db.PriceRules.Add(new PriceRule { ShopService = ccColor, RuleType = PriceRuleType.PaperType, OptionKey = "A3", Multiplier = 2.0m });

        // MakerLab: fabrication
        var mlPrint3d = Rate(makerLab, print3d, 1500, 2, setup: 20000);
        var mlLaser = Rate(makerLab, laserCut, 3500, 3, setup: 30000);
        db.ShopServices.AddRange(mlPrint3d, mlLaser);
        db.PriceRules.AddRange(
            new PriceRule { ShopService = mlPrint3d, RuleType = PriceRuleType.Material, OptionKey = "PETG", Multiplier = 1.3m },
            new PriceRule { ShopService = mlPrint3d, RuleType = PriceRuleType.QualityProfile, OptionKey = "Fine", Multiplier = 1.5m });

        // Print Corner: cheap everyday documents
        var pcBw = Rate(printCorner, docBwA4, 750, 1);
        var pcColor = Rate(printCorner, docColorA4, 2800, 1);
        var pcCopy = Rate(printCorner, photocopy, 650, 1);
        var pcSpiral = Rate(printCorner, bindSpiral, 12000, 5);
        db.ShopServices.AddRange(pcBw, pcColor, pcCopy, pcSpiral);
        db.PriceRules.Add(new PriceRule { ShopService = pcBw, RuleType = PriceRuleType.Sides, OptionKey = "Duplex", Multiplier = 1.7m });

        // Sinh Vien Print: student-budget pricing
        var svBw = Rate(sinhVien, docBwA4, 600, 1);
        var svCopy = Rate(sinhVien, photocopy, 550, 1);
        var svSpiral = Rate(sinhVien, bindSpiral, 10000, 5);
        var svLaminate = Rate(sinhVien, laminate, 4000, 3);
        db.ShopServices.AddRange(svBw, svCopy, svSpiral, svLaminate);
        db.PriceRules.Add(new PriceRule { ShopService = svBw, RuleType = PriceRuleType.QuantityTier, OptionKey = "bulk", Multiplier = 0.85m, MinQuantity = 100 });

        // ColorZone: colour + large format
        var czColor = Rate(colorZone, docColorA4, 2200, 1);
        var czPoster = Rate(colorZone, posterA2, 18000, 8);
        var czDecal = Rate(colorZone, decal, 8000, 10);
        var czA3 = Rate(colorZone, docBwA3, 1400, 2);
        db.ShopServices.AddRange(czColor, czPoster, czDecal, czA3);
        db.PriceRules.Add(new PriceRule { ShopService = czColor, RuleType = PriceRuleType.Sides, OptionKey = "Duplex", Multiplier = 1.8m });

        // BinderPro: finishing specialist
        var bpSpiral = Rate(binderPro, bindSpiral, 14000, 5);
        var bpThermal = Rate(binderPro, bindThermal, 18000, 8);
        var bpLaminate = Rate(binderPro, laminate, 4500, 3);
        var bpHardcover = Rate(binderPro, bindHardcover, 45000, 20);
        var bpCard = Rate(binderPro, nameCard, 550, 25, setup: 15000, minQty: 100);
        db.ShopServices.AddRange(bpSpiral, bpThermal, bpLaminate, bpHardcover, bpCard);

        // Rainbow Prints (suspended, kept its rate card from before suspension)
        var rpBw = Rate(rainbowPrints, docBwA4, 900, 1);
        var rpPrint3d = Rate(rainbowPrints, print3d, 1600, 2, setup: 25000);
        var rpLaser = Rate(rainbowPrints, laserCut, 3200, 3, setup: 28000);
        db.ShopServices.AddRange(rpBw, rpPrint3d, rpLaser);

        db.Machines.AddRange(
            new Machine { Shop = quickPrint, Name = "Ricoh MP-1", MachineType = MachineType.Printer, ServiceGroup = ServiceGroup.Document, Status = MachineStatus.Idle, PhotoUrl = Photo("machine-ricoh", 600, 400) },
            new Machine { Shop = quickPrint, Name = "Binder-1", MachineType = MachineType.Finishing, ServiceGroup = ServiceGroup.Finishing, Status = MachineStatus.Idle, PhotoUrl = Photo("machine-binder", 600, 400) },
            new Machine { Shop = campusCopy, Name = "Xerox-Color-1", MachineType = MachineType.Printer, ServiceGroup = ServiceGroup.Document, Status = MachineStatus.Idle, PhotoUrl = Photo("machine-xerox", 600, 400) },
            new Machine { Shop = campusCopy, Name = "Plotter-A1", MachineType = MachineType.Plotter, ServiceGroup = ServiceGroup.Document, Status = MachineStatus.Idle, PhotoUrl = Photo("machine-plotter", 600, 400) },
            new Machine { Shop = makerLab, Name = "Prusa-MK4", MachineType = MachineType.Printer3D, ServiceGroup = ServiceGroup.Fabrication, Status = MachineStatus.Idle, PhotoUrl = Photo("machine-prusa", 600, 400) },
            new Machine { Shop = makerLab, Name = "Laser-60W", MachineType = MachineType.LaserCutter, ServiceGroup = ServiceGroup.Fabrication, Status = MachineStatus.Maintenance, PhotoUrl = Photo("machine-laser", 600, 400) },
            new Machine { Shop = printCorner, Name = "Ricoh MP-2", MachineType = MachineType.Printer, ServiceGroup = ServiceGroup.Document, Status = MachineStatus.Idle, PhotoUrl = Photo("machine-pc1", 600, 400) },
            new Machine { Shop = sinhVien, Name = "Canon-SV1", MachineType = MachineType.Printer, ServiceGroup = ServiceGroup.Document, Status = MachineStatus.Idle, PhotoUrl = Photo("machine-sv1", 600, 400) },
            new Machine { Shop = sinhVien, Name = "Binder-SV1", MachineType = MachineType.Finishing, ServiceGroup = ServiceGroup.Finishing, Status = MachineStatus.Idle, PhotoUrl = Photo("machine-sv2", 600, 400) },
            new Machine { Shop = colorZone, Name = "Epson-Wide1", MachineType = MachineType.Printer, ServiceGroup = ServiceGroup.Document, Status = MachineStatus.Idle, PhotoUrl = Photo("machine-cz1", 600, 400) },
            new Machine { Shop = colorZone, Name = "Plotter-CZ1", MachineType = MachineType.Plotter, ServiceGroup = ServiceGroup.Document, Status = MachineStatus.Busy, PhotoUrl = Photo("machine-cz2", 600, 400) },
            new Machine { Shop = binderPro, Name = "Binder-Pro1", MachineType = MachineType.Finishing, ServiceGroup = ServiceGroup.Finishing, Status = MachineStatus.Idle, PhotoUrl = Photo("machine-bp1", 600, 400) },
            new Machine { Shop = binderPro, Name = "Laminator-Pro1", MachineType = MachineType.Finishing, ServiceGroup = ServiceGroup.Finishing, Status = MachineStatus.Busy, PhotoUrl = Photo("machine-bp2", 600, 400) },
            new Machine { Shop = rainbowPrints, Name = "Ender-RP1", MachineType = MachineType.Printer3D, ServiceGroup = ServiceGroup.Fabrication, Status = MachineStatus.Idle, PhotoUrl = Photo("machine-rp1", 600, 400) },
            new Machine { Shop = rainbowPrints, Name = "Laser-RP1", MachineType = MachineType.LaserCutter, ServiceGroup = ServiceGroup.Fabrication, Status = MachineStatus.Offline, PhotoUrl = Photo("machine-rp2", 600, 400) });

        db.Materials.AddRange(
            new Material { Shop = quickPrint, Name = "A4 80gsm", MaterialType = MaterialType.Paper, Unit = "sheet", StockQuantity = 5000, LowStockThreshold = 500, UnitCost = 150, ImageUrl = Photo("mat-a4", 400, 400) },
            new Material { Shop = campusCopy, Name = "A4 Glossy 120gsm", MaterialType = MaterialType.Paper, Unit = "sheet", StockQuantity = 800, LowStockThreshold = 200, UnitCost = 500, ImageUrl = Photo("mat-glossy", 400, 400) },
            new Material { Shop = campusCopy, Name = "A1 Roll", MaterialType = MaterialType.Paper, Unit = "sheet", StockQuantity = 120, LowStockThreshold = 30, UnitCost = 8000, ImageUrl = Photo("mat-a1roll", 400, 400) },
            new Material { Shop = makerLab, Name = "PLA White", MaterialType = MaterialType.Filament, Unit = "gram", StockQuantity = 4000, LowStockThreshold = 500, UnitCost = 400, ImageUrl = Photo("mat-pla", 400, 400) },
            new Material { Shop = makerLab, Name = "PETG Black", MaterialType = MaterialType.Filament, Unit = "gram", StockQuantity = 300, LowStockThreshold = 400, UnitCost = 550, ImageUrl = Photo("mat-petg", 400, 400) },
            new Material { Shop = makerLab, Name = "Plywood 3mm", MaterialType = MaterialType.Sheet, Unit = "gram", StockQuantity = 6000, LowStockThreshold = 1000, UnitCost = 120, ImageUrl = Photo("mat-ply", 400, 400) },
            new Material { Shop = printCorner, Name = "A4 70gsm", MaterialType = MaterialType.Paper, Unit = "sheet", StockQuantity = 3000, LowStockThreshold = 400, UnitCost = 130, ImageUrl = Photo("mat-pc-a4", 400, 400) },
            new Material { Shop = sinhVien, Name = "A4 70gsm Bulk", MaterialType = MaterialType.Paper, Unit = "sheet", StockQuantity = 350, LowStockThreshold = 500, UnitCost = 120, ImageUrl = Photo("mat-sv-a4", 400, 400) },
            new Material { Shop = colorZone, Name = "A2 Photo Paper", MaterialType = MaterialType.Paper, Unit = "sheet", StockQuantity = 250, LowStockThreshold = 50, UnitCost = 6000, ImageUrl = Photo("mat-cz-a2", 400, 400) },
            new Material { Shop = colorZone, Name = "Vinyl Decal Roll", MaterialType = MaterialType.Sheet, Unit = "sheet", StockQuantity = 60, LowStockThreshold = 20, UnitCost = 15000, ImageUrl = Photo("mat-cz-decal", 400, 400) },
            new Material { Shop = binderPro, Name = "Spiral Coil Assorted", MaterialType = MaterialType.Consumable, Unit = "piece", StockQuantity = 900, LowStockThreshold = 150, UnitCost = 1200, ImageUrl = Photo("mat-bp-coil", 400, 400) },
            new Material { Shop = binderPro, Name = "Hardcover Shell", MaterialType = MaterialType.Consumable, Unit = "piece", StockQuantity = 80, LowStockThreshold = 20, UnitCost = 12000, ImageUrl = Photo("mat-bp-hc", 400, 400) },
            new Material { Shop = rainbowPrints, Name = "PLA Assorted", MaterialType = MaterialType.Filament, Unit = "gram", StockQuantity = 1200, LowStockThreshold = 300, UnitCost = 420, ImageUrl = Photo("mat-rp-pla", 400, 400) });

        await db.SaveChangesAsync();

        // ---- Vouchers ----
        db.Vouchers.AddRange(
            new Voucher { Code = "WELCOME10", Name = "Welcome discount", DiscountType = VoucherDiscountType.Percent, DiscountValue = 10, MinOrderAmount = 50_000, MaxDiscountAmount = 30_000, UsageLimit = 1000, UsedCount = 42, PerUserLimit = 1, ValidFrom = DateTime.UtcNow.AddMonths(-3), ValidTo = DateTime.UtcNow.AddMonths(3), IsActive = true },
            new Voucher { Code = "STUDENT20K", Name = "Student flat discount", DiscountType = VoucherDiscountType.FixedAmount, DiscountValue = 20_000, MinOrderAmount = 100_000, UsageLimit = 500, UsedCount = 18, PerUserLimit = 2, ValidFrom = DateTime.UtcNow.AddDays(-30), ValidTo = DateTime.UtcNow.AddMonths(2), IsActive = true },
            new Voucher { Code = "NEWUSER50K", Name = "New customer bonus", DiscountType = VoucherDiscountType.FixedAmount, DiscountValue = 50_000, MinOrderAmount = 150_000, UsageLimit = 300, UsedCount = 7, PerUserLimit = 1, ValidFrom = DateTime.UtcNow.AddMonths(-2), ValidTo = DateTime.UtcNow.AddMonths(4), IsActive = true },
            new Voucher { Code = "SUMMER5", Name = "Summer promo (ended)", DiscountType = VoucherDiscountType.Percent, DiscountValue = 5, MinOrderAmount = 30_000, MaxDiscountAmount = 15_000, UsageLimit = 200, UsedCount = 200, PerUserLimit = 1, ValidFrom = DateTime.UtcNow.AddMonths(-6), ValidTo = DateTime.UtcNow.AddMonths(-2), IsActive = false });
        await db.SaveChangesAsync();

        // ---- Customer document library ----
        var fileKinds = new (string Name, string ContentType, int Pages, int SizeKb)[]
        {
            ("BaoCao_DoAnTotNghiep.pdf", "application/pdf", 68, 3200),
            ("SlideThuyetTrinh.pdf", "application/pdf", 22, 1450),
            ("CV_UngTuyen.pdf", "application/pdf", 2, 180),
            ("BanVe_KyThuat.png", "image/png", 1, 2600)
        };
        var docsByCustomer = new Dictionary<int, List<DocumentFile>>();
        foreach (var c in customers)
        {
            var docs = new List<DocumentFile>();
            for (var i = 0; i < 2; i++)
            {
                var kind = fileKinds[(c.Id + i) % fileKinds.Length];
                docs.Add(new DocumentFile
                {
                    OwnerUserId = c.Id,
                    FileName = kind.Name,
                    StoragePath = $"{c.Id}/{Guid.NewGuid():N}_{kind.Name}",
                    ContentType = kind.ContentType,
                    FileSizeKb = kind.SizeKb,
                    DeclaredPageCount = kind.Pages,
                    RightsDeclared = true,
                    UploadedAt = DateTime.UtcNow.AddDays(-rng.Next(5, 90)),
                    Checksum = Guid.NewGuid().ToString("N"),
                    ThumbnailUrl = kind.ContentType.StartsWith("image") ? Photo($"doc-{c.Id}-{i}", 300, 300) : null
                });
            }
            docsByCustomer[c.Id] = docs;
            db.DocumentFiles.AddRange(docs);
        }
        await db.SaveChangesAsync();

        // ---- Orders across every shop, customer, and lifecycle status ----
        await SeedOrdersAsync(db, customers, docsByCustomer,
            quickPrint, campusCopy, makerLab, printCorner, sinhVien, colorZone, binderPro,
            docBwA4, docColorA4, docBwA3, photocopy, plotA1, posterA2,
            bindSpiral, bindThermal, bindHardcover, laminate, nameCard, decal, print3d, laserCut);
    }

    private static async Task SeedOrdersAsync(
        PrintHubDbContext db, User[] customers, Dictionary<int, List<DocumentFile>> docsByCustomer,
        Shop quickPrint, Shop campusCopy, Shop makerLab, Shop printCorner, Shop sinhVien, Shop colorZone, Shop binderPro,
        ServiceType docBwA4, ServiceType docColorA4, ServiceType docBwA3, ServiceType photocopy, ServiceType plotA1, ServiceType posterA2,
        ServiceType bindSpiral, ServiceType bindThermal, ServiceType bindHardcover, ServiceType laminate, ServiceType nameCard, ServiceType decal,
        ServiceType print3d, ServiceType laserCut)
    {
        var seq = 1;
        var balances = customers.ToDictionary(c => c.Id, c => c.WalletBalance);
        var orders = new List<Order>();
        var reviewsByShop = new Dictionary<int, List<Review>>();

        Order Place(User customer, Shop shop, ServiceType type, int quantity, int? pageCount, ColorMode? color, Sides? sides,
            decimal unitPrice, decimal lineTotal, OrderStatus status, int daysAgo, int hoursAgo = 0, DocumentFile? doc = null,
            string? materialName = null, string? qualityProfile = null, decimal? grams = null)
        {
            var placed = DateTime.UtcNow.AddDays(-daysAgo).AddHours(-hoursAgo);
            var order = new Order
            {
                OrderCode = $"PH-{placed:yyMMdd}-{seq++:D4}",
                Customer = customer, Shop = shop,
                Status = status, FulfilmentMethod = FulfilmentMethod.Pickup,
                SubTotal = lineTotal, TotalAmount = lineTotal,
                ProgressPercent = 0, PlacedAt = placed
            };

            order.Items.Add(new OrderItem
            {
                ServiceType = type, Quantity = quantity, PageCount = pageCount,
                ColorMode = color, Sides = sides, MaterialName = materialName, QualityProfile = qualityProfile,
                EstimatedGrams = grams, UnitPrice = unitPrice, LineTotal = lineTotal,
                EstimatedMinutes = 15 + quantity,
                DocumentFileId = doc?.Id, SnapshotFileName = doc?.FileName
            });

            balances[customer.Id] -= lineTotal;
            order.WalletTransactions.Add(new WalletTransaction
            {
                User = customer, Type = WalletTransactionType.Payment, Amount = -lineTotal,
                BalanceAfter = balances[customer.Id], RefCode = $"PAY-{order.OrderCode}",
                Status = WalletTransactionStatus.Completed, Description = $"Payment for order {order.OrderCode}", CreatedAt = placed
            });

            switch (status)
            {
                case OrderStatus.Accepted:
                    order.AcceptedAt = placed.AddMinutes(10);
                    order.EstimatedReadyAt = placed.AddHours(3);
                    break;
                case OrderStatus.InProduction:
                    order.AcceptedAt = placed.AddMinutes(10);
                    order.EstimatedReadyAt = placed.AddHours(2);
                    order.ProgressPercent = 40;
                    order.StatusHistory.Add(new OrderStatusHistory { FromStatus = OrderStatus.Accepted, ToStatus = OrderStatus.InProduction, ActorRole = UserRole.ShopOwner, Reason = "Production started.", CreatedAt = placed.AddMinutes(15) });
                    break;
                case OrderStatus.ReadyForPickup:
                    order.AcceptedAt = placed.AddMinutes(10);
                    order.EstimatedReadyAt = placed.AddHours(2);
                    order.ProgressPercent = 90;
                    order.StatusHistory.Add(new OrderStatusHistory { FromStatus = OrderStatus.InProduction, ToStatus = OrderStatus.ReadyForPickup, ActorUserId = null, ActorRole = null, Reason = "Production completed by agent.", CreatedAt = placed.AddHours(2) });
                    break;
                case OrderStatus.Completed:
                    order.AcceptedAt = placed.AddMinutes(10);
                    order.CompletedAt = placed.AddHours(4);
                    order.ProgressPercent = 100;
                    order.CommissionRate = 0.10m;
                    order.CommissionAmount = Math.Round(lineTotal * 0.10m, 2);
                    order.StatusHistory.Add(new OrderStatusHistory { FromStatus = OrderStatus.InProduction, ToStatus = OrderStatus.ReadyForPickup, ActorUserId = null, ActorRole = null, Reason = "Production completed by agent.", CreatedAt = placed.AddHours(2) });
                    order.StatusHistory.Add(new OrderStatusHistory { FromStatus = OrderStatus.ReadyForPickup, ToStatus = OrderStatus.Completed, ActorUserId = customer.Id, ActorRole = UserRole.Customer, Reason = "Picked up.", CreatedAt = order.CompletedAt.Value });
                    break;
                case OrderStatus.Declined:
                    order.DeclinedAt = placed.AddMinutes(20);
                    order.DeclineReason = (Domain.Enums.DeclineReason)(daysAgo % 3);
                    order.RefundedAmount = lineTotal;
                    balances[customer.Id] += lineTotal;
                    order.WalletTransactions.Add(new WalletTransaction { User = customer, Type = WalletTransactionType.Refund, Amount = lineTotal, BalanceAfter = balances[customer.Id], RefCode = $"REF-{order.OrderCode}", Status = WalletTransactionStatus.Completed, Description = "Refund: order declined by shop.", CreatedAt = order.DeclinedAt.Value });
                    order.StatusHistory.Add(new OrderStatusHistory { FromStatus = OrderStatus.AwaitingAcceptance, ToStatus = OrderStatus.Declined, ActorRole = UserRole.ShopOwner, Reason = order.DeclineReason.ToString(), CreatedAt = order.DeclinedAt.Value });
                    break;
                case OrderStatus.Cancelled:
                    order.CancelledAt = placed.AddMinutes(15);
                    order.CancellationReason = "Customer changed their mind.";
                    order.RefundedAmount = lineTotal;
                    balances[customer.Id] += lineTotal;
                    order.WalletTransactions.Add(new WalletTransaction { User = customer, Type = WalletTransactionType.Refund, Amount = lineTotal, BalanceAfter = balances[customer.Id], RefCode = $"REF-{order.OrderCode}", Status = WalletTransactionStatus.Completed, Description = "Refund: cancelled by customer.", CreatedAt = order.CancelledAt.Value });
                    order.StatusHistory.Add(new OrderStatusHistory { FromStatus = OrderStatus.AwaitingAcceptance, ToStatus = OrderStatus.Cancelled, ActorUserId = customer.Id, ActorRole = UserRole.Customer, Reason = order.CancellationReason, CreatedAt = order.CancelledAt.Value });
                    break;
            }

            orders.Add(order);
            return order;
        }

        void Review(Order order, int rating, string comment, bool withPhoto = false)
        {
            var review = new Review
            {
                Order = order, Customer = order.Customer, Shop = order.Shop, Rating = rating, Comment = comment,
                PhotoUrls = withPhoto ? $"https://picsum.photos/seed/rev-{order.OrderCode}/400/300" : null,
                CreatedAt = order.CompletedAt!.Value.AddDays(1)
            };
            db.Reviews.Add(review);
            if (!reviewsByShop.TryGetValue(order.Shop.Id, out var list))
                reviewsByShop[order.Shop.Id] = list = new List<Review>();
            list.Add(review);
        }

        var c = customers; // shorthand: c[0]=cust1 .. c[11]=cust12

        // QuickPrint
        var o = Place(c[0], quickPrint, docBwA4, 1, 150, ColorMode.BlackWhite, Sides.Duplex, 900, 135_000, OrderStatus.Completed, 60, doc: docsByCustomer[c[0].Id][0]);
        Review(o, 5, "Fast and cheap, right by the gate.", withPhoto: true);
        o = Place(c[0], quickPrint, docBwA4, 1, 120, ColorMode.BlackWhite, Sides.Simplex, 500, 60_000, OrderStatus.Completed, 45);
        Review(o, 4, "Good quality, slightly slow at lunch time.");
        o = Place(c[8], quickPrint, docColorA4, 1, 60, ColorMode.Color, Sides.Simplex, 3000, 180_000, OrderStatus.Completed, 33);
        Review(o, 5, "Colours came out vivid.");
        Place(c[0], quickPrint, photocopy, 1, 300, ColorMode.BlackWhite, Sides.Simplex, 700, 210_000, OrderStatus.Completed, 20);
        Place(c[8], quickPrint, bindSpiral, 3, null, null, null, 15000, 45_000, OrderStatus.InProduction, 0, hoursAgo: 3);
        Place(c[0], quickPrint, docBwA4, 1, 200, ColorMode.BlackWhite, Sides.Duplex, 900, 180_000, OrderStatus.AwaitingAcceptance, 0, hoursAgo: 1);
        Place(c[8], quickPrint, laminate, 5, null, null, null, 5000, 25_000, OrderStatus.Declined, 12);
        Place(c[0], quickPrint, docBwA4, 1, 80, ColorMode.BlackWhite, Sides.Simplex, 800, 64_000, OrderStatus.ReadyForPickup, 1, hoursAgo: 4);

        // Campus Copy
        o = Place(c[1], campusCopy, docColorA4, 40, 40, ColorMode.Color, Sides.Simplex, 2500, 100_000, OrderStatus.Completed, 55);
        Review(o, 4, "Good colour quality.");
        o = Place(c[9], campusCopy, docBwA3, 10, 10, ColorMode.BlackWhite, Sides.Simplex, 1500, 15_000, OrderStatus.Completed, 40);
        Review(o, 4, "A3 prints were sharp.");
        o = Place(c[1], campusCopy, plotA1, 2, 2, null, null, 25000, 50_000, OrderStatus.Completed, 25);
        Review(o, 5, "Plotting quality is excellent for thesis drawings.", withPhoto: true);
        Place(c[9], campusCopy, nameCard, 200, null, null, null, 600, 140_000, OrderStatus.Completed, 15);
        Place(c[1], campusCopy, bindThermal, 2, null, null, null, 20000, 40_000, OrderStatus.AwaitingAcceptance, 0, hoursAgo: 2);
        Place(c[9], campusCopy, docColorA4, 20, 20, ColorMode.Color, Sides.Simplex, 2500, 50_000, OrderStatus.Cancelled, 8);
        Place(c[1], campusCopy, docBwA3, 15, 15, ColorMode.BlackWhite, Sides.Simplex, 1500, 22_500, OrderStatus.Accepted, 0, hoursAgo: 5);

        // MakerLab
        o = Place(c[2], makerLab, print3d, 1, null, null, null, 2925, 292_500, OrderStatus.Completed, 50, materialName: "PETG", qualityProfile: "Fine", grams: 100);
        Review(o, 5, "The print came out with amazing detail.", withPhoto: true);
        o = Place(c[10], makerLab, laserCut, 1, null, null, null, 3500, 175_000, OrderStatus.Completed, 30, materialName: "Plywood", grams: 50);
        Review(o, 4, "Clean cuts, a bit pricier than expected.");
        Place(c[2], makerLab, print3d, 1, null, null, null, 1500, 150_000, OrderStatus.InProduction, 0, hoursAgo: 6, materialName: "PLA", grams: 100);
        Place(c[10], makerLab, laserCut, 1, null, null, null, 3500, 105_000, OrderStatus.AwaitingAcceptance, 0, hoursAgo: 1, materialName: "Acrylic", grams: 30);

        // Print Corner
        o = Place(c[3], printCorner, docBwA4, 1, 100, ColorMode.BlackWhite, Sides.Simplex, 750, 75_000, OrderStatus.Completed, 42);
        Review(o, 4, "Good value for everyday printing.");
        o = Place(c[5], printCorner, photocopy, 1, 50, ColorMode.BlackWhite, Sides.Simplex, 650, 32_500, OrderStatus.Completed, 22);
        Review(o, 3, "Decent, queue was a bit long.");
        Place(c[3], printCorner, docColorA4, 1, 30, ColorMode.Color, Sides.Simplex, 2800, 84_000, OrderStatus.ReadyForPickup, 0, hoursAgo: 3);
        Place(c[5], printCorner, bindSpiral, 2, null, null, null, 12000, 24_000, OrderStatus.AwaitingAcceptance, 0, hoursAgo: 1);
        Place(c[3], printCorner, docBwA4, 1, 250, ColorMode.BlackWhite, Sides.Duplex, 750, 180_000, OrderStatus.Declined, 6);

        // Sinh Vien Print
        o = Place(c[5], sinhVien, docBwA4, 1, 300, ColorMode.BlackWhite, Sides.Simplex, 600, 180_000, OrderStatus.Completed, 38);
        Review(o, 5, "Cheapest around campus, great for students.");
        o = Place(c[7], sinhVien, docBwA4, 1, 120, ColorMode.BlackWhite, Sides.Duplex, 600, 72_000, OrderStatus.Completed, 18);
        Review(o, 4, "Reliable, fast turnaround.");
        Place(c[5], sinhVien, bindSpiral, 4, null, null, null, 10000, 40_000, OrderStatus.Completed, 9);
        Place(c[7], sinhVien, laminate, 6, null, null, null, 4000, 24_000, OrderStatus.InProduction, 0, hoursAgo: 2);
        Place(c[5], sinhVien, photocopy, 1, 90, ColorMode.BlackWhite, Sides.Simplex, 550, 49_500, OrderStatus.AwaitingAcceptance, 0, hoursAgo: 1);

        // ColorZone Studio
        o = Place(c[4], colorZone, docColorA4, 1, 25, ColorMode.Color, Sides.Simplex, 2200, 55_000, OrderStatus.Completed, 28);
        Review(o, 5, "Best colour accuracy in the area.", withPhoto: true);
        o = Place(c[6], colorZone, posterA2, 3, 3, null, null, 18000, 54_000, OrderStatus.Completed, 14);
        Review(o, 4, "Posters looked great for the exhibition.");
        Place(c[4], colorZone, decal, 10, null, null, null, 8000, 80_000, OrderStatus.ReadyForPickup, 0, hoursAgo: 5);
        Place(c[6], colorZone, docBwA3, 8, 8, ColorMode.BlackWhite, Sides.Simplex, 1400, 11_200, OrderStatus.AwaitingAcceptance, 0, hoursAgo: 1);
        Place(c[4], colorZone, posterA2, 2, 2, null, null, 18000, 36_000, OrderStatus.Cancelled, 4);

        // BinderPro
        o = Place(c[6], binderPro, bindHardcover, 1, null, null, null, 45000, 45_000, OrderStatus.Completed, 20);
        Review(o, 5, "Beautiful hardcover finish for my thesis.");
        o = Place(c[11], binderPro, bindThermal, 3, null, null, null, 18000, 54_000, OrderStatus.Completed, 10);
        Review(o, 4, "Thermal binding held up well.");
        Place(c[6], binderPro, laminate, 8, null, null, null, 4500, 36_000, OrderStatus.InProduction, 0, hoursAgo: 4);
        Place(c[11], binderPro, nameCard, 150, null, null, null, 550, 82_500, OrderStatus.AwaitingAcceptance, 0, hoursAgo: 2);
        Place(c[6], binderPro, bindSpiral, 5, null, null, null, 14000, 70_000, OrderStatus.Cancelled, 3);

        db.Orders.AddRange(orders);
        await db.SaveChangesAsync();

        foreach (var cust in customers)
            cust.WalletBalance = balances[cust.Id];

        foreach (var (shopId, list) in reviewsByShop)
        {
            var ratedShop = orders.First(o => o.Shop.Id == shopId).Shop;
            ratedShop.RatingCount = list.Count;
            ratedShop.RatingAverage = Math.Round(list.Average(r => r.Rating), 2);
        }

        await db.SaveChangesAsync();
    }
}
