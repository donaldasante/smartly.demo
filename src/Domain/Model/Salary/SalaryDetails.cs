using System;
using SmartlyDemo.RiotSPA.ExtensionMethods;

namespace SmartlyDemo.RiotSPA.Domain.Model.Salary
{
    public class SalaryDetails
    {
        //Input
        public virtual decimal GrossAnnualSalary { get; protected set; } = 0;
        public virtual decimal SuperRatePercentage { get; protected set; } = 0;
        public virtual string MonthOfTheYearStr { get;protected set; } = string.Empty;

        //Calculated
        public bool MonthlyDetailsCalculated { get; protected set; } = false;
        public virtual decimal GrossMonthlySalary { get;protected set; } = 0;
        public virtual decimal MonthlyIncomeTax { get; protected set; } = 0;

        public virtual decimal MonthlyNetSalary { get; protected set; } = 0;

        public virtual decimal MonthlySuperRateCalculated { get; protected set; } = 0;

        public virtual string MonthlyPeriodCalculated { get; protected set; }

        public SalaryDetails(
            decimal grossAnnualSalary, 
            decimal superRatePercentage,
            string monthOfTheYearStr)
        {
            if (grossAnnualSalary < 100) throw new ArgumentException("gross annual salary cannot be less than 100");
            if (superRatePercentage < 0 || superRatePercentage > 50) throw new ArgumentException("super rate percentage should be between 0 and 50");
            if (!monthOfTheYearStr.IsConvertibleToMonth() ) throw new ArgumentException("Invalid month. e.g. January, February");

            GrossAnnualSalary = grossAnnualSalary;
            SuperRatePercentage = superRatePercentage;
            MonthOfTheYearStr = monthOfTheYearStr;
        }

        public void SetMonthlySalaryDetails(
            decimal grossMonthlySalary,
            decimal monthlyIncomeTax,
            decimal monthlyNetSalary,
            decimal monthlySuperRateCalculated,
            string monthlyPeriodCalculated)
        {
            MonthlyIncomeTax = monthlyIncomeTax;
            MonthlyNetSalary = monthlyNetSalary;
            GrossMonthlySalary = grossMonthlySalary;
            MonthlySuperRateCalculated = monthlySuperRateCalculated;
            MonthlyDetailsCalculated = true;
            MonthlyPeriodCalculated = monthlyPeriodCalculated;
        }

    }
}
