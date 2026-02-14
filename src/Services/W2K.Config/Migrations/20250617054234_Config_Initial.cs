using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace W2K.Config.Migrations
{
    /// <inheritdoc />
    public partial class Config_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dfi");

            migrationBuilder.CreateTable(
                name: "Configs",
                schema: "dfi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<short>(type: "smallint", nullable: false),
                    IsInternal = table.Column<bool>(type: "bit", nullable: false),
                    IsEncrypted = table.Column<bool>(type: "bit", nullable: false),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    CreateUserId = table.Column<int>(type: "int", nullable: true),
                    CreateUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true).Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    ModifyDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ModifyUserId = table.Column<int>(type: "int", nullable: true),
                    ModifyUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifySource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true).Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false).Annotation("SqlServer:TemporalIsPeriodEndColumn", true),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false).Annotation("SqlServer:TemporalIsPeriodStartColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.Id);
                })
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Configs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dfi")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateTable(
                name: "OfficeConfigFields",
                schema: "dfi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                                    .Annotation("SqlServer:Identity", "1, 1"),
                    OfficeType = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FieldType = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DefaultValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<short>(type: "smallint", nullable: false),
                    MinValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RegexValidator = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsInternal = table.Column<bool>(type: "bit", nullable: false),
                    IsEncrypted = table.Column<bool>(type: "bit", nullable: false),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    CreateUserId = table.Column<int>(type: "int", nullable: true),
                    CreateUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true).Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    ModifyDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ModifyUserId = table.Column<int>(type: "int", nullable: true),
                    ModifyUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifySource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true).Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false).Annotation("SqlServer:TemporalIsPeriodEndColumn", true),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false).Annotation("SqlServer:TemporalIsPeriodStartColumn", true),
                    FieldValues = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeConfigFields", x => x.Id);
                })
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_OfficeConfigFields")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dfi")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateTable(
                name: "OfficeConfigFieldValues",
                schema: "dfi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    OfficeId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    CreateUserId = table.Column<int>(type: "int", nullable: true),
                    CreateUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true).Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    ModifyDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ModifyUserId = table.Column<int>(type: "int", nullable: true),
                    ModifyUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifySource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true).Annotation("SensitivityClassification", "{\"Label\":\"Confidential - GDPR\",\"InformationType\":\"Networking\",\"Rank\":\"MEDIUM\"}"),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false).Annotation("SqlServer:TemporalIsPeriodEndColumn", true),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false).Annotation("SqlServer:TemporalIsPeriodStartColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeConfigFieldValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficeConfigFieldValues_OfficeConfigFields_FieldId",
                        column: x => x.FieldId,
                        principalSchema: "dfi",
                        principalTable: "OfficeConfigFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_OfficeConfigFieldValues")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dfi")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeConfigFieldValues_FieldId_OfficeId",
                schema: "dfi",
                table: "OfficeConfigFieldValues",
                columns: new[] { "FieldId", "OfficeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configs",
                schema: "dfi")
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Configs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dfi")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "OfficeConfigFieldValues",
                schema: "dfi")
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_OfficeConfigFieldValues")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dfi")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "OfficeConfigFields",
                schema: "dfi")
                .Annotation("HistoryRetention", 90)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_OfficeConfigFields")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dfi")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");
        }
    }
}
