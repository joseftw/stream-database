using System;
using System.Runtime.CompilerServices;

namespace JOS.StreamDatabase.Core;

public class ValueTooLongError : ValidationError
{
    public ValueTooLongError(int max, int actual, [CallerMemberName]string source = "") :
        base($"'{source}' has a max length of {max}. Was {actual}")
    {
        if(string.IsNullOrWhiteSpace(source))
        {
            throw new Exception("You need to provide the source parameter");
        }
    }
}
