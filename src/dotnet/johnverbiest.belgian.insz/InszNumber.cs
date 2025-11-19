using System;

namespace johnverbiest.belgian.insz;

/// <summary>
/// Represents a Belgian National Number (INSZ/NISS - Identificatienummer van de Sociale Zekerheid/Numéro d'identification de la sécurité sociale) 
/// and provides access to its constituent components and validation status.
/// </summary>
/// <remarks>
/// <para>
/// This record encapsulates a Belgian national identification number with lazy-loaded properties for efficient access to parsed components.
/// The INSZ number format is: YYMMDD-NNN-CC where:
/// </para>
/// <list type="bullet">
/// <item><term>YY</term><description>Birth year (last two digits)</description></item>
/// <item><term>MM</term><description>Birth month (01-12, or modified for BIS numbers)</description></item>
/// <item><term>DD</term><description>Birth day (01-31)</description></item>
/// <item><term>NNN</term><description>Sequence number (001-998, determines sex for some cases)</description></item>
/// <item><term>CC</term><description>Check digits (modulo 97 checksum)</description></item>
/// </list>
/// <para>
/// The class supports both regular INSZ numbers and BIS (Bis-nummer) numbers, which are used for individuals 
/// not registered in the Belgian population register.
/// </para>
/// <para>
/// <strong>Important:</strong> To create a validated INSZ number instance, use the <see cref="InszValidator"/> class 
/// or any implementation of <see cref="IInszValidator"/>. The validator will return an <see cref="InszValidationResult"/> 
/// containing a properly validated <see cref="InszNumber"/> instance with all validation flags set correctly.
/// </para>
/// </remarks>
/// <example>
/// <para>Creating a validated INSZ number:</para>
/// <code>
/// var validator = new InszValidator();
/// var result = validator.Validate("85073003328");
/// 
/// if (result.IsValid)
/// {
///     InszNumber validatedNumber = result.InszNumber;
///     Console.WriteLine($"Birth year: {validatedNumber.BirthYear}");
///     Console.WriteLine($"Formatted: {validatedNumber.ToFormattedString()}");
/// }
/// </code>
/// </example>
/// <seealso cref="InszValidator"/>
/// <seealso cref="IInszValidator"/>
/// <seealso cref="InszValidationResult"/>
public record InszNumber
{
    private readonly Lazy<string> _inszString;
    private readonly Lazy<DateTime?> _birthDate;
    private readonly Lazy<int?> _birthYear;
    private readonly Lazy<string> _formattedString;
    private readonly Lazy<bool?> _isBis;

    /// <summary>
    /// Initializes a new instance of the <see cref="InszNumber"/> record.
    /// </summary>
    /// <remarks>
    /// This constructor initializes lazy-loaded properties for efficient access to parsed components.
    /// It's recommended to create instances through <see cref="InszValidator"/> to ensure proper validation.
    /// </remarks>
    public InszNumber()
    {
        _inszString = new Lazy<string>(() => InszValidator.GetInszString(Value), isThreadSafe: true);
        _birthDate = new Lazy<DateTime?>(() => InszValidator.GetDate(_inszString.Value), isThreadSafe: true);
        _formattedString = new Lazy<string>(() => $"{_inszString.Value.Substring(0, 2)}.{_inszString.Value.Substring(2, 2)}.{_inszString.Value.Substring(4, 2)}-{_inszString.Value.Substring(6, 3)}.{_inszString.Value.Substring(9, 2)}", isThreadSafe:true);
        _birthYear = new Lazy<int?>(() => InszValidator.GetBirthYear(_inszString.Value), isThreadSafe: true);
        _isBis = new Lazy<bool?>(() => InszValidator.IsBisNumber(_inszString.Value), isThreadSafe: true);
    }

    /// <summary>
    /// Gets or initializes the numeric value of the INSZ number as an 11-digit long integer.
    /// </summary>
    /// <value>
    /// The complete INSZ number represented as a long integer, including leading zeros when converted to string format.
    /// Valid range is typically 00000000001 to 99123199999 (considering valid dates and sequence numbers).
    /// </value>
    /// <remarks>
    /// While you can set this property directly, it's recommended to use <see cref="InszValidator"/> to ensure 
    /// the number is valid and all validation properties are properly set.
    /// </remarks>
    /// <example>
    /// <para>Recommended approach using validator:</para>
    /// <code>
    /// var validator = new InszValidator();
    /// var result = validator.Validate(85073003328L);
    /// if (result.IsValid)
    /// {
    ///     var validInsz = result.InszNumber; // Properly validated instance
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="InszValidator.Validate(long)"/>
    public long Value { get; init; }
    
    /// <summary>
    /// Gets or internally initializes a value indicating whether this INSZ number has undergone validation processing.
    /// </summary>
    /// <value>
    /// <c>true</c> if the number has been processed through validation logic; <c>false</c> if it has not been validated yet.
    /// This flag is set regardless of whether the validation passed or failed.
    /// </value>
    /// <remarks>
    /// <para>
    /// This property is useful for distinguishing between numbers that haven't been validated and numbers that 
    /// have been validated but found to be invalid. It's set internally by the validation process when using 
    /// <see cref="InszValidator"/> or any <see cref="IInszValidator"/> implementation.
    /// </para>
    /// <para>
    /// To ensure this flag is properly set, always create INSZ number instances through the validation process 
    /// rather than directly instantiating this record.
    /// </para>
    /// </remarks>
    /// <seealso cref="InszValidator"/>
    /// <seealso cref="IsValid"/>
    public bool HasBeenValidated { get; internal init; } = false;
    
