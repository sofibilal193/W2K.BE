using W2K.Common.Infrastructure.AppConfig;
using W2K.Common.Persistence.Context;
using W2K.Common.Persistence.Extensions;
using W2K.Config.Persistence.Context;
using W2K.Config.Repositories;

namespace W2K.Mono;

public static class Program
{
    public static void Main(string[] args)
    {
        var hostBuilder = args.CreateHostBuilder<Startup>()
            .AddAppConfig();

        // Configure Kestrel to only bind to HTTP in non-development environments
        hostBuilder = hostBuilder.ConfigureWebHostDefaults(x =>
            {
                _ = x.ConfigureKestrel((context, serverOptions) =>
                    {
                        var env = context.HostingEnvironment;
                        if (!env.IsDevelopment())
                        {
                            var portStr = Environment.GetEnvironmentVariable("ASPNETCORE_HTTP_PORTS") ?? "8080";
                            if (int.TryParse(portStr, out var port))
                            {
                                serverOptions.ListenAnyIP(port);
                            }
                        }
                        // In Development, use default Kestrel config (which may include HTTPS)
                    });
            });

        hostBuilder.Build()
            .MigrateDbContext<EventDbContext>(
                (_, _) => { })
            .MigrateDbContext<IdentityDbContext>(
                (context, services) =>
                {
                    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                    var officesRepo = services.GetRequiredService<IOfficeRepository>();
                    var usersRepo = services.GetRequiredService<IUserRepository>();
                    var rolesRepo = services.GetRequiredService<IRoleRepository>();
                    var permissionsRepo = services.GetRequiredService<IPermissionRepository>();
                    var officeUserRepo = services.GetRequiredService<IOfficeUserRepository>();
                    var sessionLogsRepo = services.GetRequiredService<ISessionLogsRepository>();
                    var unitOfWork = new IdentityUnitOfWork(context, officesRepo, usersRepo, rolesRepo, permissionsRepo, officeUserRepo, sessionLogsRepo);
                    new IdentityDbSeed().SeedAsync(unitOfWork, loggerFactory.CreateLogger<IdentityDbSeed>()).Wait();
                })
            .MigrateDbContext<FilesDbContext>(
                (_, _) => { })
            .MigrateDbContext<MessagingDbContext>(
                (_, _) => { })
            .MigrateDbContext<ConfigDbContext>(
                (context, services) =>
                {
                    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                    var configRepo = services.GetRequiredService<IConfigRepository>();
                    var officeConfigFields = services.GetRequiredService<IOfficeConfigFieldRepository>();
                    var officeConfigFieldValues = services.GetRequiredService<IOfficeConfigFieldValueRepository>();
                    var unitOfWork = new ConfigUnitOfWork(context, configRepo, officeConfigFields, officeConfigFieldValues);
                    ConfigItemsDbSeed.SeedAsync(unitOfWork, loggerFactory.CreateLogger("ConfigItemsDbSeed")).Wait();
                })
            .MigrateDbContext<FundingDbContext>(
                (_, _) => { })
            .MigrateDbContext<LoansDbContext>(
                (_, _) => { })
            .MigrateDbContext<LendersDbContext>(
                (context, services) =>
                {
                    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                    var lenderOfficesRepo = services.GetRequiredService<ILenderOfficeRepository>();
                    var lenderProgramsRepo = services.GetRequiredService<ILenderProgramRepository>();
                    var unitOfWork = new LendersUnitOfWork(context, lenderProgramsRepo, lenderOfficesRepo);
                    LendersDbSeed
                        .SeedAsync(unitOfWork, loggerFactory.CreateLogger("LendersDbSeed"))
                        .Wait();
                })
            .Run();
    }
}
