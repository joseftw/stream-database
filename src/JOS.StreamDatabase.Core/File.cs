using System;
using System.IO;
using System.Threading.Tasks;

namespace JOS.StreamDatabase.Core;

public abstract class File : Entity<Guid>
{
    private protected FileMetadata _metadata = null!;
    private protected Stream Data = Stream.Null;
    public abstract FileType Type { get; init; }
    public abstract FileMetadata GetMetadata();

    public async Task<Result.Result> Save(ISaveFileCommand saveFileCommand)
    {
        if(Data.Equals(Stream.Null) || !Data.CanRead)
        {
            return Result.Result.Failure(new ValidationError("The Data stream needs to be readable"));
        }

        return await saveFileCommand.Execute(this, Data);
    }
}

public abstract class File<T> : File where T : FileMetadata
{
    protected File()
    {
        _metadata = Metadata;
    }

    public required T Metadata { get; init; } = null!;

    public override T GetMetadata()
    {
        return Metadata;
    }
}

