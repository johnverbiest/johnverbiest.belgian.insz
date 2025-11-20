using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace johnverbiest.belgian.insz;

/// <summary>
/// Default implementation of the Belgian INSZ / national number validator.
/// </summary>
public class InszValidator: IInszValidator
{
    
    /// <summary>
    /// Validates a Belgian INSZ number provided as a long integer.
    /// </summary>
    /// <param name="inszNumber">The INSZ number as a long integer (11 digits).</param>
    /// <returns>
    /// An <see cref="InszValidationResult"/> containing the validation outcome, the parsed <see cref="InszNumber"/> if valid, 
    /// and any validation errors if invalid.
    /// </returns>
    /// <remarks>
    /// This is a convenience method that calls the static validation logic directly.
    /// Prefer this method for simple validation scenarios where you don't need dependency injection.
    /// </remarks>
    /// <example>
    /// <code>
    /// var result = InszValidator.ValidateInsz(85073003328L);
    /// if (result.IsValid)
    /// {
    ///     Console.WriteLine($"Valid INSZ: {result.InszNumber.ToFormattedString()}");
    /// }
    /// </code>
    /// </example>
    public static InszValidationResult ValidateInsz(long inszNumber) => ValidateInsz(GetInszString(inszNumber));
    
    /// <summary>
    /// Validates a Belgian INSZ number provided as an <see cref="InszNumber"/> object.
    /// </summary>
    /// <param name="inszNumber">The INSZ number object to validate.</param>
    /// <returns>
    /// An <see cref="InszValidationResult"/> containing the validation outcome, the parsed <see cref="InszNumber"/> if valid, 
    /// and any validation errors if invalid.
    /// </returns>
    /// <remarks>
    /// This is a convenience method that calls the static validation logic directly.
    /// Prefer this method for simple validation scenarios where you don't need dependency injection.
    /// </remarks>
    /// <example>
    /// <code>
    /// var insz = new InszNumber { Value = 85073003328L };
    /// var result = InszValidator.ValidateInsz(insz);
    /// if (result.IsValid)
    /// {
    ///     Console.WriteLine($"Valid INSZ: {result.InszNumber.ToFormattedString()}");
    /// }
    /// </code>
    /// </example>
    public static InszValidationResult ValidateInsz(InszNumber inszNumber) => ValidateInsz(inszNumber.Value);
    
    /// <summary>
    /// Validates a Belgian INSZ number provided as a string.
    /// </summary>
    /// <param name="inszString">The INSZ number as a string. May contain formatting characters (spaces, dashes, dots, etc.) which will be automatically removed.</param>
    /// <returns>
    /// An <see cref="InszValidationResult"/> containing the validation outcome, the parsed <see cref="InszNumber"/> if valid, 
    /// and any validation errors if invalid.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is a convenience method that calls the static validation logic directly.
    /// Prefer this method for simple validation scenarios where you don't need dependency injection.
    /// </para>
    /// <para>
    /// The method automatically strips common formatting characters before validation, so inputs like 
    /// "85.07.30-033.28", "85073003328", and "85-07-30-033-28" are all accepted.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var result = InszValidator.ValidateInsz("85.07.30-033.28");
    /// if (result.IsValid)
    /// {
    ///     Console.WriteLine($"Birth Year: {result.InszNumber.BirthYear}");
    /// }
    /// else
    /// {
    ///     foreach (var error in result.ValidationErrors)
    ///     {
    ///         Console.WriteLine($"Error: {error}");
    ///     }
    /// }
    /// </code>
    /// </example>
    public static InszValidationResult ValidateInsz(string inszString)
    {
        var validationResults = new List<ValidationError>();
        
        inszString = CheckForNonNumericalCharactersAndReturnCleanVersion(inszString, validationResults);
        // If there are non-numerical characters, skip the rest of the validation as they may cause exceptions.
        if (validationResults.Any())
        {
            return InszValidationResult.Invalid(null,validationResults.ToArray());
        }
        
        CheckLength(inszString, validationResults);
        CheckCheckSumAndReturnMode(inszString, validationResults);
        CheckDateAndReturnDateData(inszString, validationResults);
        CheckSequenceNumber(inszString, validationResults);

        var numberValue = long.Parse(inszString);

        return validationResults.Any()
            ? InszValidationResult.Invalid(new InszNumber()
            {
                HasBeenValidated = true,
                IsValid = false,
                Value = numberValue
            },validationResults.ToArray())
            : InszValidationResult.Valid(new InszNumber()
                {
                    HasBeenValidated = true,
                    IsValid = true,
                    Value = numberValue
                });
    }

