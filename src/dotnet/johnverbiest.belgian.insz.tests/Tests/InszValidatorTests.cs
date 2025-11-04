using FluentAssertions;
using johnverbiest.belgian.insz.tests.TestArchitecture;
using Xunit;
using Xunit.Abstractions;

namespace johnverbiest.belgian.insz.tests.Tests
{
    public class InszValidatorTests: AbstractTestClass
    {
        private readonly InszValidator _validator = new();
        
        public InszValidatorTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory(DisplayName = "Pre-2000 RN Vectors")]
        [JsonFileData("test-vectors/pre2000-rn-tests.json")]
        public void Pre2000RnVectors(TestCase testCase) => ValidateTestCase(testCase);
        
        [Theory(DisplayName = "Post-2000 RN Vectors")]
        [JsonFileData("test-vectors/post2000-rn-tests.json")]
        public void Post2000RnVectors(TestCase testCase) => ValidateTestCase(testCase);
        
        [Theory(DisplayName = "Pre-2000 BIS Vectors")]
        [JsonFileData("test-vectors/pre2000-bis-tests.json")]
        public void Pre2000BisVectors(TestCase testCase) => ValidateTestCase(testCase);
        
        [Theory(DisplayName = "Post-2000 BIS Vectors")]
        [JsonFileData("test-vectors/post2000-bis-tests.json")]
        public void Post2000BisVectors(TestCase testCase) => ValidateTestCase(testCase);

        private void ValidateTestCase(TestCase testCase)
        {
            DocumentTest(testCase);
            
            var result = _validator.Validate(testCase.Input);
            
            DocumentResult(result);
            testCase.Expected.IsValid.Should().Be(result.IsValid, because: testCase.Because);
        }
    }
}