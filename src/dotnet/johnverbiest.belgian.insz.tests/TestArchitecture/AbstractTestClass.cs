using System.Linq;
using System.Text.Json;
using johnverbiest.belgian.insz;
using Xunit.Abstractions;

namespace johnverbiest.belgian.insz.tests.TestArchitecture
{
    public abstract class AbstractTestClass
    {
        protected readonly ITestOutputHelper Output;

        protected AbstractTestClass(ITestOutputHelper output)
        {
            Output = output;
        }

        protected void DocumentTest(TestCase c)
        {
            Output.WriteLine("# Test Documentation");
            Output.WriteLine("");
            Output.WriteLine($"Test Suite: {c?.Name ?? "Unknown"}");
            Output.WriteLine($"Description: {c?.Description ?? "No description"}");
            Output.WriteLine("");
            Output.WriteLine($"## {c.TestName ?? "Unnamed Test"}");
            Output.WriteLine("");
            if (!string.IsNullOrWhiteSpace(c.TestDescription)) 
                Output.WriteLine($"> {c.TestDescription}\n");
            
            Output.WriteLine($"Input: `{c.Input}`");
            Output.WriteLine("");
            Output.WriteLine("Expected: ");
            Output.WriteLine("```");
            Output.WriteLine($"IsValid: {c.Expected.IsValid}");
            Output.WriteLine($"IsBis: {c.Expected.IsBis}");
            Output.WriteLine($"Birthdate: {c.Expected.BirthDate}");
            Output.WriteLine($"BirthYear: {c.Expected.BirthYear}");
            Output.WriteLine($"Sex: {c.Expected.SexEnum}");
            Output.WriteLine("```");
            Output.WriteLine("");
            Output.WriteLine($"Because {c.Because}");
            Output.WriteLine("");
            
        }

        protected void DocumentResult(InszValidationResult result)
        {
            Output.WriteLine("## Result");
            Output.WriteLine("");
            
            if (result.IsValid)
            {
                Output.WriteLine("✅ **VALID**");
                Output.WriteLine("");
                
                if (result.InszNumber != null)
                {
                    Output.WriteLine($"Number: `{result.InszNumber}`");
                    Output.WriteLine("");
                }
            }
            else
            {
                Output.WriteLine("❌ **INVALID**");
                Output.WriteLine("");
                
                if (result.ValidationErrors.Any())
                {
                    Output.WriteLine($"Errors: {string.Join(", ", result.ValidationErrors)}");
                    Output.WriteLine("");
                }
            }
            
            Output.WriteLine("```json");
            Output.WriteLine("{");
            Output.WriteLine($"  \"isValid\": {result.IsValid.ToString().ToLowerInvariant()},");
            if (result.InszNumber != null)
            {
                var inszJson = JsonSerializer.Serialize(result.InszNumber, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                Output.WriteLine($"  \"inszNumber\": {inszJson},");
            }
            else
            {
                Output.WriteLine($"  \"inszNumber\": null,");
            }
            Output.WriteLine($"  \"errorMessages\": \"{string.Join(",", result.ValidationErrors)}\"");
            Output.WriteLine("}");
            Output.WriteLine("```");
            Output.WriteLine("");
        }
    }
}