    /// <inheritdoc/>
    public InszValidationResult Validate(long inszNumber) => ValidateInsz(inszNumber);

    /// <inheritdoc/>
    public InszValidationResult Validate(InszNumber inszNumber) => ValidateInsz(inszNumber);

    /// <inheritdoc/>
    public InszValidationResult Validate(string inszString) => ValidateInsz(inszString);

    /// <summary>
    /// Validates the sequence number component of the INSZ number.
    /// </summary>
    /// <param name="inszString">The INSZ number as a string.</param>
    /// <param name="validationResults">The list to add validation errors to.</param>
    /// <remarks>
    /// Sequence numbers 000 and 999 are considered invalid as they are reserved values.
    /// </remarks>
    private static void CheckSequenceNumber(string inszString, List<ValidationError> validationResults)
    {
        var sequenceNumber = GetSequenceNumber(inszString);
        if (sequenceNumber == 0 || sequenceNumber == 999)
        {
            validationResults.Add(ValidationError.InvalidSequenceNumber);
        }
    }

    /// <summary>
    /// Validates and extracts the date information from an INSZ number.
    /// </summary>
    /// <param name="inszString">The INSZ number as a string.</param>
    /// <param name="validationResults">Optional list to add validation errors to.</param>
    /// <returns>A tuple containing the parsed DateTime (if valid) and the year.</returns>
    /// <remarks>
    /// Handles special cases for BIS numbers including unknown dates and adjusts for post-2000 dates.
    /// BIS numbers use modified month values: +20 for unknown sex, +40 for known sex.
    /// </remarks>
    private static (DateTime? dateTime, int? Year) CheckDateAndReturnDateData(string inszString, List<ValidationError>? validationResults = null)
    {
        var year = GetBaseYear(inszString);
        var isBisSexKnown = IsBisWithSexKnown(inszString);
        var isBisSexUnknown = IsBisWithSexUnknown(inszString);
        var isBis = IsBisNumber(inszString);
        var month = GetMonthNumber(inszString);
        var day = GetDay(inszString);
        
        // Adjust month for BIS numbers
        month = isBisSexKnown
            ? month - 40
            : isBisSexUnknown
                ? month - 20 
                : month;
        
        // Adjust year for post-2000 dates
        var mode = CheckCheckSumAndReturnMode(inszString, []);
        if (mode == ValidationMode.After2000)
        {
            year += 100;;
        }
        
        // Special case for BIS numbers with unknown month
        if (isBis && month == 0 && day == 0) { 
            return (null, year); // skip date validation, date is unknown but valid
        }
        
        // Special case for BIS numbers with unknown year
        if (GetYearString(inszString) == "00" && isBis && month == 0 && day is > 0 and <= 10) {
            return (null, null); // skip date validation, date is unknown but valid
        }
        
        
        if (!DateTime.TryParseExact(
                $"{year:D4}-{month:D2}-{day:D2}",
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsed))
        {
            validationResults?.Add(ValidationError.DateIsInvalid);
            return (null, null);
        }
        return (parsed, parsed.Year);
        
    }

    /// <summary>
    /// Determines if the INSZ number is a BIS number with unknown sex.
    /// </summary>
    /// <param name="inszString">The INSZ number as a string.</param>
    /// <returns>True if this is a BIS number with unknown sex (month 20-32).</returns>
    internal static bool IsBisWithSexUnknown(string inszString) => GetMonthNumber(inszString) >= 20 && GetMonthNumber(inszString) <= 32;
    
