namespace johnverbiest.belgian.insz;

/// <summary>
/// Represents a Belgian National Number (INSZ/NISS) and its properties
/// </summary>
public record InszNumber
{
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
}

