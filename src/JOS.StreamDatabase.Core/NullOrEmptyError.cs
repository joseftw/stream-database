namespace JOS.StreamDatabase.Core;

public class NullOrEmptyError : ValidationError
{
    public NullOrEmptyError(string source) : base($"'{source}' can not be null or empty")
    {
    }
}
