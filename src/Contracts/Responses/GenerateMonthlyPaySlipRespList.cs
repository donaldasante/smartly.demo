using System;
using System.Collections.Generic;

namespace SmartlyDemo.RiotSPA.Contracts.Responses
{
    public class GenerateMonthlyPaySlipRespList
    {
        IEnumerable<GenerateMonthlyPaySlipResp> MonthlyPaySlips { get; set; }
    }
}
