using System.Globalization;

namespace johnverbiest.belgian.insz;

/// <summary>
/// Default implementation of the Belgian INSZ / national number validator.
/// </summary>
public class InszValidator: IInszValidator
{
    /// <inheritdoc/>
    public InszValidationResult Validate(long inszNumber) => Validate(inszNumber.ToString());

    /// <inheritdoc/>
    public InszValidationResult Validate(InszNumber inszNumber) => Validate(inszNumber.Value);
    
    /// <inheritdoc/>
    public InszValidationResult Validate(string inszString)
    {
        var validationResults = new List<ValidationError>();
        
        var inszNumber = CheckForNonNumericalCharacters(inszString, validationResults);
        // If there are non-numerical characters, skip the rest of the validation as they may cause exceptions.
        if (validationResults.Any())
        {
            return InszValidationResult.Invalid(null,validationResults.ToArray());
        }
        
        CheckLength(inszString, validationResults);
        CheckCheckSumAndReturnBase(inszString, validationResults);
        CheckDate(inszString, validationResults);
        CheckSequenceNumber(inszString, validationResults);


        return validationResults.Any()
            ? InszValidationResult.Invalid(new InszNumber()
            {
                HasBeenValidated = true,
                IsValid = false,
                Value = inszNumber
            },validationResults.ToArray())
            : InszValidationResult.Valid(new InszNumber()
                {
                    HasBeenValidated = true,
                    IsValid = true,
                    Value = inszNumber
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


    private static void CheckDate(string inszString, List<ValidationError> validationResults)
    {
        if (!DateTime.TryParseExact(
                $"{GetYear(inszString):D4}-{GetMonth(inszString):D2}-{GetDay(inszString):D2}",
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsed))
        {
            validationResults.Add(ValidationError.DateIsInvalid);
        }
    }

    internal static int GetYear(string inszString) => int.Parse(inszString.Substring(0, 2)) + 1900;

    internal static int GetMonth(string inszString) => int.Parse(inszString.Substring(2, 2));
    
    internal static int GetDay(string inszString) => int.Parse(inszString.Substring(4, 2));
    internal static int GetSequenceNumber(string inszString) => int.Parse(inszString.Substring(6, 3));
    
    internal static DateTime GetDate(string inszString) => new DateTime(GetYear(inszString), GetMonth(inszString), GetDay(inszString));
    
    private static void CheckCheckSumAndReturnBase(string input, List<ValidationError> validationResults)
    {
        var checkNumber = int.Parse(input.Substring(input.Length - 2, 2));
        var baseString = input.Substring(0, input.Length - 2);
        var baseNumber = long.Parse(baseString);
        
        if ((97 - (baseNumber % 97)) != checkNumber)
        {
            validationResults.Add(ValidationError.ChecksumIsInvalid);
        }
    }

    private static long CheckForNonNumericalCharacters(string input, List<ValidationError> validationResults)
    {
        var isNumber = long.TryParse(input, out var inszNumber);
        if (!isNumber)
        {
            validationResults.Add(ValidationError.InputIsNotANumber);
        }

        return inszNumber;
    }

    private static void CheckLength(string input, List<ValidationError> validationResults)
    {
        if (input.Length != 11)
        {
            validationResults.Add(ValidationError.InputIsWrongLength);
        }
    }
}