    /// <summary>
    /// Determines if the INSZ number is a BIS number with known sex.
    /// </summary>
    /// <param name="inszString">The INSZ number as a string.</param>
    /// <returns>True if this is a BIS number with known sex (month 40-52).</returns>
    internal static bool IsBisWithSexKnown(string inszString) => GetMonthNumber(inszString) >= 40 && GetMonthNumber(inszString) <= 52;
    
    /// <summary>
    /// Determines if the INSZ number is any type of BIS number.
    /// </summary>
    /// <param name="inszString">The INSZ number as a string.</param>
    /// <returns>True if this is a BIS number (either with known or unknown sex).</returns>
    internal static bool IsBisNumber(string inszString) => IsBisWithSexKnown(inszString) || IsBisWithSexUnknown(inszString);
    
    /// <summary>
    /// Extracts the year portion from an INSZ number as a string.
    /// </summary>
    /// <param name="inszString">The INSZ number as a string.</param>
    /// <returns>The first two digits representing the year.</returns>
    private static string GetYearString(string inszString) => inszString.Substring(0, 2);
    
    /// <summary>
    /// Extracts the base year (assuming 1900s) from an INSZ number.
    /// </summary>
    /// <param name="inszString">The INSZ number as a string.</param>
    /// <returns>The year as a base year starting from 1900.</returns>
    internal static short GetBaseYear(string inszString) => (short)(short.Parse(GetYearString(inszString)) + 1900);

    /// <summary>
    /// Extracts the birth year from an INSZ number, accounting for century determination.
    /// </summary>
    /// <param name="inszString">The INSZ number as a string.</param>
    /// <returns>The birth year if the date is valid, otherwise null.</returns>
    internal static int? GetBirthYear(string inszString) => CheckDateAndReturnDateData(inszString).Year;

    /// <summary>
    /// Extracts the month portion from an INSZ number as a string.
    /// </summary>
    /// <param name="inszString">The INSZ number as a string.</param>
    /// <returns>The two digits representing the month (may be modified for BIS numbers).</returns>
    private static string GetMonthString(string inszString) => inszString.Substring(2, 2);
    
    /// <summary>
    /// Extracts the month number from an INSZ number.
    /// </summary>
    /// <param name="inszString">The INSZ number as a string.</param>
    /// <returns>The month as an integer (may be modified for BIS numbers).</returns>
    internal static int GetMonthNumber(string inszString) => int.Parse(inszString.Substring(2, 2));
    
    /// <summary>
    /// Extracts the day from an INSZ number.
    /// </summary>
    /// <param name="inszString">The INSZ number as a string.</param>
    /// <returns>The day as an integer.</returns>
    internal static int GetDay(string inszString) => int.Parse(inszString.Substring(4, 2));
    /// <summary>
    /// Extracts the sequence number from an INSZ number.
    /// </summary>
    /// <param name="inszString">The INSZ number as a string.</param>
    /// <returns>The sequence number as an integer (positions 7-9).</returns>
    internal static int GetSequenceNumber(string inszString) => int.Parse(inszString.Substring(6, 3));
    
    /// <summary>
    /// Converts a long INSZ number to its string representation.
    /// </summary>
    /// <param name="inszNumber">The INSZ number as a long.</param>
    /// <returns>The INSZ number formatted as an 11-digit string with leading zeros.</returns>
    internal static string GetInszString(long inszNumber) => inszNumber.ToString("D11");
    
    /// <summary>
    /// Extracts the birth date from an INSZ number.
    /// </summary>
    /// <param name="inszString">The INSZ number as a string.</param>
    /// <returns>The birth date if valid, otherwise null.</returns>
    internal static DateTime? GetDate(string inszString) => CheckDateAndReturnDateData(inszString).dateTime;

