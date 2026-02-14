using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace W2K.Identity.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "id");

            migrationBuilder.CreateTable(
                name: "Offices",
                schema: "id",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Tenant = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(108)", maxLength: 108, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Contact Info\",\"Rank\":\"NONE\"}"),
                    Fax = table.Column<string>(type: "nvarchar(108)", maxLength: 108, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Contact Info\",\"Rank\":\"NONE\"}"),
                    Website = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LicenseNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Other\",\"Rank\":\"NONE\"}"),
                    LicenseState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OriginalLicenseDate = table.Column<DateOnly>(type: "date", nullable: true),
                    YearsCurrentOwnership = table.Column<short>(type: "smallint", nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(108)", maxLength: 108, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential\",\"InformationType\":\"National ID\",\"Rank\":\"CRITICAL\"}"),
                    BusinessType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AnnualRevenue = table.Column<int>(type: "int", nullable: true),
                    PromoCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    HowHeard = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsEnrollmentCompleted = table.Column<bool>(type: "bit", nullable: false),
                    IsReviewed = table.Column<bool>(type: "bit", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    UtmSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UtmMedium = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UtmCampaign = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastLoginDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    CreateUserId = table.Column<int>(type: "int", nullable: true),
                    CreateUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    ModifyDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ModifyUserId = table.Column<int>(type: "int", nullable: true),
                    ModifyUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifySource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offices", x => x.Id);
                })
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Offices")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateTable(
                name: "Permissions",
                schema: "id",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    CreateUserId = table.Column<int>(type: "int", nullable: true),
                    CreateUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    ModifyDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ModifyUserId = table.Column<int>(type: "int", nullable: true),
                    ModifyUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifySource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                })
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Permissions")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "id",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    OfficeType = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    OfficeId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    CreateUserId = table.Column<int>(type: "int", nullable: true),
                    CreateUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    ModifyDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ModifyUserId = table.Column<int>(type: "int", nullable: true),
                    ModifyUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifySource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                })
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Roles")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateTable(
                name: "SessionLogs",
                schema: "id",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(364)", maxLength: 364, nullable: true),
                    Fingerprint = table.Column<string>(type: "nvarchar(max)", maxLength: 5376, nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldSessionId = table.Column<string>(type: "nvarchar(364)", maxLength: 364, nullable: true),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    CreateUserId = table.Column<int>(type: "int", nullable: true),
                    CreateUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    ModifyDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ModifyUserId = table.Column<int>(type: "int", nullable: true),
                    ModifyUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifySource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "id",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential\",\"InformationType\":\"Name\",\"Rank\":\"MEDIUM\"}"),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential\",\"InformationType\":\"Name\",\"Rank\":\"MEDIUM\"}"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                        .Annotation("DynamicDataMask", "default()")
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential\",\"InformationType\":\"Contact Info\",\"Rank\":\"MEDIUM\"}"),
                    MobilePhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("DynamicDataMask", "default()")
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential\",\"InformationType\":\"Contact Info\",\"Rank\":\"MEDIUM\"}"),
                    LastLoginDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLogoutDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    CreateUserId = table.Column<int>(type: "int", nullable: true),
                    CreateUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    ModifyDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ModifyUserId = table.Column<int>(type: "int", nullable: true),
                    ModifyUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifySource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Users")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateTable(
                name: "OfficeAddresses",
                schema: "id",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfficeId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Address1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Contact Info\",\"Rank\":\"NONE\"}"),
                    Address2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Contact Info\",\"Rank\":\"NONE\"}"),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Contact Info\",\"Rank\":\"NONE\"}"),
                    County = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Contact Info\",\"Rank\":\"NONE\"}"),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Contact Info\",\"Rank\":\"NONE\"}"),
                    ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Contact Info\",\"Rank\":\"NONE\"}"),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Contact Info\",\"Rank\":\"NONE\"}"),
                    TimeZone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GooglePlaceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficeAddresses_Offices_OfficeId",
                        column: x => x.OfficeId,
                        principalSchema: "id",
                        principalTable: "Offices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_OfficeAddresses")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateTable(
                name: "OfficeBankAccounts",
                schema: "id",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfficeId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Highly Confidential - GDPR\",\"InformationType\":\"Banking\",\"Rank\":\"HIGH\"}"),
                    RoutingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Highly Confidential - GDPR\",\"InformationType\":\"Banking\",\"Rank\":\"CRITICAL\"}"),
                    AccountName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Highly Confidential - GDPR\",\"InformationType\":\"Banking\",\"Rank\":\"HIGH\"}"),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Highly Confidential - GDPR\",\"InformationType\":\"Banking\",\"Rank\":\"CRITICAL\"}"),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    CreateUserId = table.Column<int>(type: "int", nullable: true),
                    CreateUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    ModifyDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ModifyUserId = table.Column<int>(type: "int", nullable: true),
                    ModifyUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifySource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeBankAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficeBankAccounts_Offices_OfficeId",
                        column: x => x.OfficeId,
                        principalSchema: "id",
                        principalTable: "Offices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_OfficeBankAccounts")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateTable(
                name: "OfficeGroups",
                schema: "id",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfficeId = table.Column<int>(type: "int", nullable: false),
                    ParentOfficeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficeGroups_Offices_OfficeId",
                        column: x => x.OfficeId,
                        principalSchema: "id",
                        principalTable: "Offices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfficeOwners",
                schema: "id",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfficeId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Name\",\"Rank\":\"MEDIUM\"}"),
                    MiddleName = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Name\",\"Rank\":\"MEDIUM\"}"),
                    LastName = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Name\",\"Rank\":\"MEDIUM\"}"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Highly Confidential - GDPR\",\"InformationType\":\"Contact Info\",\"Rank\":\"HIGH\"}"),
                    MobilePhone = table.Column<string>(type: "nvarchar(108)", maxLength: 108, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Highly Confidential - GDPR\",\"InformationType\":\"Contact Info\",\"Rank\":\"HIGH\"}"),
                    DOB = table.Column<string>(type: "nvarchar(108)", maxLength: 108, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Highly Confidential - GDPR\",\"InformationType\":\"Date of Birth\",\"Rank\":\"CRITICAL\"}"),
                    SSN = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Highly Confidential - GDPR\",\"InformationType\":\"National ID\",\"Rank\":\"CRITICAL\"}"),
                    Ownership = table.Column<short>(type: "smallint", nullable: false),
                    AnnualIncome = table.Column<int>(type: "int", nullable: true),
                    NetWorth = table.Column<int>(type: "int", nullable: true),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    CreateUserId = table.Column<int>(type: "int", nullable: true),
                    CreateUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    ModifyDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ModifyUserId = table.Column<int>(type: "int", nullable: true),
                    ModifyUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifySource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeOwners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficeOwners_Offices_OfficeId",
                        column: x => x.OfficeId,
                        principalSchema: "id",
                        principalTable: "Offices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_OfficeOwners")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateTable(
                name: "RolesPermissions",
                schema: "id",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesPermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolesPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "id",
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolesPermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "id",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_RolesPermissions")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateTable(
                name: "OfficeUsers",
                schema: "id",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfficeId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsInvited = table.Column<bool>(type: "bit", nullable: false),
                    IsInviteProcessed = table.Column<bool>(type: "bit", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    IsManager = table.Column<bool>(type: "bit", nullable: false),
                    InviteAcceptedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    CreateUserId = table.Column<int>(type: "int", nullable: true),
                    CreateUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    ModifyDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ModifyUserId = table.Column<int>(type: "int", nullable: true),
                    ModifyUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifySource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficeUsers_Offices_OfficeId",
                        column: x => x.OfficeId,
                        principalSchema: "id",
                        principalTable: "Offices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfficeUsers_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "id",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OfficeUsers_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "id",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_OfficeUsers")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateTable(
                name: "OfficeOwnerAddress",
                schema: "id",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfficeOwnerId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Address1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Highly Confidential - GDPR\",\"InformationType\":\"Contact Info\",\"Rank\":\"HIGH\"}"),
                    Address2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Highly Confidential - GDPR\",\"InformationType\":\"Contact Info\",\"Rank\":\"HIGH\"}"),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Highly Confidential - GDPR\",\"InformationType\":\"Contact Info\",\"Rank\":\"HIGH\"}"),
                    County = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Highly Confidential - GDPR\",\"InformationType\":\"Contact Info\",\"Rank\":\"HIGH\"}"),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Highly Confidential - GDPR\",\"InformationType\":\"Contact Info\",\"Rank\":\"HIGH\"}"),
                    ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Highly Confidential - GDPR\",\"InformationType\":\"Contact Info\",\"Rank\":\"HIGH\"}"),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Highly Confidential - GDPR\",\"InformationType\":\"Contact Info\",\"Rank\":\"HIGH\"}"),
                    TimeZone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GooglePlaceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeOwnerAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficeOwnerAddress_OfficeOwners_OfficeOwnerId",
                        column: x => x.OfficeOwnerId,
                        principalSchema: "id",
                        principalTable: "OfficeOwners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_OfficeOwnerAddress")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeAddresses_OfficeId",
                schema: "id",
                table: "OfficeAddresses",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeBankAccounts_OfficeId",
                schema: "id",
                table: "OfficeBankAccounts",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeGroups_OfficeId",
                schema: "id",
                table: "OfficeGroups",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeOwnerAddress_OfficeOwnerId",
                schema: "id",
                table: "OfficeOwnerAddress",
                column: "OfficeOwnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OfficeOwners_OfficeId",
                schema: "id",
                table: "OfficeOwners",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeUsers_OfficeId",
                schema: "id",
                table: "OfficeUsers",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeUsers_RoleId",
                schema: "id",
                table: "OfficeUsers",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeUsers_UserId",
                schema: "id",
                table: "OfficeUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                schema: "id",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolesPermissions_PermissionId",
                schema: "id",
                table: "RolesPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "id",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProviderId",
                schema: "id",
                table: "Users",
                column: "ProviderId",
                unique: true,
                filter: "[ProviderId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfficeAddresses",
                schema: "id")
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_OfficeAddresses")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "OfficeBankAccounts",
                schema: "id")
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_OfficeBankAccounts")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "OfficeGroups",
                schema: "id");

            migrationBuilder.DropTable(
                name: "OfficeOwnerAddress",
                schema: "id")
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_OfficeOwnerAddress")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "OfficeUsers",
                schema: "id")
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_OfficeUsers")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "RolesPermissions",
                schema: "id")
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_RolesPermissions")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "SessionLogs",
                schema: "id");

            migrationBuilder.DropTable(
                name: "OfficeOwners",
                schema: "id")
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_OfficeOwners")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "id")
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Users")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "id")
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Permissions")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "id")
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Roles")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "Offices",
                schema: "id")
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Offices")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");
        }
    }
}
