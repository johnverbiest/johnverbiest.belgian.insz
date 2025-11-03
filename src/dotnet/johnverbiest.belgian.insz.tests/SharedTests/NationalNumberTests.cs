using System;
using Xunit;

namespace johnverbiest.belgian.insz.tests.SharedTests
{
    public class NationalNumberTests
    {
        [Fact]
        public void CanCreateNationalNumber()
        {
            var birthDate = new DateTime(1990, 5, 15);
            var nationalNumber = new NationalNumber(birthDate, 123, 45, "90051512345");
            
            Assert.Equal(birthDate, nationalNumber.BirthDate);
            Assert.Equal(123, nationalNumber.SequenceNumber);
            Assert.Equal(45, nationalNumber.CheckDigit);
            Assert.Equal("90051512345", nationalNumber.Value);
        }

        [Fact]
        public void IsMale_ReturnsTrue_ForOddSequenceNumber()
        {
            var nationalNumber = new NationalNumber(DateTime.Now, 123, 45, "12345678901");
            Assert.True(nationalNumber.IsMale());
        }

        [Fact]
        public void IsMale_ReturnsFalse_ForEvenSequenceNumber()
        {
            var nationalNumber = new NationalNumber(DateTime.Now, 124, 45, "12345678901");
            Assert.False(nationalNumber.IsMale());
        }

        [Fact]
        public void Constructor_ThrowsException_ForInvalidSequenceNumber()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new NationalNumber(DateTime.Now, 0, 45, "12345678901"));
                
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new NationalNumber(DateTime.Now, 1000, 45, "12345678901"));
        }

        [Fact]
        public void Constructor_ThrowsException_ForInvalidCheckDigit()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new NationalNumber(DateTime.Now, 123, -1, "12345678901"));
                
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new NationalNumber(DateTime.Now, 123, 98, "12345678901"));
        }

        [Fact]
        public void Constructor_ThrowsException_ForInvalidValue()
        {
            Assert.Throws<ArgumentException>(() => 
                new NationalNumber(DateTime.Now, 123, 45, ""));
                
            Assert.Throws<ArgumentException>(() => 
                new NationalNumber(DateTime.Now, 123, 45, "123456789"));
                
            Assert.Throws<ArgumentException>(() => 
                new NationalNumber(DateTime.Now, 123, 45, "123456789012"));
        }
    }
}
