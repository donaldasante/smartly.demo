using Microsoft.AspNetCore.Http;

namespace SmartlyDemo.RiotSPA.Contracts.Requests
{
    public class GenerateMonthlyPaySlipCsvReq
    {
        public IFormFile File1 { get; set; }
    }
}

