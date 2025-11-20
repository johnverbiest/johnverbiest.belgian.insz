using FluentAssertions;
using johnverbiest.belgian.insz.tests.TestArchitecture;
using Xunit;
using Xunit.Abstractions;

namespace johnverbiest.belgian.insz.tests.Tests
{
    public class InszNumberTests: AbstractTestClass
    {
        public InszNumberTests(ITestOutputHelper output) : base(output) { }

        [Theory(DisplayName = "Birth Date Extraction Tests")]
        [JsonFileData("test-vectors/pre2000-rn-tests.json")]
        [JsonFileData("test-vectors/post2000-rn-tests.json")]
        [JsonFileData("test-vectors/pre2000-bis-tests.json")]
        [JsonFileData("test-vectors/post2000-bis-tests.json")]
        public void BirthDateTest(TestCase testCase)
        {
            DocumentTest(testCase);
            var result = new InszValidator().Validate(testCase.Input);
            DocumentResult(result);

            result.IsValid.Should().Be(testCase.Expected.IsValid ?? false, because: testCase.Because);
            if (testCase.Expected.BirthDateTime.HasValue)
            {
                result.InszNumber!.BirthDate.Should().HaveValue(because: testCase.Because);
                result.InszNumber!.BirthDate.Value.Date.Should().Be(testCase.Expected.BirthDateTime.Value.Date, because: testCase.Because);
            }
            else
            {
                if (result.InszNumber == null)
                {
                    // If the INZ number is null, we cannot check the BirthDate property.
                    return;
                }
                result!.InszNumber!.BirthDate.Should().BeNull(because: testCase.Because);
            }
        }
        
        [Theory(DisplayName = "Birth Year Extraction Tests")]
        [JsonFileData("test-vectors/pre2000-rn-tests.json")]
        [JsonFileData("test-vectors/post2000-rn-tests.json")]
        [JsonFileData("test-vectors/pre2000-bis-tests.json")]
        [JsonFileData("test-vectors/post2000-bis-tests.json")]
        public void BirthYearTest(TestCase testCase)
        {
            DocumentTest(testCase);
            var result = new InszValidator().Validate(testCase.Input);
            DocumentResult(result);

            result.IsValid.Should().Be(testCase.Expected.IsValid ?? false, because: testCase.Because);
            if (testCase.Expected.BirthYear.HasValue)
            {
                result.InszNumber!.BirthYear.Should().HaveValue(because: testCase.Because);
                result.InszNumber!.BirthYear.Value.Should().Be(testCase.Expected.BirthYear.Value, because: testCase.Because);
            }
            else
            {
                if (result.InszNumber == null)
                {
                    // If the INZ number is null, we cannot check the BirthDate property.
                    return;
                }
                result!.InszNumber!.BirthYear.Should().BeNull(because: testCase.Because);
            }
        }
        
        [Theory(DisplayName = "Bis Number Extraction Tests")]
        [JsonFileData("test-vectors/pre2000-rn-tests.json")]
        [JsonFileData("test-vectors/post2000-rn-tests.json")]
        [JsonFileData("test-vectors/pre2000-bis-tests.json")]
        [JsonFileData("test-vectors/post2000-bis-tests.json")]
        public void BisNumberTest(TestCase testCase)
        {
            DocumentTest(testCase);
            var result = new InszValidator().Validate(testCase.Input);
            DocumentResult(result);

            result.IsValid.Should().Be(testCase.Expected.IsValid ?? false, because: testCase.Because);
            if (testCase.Expected.IsBis.HasValue)
            {
                result.InszNumber!.IsBis.Should().HaveValue(because: testCase.Because);
                result.InszNumber!.IsBis.Value.Should().Be(testCase.Expected.IsBis.Value, because: testCase.Because);
            }
            else
            {
                if (result.InszNumber == null)
                {
                    // If the INZ number is null, we cannot check the BirthDate property.
                    return;
                }
                result!.InszNumber!.IsBis.Should().BeNull(because: testCase.Because);
            }
        }
        
        
        [Theory(DisplayName = "Sex Extraction Tests")]
        [JsonFileData("test-vectors/pre2000-rn-tests.json")]
        [JsonFileData("test-vectors/post2000-rn-tests.json")]
        [JsonFileData("test-vectors/pre2000-bis-tests.json")]
        [JsonFileData("test-vectors/post2000-bis-tests.json")]
        public void SexTest(TestCase testCase)
        {
            DocumentTest(testCase);
            var result = new InszValidator().Validate(testCase.Input);
            DocumentResult(result);

            result.IsValid.Should().Be(testCase.Expected.IsValid ?? false, because: testCase.Because);
            if (testCase.Expected.SexEnum.HasValue)
            {
                result.InszNumber!.Sex.Should().HaveValue(because: testCase.Because);
                result.InszNumber!.Sex.Value.Should().Be(testCase.Expected.SexEnum.Value, because: testCase.Because);
            }
            else
            {
                if (result.InszNumber == null)
                {
                    // If the INZ number is null, we cannot check the BirthDate property.
                    return;
                }
                result!.InszNumber!.Sex.Should().BeNull(because: testCase.Because);
            }
        }
    }
}