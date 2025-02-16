using JOS.StreamDatabase.Database;
using JOS.StreamDatabase.Migrator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.CommandLine;

var builder = Host.CreateDefaultBuilder(args);
builder.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});
builder.ConfigureServices((ctx, services) =>
{
    services.AddDatabase(ctx.Configuration);
    services.AddScoped<MigrateDatabaseCommand>();
    services.AddScoped<ClearDatabaseCommand>();
    services.AddScoped<SeedDatabaseCommand>();
});

IHost app = builder.Build();
var hostEnv = app.Services.GetRequiredService<IHostEnvironment>();
ILogger logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Running in {HostEnv} mode", hostEnv.EnvironmentName);

await using var scope = app.Services.CreateAsyncScope();
var serviceProvider = scope.ServiceProvider;
var rootCommand = new RootCommand("JOS.StreamDatabase.Migrator");
AddMigrateDatabaseCommand(rootCommand, serviceProvider);
AddClearDatabaseCommand(rootCommand, serviceProvider);
AddSeedDatabaseCommand(rootCommand, serviceProvider);
await rootCommand.InvokeAsync(args);

static void AddMigrateDatabaseCommand(RootCommand rootCommand, IServiceProvider serviceProvider)
{
    var command = new Command("migrate", "Migrates the database");
    command.SetHandler(() => serviceProvider.GetRequiredService<MigrateDatabaseCommand>().Execute());
    rootCommand.Add(command);
}

static void AddClearDatabaseCommand(RootCommand rootCommand, IServiceProvider serviceProvider)
{
    var command = new Command("clear", "Clears all tables and resets id counter, foreign keys etc");
    command.SetHandler(() => serviceProvider.GetRequiredService<ClearDatabaseCommand>().Execute());
    rootCommand.Add(command);
}

static void AddSeedDatabaseCommand(RootCommand rootCommand, IServiceProvider serviceProvider)
{
    var command = new Command("seed", "Seeds database with pre-defined data");
    command.SetHandler(() => serviceProvider.GetRequiredService<SeedDatabaseCommand>().Execute());
    rootCommand.Add(command);
}
