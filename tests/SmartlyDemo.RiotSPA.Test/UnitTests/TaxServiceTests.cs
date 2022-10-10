using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SmartlyDemo.RiotSPA.Domain.Interface;
using SmartlyDemo.RiotSPA.Domain.Model.Salary;
using SmartlyDemo.RiotSPA.Domain.Model.Tax;
using SmartlyDemo.RiotSPA.Domain.Service;
using System;
using System.IO;
using Xunit;

namespace SmartlyDemo.RiotSPA.Test.UnitTests
{
    public class TaxServiceTests
    {
        private IOptions<TaxCalculator> _taxCalculatorOptions;

        public TaxServiceTests()
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", false)
               .Build();

            _taxCalculatorOptions = Options.Create(configuration.GetSection("TaxCalculator").Get<TaxCalculator>());
        }

        [Theory]
        [InlineData(60050,0.09, "February", 5004,919,4084,450, "01 February - 28 February")]
        [InlineData(120000, 0.10, "March", 10000, 2543, 7456, 1000, "01 March - 31 March")]
        [InlineData(300000, 0.10, "April", 25000, 8093, 16906, 2500, "01 April - 30 April")]
        [InlineData(400000, 0.05, "May", 33333, 11343, 21990, 1666, "01 May - 31 May")]
        public void TaxCalculator_CalculateMonthlyPayslip_For_Employee_Should_Pass(
            decimal annualGross,
            decimal superPercentage,
            string month,
            decimal monthlyGrossResult,
            decimal monthlyTaxResult,
            decimal monthlyNetResult,
            decimal superCalcResult,
            string monthResult)
        {
            SalaryDetails salaryDetails = new SalaryDetails(annualGross, superPercentage,month);
            var taxService = new TaxService(_taxCalculatorOptions);
            taxService.CalculateMonthlyPayslipForEmployee(salaryDetails);
            Math.Floor(salaryDetails.MonthlyNetSalary).Should().Be(monthlyNetResult);
            Math.Floor(salaryDetails.MonthlyIncomeTax).Should().Be(monthlyTaxResult);
            Math.Floor(salaryDetails.GrossMonthlySalary).Should().Be(monthlyGrossResult);
            Math.Floor(salaryDetails.MonthlySuperRateCalculated).Should().Be(superCalcResult);
            salaryDetails.MonthlyPeriodCalculated.Should().Be(monthResult);
            salaryDetails.MonthlyDetailsCalculated.Should().Be(true);
        }

        [Theory]
        [InlineData(60050, 0.09, "Test")]
        [InlineData(120000, 0.10, "Blah")]
        [InlineData(300000, 0.10, "Yeah")]
        [InlineData(400000, 0.05, "Whatever")]
        public void TaxCalculator_CalculateMonthlyPayslip_For_Employee_Invalid_MonthOfYear_Should_Fail(
            decimal annualGross,
            decimal superPercentage,
            string month)
        {
            Assert.Throws<ArgumentException>(() => new SalaryDetails(annualGross, superPercentage, month));
        }

    }
}
