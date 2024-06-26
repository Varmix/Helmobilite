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

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [UserType] int NOT NULL,
    [PicturePath] nvarchar(500) NULL,
    [IsBadPayer] bit NULL,
    [LastName] nvarchar(100) NULL,
    [FirstName] nvarchar(100) NULL,
    [Matricule] nvarchar(max) NULL,
    [Permis] int NULL,
    [BirthDate] datetime2 NULL,
    [StudyLevel] int NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [CompanyAddresses] (
    [IdCompanyAdress] int NOT NULL IDENTITY,
    [Street] nvarchar(50) NOT NULL,
    [Number] nvarchar(5) NOT NULL,
    [Locality] nvarchar(50) NOT NULL,
    [PostalCode] int NOT NULL,
    [Coutry] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_CompanyAddresses] PRIMARY KEY ([IdCompanyAdress])
);
GO

CREATE TABLE [Truck] (
    [IdTruck] int NOT NULL IDENTITY,
    [Brand] nvarchar(25) NOT NULL,
    [Model] nvarchar(25) NOT NULL,
    [NumberPlate] nvarchar(11) NOT NULL,
    [RequiredDrivingLiscence] int NOT NULL,
    [MaximumTonnage] int NOT NULL,
    [PictureTruckPath] nvarchar(500) NULL,
    CONSTRAINT [PK_Truck] PRIMARY KEY ([IdTruck])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Companies] (
    [IdCompany] int NOT NULL IDENTITY,
    [NumberCompany] nvarchar(20) NOT NULL,
    [CompanyName] nvarchar(50) NOT NULL,
    [CompanyAdressIdCompanyAdress] int NOT NULL,
    [CompanyOfTheClientId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Companies] PRIMARY KEY ([IdCompany]),
    CONSTRAINT [FK_Companies_AspNetUsers_CompanyOfTheClientId] FOREIGN KEY ([CompanyOfTheClientId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Companies_CompanyAddresses_CompanyAdressIdCompanyAdress] FOREIGN KEY ([CompanyAdressIdCompanyAdress]) REFERENCES [CompanyAddresses] ([IdCompanyAdress]) ON DELETE CASCADE
);
GO

CREATE TABLE [Delivery] (
    [IdDelivery] int NOT NULL IDENTITY,
    [PlaceLoadingDelivery] nvarchar(75) NOT NULL,
    [PlaceUnLoadingDeliver] nvarchar(75) NOT NULL,
    [Content] nvarchar(50) NOT NULL,
    [DateAndTimeOfLoading] datetime2 NOT NULL,
    [DateAndTimeOfUnLoading] datetime2 NOT NULL,
    [IsFinish] bit NOT NULL,
    [IsSucces] bit NOT NULL,
    [LinkedClientId] nvarchar(450) NULL,
    [LinkedTruckDriverId] nvarchar(450) NULL,
    [LinkedTruckIdTruck] int NULL,
    [Comment] nvarchar(500) NULL,
    CONSTRAINT [PK_Delivery] PRIMARY KEY ([IdDelivery]),
    CONSTRAINT [FK_Delivery_AspNetUsers_LinkedClientId] FOREIGN KEY ([LinkedClientId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_Delivery_AspNetUsers_LinkedTruckDriverId] FOREIGN KEY ([LinkedTruckDriverId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_Delivery_Truck_LinkedTruckIdTruck] FOREIGN KEY ([LinkedTruckIdTruck]) REFERENCES [Truck] ([IdTruck])
);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_Companies_CompanyAdressIdCompanyAdress] ON [Companies] ([CompanyAdressIdCompanyAdress]);
GO

CREATE UNIQUE INDEX [IX_Companies_CompanyOfTheClientId] ON [Companies] ([CompanyOfTheClientId]);
GO

CREATE INDEX [IX_Delivery_LinkedClientId] ON [Delivery] ([LinkedClientId]);
GO

CREATE INDEX [IX_Delivery_LinkedTruckDriverId] ON [Delivery] ([LinkedTruckDriverId]);
GO

CREATE INDEX [IX_Delivery_LinkedTruckIdTruck] ON [Delivery] ([LinkedTruckIdTruck]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230518122616_InitDatabase', N'6.0.16');
GO

COMMIT;
GO

