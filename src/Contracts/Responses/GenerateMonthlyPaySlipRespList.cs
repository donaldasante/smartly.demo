using System;
using System.Collections.Generic;

namespace SmartlyDemo.RiotSPA.Contracts.Responses
{
    public class GenerateMonthlyPaySlipRespList
    {
        public List<GenerateMonthlyPaySlipResp> MonthlyPaySlips { get; set; } = new List<GenerateMonthlyPaySlipResp>();
    }
}
