using W2K.Common.Persistence.Configurations;
using W2K.Common.Persistence.Extensions;
using W2K.Common.Persistence.Security;
using W2K.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace W2K.Identity.Persistence.Configurations;

public class OfficeConfiguration : BaseEntityConfiguration<Office>
{
    public override void Configure(EntityTypeBuilder<Office> builder)
    {
        base.Configure(builder);
        ApplyOfficeConfigurations(builder);

        // Owned collections
        ApplyOfficeAddressConfigurations(builder);
        ApplyOfficeOwnerConfigurations(builder);
        ApplyOfficeBankAccountConfigurations(builder);
        ApplyOfficeGroupConfigurations(builder);

        // Has collections
        _ = builder.HasMany(x => x.Users)
            .WithOne(x => x.Office)
            .HasForeignKey(x => x.OfficeId);
    }

    private static void ApplyOfficeConfigurations(EntityTypeBuilder<Office> builder)
    {
        _ = builder.ToTableWithHistory(PersistenceConstants.OfficesTableName, PersistenceConstants.DefaultTableSchema);
        _ = builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(25)
            .IsRequired()
            .HasColumnOrder(2);
        _ = builder.Property(x => x.Tenant)
            .HasMaxLength(25)
            .HasColumnOrder(3);
        _ = builder.Property(x => x.Category)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnOrder(4);
        _ = builder.Property(x => x.Code)
            .HasMaxLength(25)
            .HasColumnOrder(5);
        _ = builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnOrder(6);
        _ = builder.Property(x => x.LegalName)
            .HasMaxLength(100)
            .HasColumnOrder(7);
        _ = builder.Property(x => x.Phone)
            .HasEncryption()
            .HasMaxLength(50)
            .HasColumnOrder(8)
            .HasSensitivityClassification(
                SensitivityLabels.Public,
                SensitivityInformationTypes.ContactInfo,
                SensitivityRank.NONE);
        _ = builder.Property(x => x.Fax!)
            .HasEncryption()
            .HasMaxLength(50)
            .HasColumnOrder(9)
            .HasSensitivityClassification(
                SensitivityLabels.Public,
                SensitivityInformationTypes.ContactInfo,
                SensitivityRank.NONE);
        _ = builder.Property(x => x.Website)
            .HasMaxLength(255)
            .HasColumnOrder(10);
        _ = builder.Property(x => x.LicenseNo)
            .HasMaxLength(50)
            .HasColumnOrder(11)
            .HasSensitivityClassification(
                SensitivityLabels.Public,
                SensitivityInformationTypes.Other,
                SensitivityRank.NONE);
        _ = builder.Property(x => x.LicenseState)
            .HasMaxLength(50)
            .HasColumnOrder(12);
        _ = builder.Property(x => x.OriginalLicenseDate)
            .HasColumnOrder(13);
        _ = builder.Property(x => x.YearsCurrentOwnership)
            .HasColumnOrder(14);
        _ = builder.Property(x => x.TaxId)
            .HasEncryption()
            .HasMaxLength(50)
            .HasColumnOrder(15)
            .HasSensitivityClassification(
                SensitivityLabels.Confidential,
                SensitivityInformationTypes.NationalId,
                SensitivityRank.CRITICAL);
        _ = builder.Property(x => x.BusinessType)
            .HasMaxLength(50)
            .HasColumnOrder(16);
        _ = builder.Property(x => x.AnnualRevenue)
            .HasColumnOrder(17);
        _ = builder.Property(x => x.PromoCode)
            .HasMaxLength(25)
            .HasColumnOrder(18);
        _ = builder.Property(x => x.HowHeard)
            .HasMaxLength(50)
            .HasColumnOrder(19);
        _ = builder.Property(x => x.IsEnrollmentCompleted)
            .HasColumnOrder(20);
        _ = builder.Property(x => x.IsReviewed)
            .HasColumnOrder(21);
        _ = builder.Property(x => x.IsApproved)
            .HasColumnOrder(22);
        _ = builder.Property(x => x.UtmSource)
            .HasMaxLength(50)
            .HasColumnOrder(23);
        _ = builder.Property(x => x.UtmMedium)
            .HasMaxLength(50)
            .HasColumnOrder(24);
        _ = builder.Property(x => x.UtmCampaign)
            .HasMaxLength(50)
            .HasColumnOrder(25);
        _ = builder.Property(x => x.LastLoginDateTimeUtc)
            .HasColumnOrder(26);
    }

