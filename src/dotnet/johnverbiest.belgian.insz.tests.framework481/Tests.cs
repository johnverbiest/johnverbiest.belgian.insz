using System;
using Xunit;

namespace johnverbiest.belgian.insz.tests.dotnet481
{
    public class Tests
    {
        [Fact]
        public void Test1()
        {
            var test = new NationalNumber(DateTime.Now, 123, 45, "12345678901");
            Assert.True(true);
        }
    }
}