// https://devblogs.microsoft.com/azure-sdk/azure-identity-with-sql-graph-ef/

using System.Data.Common;
using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Data;

namespace W2K.Common.Persistence.Context;

[ExcludeFromCodeCoverage(Justification = "Excluded from code coverage as we don't need coverage for this class.")]
public class AzureAdAuthDbConnectionInterceptor : DbConnectionInterceptor
{
    // See https://docs.microsoft.com/azure/active-directory/managed-identities-azure-resources/services-support-managed-identities#azure-sql
    private static readonly string[] _azureSqlScopes = ["https://database.windows.net//.default"];

    public override InterceptionResult ConnectionOpening(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result)
    {
        var sqlConnection = (SqlConnection)connection;
        if (DoesConnectionNeedAccessToken(sqlConnection))
        {
            var tokenRequestContext = new TokenRequestContext(_azureSqlScopes);
            var token = new DefaultAzureCredential().GetToken(tokenRequestContext);
            sqlConnection.AccessToken = token.Token;
        }
        return base.ConnectionOpening(connection, eventData, result);
    }

    public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result,
        CancellationToken cancellationToken = default)
    {
        var sqlConnection = (SqlConnection)connection;
        if (DoesConnectionNeedAccessToken(sqlConnection))
        {
            var tokenRequestContext = new TokenRequestContext(_azureSqlScopes);
            var token = await new DefaultAzureCredential()
                .GetTokenAsync(tokenRequestContext, cancellationToken);
            sqlConnection.AccessToken = token.Token;
        }

        return await base.ConnectionOpeningAsync(connection, eventData, result, cancellationToken);
    }

    [ExcludeFromCodeCoverage(Justification = "This method handles infrastructure-specific logic and does not require unit test coverage.")]
    private static bool DoesConnectionNeedAccessToken(SqlConnection connection)
    {
        //
        // Only try to get a token from AAD if
        //  - We connect to an Azure SQL instance; and
        //  - The connection doesn't specify a username.
        //
        var connectionStringBuilder = new SqlConnectionStringBuilder(connection.ConnectionString);
        return connectionStringBuilder.DataSource.Contains("database.windows.net", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(connectionStringBuilder.UserID);
    }
}
