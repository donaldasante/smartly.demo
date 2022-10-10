using FluentAssertions;
using SmartlyDemo.RiotSPA.Domain.Model.Salary;
using System;
using Xunit;

namespace SmartlyDemo.RiotSPA.Test.UnitTests
{
    public  class SalaryDetailsTests
    {
        [Theory]
        [InlineData(99,0.01,"January")]
        [InlineData(100, -0.01, "January")]
        [InlineData(100, 0.51, "January")]
        [InlineData(100, 0.10, "")]
        [InlineData(100, 0.10, "Blah")]
        public void SalaryDetails_Validation_On_All_Tests_Should_Throw_Exception(decimal grossAnnualSalary,decimal super, string month)
        {
            Assert.Throws<ArgumentException>(() => new SalaryDetails(grossAnnualSalary, super,month));
        }

        [Theory]
        [InlineData(100, 0.10, "January")]
        [InlineData(100000, 0.0902, "February")]
        [InlineData(10000000, 0.45223, "March")]
        public void SalaryDetails_Validation_On_All_Tests_Should_Pass(decimal grossAnnualSalary, decimal super, string month)
        {
            var salaryDetails = new SalaryDetails(grossAnnualSalary, super,month);
            salaryDetails.GrossAnnualSalary.Should().Be(grossAnnualSalary);
            salaryDetails.SuperRatePercentage.Should().Be(super);
        }
    }
}
