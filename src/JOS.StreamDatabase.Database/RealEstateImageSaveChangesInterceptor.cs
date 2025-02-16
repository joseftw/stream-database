using JOS.StreamDatabase.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JOS.StreamDatabase.Database;

internal class RealEstateImageSaveChangesInterceptor : FileSaveChangesInterceptor
{
    private protected override HashSet<Type> SupportedTypes => [typeof(RealEstate), typeof(RealEstateImage)];

    public async override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = new())
    {
        ;
        await base.SavingChangesAsync(eventData, result, cancellationToken);

        if(Entries.Count == 0)
        {
            return result;
        }

        var realEstateItems = new HashSet<Guid>();
        var realEstateImages = new HashSet<RealEstateImage>();
        foreach(var entry in Entries)
        {
            switch(entry.Entity)
            {
                case RealEstate realEstate:
                    realEstateItems.Add(realEstate.Id);
                    break;
                case RealEstateImage realEstateImage:
                    entry.State = EntityState.Unchanged;
                    realEstateImages.Add(realEstateImage);
                    break;
            }
        }

        foreach(var realEstateImage in realEstateImages)
        {
            if(realEstateItems.Contains(realEstateImage.RealEstateId))
            {
                AfterSavingChangesFiles.Add(realEstateImage);
            }
            else
            {
                DuringSavingChangesFiles.Add(realEstateImage);
            }
        }

        await base.SaveDuringSavingChanges();

        return result;
    }
}
