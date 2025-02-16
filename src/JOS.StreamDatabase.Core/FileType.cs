using JOS.Enumeration;

namespace JOS.StreamDatabase.Core;

public partial class FileType : IEnumeration<FileType>
{
    public static readonly FileType Image = new(1, "Image");
}

