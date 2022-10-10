using Microsoft.Extensions.Options;
using SmartlyDemo.RiotSPA.Domain.Interface;
using SmartlyDemo.RiotSPA.Domain.Model.Salary;
using SmartlyDemo.RiotSPA.Domain.Model.Tax;
using System;
using System.Globalization;

namespace SmartlyDemo.RiotSPA.Domain.Service
{
    public class TaxService : ITaxService
    {
        public TaxCalculator TaxCalculator { get; protected set; } = null;

        public TaxService(IOptions<TaxCalculator> taxCalculatorOptions) {
            TaxCalculator = taxCalculatorOptions.Value ?? throw new ArgumentNullException(nameof(TaxCalculator));

        }

        public void CalculateMonthlyPayslipForEmployee(SalaryDetails salaryDetails)
        {
            var grossMonthlySalary = salaryDetails.GrossAnnualSalary / 12;

            var totalAnnualGross = salaryDetails.GrossAnnualSalary;
            decimal totalAnnualIncomeTax = 0;
            foreach (var band in TaxCalculator.Bands)
            {
                if (totalAnnualGross > band.Maximum && !band.NoMaximumLimit)
                {
                    totalAnnualIncomeTax += (band.TaxRate * (band.Maximum - band.Minimum));
                    continue;
                }

                if (totalAnnualGross < band.Maximum || band.NoMaximumLimit)
                {
                    totalAnnualIncomeTax += (band.TaxRate * (totalAnnualGross - band.Minimum));
                    break;
                }

            }

            var monthlyIncomeTax = totalAnnualIncomeTax / 12;
            var netMonthlySalary = grossMonthlySalary - monthlyIncomeTax;
            var superCalc = grossMonthlySalary * (salaryDetails.SuperRatePercentage);

            var calculatedMonthlyPeriod = OutputFirstandLastdayOfMonth(salaryDetails.MonthOfTheYearStr);

            salaryDetails.SetMonthlySalaryDetails(
                grossMonthlySalary,
                monthlyIncomeTax,
                netMonthlySalary,
                superCalc,
                calculatedMonthlyPeriod);

        }

        private string OutputFirstandLastdayOfMonth(string monthOfTheYear)
        {
            var dateMonth = DateTime.ParseExact(monthOfTheYear, "MMMM", CultureInfo.CurrentCulture);

            return $"{dateMonth.ToString("dd")} {dateMonth.ToString("MMMM")} - {dateMonth.AddMonths(1).AddDays(-1).ToString("dd") } {dateMonth.ToString("MMMM")}";
        }
    }
}
