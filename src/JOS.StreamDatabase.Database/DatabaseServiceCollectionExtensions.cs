using JOS.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace JOS.StreamDatabase.Database;

public static class DatabaseServiceCollectionExtensions
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<NpgsqlDataSource>(x =>
        {
            var connectionString = GetPostgresConnectionString(configuration);
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.UseNodaTime();
            dataSourceBuilder.EnableDynamicJson();
            return dataSourceBuilder.Build();
        });
        services.AddTransient<IInterceptor, RealEstateImageSaveChangesInterceptor>();
        services.AddDbContext<MyDbContext>(static (x, opts) =>
        {
            var dataSource = x.GetRequiredService<NpgsqlDataSource>();
            opts.UseNpgsql(dataSource, options =>
            {
                options.MigrationsAssembly("JOS.StreamDatabase.Database").UseNodaTime();
            }).UseSnakeCaseNamingConvention();
            var interceptors = x.GetServices<IInterceptor>();
            opts.AddInterceptors(interceptors);
        });

        services.AddScoped<DbContext>(static x => x.GetRequiredService<MyDbContext>());

        services.AddScoped<NpgsqlConnection>(static x =>
        {
            var dataSource = x.GetRequiredService<NpgsqlDataSource>();
            return dataSource.CreateConnection();
        });
        services.AddScoped<IDbConnection>(x => x.GetRequiredService<NpgsqlConnection>());
    }
    private static string GetPostgresConnectionString(IConfiguration configuration)
    {
        return configuration.GetRequiredValue<string>("Postgres:ConnectionString");
    }
}
