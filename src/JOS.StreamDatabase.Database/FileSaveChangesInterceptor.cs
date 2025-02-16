using JOS.StreamDatabase.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JOS.StreamDatabase.Database;

internal abstract class FileSaveChangesInterceptor : ISaveChangesInterceptor, IDisposable
{
    private static readonly HashSet<EntityState> EntityStates;
    private protected List<EntityEntry> Entries { get; private set; }
    private NpgsqlConnection Connection { get; set; } = null!;
    private protected HashSet<File> DuringSavingChangesFiles { get; }
    private protected HashSet<File> AfterSavingChangesFiles { get; }
    private protected abstract HashSet<Type> SupportedTypes { get; }

    static FileSaveChangesInterceptor()
    {
        EntityStates = new HashSet<EntityState> { EntityState.Added, EntityState.Modified };
    }

    protected FileSaveChangesInterceptor()
    {
        Entries = new List<EntityEntry>();
        DuringSavingChangesFiles = [];
        AfterSavingChangesFiles = [];
    }

    public virtual ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = new())
    {
        var dbContext = eventData.Context;
        if(dbContext is null)
        {
            return ValueTask.FromResult(result);
        }

        Entries = dbContext.ChangeTracker.Entries()
                           .Where(x =>
                               EntityStates.Contains(x.State) && SupportedTypes.Contains(x.Entity.GetType()))
                           .ToList();

        Connection = (NpgsqlConnection)dbContext.Database.GetDbConnection();

        return ValueTask.FromResult(result);
    }

    public async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = new())
    {
        if(AfterSavingChangesFiles.Count < 1)
        {
            return result;
        }

        if(eventData.Context is null)
        {
            return result;
        }

        await Save(AfterSavingChangesFiles);

        return result;
    }

    private protected async Task SaveDuringSavingChanges()
    {
        await Save(DuringSavingChangesFiles);
    }

    private async Task Save(HashSet<File> files)
    {
        var saveFileCommand = new PostgresSaveFileCommand(Connection);
        foreach(var file in files)
        {
            var result = await file.Save(saveFileCommand);
            if(result.Failed)
            {
                throw new Exception(
                    $"Failed to save {file.GetType().Name}", new Exception(result.Error!.ErrorMessage));
            }
        }
    }

    public void Dispose()
    {
        Connection = null!;
        Entries.Clear();
        AfterSavingChangesFiles.Clear();
        DuringSavingChangesFiles.Clear();
    }
}
