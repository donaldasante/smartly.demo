namespace SmartlyDemo.RiotSPA.Contracts.Requests
{
    public class GenerateMonthlyPaySlipReq
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public decimal AnnualGrossSalary { get; set; }
        public decimal SuperRatePercentage { get; set; }
        public string MonthOfTheYear { get; set; }
    }
}

