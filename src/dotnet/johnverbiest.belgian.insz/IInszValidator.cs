namespace johnverbiest.belgian.insz;

/// <summary>
/// Defines a validator for Belgian INSZ (Identificatienummer van de Sociale Zekerheid) / national number strings.
/// </summary>
public interface IInszValidator
{
    /// <summary>
    /// Validates the supplied national number string.
    /// </summary>
    /// <param name="inszString">The raw national number to validate. May contain formatting characters (spaces, dashes, etc.).</param>
    /// <returns>
    /// An <see cref="InszValidationResult"/> describing whether the value is valid and, when valid, the canonical formatted value.
    /// </returns>
    InszValidationResult Validate(string inszString);
    
    /// <summary>
    /// Validates the supplied national number as a long integer.
    /// </summary>
    /// <param name="inszNumber">The numeric national number to validate (11 digits).</param>
    /// <returns>
    /// An <see cref="InszValidationResult"/> describing whether the value is valid and, when valid, the canonical formatted value.
    /// </returns>
    InszValidationResult Validate(long inszNumber);
    
    /// <summary>
    /// Validates the supplied strongly-typed INSZ number.
    /// </summary>
    /// <param name="inszNumber">The INSZ number object to validate.</param>
    /// <returns>
    /// An <see cref="InszValidationResult"/> describing whether the value is valid and, when valid, the canonical formatted value.
    /// </returns>
    InszValidationResult Validate(InszNumber inszNumber);
    
    
}