    /// <summary>
    /// Validates the checksum of an INSZ number and determines the validation mode (before or after 2000).
    /// </summary>
    /// <param name="input">The INSZ number as a string.</param>
    /// <param name="validationResults">The list to add validation errors to.</param>
    /// <returns>The validation mode indicating whether the number is from before or after 2000.</returns>
    /// <remarks>
    /// The checksum is calculated using modulo 97. For dates after 2000, 2,000,000,000 is added to the base number before validation.
    /// </remarks>
    private static ValidationMode CheckCheckSumAndReturnMode(string input, List<ValidationError> validationResults)
    {
        var checkNumber = int.Parse(input.Substring(input.Length - 2, 2));
        var baseString = input.Substring(0, input.Length - 2);
        var baseNumber = long.Parse(baseString);
        
        // First check for dates before 2000
        if ((97 - (baseNumber % 97)) != checkNumber)
        {
            
            // If that fails, add 2,000,000,000 and check again for dates after 2000
            baseNumber = baseNumber + 2000000000;
            if ((97 - (baseNumber % 97)) != checkNumber)
            {               
                validationResults.Add(ValidationError.ChecksumIsInvalid);
            }
            return ValidationMode.After2000;
        }
        return ValidationMode.Before2000;
    }
    
    /// <summary>
    /// Enumeration representing the validation mode for INSZ numbers based on birth year.
    /// </summary>
    private enum ValidationMode
    {
        /// <summary>
        /// Validation mode for birth years before 2000.
        /// </summary>
        Before2000,
        /// <summary>
        /// Validation mode for birth years from 2000 onwards.
        /// </summary>
        After2000
    }

    /// <summary>
    /// Checks for non-numerical characters in the input string and returns the cleaned version.
    /// </summary>
    /// <param name="input">The input string to validate.</param>
    /// <param name="validationResults">Optional list to add validation errors to.</param>
    /// <returns>The input string with common formatting characters removed (spaces, dashes, dots, slashes, underscores).</returns>
    /// <remarks>
    /// <para>
    /// Removes common formatting characters that are typically used in INSZ number display formats:
    /// spaces, hyphens/dashes, dots, forward slashes, and underscores.
    /// </para>
    /// <para>
    /// Adds a validation error if the cleaned input still contains non-numerical characters that prevent parsing as a long.
    /// </para>
    /// </remarks>
    private static string CheckForNonNumericalCharactersAndReturnCleanVersion(string input, List<ValidationError>? validationResults)
    {
        // Remove common formatting characters used in INSZ number formats
        input = input
            .Replace(" ", "")      // spaces
            .Replace("-", "")      // hyphens/dashes  
            .Replace(".", "")      // dots
            .Replace("/", "")      // forward slashes
            .Replace("_", "")      // underscores
            .Trim();               // leading/trailing whitespace
            
        var isNumber = long.TryParse(input, out _);
        if (!isNumber)
        {
            validationResults?.Add(ValidationError.InputIsNotANumber);
        }

        return input;
    }

    /// <summary>
    /// Validates that the input string has the correct length for an INSZ number.
    /// </summary>
    /// <param name="input">The input string to validate.</param>
    /// <param name="validationResults">Optional list to add validation errors to.</param>
    /// <remarks>
    /// INSZ numbers must be exactly 11 characters long.
    /// </remarks>
    private static void CheckLength(string input, List<ValidationError>? validationResults)
    {
        if (input.Length != 11)
        {
            validationResults?.Add(ValidationError.InputIsWrongLength);
        }
    }

    /// <summary>
    /// Determines the sex from an INSZ number.
    /// </summary>
    /// <param name="inszStringValue">The INSZ number as a string.</param>
    /// <returns>The determined sex: Male, Female, or Unknown for BIS numbers with unknown sex.</returns>
    /// <remarks>
    /// For BIS numbers with known sex, the sex is determined by the sequence number (even = female, odd = male).
    /// For regular numbers or BIS numbers with unknown sex, returns Unknown.
    /// </remarks>
    public static Sex? GetSex(string inszStringValue) =>
        IsBisWithSexKnown(inszStringValue) || !IsBisNumber(inszStringValue)
            ? int.Parse(inszStringValue.Substring(6, 3)) % 2 == 0
                ? Sex.Female
                : Sex.Male
            : Sex.Unknown;
}