    /// <summary>
    /// Gets or internally initializes a value indicating the validation status of the INSZ number.
    /// </summary>
    /// <value>
    /// <c>true</c> if the number has been validated and is valid; 
    /// <c>false</c> if the number has been validated but is invalid; 
    /// <c>null</c> if the number has not been validated yet.
    /// </value>
    /// <remarks>
    /// <para>
    /// This property is set internally by the validation process. To validate an INSZ number and get a properly 
    /// initialized instance, use <see cref="InszValidator.Validate(string)"/> or other validation methods.
    /// </para>
    /// <para>
    /// When this property is <c>null</c>, most other properties (like <see cref="BirthDate"/>, <see cref="Sex"/>, etc.) 
    /// will also return <c>null</c> to prevent access to potentially invalid parsed data.
    /// </para>
    /// </remarks>
    /// <seealso cref="InszValidator"/>
    /// <seealso cref="HasBeenValidated"/>
    public bool? IsValid { get; internal init; } = null;
    
    /// <summary>
    /// Gets a value indicating whether this is a BIS (Bis-nummer) number.
    /// </summary>
    /// <value>
    /// <c>true</c> if this is a BIS number, <c>false</c> if it's a regular INSZ number, or <c>null</c> if the number is invalid or not validated.
    /// </value>
    /// <remarks>
    /// <para>
    /// BIS numbers are assigned to individuals who are not registered in the Belgian population register but need identification for administrative purposes.
    /// BIS numbers are identified by their month field being modified (+20 for unknown sex, +40 for known sex).
    /// </para>
    /// <para>
    /// This property returns <c>null</c> for unvalidated numbers. Use <see cref="InszValidator"/> to validate 
    /// the number first to ensure accurate BIS detection.
    /// </para>
    /// </remarks>
    /// <seealso cref="InszValidator"/>
    /// <seealso cref="IsValid"/>
    public bool? IsBis => IsValid == true ? _isBis.Value : null;

    /// <summary>
    /// Returns the INSZ number as an unformatted 11-digit string.
    /// </summary>
    /// <returns>The INSZ number as a string with leading zeros if necessary (e.g., "85073003328").</returns>
    /// <remarks>
    /// This method returns the raw 11-digit representation. For a formatted version with dots and dashes, 
    /// use <see cref="ToFormattedString()"/>. To ensure the number is valid before using, validate it with 
    /// <see cref="InszValidator"/> first.
    /// </remarks>
    /// <seealso cref="ToFormattedString()"/>
    /// <seealso cref="InszValidator"/>
    public override string ToString() => _inszString.Value;
    
    /// <summary>
    /// Returns the INSZ number in formatted display format (YY.MM.DD-NNN.CC).
    /// </summary>
    /// <returns>The formatted INSZ number string if valid (e.g., "85.07.30-033.28"), otherwise <c>null</c>.</returns>
    /// <remarks>
    /// <para>
    /// The format is: YY.MM.DD-NNN.CC where YY is year, MM is month, DD is day, NNN is sequence number, and CC is check digits.
    /// This method returns <c>null</c> for invalid or unvalidated numbers.
    /// </para>
    /// <para>
    /// To ensure this method returns a value, validate the INSZ number using <see cref="InszValidator"/> first.
    /// </para>
    /// </remarks>
    /// <seealso cref="ToString()"/>
    /// <seealso cref="InszValidator"/>
    public string? ToFormattedString() => IsValid == true ? _formattedString.Value : null; 

    /// <summary>
    /// Gets the birth date extracted from the INSZ number.
    /// </summary>
    /// <value>
    /// The birth date if the number is valid and contains a valid date, otherwise <c>null</c>.
    /// </value>
    /// <remarks>
    /// <para>
    /// For BIS numbers, this may be <c>null</c> if the date information is unknown or invalid.
    /// This property returns <c>null</c> for unvalidated numbers.
    /// </para>
    /// <para>
    /// Use <see cref="InszValidator"/> to validate the number first to ensure accurate date extraction.
    /// </para>
    /// </remarks>
    /// <seealso cref="BirthYear"/>
    /// <seealso cref="InszValidator"/>
    public DateTime? BirthDate => IsValid == true ? InszValidator.GetDate(_inszString.Value) : null;
    
    /// <summary>
    /// Gets the birth year extracted from the INSZ number.
    /// </summary>
    /// <value>
    /// The birth year if the number is valid, otherwise <c>null</c>.
    /// </value>
    /// <remarks>
    /// <para>
    /// The year is calculated considering the century determination logic (before/after 2000).
    /// For BIS numbers, this may be <c>null</c> if the year information is unknown.
    /// </para>
    /// <para>
    /// This property returns <c>null</c> for unvalidated numbers. Use <see cref="InszValidator"/> to validate 
    /// the number first to ensure accurate year extraction.
    /// </para>
    /// </remarks>
    /// <seealso cref="BirthDate"/>
    /// <seealso cref="InszValidator"/>
    public int? BirthYear => IsValid == true ? _birthYear.Value : null;
    
    /// <summary>
    /// Gets the sex/gender extracted from the INSZ number.
    /// </summary>
    /// <value>
    /// The determined sex (Male, Female, or Unknown) if the number is valid, otherwise <c>null</c>.
    /// </value>
    /// <remarks>
    /// <para>
    /// For BIS numbers with known sex, the sex is determined by the sequence number (even = female, odd = male).
    /// For regular INSZ numbers or BIS numbers with unknown sex, returns Unknown.
    /// </para>
    /// <para>
    /// This property returns <c>null</c> for unvalidated numbers. Use <see cref="InszValidator"/> to validate 
    /// the number first to ensure accurate sex determination.
    /// </para>
    /// </remarks>
    /// <seealso cref="IsBis"/>
    /// <seealso cref="InszValidator"/>
    public Sex? Sex => IsValid == true ? InszValidator.GetSex(_inszString.Value) : null;
}

