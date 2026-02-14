namespace W2K.Config.Persistence;

public static class PersistenceConstants
{
    public const string DefaultTableSchema = "config";

    public const string DefaultIdColumn = "Id";

    public const string DefaultSqlDateValue = "GETUTCDATE()";

    public const string ConfigsTableName = "Configs";

    public const string OfficeConfigFieldsTableName = "OfficeConfigFields";

    public const string OfficeConfigFieldValuesTableName = "OfficeConfigFieldValues";

    public const string OfficeConfigFieldValuesJSONColumnName = "FieldValues";
}
