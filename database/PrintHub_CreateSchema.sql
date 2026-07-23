-- =====================================================================
-- PrintHub - SQL Server schema (generated from EF Core code-first migrations)
-- Creates the PrintHubDb database and all 22 tables, keys, indexes and FKs.
-- Run in SSMS (open + Execute) or:  sqlcmd -S (localdb)\MSSQLLocalDB -i PrintHub_CreateSchema.sql
-- The app is code-first; this file is provided so the schema can be created/
-- inspected directly in SQL Server without running the application.
-- =====================================================================
IF DB_ID(N'PrintHubDb') IS NULL
    CREATE DATABASE [PrintHubDb];
GO
USE [PrintHubDb];
GO

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [ServiceTypes] (
        [Id] int NOT NULL IDENTITY,
        [Code] nvarchar(50) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [ServiceGroup] int NOT NULL,
        [PricingModel] int NOT NULL,
        [UnitOfMeasure] nvarchar(20) NOT NULL,
        [RequiresFile] bit NOT NULL,
        [Description] nvarchar(1000) NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_ServiceTypes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [FullName] nvarchar(100) NOT NULL,
        [Email] nvarchar(256) NOT NULL,
        [PhoneNumber] nvarchar(20) NULL,
        [PasswordHash] nvarchar(200) NOT NULL,
        [Role] int NOT NULL,
        [Status] int NOT NULL,
        [WalletBalance] decimal(18,2) NOT NULL,
        [DefaultAddress] nvarchar(300) NULL,
        [Latitude] float NULL,
        [Longitude] float NULL,
        [EmailVerifiedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [Vouchers] (
        [Id] int NOT NULL IDENTITY,
        [Code] nvarchar(50) NOT NULL,
        [DiscountType] int NOT NULL,
        [DiscountValue] decimal(18,2) NOT NULL,
        [MinOrderAmount] decimal(18,2) NOT NULL,
        [MaxDiscountAmount] decimal(18,2) NULL,
        [UsageLimit] int NOT NULL,
        [UsedCount] int NOT NULL,
        [ValidFrom] datetime2 NOT NULL,
        [ValidTo] datetime2 NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Vouchers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] int NOT NULL IDENTITY,
        [ActorUserId] int NULL,
        [Action] nvarchar(100) NOT NULL,
        [EntityName] nvarchar(100) NOT NULL,
        [EntityId] nvarchar(50) NULL,
        [OldValue] nvarchar(max) NULL,
        [NewValue] nvarchar(max) NULL,
        [IpAddress] nvarchar(50) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AuditLogs_Users_ActorUserId] FOREIGN KEY ([ActorUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [DocumentFiles] (
        [Id] int NOT NULL IDENTITY,
        [OwnerUserId] int NOT NULL,
        [FileName] nvarchar(260) NOT NULL,
        [StoragePath] nvarchar(500) NOT NULL,
        [ContentType] nvarchar(100) NOT NULL,
        [FileSizeKb] bigint NOT NULL,
        [DeclaredPageCount] int NULL,
        [RightsDeclared] bit NOT NULL,
        [UploadedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_DocumentFiles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DocumentFiles_Users_OwnerUserId] FOREIGN KEY ([OwnerUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [Notifications] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Content] nvarchar(1000) NOT NULL,
        [Type] int NOT NULL,
        [RelatedOrderId] int NULL,
        [IsRead] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [RefreshTokens] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [Token] nvarchar(500) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [RevokedAt] datetime2 NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [Shops] (
        [Id] int NOT NULL IDENTITY,
        [OwnerId] int NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [AddressLine] nvarchar(300) NOT NULL,
        [District] nvarchar(100) NOT NULL,
        [City] nvarchar(100) NOT NULL,
        [Latitude] float NULL,
        [Longitude] float NULL,
        [PhoneNumber] nvarchar(20) NULL,
        [OpenTime] time NOT NULL,
        [CloseTime] time NOT NULL,
        [Status] int NOT NULL,
        [ReviewNote] nvarchar(1000) NULL,
        [RatingAverage] float NOT NULL,
        [RatingCount] int NOT NULL,
        [ApprovedAt] datetime2 NULL,
        [ApprovedBy] int NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Shops] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Shops_Users_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [Favourites] (
        [Id] int NOT NULL IDENTITY,
        [CustomerId] int NOT NULL,
        [ShopId] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Favourites] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Favourites_Shops_ShopId] FOREIGN KEY ([ShopId]) REFERENCES [Shops] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Favourites_Users_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [Machines] (
        [Id] int NOT NULL IDENTITY,
        [ShopId] int NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [MachineType] int NOT NULL,
        [ServiceGroup] int NOT NULL,
        [Status] int NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Machines] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Machines_Shops_ShopId] FOREIGN KEY ([ShopId]) REFERENCES [Shops] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [Materials] (
        [Id] int NOT NULL IDENTITY,
        [ShopId] int NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [MaterialType] int NOT NULL,
        [Unit] nvarchar(20) NOT NULL,
        [StockQuantity] decimal(18,3) NOT NULL,
        [LowStockThreshold] decimal(18,3) NOT NULL,
        [UnitCost] decimal(18,2) NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Materials] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Materials_Shops_ShopId] FOREIGN KEY ([ShopId]) REFERENCES [Shops] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [Quotes] (
        [Id] int NOT NULL IDENTITY,
        [CustomerId] int NOT NULL,
        [ShopId] int NOT NULL,
        [SubTotal] decimal(18,2) NOT NULL,
        [EstimatedMinutes] int NOT NULL,
        [DistanceMeters] float NULL,
        [BreakdownJson] nvarchar(max) NULL,
        [IsIndicative] bit NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Quotes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Quotes_Shops_ShopId] FOREIGN KEY ([ShopId]) REFERENCES [Shops] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Quotes_Users_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [ShopServices] (
        [Id] int NOT NULL IDENTITY,
        [ShopId] int NOT NULL,
        [ServiceTypeId] int NOT NULL,
        [UnitPrice] decimal(18,2) NOT NULL,
        [SetupFee] decimal(18,2) NOT NULL,
        [MinQuantity] int NOT NULL,
        [LeadTimeMinutes] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_ShopServices] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ShopServices_ServiceTypes_ServiceTypeId] FOREIGN KEY ([ServiceTypeId]) REFERENCES [ServiceTypes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ShopServices_Shops_ShopId] FOREIGN KEY ([ShopId]) REFERENCES [Shops] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [ShopStaff] (
        [Id] int NOT NULL IDENTITY,
        [ShopId] int NOT NULL,
        [UserId] int NOT NULL,
        [Position] nvarchar(100) NULL,
        [JoinedAt] datetime2 NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_ShopStaff] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ShopStaff_Shops_ShopId] FOREIGN KEY ([ShopId]) REFERENCES [Shops] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ShopStaff_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [Orders] (
        [Id] int NOT NULL IDENTITY,
        [OrderCode] nvarchar(30) NOT NULL,
        [CustomerId] int NOT NULL,
        [ShopId] int NOT NULL,
        [QuoteId] int NULL,
        [MachineId] int NULL,
        [VoucherId] int NULL,
        [Status] int NOT NULL,
        [FulfilmentMethod] int NOT NULL,
        [PickupSlotStart] datetime2 NULL,
        [PickupSlotEnd] datetime2 NULL,
        [DeliveryAddress] nvarchar(300) NULL,
        [SubTotal] decimal(18,2) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [CommissionRate] decimal(18,4) NOT NULL,
        [CommissionAmount] decimal(18,2) NOT NULL,
        [ProgressPercent] int NOT NULL,
        [CustomerNote] nvarchar(1000) NULL,
        [DeclineReason] int NULL,
        [PlacedAt] datetime2 NULL,
        [AcceptedAt] datetime2 NULL,
        [CompletedAt] datetime2 NULL,
        [CancelledAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Orders_Machines_MachineId] FOREIGN KEY ([MachineId]) REFERENCES [Machines] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Orders_Quotes_QuoteId] FOREIGN KEY ([QuoteId]) REFERENCES [Quotes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Orders_Shops_ShopId] FOREIGN KEY ([ShopId]) REFERENCES [Shops] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Orders_Users_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Orders_Vouchers_VoucherId] FOREIGN KEY ([VoucherId]) REFERENCES [Vouchers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [PriceRules] (
        [Id] int NOT NULL IDENTITY,
        [ShopServiceId] int NOT NULL,
        [RuleType] int NOT NULL,
        [OptionKey] nvarchar(50) NOT NULL,
        [Multiplier] decimal(18,4) NOT NULL,
        [FlatExtra] decimal(18,2) NOT NULL,
        [MinQuantity] int NULL,
        [MaxQuantity] int NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_PriceRules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PriceRules_ShopServices_ShopServiceId] FOREIGN KEY ([ShopServiceId]) REFERENCES [ShopServices] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [Complaints] (
        [Id] int NOT NULL IDENTITY,
        [OrderId] int NOT NULL,
        [CustomerId] int NOT NULL,
        [ShopId] int NOT NULL,
        [Reason] int NOT NULL,
        [Description] nvarchar(2000) NOT NULL,
        [Status] int NOT NULL,
        [ProposedResolution] int NULL,
        [ShopResponse] nvarchar(2000) NULL,
        [AdminRuling] nvarchar(2000) NULL,
        [RefundAmount] decimal(18,2) NULL,
        [ReplacementOrderId] int NULL,
        [ResolvedBy] int NULL,
        [RespondedAt] datetime2 NULL,
        [ResolvedAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Complaints] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Complaints_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Complaints_Orders_ReplacementOrderId] FOREIGN KEY ([ReplacementOrderId]) REFERENCES [Orders] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Complaints_Shops_ShopId] FOREIGN KEY ([ShopId]) REFERENCES [Shops] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Complaints_Users_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [OrderItems] (
        [Id] int NOT NULL IDENTITY,
        [OrderId] int NOT NULL,
        [ServiceTypeId] int NOT NULL,
        [DocumentFileId] int NULL,
        [Quantity] int NOT NULL,
        [PageCount] int NULL,
        [PaperType] nvarchar(50) NULL,
        [ColorMode] int NULL,
        [Sides] int NULL,
        [BindingType] nvarchar(50) NULL,
        [MaterialName] nvarchar(100) NULL,
        [QualityProfile] nvarchar(50) NULL,
        [EstimatedGrams] decimal(18,3) NULL,
        [UnitPrice] decimal(18,2) NOT NULL,
        [LineTotal] decimal(18,2) NOT NULL,
        [ItemNote] nvarchar(500) NULL,
        CONSTRAINT [PK_OrderItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrderItems_DocumentFiles_DocumentFileId] FOREIGN KEY ([DocumentFileId]) REFERENCES [DocumentFiles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrderItems_ServiceTypes_ServiceTypeId] FOREIGN KEY ([ServiceTypeId]) REFERENCES [ServiceTypes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [OrderStatusHistories] (
        [Id] int NOT NULL IDENTITY,
        [OrderId] int NOT NULL,
        [FromStatus] int NULL,
        [ToStatus] int NOT NULL,
        [ActorUserId] int NULL,
        [ActorRole] int NULL,
        [Reason] nvarchar(1000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_OrderStatusHistories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrderStatusHistories_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrderStatusHistories_Users_ActorUserId] FOREIGN KEY ([ActorUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [Reviews] (
        [Id] int NOT NULL IDENTITY,
        [OrderId] int NOT NULL,
        [CustomerId] int NOT NULL,
        [ShopId] int NOT NULL,
        [Rating] int NOT NULL,
        [Comment] nvarchar(2000) NULL,
        [ShopReply] nvarchar(2000) NULL,
        [RepliedAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Reviews] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Reviews_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Reviews_Shops_ShopId] FOREIGN KEY ([ShopId]) REFERENCES [Shops] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Reviews_Users_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE TABLE [WalletTransactions] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [OrderId] int NULL,
        [Type] int NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [BalanceAfter] decimal(18,2) NOT NULL,
        [RefCode] nvarchar(50) NOT NULL,
        [Status] int NOT NULL,
        [ConfirmedBy] int NULL,
        [CreatedAt] datetime2 NOT NULL,
        [ConfirmedAt] datetime2 NULL,
        CONSTRAINT [PK_WalletTransactions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_WalletTransactions_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_WalletTransactions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_ActorUserId] ON [AuditLogs] ([ActorUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_CreatedAt] ON [AuditLogs] ([CreatedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Complaints_CustomerId] ON [Complaints] ([CustomerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Complaints_OrderId] ON [Complaints] ([OrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Complaints_ReplacementOrderId] ON [Complaints] ([ReplacementOrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Complaints_ShopId] ON [Complaints] ([ShopId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_DocumentFiles_OwnerUserId] ON [DocumentFiles] ([OwnerUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Favourites_CustomerId_ShopId] ON [Favourites] ([CustomerId], [ShopId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Favourites_ShopId] ON [Favourites] ([ShopId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Machines_ShopId] ON [Machines] ([ShopId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Materials_ShopId] ON [Materials] ([ShopId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Notifications_UserId_IsRead] ON [Notifications] ([UserId], [IsRead]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_OrderItems_DocumentFileId] ON [OrderItems] ([DocumentFileId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_OrderItems_OrderId] ON [OrderItems] ([OrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_OrderItems_ServiceTypeId] ON [OrderItems] ([ServiceTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Orders_CustomerId] ON [Orders] ([CustomerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Orders_MachineId] ON [Orders] ([MachineId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Orders_OrderCode] ON [Orders] ([OrderCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Orders_QuoteId] ON [Orders] ([QuoteId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Orders_ShopId] ON [Orders] ([ShopId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Orders_Status] ON [Orders] ([Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Orders_VoucherId] ON [Orders] ([VoucherId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_OrderStatusHistories_ActorUserId] ON [OrderStatusHistories] ([ActorUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_OrderStatusHistories_OrderId] ON [OrderStatusHistories] ([OrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_PriceRules_ShopServiceId] ON [PriceRules] ([ShopServiceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Quotes_CustomerId] ON [Quotes] ([CustomerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Quotes_ShopId] ON [Quotes] ([ShopId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RefreshTokens_Token] ON [RefreshTokens] ([Token]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Reviews_CustomerId] ON [Reviews] ([CustomerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Reviews_OrderId] ON [Reviews] ([OrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Reviews_ShopId] ON [Reviews] ([ShopId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ServiceTypes_Code] ON [ServiceTypes] ([Code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Shops_District] ON [Shops] ([District]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Shops_OwnerId] ON [Shops] ([OwnerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Shops_Status] ON [Shops] ([Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ShopServices_ServiceTypeId] ON [ShopServices] ([ServiceTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ShopServices_ShopId_ServiceTypeId] ON [ShopServices] ([ShopId], [ServiceTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ShopStaff_ShopId_UserId] ON [ShopStaff] ([ShopId], [UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ShopStaff_UserId] ON [ShopStaff] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Vouchers_Code] ON [Vouchers] ([Code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_WalletTransactions_OrderId] ON [WalletTransactions] ([OrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_WalletTransactions_RefCode] ON [WalletTransactions] ([RefCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_WalletTransactions_UserId] ON [WalletTransactions] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721070802_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260721070802_InitialCreate', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [WalletTransactions] ADD [BankReference] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [WalletTransactions] ADD [Description] nvarchar(300) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Vouchers] ADD [CreatedBy] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Vouchers] ADD [Description] nvarchar(300) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Vouchers] ADD [Name] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Vouchers] ADD [PerUserLimit] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Vouchers] ADD [UpdatedBy] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Users] ADD [AvatarUrl] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Users] ADD [CreatedBy] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Users] ADD [LastLoginAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Users] ADD [UpdatedBy] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [ShopStaff] ADD [InvitedByUserId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [ShopStaff] ADD [RevokedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [ShopServices] ADD [CreatedBy] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [ShopServices] ADD [MaxQuantity] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [ShopServices] ADD [Notes] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [ShopServices] ADD [UpdatedBy] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Shops] ADD [BusinessLicenseNo] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Shops] ADD [CoverImageUrl] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Shops] ADD [CreatedBy] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Shops] ADD [Email] nvarchar(256) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Shops] ADD [IsTemporarilyClosed] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Shops] ADD [LogoUrl] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Shops] ADD [TaxCode] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Shops] ADD [UpdatedBy] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [ServiceTypes] ADD [CreatedBy] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [ServiceTypes] ADD [DisplayOrder] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [ServiceTypes] ADD [IconUrl] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [ServiceTypes] ADD [UpdatedBy] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Reviews] ADD [CreatedBy] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Reviews] ADD [IsVisible] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Reviews] ADD [RepliedByUserId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Reviews] ADD [UpdatedBy] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [CreatedByIp] nvarchar(45) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [ReplacedByToken] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [RevokedByIp] nvarchar(45) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Quotes] ADD [TravelMinutes] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [PriceRules] ADD [Description] nvarchar(200) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [PriceRules] ADD [DisplayOrder] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [OrderStatusHistories] ADD [IpAddress] nvarchar(45) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Orders] ADD [CancellationReason] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Orders] ADD [CreatedBy] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Orders] ADD [DeclinedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Orders] ADD [EstimatedReadyAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Orders] ADD [RefundedAmount] decimal(18,2) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Orders] ADD [UpdatedBy] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [OrderItems] ADD [EstimatedMinutes] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [OrderItems] ADD [SnapshotFileName] nvarchar(260) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Notifications] ADD [LinkUrl] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Notifications] ADD [ReadAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Materials] ADD [Color] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Materials] ADD [CreatedBy] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Materials] ADD [ReorderQuantity] decimal(18,3) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Materials] ADD [Sku] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Materials] ADD [UpdatedBy] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Machines] ADD [CreatedBy] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Machines] ADD [LastMaintenanceAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Machines] ADD [Model] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Machines] ADD [SerialNumber] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Machines] ADD [UpdatedBy] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Favourites] ADD [Note] nvarchar(200) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [DocumentFiles] ADD [Checksum] nvarchar(128) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [DocumentFiles] ADD [LastAccessedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [DocumentFiles] ADD [PurgeAfter] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Complaints] ADD [AttachmentUrls] nvarchar(1000) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Complaints] ADD [ClosedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Complaints] ADD [CreatedBy] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Complaints] ADD [EscalatedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [Complaints] ADD [UpdatedBy] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    ALTER TABLE [AuditLogs] ADD [UserAgent] nvarchar(300) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721194518_EnrichDataModel'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260721194518_EnrichDataModel', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260723110754_AddPlatformSettings'
)
BEGIN
    CREATE TABLE [PlatformSettings] (
        [Id] int NOT NULL IDENTITY,
        [CommissionRate] decimal(5,4) NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_PlatformSettings] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260723110754_AddPlatformSettings'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260723110754_AddPlatformSettings', N'8.0.11');
END;
GO

COMMIT;
GO

