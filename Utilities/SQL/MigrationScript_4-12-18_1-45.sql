USE ABCMusicData;
GO

-- Create tables in order of reverse dependency
CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                   NVARCHAR (450)     NOT NULL,
    [AccessFailedCount]    INT                NOT NULL,
    [ConcurrencyStamp]     NVARCHAR (MAX)     NULL,
    [Email]                NVARCHAR (256)     NULL,
    [EmailConfirmed]       BIT                NOT NULL,
    [LockoutEnabled]       BIT                NOT NULL,
    [LockoutEnd]           DATETIMEOFFSET (7) NULL,
    [NormalizedEmail]      NVARCHAR (256)     NULL,
    [NormalizedUserName]   NVARCHAR (256)     NULL,
    [PasswordHash]         NVARCHAR (MAX)     NULL,
    [PhoneNumber]          NVARCHAR (MAX)     NULL,
    [PhoneNumberConfirmed] BIT                NOT NULL,
    [SecurityStamp]        NVARCHAR (MAX)     NULL,
    [TwoFactorEnabled]     BIT                NOT NULL,
    [UserName]             NVARCHAR (256)     NULL,
    [Age]                  TINYINT            DEFAULT ((0)) NOT NULL,
    [FirstName]            NVARCHAR (MAX)     NULL,
    [Gender]               NVARCHAR (MAX)     NULL,
    [LastName]             NVARCHAR (MAX)     NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [EmailIndex]
    ON [dbo].[AspNetUsers]([NormalizedEmail] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
    ON [dbo].[AspNetUsers]([NormalizedUserName] ASC) WHERE ([NormalizedUserName] IS NOT NULL);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_AspNetUsers_UserName]
    ON [dbo].[AspNetUsers]([UserName] ASC) WHERE ([UserName] IS NOT NULL);

CREATE TABLE [dbo].[Reviewable] (
    [Publisher]      NVARCHAR (MAX) NULL,
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [ArtistId]       NVARCHAR (450) NULL,
    [ArtistName]     NVARCHAR (MAX) NOT NULL,
    [Discriminator]  NVARCHAR (MAX) NOT NULL,
    [Name]           NVARCHAR (100) NOT NULL,
    [ReleaseDate]    DATE           NULL,
    [AlbumId]        INT            NULL,
    [Length]         TIME (7)       NULL,
    [Song_Publisher] NVARCHAR (MAX) NULL,
    [TrackNumber]    INT            NULL,
    CONSTRAINT [PK_Reviewable] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Reviewable_AspNetUsers_ArtistId] FOREIGN KEY ([ArtistId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_Reviewable_Reviewable_AlbumId] FOREIGN KEY ([AlbumId]) REFERENCES [dbo].[Reviewable] ([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_Reviewable_ArtistId]
    ON [dbo].[Reviewable]([ArtistId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Reviewable_AlbumId]
    ON [dbo].[Reviewable]([AlbumId] ASC);

CREATE TABLE [dbo].[ReviewableArtist] (
    [ReviewableId] INT            NOT NULL,
    [ArtistId]     NVARCHAR (450) NOT NULL,
    CONSTRAINT [PK_ReviewableArtist] PRIMARY KEY CLUSTERED ([ReviewableId] ASC, [ArtistId] ASC),
    CONSTRAINT [FK_ReviewableArtist_AspNetUsers_ArtistId] FOREIGN KEY ([ArtistId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_ReviewableArtist_Reviewable_ReviewableId] FOREIGN KEY ([ReviewableId]) REFERENCES [dbo].[Reviewable] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ReviewableArtist_ArtistId]
    ON [dbo].[ReviewableArtist]([ArtistId] ASC);

CREATE TABLE [dbo].[Review] (
    [ReviewId]     INT            IDENTITY (1, 1) NOT NULL,
    [AuthorId]     NVARCHAR (450) NULL,
    [Content]      NVARCHAR (MAX) NULL,
    [Rating]       TINYINT        NOT NULL,
    [ReviewableId] INT            NOT NULL,
    [Subject]      NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Review] PRIMARY KEY CLUSTERED ([ReviewId] ASC),
    CONSTRAINT [FK_Review_AspNetUsers_AuthorId] FOREIGN KEY ([AuthorId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_Review_Reviewable_ReviewableId] FOREIGN KEY ([ReviewableId]) REFERENCES [dbo].[Reviewable] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Review_AuthorId]
    ON [dbo].[Review]([AuthorId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Review_ReviewableId]
    ON [dbo].[Review]([ReviewableId] ASC);

CREATE TABLE [dbo].[AspNetUserTokens] (
    [UserId]        NVARCHAR (450) NOT NULL,
    [LoginProvider] NVARCHAR (450) NOT NULL,
    [Name]          NVARCHAR (450) NOT NULL,
    [Value]         NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED ([UserId] ASC, [LoginProvider] ASC, [Name] ASC),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [dbo].[__EFMigrationsHistory] (
    [MigrationId]    NVARCHAR (150) NOT NULL,
    [ProductVersion] NVARCHAR (32)  NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED ([MigrationId] ASC)
);

CREATE TABLE [dbo].[AspNetRoles] (
    [Id]               NVARCHAR (450) NOT NULL,
    [ConcurrencyStamp] NVARCHAR (MAX) NULL,
    [Name]             NVARCHAR (256) NULL,
    [NormalizedName]   NVARCHAR (256) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex]
    ON [dbo].[AspNetRoles]([NormalizedName] ASC) WHERE ([NormalizedName] IS NOT NULL);


CREATE TABLE [dbo].[AspNetRoleClaims] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [ClaimType]  NVARCHAR (MAX) NULL,
    [ClaimValue] NVARCHAR (MAX) NULL,
    [RoleId]     NVARCHAR (450) NOT NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE
);

GO
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId]
    ON [dbo].[AspNetRoleClaims]([RoleId] ASC);

CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [ClaimType]  NVARCHAR (MAX) NULL,
    [ClaimValue] NVARCHAR (MAX) NULL,
    [UserId]     NVARCHAR (450) NOT NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId]
    ON [dbo].[AspNetUserClaims]([UserId] ASC);

CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider]       NVARCHAR (450) NOT NULL,
    [ProviderKey]         NVARCHAR (450) NOT NULL,
    [ProviderDisplayName] NVARCHAR (MAX) NULL,
    [UserId]              NVARCHAR (450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED ([LoginProvider] ASC, [ProviderKey] ASC),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId]
    ON [dbo].[AspNetUserLogins]([UserId] ASC);

CREATE TABLE [dbo].[AspNetUserRoles] (
    [UserId] NVARCHAR (450) NOT NULL,
    [RoleId] NVARCHAR (450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId]
    ON [dbo].[AspNetUserRoles]([RoleId] ASC);