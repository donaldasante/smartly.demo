using SmartlyDemo.RiotSPA.Contracts.Requests;
using SmartlyDemo.RiotSPA.Contracts.Responses;
using SmartlyDemo.RiotSPA.Domain.Model.Employee;
using System;
using System.Collections.Generic;

namespace SmartlyDemo.RiotSPA.Mappers
{
    public class GeneratePayRollMultipleMapper : ResponseMapper<GenerateMonthlyPaySlipRespList, IEnumerable<Employee>>
    {
        public override GenerateMonthlyPaySlipRespList FromEntity(IEnumerable<Employee> employees)
        {
            var paySlipResponses = new GenerateMonthlyPaySlipRespList();
            
            foreach (var employee in employees)
            {
                paySlipResponses.MonthlyPaySlips.Add(new GenerateMonthlyPaySlipResp()
                {
                    FirstName = employee.FirstName,
                    Surname = employee.Surname,
                    SuperRateCalculation = Math.Round(employee.Salary.MonthlySuperRateCalculated, 2),
                    MonthlyGrossSalary = Math.Round(employee.Salary.GrossMonthlySalary, 2),
                    MonthlyIncomeTax = Math.Round(employee.Salary.MonthlyIncomeTax, 2),
                    MonthlyNetSalary = Math.Round(employee.Salary.MonthlyNetSalary, 2),
                    PayPeriod = employee.Salary.MonthlyPeriodCalculated
                });
            }

            return paySlipResponses;
        }
    }
}
