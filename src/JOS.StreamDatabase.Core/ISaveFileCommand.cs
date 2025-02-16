using System.IO;
using System.Threading.Tasks;

namespace JOS.StreamDatabase.Core;

public interface ISaveFileCommand
{
    Task<Result.Result> Execute(File file, Stream data);
}