    private static void ApplyOfficeAddressConfigurations(EntityTypeBuilder<Office> builder)
    {
        _ = builder.OwnsMany(
            x => x.Addresses,
            x =>
            {
                _ = x.ToTableWithHistory(PersistenceConstants.OfficeAddressesTableName, PersistenceConstants.DefaultTableSchema);
                _ = x.WithOwner().HasForeignKey(PersistenceConstants.OfficeIdColumn);
                _ = x.Property<int>(PersistenceConstants.DefaultIdColumn)
                    .ValueGeneratedOnAdd()
                    .HasColumnOrder(1);
                _ = x.HasKey(PersistenceConstants.DefaultIdColumn);
                _ = x.Property(PersistenceConstants.OfficeIdColumn)
                    .IsRequired()
                    .HasColumnOrder(2);
                _ = x.Property(a => a.Type)
                    .IsRequired()
                    .HasMaxLength(25)
                    .HasColumnOrder(3);
                _ = x.Property(a => a.Address1)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnOrder(4)
                    .HasSensitivityClassification(SensitivityLabels.Public, SensitivityInformationTypes.ContactInfo, SensitivityRank.NONE);
                _ = x.Property(a => a.Address2)
                    .HasMaxLength(100)
                    .HasColumnOrder(5)
                    .HasSensitivityClassification(SensitivityLabels.Public, SensitivityInformationTypes.ContactInfo, SensitivityRank.NONE);
                _ = x.Property(a => a.City)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnOrder(6)
                    .HasSensitivityClassification(SensitivityLabels.Public, SensitivityInformationTypes.ContactInfo, SensitivityRank.NONE);
                _ = x.Property(a => a.County)
                    .HasMaxLength(50)
                    .HasColumnOrder(7)
                    .HasSensitivityClassification(SensitivityLabels.Public, SensitivityInformationTypes.ContactInfo, SensitivityRank.NONE);
                _ = x.Property(a => a.State)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnOrder(8)
                    .HasSensitivityClassification(SensitivityLabels.Public, SensitivityInformationTypes.ContactInfo, SensitivityRank.NONE);
                _ = x.Property(a => a.ZipCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnOrder(9)
                    .HasSensitivityClassification(SensitivityLabels.Public, SensitivityInformationTypes.ContactInfo, SensitivityRank.NONE);
                _ = x.Property(a => a.Country)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnOrder(10)
                    .HasSensitivityClassification(SensitivityLabels.Public, SensitivityInformationTypes.ContactInfo, SensitivityRank.NONE);
                _ = x.Property(a => a.TimeZone)
                    .HasMaxLength(50)
                    .HasColumnOrder(11);
                _ = x.Property(a => a.GooglePlaceId)
                    .HasMaxLength(100)
                    .HasColumnOrder(12);
                _ = builder.Navigation(x => x.Addresses).AutoInclude(false);
            });
    }

    private static void ApplyOfficeOwnerConfigurations(EntityTypeBuilder<Office> builder)
    {
        _ = builder.OwnsMany(
            x => x.Owners,
            x =>
            {
                _ = x.ToTableWithHistory(PersistenceConstants.OfficeOwnersTableName, PersistenceConstants.DefaultTableSchema);
                _ = x.WithOwner().HasForeignKey(PersistenceConstants.OfficeIdColumn);
                _ = x.Property<int>(PersistenceConstants.DefaultIdColumn)
                    .ValueGeneratedOnAdd()
                    .HasColumnOrder(1);
                _ = x.HasKey(PersistenceConstants.DefaultIdColumn);
                _ = x.Property(PersistenceConstants.OfficeIdColumn)
                    .IsRequired()
                    .HasColumnOrder(2);
                _ = x.Property(x => x.FirstName)
                    .IsRequired()
                    .HasMaxLength(25)
                    .HasColumnOrder(3)
                    .HasSensitivityClassification(SensitivityLabels.ConfidentialGdpr, SensitivityInformationTypes.Name, SensitivityRank.MEDIUM);
                _ = x.Property(x => x.MiddleName)
                    .HasMaxLength(25)
                    .HasColumnOrder(4)
                    .HasSensitivityClassification(SensitivityLabels.ConfidentialGdpr, SensitivityInformationTypes.Name, SensitivityRank.MEDIUM);
                _ = x.Property(x => x.LastName)
                    .IsRequired()
                    .HasMaxLength(25)
                    .HasColumnOrder(5)
                    .HasSensitivityClassification(SensitivityLabels.ConfidentialGdpr, SensitivityInformationTypes.Name, SensitivityRank.MEDIUM);
                _ = x.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnOrder(6)
                    .HasSensitivityClassification(SensitivityLabels.HighlyConfidentialGdpr, SensitivityInformationTypes.ContactInfo, SensitivityRank.HIGH);
                _ = x.Property(x => x.MobilePhone!)
                    .HasEncryption()
                    .HasMaxLength(50)
                    .HasColumnOrder(7)
                    .HasSensitivityClassification(SensitivityLabels.HighlyConfidentialGdpr, SensitivityInformationTypes.ContactInfo, SensitivityRank.HIGH);
                _ = x.Property(x => x.DOB!)
                    .HasEncryption()
                    .HasMaxLength(50)
                    .HasColumnOrder(8)
                    .HasSensitivityClassification(SensitivityLabels.HighlyConfidentialGdpr, SensitivityInformationTypes.DateOfBirth, SensitivityRank.CRITICAL);
                _ = x.Property(x => x.SSN!)
                    .HasEncryption()
                    .HasMaxLength(20)
                    .HasColumnOrder(9)
                    .HasSensitivityClassification(SensitivityLabels.HighlyConfidentialGdpr, SensitivityInformationTypes.NationalId, SensitivityRank.CRITICAL);
                _ = x.Property(x => x.Ownership)
                    .HasColumnOrder(10);
                _ = x.Property(x => x.AnnualIncome)
                    .HasColumnOrder(11);
                _ = x.Property(x => x.NetWorth)
                    .HasColumnOrder(12);

                _ = x.Ignore(x => x.DOBDate);

                _ = x.OwnsOne(
                    x => x.Address,
                    x =>
                    {
                        _ = x.ToTableWithHistory(PersistenceConstants.OfficeOwnerAddressTableName, PersistenceConstants.DefaultTableSchema);
                        _ = x.WithOwner().HasForeignKey(PersistenceConstants.OfficeOwnerIdColumn);
                        _ = x.Property<int>(PersistenceConstants.DefaultIdColumn)
                            .ValueGeneratedOnAdd()
                            .HasColumnOrder(1);
                        _ = x.HasKey(PersistenceConstants.DefaultIdColumn);
                        _ = x.Property(PersistenceConstants.OfficeOwnerIdColumn)
                            .IsRequired()
                            .HasColumnOrder(2);
                        _ = x.Property(a => a.Type)
                            .IsRequired()
                            .HasMaxLength(25)
                            .HasColumnOrder(21);
                        _ = x.Property(a => a.Address1)
                            .IsRequired()
                            .HasMaxLength(100)
                            .HasColumnOrder(22)
                            .HasSensitivityClassification(SensitivityLabels.HighlyConfidentialGdpr, SensitivityInformationTypes.ContactInfo, SensitivityRank.HIGH);
                        _ = x.Property(a => a.Address2)
                            .HasMaxLength(100)
                            .HasColumnOrder(23)
                            .HasSensitivityClassification(SensitivityLabels.HighlyConfidentialGdpr, SensitivityInformationTypes.ContactInfo, SensitivityRank.HIGH);
                        _ = x.Property(a => a.City)
                            .IsRequired()
                            .HasMaxLength(50)
                            .HasColumnOrder(24)
                            .HasSensitivityClassification(SensitivityLabels.HighlyConfidentialGdpr, SensitivityInformationTypes.ContactInfo, SensitivityRank.HIGH);
                        _ = x.Property(a => a.County)
                            .HasMaxLength(50)
                            .HasColumnOrder(25)
                            .HasSensitivityClassification(SensitivityLabels.HighlyConfidentialGdpr, SensitivityInformationTypes.ContactInfo, SensitivityRank.HIGH);
                        _ = x.Property(a => a.State)
                            .IsRequired()
                            .HasMaxLength(50)
                            .HasColumnOrder(26)
                            .HasSensitivityClassification(SensitivityLabels.HighlyConfidentialGdpr, SensitivityInformationTypes.ContactInfo, SensitivityRank.HIGH);
                        _ = x.Property(a => a.ZipCode)
                            .IsRequired()
                            .HasMaxLength(20)
                            .HasColumnOrder(27)
                            .HasSensitivityClassification(SensitivityLabels.HighlyConfidentialGdpr, SensitivityInformationTypes.ContactInfo, SensitivityRank.HIGH);
                        _ = x.Property(a => a.Country)
                            .IsRequired()
                            .HasMaxLength(50)
                            .HasColumnOrder(28)
                            .HasSensitivityClassification(SensitivityLabels.HighlyConfidentialGdpr, SensitivityInformationTypes.ContactInfo, SensitivityRank.HIGH);
                        _ = x.Property(a => a.TimeZone)
                            .HasMaxLength(50)
                            .HasColumnOrder(29);
                        _ = x.Property(a => a.GooglePlaceId)
                            .HasMaxLength(100)
                            .HasColumnOrder(30);
                    });

                _ = x.Property(x => x.CreateDateTimeUtc)
                    .HasDefaultValueSql(PersistenceConstants.DefaultSqlDateValue)
                    .HasColumnOrder(91);
                _ = x.Property(x => x.CreateUserId)
                    .HasColumnOrder(92);
                _ = x.Property(x => x.CreateUserName)
                    .HasMaxLength(100)
                    .HasColumnOrder(93);
                _ = x.Property(x => x.CreateSource)
                    .HasMaxLength(50)
                    .HasColumnOrder(94)
                    .HasSensitivityClassification(SensitivityLabels.ConfidentialGdpr, SensitivityInformationTypes.Networking, SensitivityRank.MEDIUM);
                _ = x.Property(x => x.ModifyDateTimeUtc)
                    .HasDefaultValueSql(PersistenceConstants.DefaultSqlDateValue)
                    .HasColumnOrder(95);
                _ = x.Property(x => x.ModifyUserId)
                    .HasColumnOrder(96);
                _ = x.Property(x => x.ModifyUserName)
                    .HasMaxLength(100)
                    .HasColumnOrder(97);
                _ = x.Property(x => x.ModifySource)
                    .HasMaxLength(50)
                    .HasColumnOrder(98)
                    .HasSensitivityClassification(SensitivityLabels.ConfidentialGdpr, SensitivityInformationTypes.Networking, SensitivityRank.MEDIUM);
                _ = builder.Navigation(x => x.Owners).AutoInclude(false);
            });
    }

    private static void ApplyOfficeBankAccountConfigurations(EntityTypeBuilder<Office> builder)
    {
        _ = builder.OwnsMany(
            x => x.BankAccounts,
            x =>
            {
                _ = x.ToTableWithHistory(PersistenceConstants.OfficeBankAccountsTableName, PersistenceConstants.DefaultTableSchema);
                _ = x.WithOwner().HasForeignKey(PersistenceConstants.OfficeIdColumn);
                _ = x.Property<int>(PersistenceConstants.DefaultIdColumn)
                    .ValueGeneratedOnAdd()
                    .HasColumnOrder(1);
                _ = x.HasKey(PersistenceConstants.DefaultIdColumn);
                _ = x.Property(PersistenceConstants.OfficeIdColumn)
                    .IsRequired()
                    .HasColumnOrder(2);
                _ = x.Property(x => x.Type)
                    .HasConversion<string>()
                    .IsRequired()
                    .HasMaxLength(25)
                    .HasColumnOrder(3);
                _ = x.Property(x => x.BankName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnOrder(4)
                    .HasSensitivityClassification(SensitivityLabels.HighlyConfidentialGdpr, SensitivityInformationTypes.Banking, SensitivityRank.HIGH);
                _ = x.Property(x => x.RoutingNumber)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnOrder(5)
                    .HasSensitivityClassification(SensitivityLabels.HighlyConfidentialGdpr, SensitivityInformationTypes.Banking, SensitivityRank.CRITICAL);
                _ = x.Property(x => x.AccountName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnOrder(6)
                    .HasSensitivityClassification(SensitivityLabels.HighlyConfidentialGdpr, SensitivityInformationTypes.Banking, SensitivityRank.HIGH);
                _ = x.Property(x => x.AccountNumber)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnOrder(7)
                    .HasSensitivityClassification(SensitivityLabels.HighlyConfidentialGdpr, SensitivityInformationTypes.Banking, SensitivityRank.CRITICAL);
                _ = x.Property(x => x.IsPrimary)
                    .HasColumnOrder(8);

                _ = x.Property(x => x.CreateDateTimeUtc)
                    .HasDefaultValueSql(PersistenceConstants.DefaultSqlDateValue)
                    .HasColumnOrder(91);
                _ = x.Property(x => x.CreateUserId)
                    .HasColumnOrder(92);
                _ = x.Property(x => x.CreateUserName)
                    .HasMaxLength(100)
                    .HasColumnOrder(93);
                _ = x.Property(x => x.CreateSource)
                    .HasMaxLength(50)
                    .HasColumnOrder(94)
                    .HasSensitivityClassification(SensitivityLabels.ConfidentialGdpr, SensitivityInformationTypes.Networking, SensitivityRank.MEDIUM);
                _ = x.Property(x => x.ModifyDateTimeUtc)
                    .HasDefaultValueSql(PersistenceConstants.DefaultSqlDateValue)
                    .HasColumnOrder(95);
                _ = x.Property(x => x.ModifyUserId)
                    .HasColumnOrder(96);
                _ = x.Property(x => x.ModifyUserName)
                    .HasMaxLength(100)
                    .HasColumnOrder(97);
                _ = x.Property(x => x.ModifySource)
                    .HasMaxLength(50)
                    .HasColumnOrder(98)
                    .HasSensitivityClassification(SensitivityLabels.ConfidentialGdpr, SensitivityInformationTypes.Networking, SensitivityRank.MEDIUM);
                _ = builder.Navigation(x => x.BankAccounts).AutoInclude(false);
            });
    }

    private static void ApplyOfficeGroupConfigurations(EntityTypeBuilder<Office> builder)
    {
        _ = builder.OwnsMany(
            x => x.Groups,
            x =>
            {
                _ = x.ToTable(PersistenceConstants.OfficeGroupsTableName, PersistenceConstants.DefaultTableSchema);
                _ = x.WithOwner().HasForeignKey(PersistenceConstants.OfficeIdColumn);
                _ = x.Property<int>(PersistenceConstants.DefaultIdColumn)
                    .ValueGeneratedOnAdd()
                    .HasColumnOrder(1);
                _ = x.HasKey(PersistenceConstants.DefaultIdColumn);
                _ = x.Property(x => x.OfficeId)
                    .IsRequired()
                    .HasColumnOrder(2);
                _ = x.Property(e => e.ParentOfficeId)
                    .IsRequired()
                    .HasColumnOrder(3);
            });
        _ = builder.Navigation(x => x.Groups).AutoInclude(false);
    }

}
