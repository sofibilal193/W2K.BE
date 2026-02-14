using W2K.Common.Persistence.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace W2K.Common.Persistence.Extensions;

public static class IHostExtensions
{
    public static IHost MigrateDbContext<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder)
        where TContext : BaseDbContext<TContext>
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<TContext>>();
            var context = services.GetRequiredService<TContext>();

            try
            {
                logger.LogInformation("Starting migration of database associated with context {DbContextName}", typeof(TContext).Name);
                InvokeSeeder(seeder, context, services);
                logger.LogInformation("Completed migration of database associated with context {DbContextName}", typeof(TContext).Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
            }
        }

        return host;
    }

    private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services)
            where TContext : BaseDbContext<TContext>
    {
        var env = services.GetRequiredService<IHostEnvironment>();
        if (env.IsDevelopment() && context.IsSql)
        {
            context.Migrate();
        }
        seeder(context, services);
    }
}
