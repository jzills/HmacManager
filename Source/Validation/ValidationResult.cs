namespace HmacManagement.Policies;

public class ValidationResult
{
    public ValidationResult(bool isValid, Exception? error = null)
    {
        IsValid = isValid;
        Error = error;
    }

    public ValidationResult(bool isValid, Exception? error, params ValidationResult[] validationResults) 
        : this(isValid && validationResults.All(validationResult => validationResult.IsValid), error)
    {
        ValidationResultAggregate.AddRange(validationResults);
    }

    public ValidationResult(params ValidationResult[] validationResults) 
        : this(validationResults.All(validationResult => validationResult.IsValid))
    {
        ValidationResultAggregate.AddRange(validationResults);
    }

    public readonly bool IsValid;
    public readonly Exception? Error;
    public readonly List<ValidationResult> ValidationResultAggregate = 
        new List<ValidationResult>();

    public Exception GetError() 
    {
        if (Error is not null)
        {
            return Error;
        }
        else
        {
            return ValidationResultAggregate.First().GetError();
        }
    }
}