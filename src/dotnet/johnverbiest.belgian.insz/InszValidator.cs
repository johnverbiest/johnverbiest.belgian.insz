using System.Globalization;

namespace johnverbiest.belgian.insz;

/// <summary>
/// Default implementation of the Belgian INSZ / national number validator.
/// </summary>
public class InszValidator: IInszValidator
{
    /// <inheritdoc/>
    public InszValidationResult Validate(long inszNumber) => Validate(GetInszString(inszNumber));

    /// <inheritdoc/>
    public InszValidationResult Validate(InszNumber inszNumber) => Validate(inszNumber.Value);
    
    /// <inheritdoc/>
    public InszValidationResult Validate(string inszString)
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

    private static void CheckSequenceNumber(string inszString, List<ValidationError> validationResults)
    {
        var sequenceNumber = GetSequenceNumber(inszString);
        if (sequenceNumber == 0 || sequenceNumber == 999)
        {
            validationResults.Add(ValidationError.InvalidSequenceNumber);
        }
    }

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
        if (GetYearString(inszString) != "00"&& isBis && month == 0 && day == 0) { 
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

    internal static bool IsBisWithSexUnknown(string inszString) => GetMonthNumber(inszString) >= 20 && GetMonthNumber(inszString) <= 32;
    internal static bool IsBisWithSexKnown(string inszString) => GetMonthNumber(inszString) >= 40 && GetMonthNumber(inszString) <= 52;
    internal static bool IsBisNumber(string inszString) => IsBisWithSexKnown(inszString) || IsBisWithSexUnknown(inszString);
    
    private static string GetYearString(string inszString) => inszString.Substring(0, 2);
    internal static short GetBaseYear(string inszString) => (short)(short.Parse(GetYearString(inszString)) + 1900);

    internal static int? GetBirthYear(string inszString) => CheckDateAndReturnDateData(inszString).Year;

    private static string GetMonthString(string inszString) => inszString.Substring(2, 2);
    internal static int GetMonthNumber(string inszString) => int.Parse(inszString.Substring(2, 2));
    
    internal static int GetDay(string inszString) => int.Parse(inszString.Substring(4, 2));
    internal static int GetSequenceNumber(string inszString) => int.Parse(inszString.Substring(6, 3));
    internal static string GetInszString(long inszNumber) => inszNumber.ToString("D11");
    
    internal static DateTime? GetDate(string inszString) => CheckDateAndReturnDateData(inszString).dateTime;

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
    
    private enum ValidationMode
    {
        Before2000,
        After2000
    }

    private static string CheckForNonNumericalCharactersAndReturnCleanVersion(string input, List<ValidationError>? validationResults)
    {
        var isNumber = long.TryParse(input, out var inszNumber);
        if (!isNumber)
        {
            validationResults?.Add(ValidationError.InputIsNotANumber);
        }

        return input;
    }

    private static void CheckLength(string input, List<ValidationError>? validationResults)
    {
        if (input.Length != 11)
        {
            validationResults?.Add(ValidationError.InputIsWrongLength);
        }
    }
}