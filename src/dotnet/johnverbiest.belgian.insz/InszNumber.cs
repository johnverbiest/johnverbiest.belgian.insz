namespace johnverbiest.belgian.insz;

/// <summary>
/// Represents a Belgian National Number (INSZ/NISS) and its properties
/// </summary>
public record InszNumber
{
    private readonly Lazy<string> _inszString;
    private readonly Lazy<DateTime?> _birthDate;
    private readonly Lazy<int?> _birthYear;
    private readonly Lazy<string> _formattedString;

    public InszNumber()
    {
        _inszString = new Lazy<string>(() => InszValidator.GetInszString(Value), isThreadSafe: true);
        _birthDate = new Lazy<DateTime?>(() => InszValidator.GetDate(_inszString.Value), isThreadSafe: true);
        _formattedString = new Lazy<string>(() => $"{_inszString.Value.Substring(0, 2)}.{_inszString.Value.Substring(2, 2)}.{_inszString.Value.Substring(4, 2)}-{_inszString.Value.Substring(6, 3)}.{_inszString.Value.Substring(9, 2)}", isThreadSafe:true);
        _birthYear = new Lazy<int?>(() => InszValidator.GetBirthYear(_inszString.Value), isThreadSafe: true);
    }

    /// <summary>
    /// Numeric value of the INSZ number
    /// </summary>
    public long Value { get; init; }
    
    /// <summary>
    /// Has the number been validated (regardless of validity)
    /// </summary>
    public bool HasBeenValidated { get; internal init; } = false;
    
    /// <summary>
    /// Is the number valid, or null if it has not been validated
    /// </summary>
    public bool? IsValid { get; internal init; } = null;
    
    public bool? IsBis => IsValid == true ? InszValidator.IsBisNumber(_inszString.Value) : null;

    public override string ToString() => _inszString.Value;
    
    public string? ToFormattedString() => IsValid == true ? _formattedString.Value : null; 

    public DateTime? BirthDate => IsValid == true ? InszValidator.GetDate(_inszString.Value) : null;
    
    public int? BirthYear => IsValid == true ? _birthYear.Value : null;
}

