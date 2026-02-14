namespace W2K.Identity.Persistence;

public static class PersistenceConstants
{
    public const string DefaultTableSchema = "w2k";

    public const string DefaultIdColumn = "Id";

    public const string DefaultSqlDateValue = "GETUTCDATE()";

    public const string PermissionsTableName = "Permissions";

    public const string RolesTableName = "Roles";

    public const string RolesPermissionsEntityName = "RolePermission";

    public const string RolesPermissionsTableName = "RolesPermissions";

    public const string RoleId = "RoleId";

    public const string PermissionId = "PermissionId";

    public const string UsersTableName = "Users";

    public const string SessionLogsTableName = "SessionLogs";

    public const string OfficesTableName = "Offices";

    public const string OfficeIdColumn = "OfficeId";

    public const string OfficeAddressesTableName = "OfficeAddresses";

    public const string OfficeUsersTableName = "OfficeUsers";

    public const string OfficeOwnersTableName = "OfficeOwners";

    public const string OfficeOwnerIdColumn = "OfficeOwnerId";

    public const string OfficeOwnerAddressTableName = "OfficeOwnerAddress";

    public const string OfficeBankAccountsTableName = "OfficeBankAccounts";

    public const string OfficeGroupsTableName = "OfficeGroups";
}
