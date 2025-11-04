namespace johnverbiest.belgian.insz;

/// <summary>
/// Represents the result of validating a Belgian INSZ / national number.
/// </summary>
public class InszValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the supplied input is considered valid.
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Gets the parsed INSZ number when <see cref="IsValid"/> is <c>true</c>; otherwise, <c>null</c>.
    /// </summary>
    public InszNumber? InszNumber { get; init; }
    
    /// <summary>
    /// Gets a human-readable error message describing why validation failed.
    /// Returns <c>null</c> when <see cref="IsValid"/> is <c>true</c>.
    /// </summary>
    public string? ErrorMessage { get; init; }
}