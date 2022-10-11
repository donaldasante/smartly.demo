using SmartlyDemo.RiotSPA.Contracts.Requests;
using SmartlyDemo.RiotSPA.Contracts.Responses;
using SmartlyDemo.RiotSPA.Domain.Model.Employee;
using System;

namespace SmartlyDemo.RiotSPA.Mappers
{
    public class GeneratePayRollSingleMapper : Mapper<GenerateMonthlyPaySlipReq, GenerateMonthlyPaySlipResp, Employee>
    {
        public override GenerateMonthlyPaySlipResp FromEntity(Employee emp)
        {
            return new GenerateMonthlyPaySlipResp()
            {
                FirstName = emp.FirstName,
                Surname = emp.Surname,
                SuperRateCalculation = Math.Round(emp.Salary.MonthlySuperRateCalculated,2),
                MonthlyGrossSalary = Math.Round(emp.Salary.GrossMonthlySalary,2),
                MonthlyIncomeTax = Math.Round(emp.Salary.MonthlyIncomeTax,2),
                MonthlyNetSalary = Math.Round(emp.Salary.MonthlyNetSalary,2),
                PayPeriod = emp.Salary.MonthlyPeriodCalculated
            };
        }

        public override Employee ToEntity(GenerateMonthlyPaySlipReq req)
        {
            Employee employee = new Employee(
                firstName: req.FirstName,
                surname: req.Surname);

            employee.SetSalaryDetails(
                annualGrossSalary: req.AnnualGrossSalary,
                superRatePercentage: (req.SuperRatePercentage/100),
                monthOfTheYear: req.MonthOfTheYear);

            return employee;
        }
    }
}
