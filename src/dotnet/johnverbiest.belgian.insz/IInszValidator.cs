namespace johnverbiest.belgian.insz;

/// <summary>
/// Defines a validator for Belgian INSZ (Identificatienummer van de Sociale Zekerheid) / national number strings.
/// </summary>
public interface IInszValidator
{
    /// <summary>
    /// Validates the supplied national number string.
    /// </summary>
    /// <param name="input">The raw national number to validate. May contain formatting characters (spaces, dashes, etc.).</param>
    /// <returns>
    /// An <see cref="InszValidationResult"/> describing whether the value is valid and, when valid, the canonical formatted value.
    /// </returns>
    InszValidationResult Validate(string input);
}