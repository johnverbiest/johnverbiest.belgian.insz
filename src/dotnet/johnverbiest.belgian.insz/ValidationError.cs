namespace johnverbiest.belgian.insz;

/// <summary>
/// Defines validation error types that can occur during INSZ (Belgian national number) validation.
/// </summary>
public enum ValidationError
{
    /// <summary>
    /// The input string cannot be parsed as a valid number format.
    /// This error occurs when the input contains non-numeric characters (excluding allowed formatting characters)
    /// </summary>
    InputIsNotANumber,
    
    /// <summary>
    /// The checksum of the INSZ number is invalid.
    /// </summary>
    ChecksumIsInvalid,
    
    /// <summary>
    /// The input string does not have the correct length (11) for a valid INSZ number.
    /// </summary>
    InputIsWrongLength,
    
    /// <summary>
    /// The first six digits of the INSZ number do not correspond to a valid date (YYMMDD).
    /// </summary>
    DateIsInvalid,
    
    /// <summary>
    /// The sequence number (digits 7 to 9) is invalid (cannot be 000 or 999).
    /// </summary>
    InvalidSequenceNumber
}