using FluentAssertions;
using johnverbiest.belgian.insz.tests.TestArchitecture;
using Xunit;
using Xunit.Abstractions;

namespace johnverbiest.belgian.insz.tests.Tests
{
    public class InszValidatorTests: AbstractTestClass
    {
        public InszValidatorTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory(DisplayName = "Pre-2000 RN Vectors")]
        [JsonFileData("test-vectors/pre2000-rn-tests.json")]
        public void Pre2000RnVectors(TestCase testCase)
        {
            DocumentTest(testCase);
            
            var validator = new InszValidator();
            var result = validator.Validate(testCase.Input);
            
            DocumentResult(result);
            testCase.Expected.IsValid.Should().Be(result.IsValid, because: testCase.Because);
        }
        
        [Theory(DisplayName = "Post-2000 RN Vectors")]
        [JsonFileData("test-vectors/post2000-rn-tests.json")]
        public void Post2000RnVectors(TestCase testCase)
        {
            DocumentTest(testCase);
            
            var validator = new InszValidator();
            var result = validator.Validate(testCase.Input);
            
            DocumentResult(result);
            testCase.Expected.IsValid.Should().Be(result.IsValid, because: testCase.Because);
        }
    }
}