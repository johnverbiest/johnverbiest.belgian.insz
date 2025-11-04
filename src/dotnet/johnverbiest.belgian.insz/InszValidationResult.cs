namespace johnverbiest.belgian.insz;

/// <summary>
/// Represents the result of validating a Belgian INSZ / national number.
/// </summary>
public record InszValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the supplied input is considered valid.
    /// </summary>
    public bool IsValid { get; private init; }

    /// <summary>
    /// Gets the parsed INSZ number when <see cref="IsValid"/> is <c>true</c>; otherwise, <c>null</c>.
    /// </summary>
    public InszNumber? InszNumber { get; private init; }

    /// <summary>
    /// Gets a human-readable error message describing why validation failed.
    /// Returns <c>null</c> when <see cref="IsValid"/> is <c>true</c>.
    /// </summary>
    public ValidationError[] ValidationErrors { get; private init; } = [];

    internal static InszValidationResult Invalid(InszNumber? inszNumber, params ValidationError[] errorMessage) => new InszValidationResult()
    {
        ValidationErrors = errorMessage,
        InszNumber = inszNumber,
        IsValid = false,
    };
    
    internal static InszValidationResult Valid(InszNumber inszNumber) => new InszValidationResult()
    {
        InszNumber = inszNumber,
        IsValid = true,
    };
}