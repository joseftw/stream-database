using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace JOS.StreamDatabase.Migrator;

public class ClearDatabaseCommand
{
    private readonly IEnumerable<DbContext> _dbContexts;
    private readonly ILogger<ClearDatabaseCommand> _logger;

    public ClearDatabaseCommand(IEnumerable<DbContext> dbContexts, ILogger<ClearDatabaseCommand> logger)
    {
        _dbContexts = dbContexts ?? throw new ArgumentNullException(nameof(dbContexts));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Execute()
    {
        foreach(var dbContext in _dbContexts)
        {
            var dbSets = dbContext
                         .GetType()
                         .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                         .Where(IsDbSet)
                         .Select(x => ((IQueryable<object>)x.GetValue(dbContext)!, x.Name));
            var remainingTables = new Queue<(IQueryable<object> DbSet, string Name)>(dbSets);

            while(remainingTables.TryDequeue(out var dbSet))
            {
                try
                {
                    var deletedRows = await dbSet.DbSet.ExecuteDeleteAsync();
                    _logger.LogInformation(
                        "Removed {NumberOfRows} rows from DbSet {DbSetName}", deletedRows, dbSet.Name);
                }
                catch(DbException)
                {
                    remainingTables.Enqueue(dbSet);
                }
            }
        }
    }

    private static bool IsDbSet(PropertyInfo propertyInfo)
    {
        return propertyInfo.PropertyType.IsGenericType &&
               (typeof(DbSet<>).IsAssignableFrom(propertyInfo.PropertyType.GetGenericTypeDefinition()));
    }
}
