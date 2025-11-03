namespace johnverbiest.belgian.insz;

/// <summary>
/// Represents a Belgian National Number (INSZ/NISS) with birth date and sequence number.
/// This is a demonstration of a record that works across multiple target frameworks.
/// </summary>
public record NationalNumber
{
    /// <summary>
    /// Birth date extracted from the national number
    /// </summary>
    public DateTime BirthDate { get; init; }
    
    /// <summary>
    /// Sequence number (typically odd for males, even for females)
    /// </summary>
    public int SequenceNumber { get; init; }
    
    /// <summary>
    /// Check digit for validation (0-97)
    /// </summary>
    public int CheckDigit { get; init; }
    
    /// <summary>
    /// Full national number string representation (11 digits)
    /// </summary>
    public string Value { get; init; }

    public NationalNumber(DateTime birthDate, int sequenceNumber, int checkDigit, string value)
    {
        if (sequenceNumber < 1 || sequenceNumber > 999)
            throw new ArgumentOutOfRangeException(nameof(sequenceNumber), "Sequence number must be between 1 and 999");
        
        if (checkDigit < 0 || checkDigit > 97)
            throw new ArgumentOutOfRangeException(nameof(checkDigit), "Check digit must be between 0 and 97");
        
        if (string.IsNullOrWhiteSpace(value) || value.Length != 11)
            throw new ArgumentException("National number must be 11 digits", nameof(value));

        BirthDate = birthDate;
        SequenceNumber = sequenceNumber;
        CheckDigit = checkDigit;
        Value = value;
    }

    /// <summary>
    /// Returns true if the sequence number suggests male gender (odd number)
    /// </summary>
    public bool IsMale() => SequenceNumber % 2 == 1;

    /// <summary>
    /// Returns true if the sequence number suggests female gender (even number)
    /// </summary>
    public bool IsFemale() => SequenceNumber % 2 == 0;

    public override string ToString() => Value;
}

