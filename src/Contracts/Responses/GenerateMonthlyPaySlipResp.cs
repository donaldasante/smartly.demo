using System;

namespace SmartlyDemo.RiotSPA.Contracts.Responses
{
    public class GenerateMonthlyPaySlipResp
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string PayPeriod { get; set; }
        public decimal MonthlyGrossSalary { get; set; }
        public decimal MonthlyIncomeTax { get; set; }
        public decimal MonthlyNetSalary { get; set; }
        public decimal SuperRateCalculation { get; set; }
    }
}
