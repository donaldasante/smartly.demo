using Microsoft.Extensions.Logging;
using SmartlyDemo.RiotSPA.Contracts.Requests;
using SmartlyDemo.RiotSPA.Contracts.Responses;
using SmartlyDemo.RiotSPA.Domain.Interface;
using SmartlyDemo.RiotSPA.Domain.Model.Employee;
using SmartlyDemo.RiotSPA.Mappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SmartlyDemo.RiotSPA.Validators;

namespace SmartlyDemo.RiotSPA.Endpoints
{
    public class GenerateMonthlyPaySlipCsvEndpoint : Endpoint<GenerateMonthlyPaySlipCsvReq,GenerateMonthlyPaySlipRespList,GeneratePayRollMultipleMapper>
    {
        private readonly ILogger<GenerateMonthlyPaySlipCsvEndpoint> _logger;
        private readonly ITaxService _taxService;
        
        public GenerateMonthlyPaySlipCsvEndpoint(
            ILogger<GenerateMonthlyPaySlipCsvEndpoint> logger,
            ITaxService taxService
            )
            
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _taxService = taxService ?? throw new ArgumentNullException(nameof(taxService));
        }

        public override void Configure()
        {
            Verbs(Http.POST);
            Routes("/api/employee/monthlypayslip/csv");
            AllowAnonymous();
            AllowFileUploads();
            DontThrowIfValidationFails();
        }

        public override async Task HandleAsync(GenerateMonthlyPaySlipCsvReq req,CancellationToken ct)
        {
            if (Files.Count > 0 && Files[0].ContentType == "text/csv")
            {
                var file = Files[0];
                var employees = new List<Employee>();
                
                using (var fileStream = file.OpenReadStream())
                using (var reader = new StreamReader(fileStream))
                {
                    int i = 0;
                    string row;
                    while ((row = await reader.ReadLineAsync()) != null)
                    {
                        if (i == 0)
                        {
                            //skip title
                            i++;
                            continue;
                        }

                        if (string.IsNullOrEmpty(row))
                            continue;

                        string[] parseLine = row.Split(',');

                        if (parseLine.Length == 0)
                            continue;

                        var firstName = parseLine[0];
                        var lastName = parseLine[1];
                        decimal.TryParse(parseLine[2], out var annualGrossSalary);
                        decimal.TryParse(parseLine[3], out var superRatePercentage);
                        var monthOfYear = parseLine[4];

                        var validator = new GenerateMonthlyPaySlipValidator();
                        //we need to manually validate csv input
                        var validationResult = validator.Validate(new GenerateMonthlyPaySlipReq()
                        {
                            FirstName = firstName,
                            Surname = lastName,
                            AnnualGrossSalary = annualGrossSalary,
                            SuperRatePercentage = superRatePercentage,
                            MonthOfTheYear = monthOfYear
                        });

                        if (!validationResult.IsValid)
                        {
                            ThrowError($"CSV Import failed for input {firstName}, " +
                                       $"{lastName},{annualGrossSalary},{superRatePercentage}" +
                                       $",{monthOfYear} - {validationResult.Errors.FirstOrDefault()?.ErrorMessage}");
                        }
                        
                        var employee = new Employee(
                            firstName: firstName,
                            surname: lastName);
                        
                        employee.SetSalaryDetails(
                            annualGrossSalary: annualGrossSalary,
                            superRatePercentage: superRatePercentage,
                            monthOfTheYear:monthOfYear);

                        _taxService.CalculateMonthlyPayslipForEmployee(employee.Salary);
                        employees.Add(employee);
                    }
                }
                
                var monthlyPaySlips = Map.FromEntity(employees);
                await SendAsync(monthlyPaySlips, cancellation: ct);
            }
            await SendNoContentAsync(ct);
        }
    }
}
