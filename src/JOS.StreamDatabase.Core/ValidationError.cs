using JOS.Result;

namespace JOS.StreamDatabase.Core;

public class ValidationError : Error
{
    public ValidationError(string errorMessage) : base("Validation", errorMessage)
    {
    }
}
