using johnverbiest.belgian.insz.tests.TestArchitecture;
using Xunit;
using Xunit.Abstractions;

namespace johnverbiest.belgian.insz.tests.SystemTests
{
    public class RunnerTests
    {
        private readonly ITestOutputHelper _output;

        public RunnerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestsAreRunning()
        {
            _output.WriteLine("Test is running successfully");
            Assert.True(true);
        }
        
        [Theory, JsonFileData("test-vectors\\runner-tests.json")]
        public void Runner_CanBeCreated(TestCase testCase)
        {
            _output.WriteLine($"Testing case: {testCase?.Input ?? "null"}");
            _output.WriteLine($"Expected valid: {testCase?.Expected?.IsValid?.ToString() ?? "null"}");
            _output.WriteLine($"Because: {testCase?.Because ?? "no reason given"}");
            
            // Your test implementation here
            // Assert.Equal(testCase.Expected.IsValid, runner.IsValid);
        }
    }
}
