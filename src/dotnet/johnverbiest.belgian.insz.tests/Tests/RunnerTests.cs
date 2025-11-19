using johnverbiest.belgian.insz.tests.TestArchitecture;
using Xunit;
using Xunit.Abstractions;

namespace johnverbiest.belgian.insz.tests.Tests
{
    public class RunnerTests: AbstractTestClass
    {
        public RunnerTests(ITestOutputHelper output): base(output) { }

        [Fact]
        public void TestsAreRunning()
        {
            Output.WriteLine("Test is running successfully");
            Assert.True(true);
        }
        
        [Theory, JsonFileData("test-vectors/runner-tests.json")]
        public void RunnerDoesDocumentation(TestCase testCase)
        {
            DocumentTest(testCase);
        }
    }
}
