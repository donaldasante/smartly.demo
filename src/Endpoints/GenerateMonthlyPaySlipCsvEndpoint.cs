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
using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace SmartlyDemo.RiotSPA.Endpoints
{
    public class GenerateMonthlyPaySlipCsvEndpoint : Endpoint<GenerateMonthlyPaySlipReqCsv,GenerateMonthlyPaySlipRespList, GeneratePayRollMultipleMapper>
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
            Summary(s => {
                s.Summary = "Upload employees CSV file";
                s.Description = "Upload employees CSV file and get back bulk monthly salary information";
                s.ExampleRequest = new GenerateMonthlyPaySlipReq { };
                s.Responses[200] = "payslip generation successful";
                s.Responses[400] = "Bad request. Check parameters";
                s.Responses[500] = "Internal Server Error";
            });
            Routes("/employee/monthlypayslip/csv");
            AllowAnonymous();
            AllowFileUploads();
            DontThrowIfValidationFails();
        }

        public override async Task HandleAsync(GenerateMonthlyPaySlipReqCsv req,CancellationToken ct)
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
                        decimal.TryParse(parseLine[2], NumberStyles.Currency, CultureInfo.InvariantCulture, out var annualGrossSalary);
                        decimal.TryParse(parseLine[3], NumberStyles.Number, CultureInfo.InvariantCulture, out var superRatePercentage);
                        var monthOfYear = parseLine[4];

                        var validator = new GenerateMonthlyPaySlipValidator();
                        //we need to manually validate csv input
                        var reqPaySlip = new GenerateMonthlyPaySlipReq()
                        {
                            FirstName = firstName,
                            Surname = lastName,
                            AnnualGrossSalary = annualGrossSalary,
                            SuperRatePercentage = superRatePercentage,
                            MonthOfTheYear = monthOfYear
                        };

                        var validationResult = validator.Validate(reqPaySlip);

                        if (!validationResult.IsValid)
                        {
                            ThrowError($"CSV Import failed for input {firstName}, " +
                                       $"{lastName},{annualGrossSalary},{superRatePercentage}" +
                                       $",{monthOfYear} - {validationResult.Errors.FirstOrDefault()?.ErrorMessage}");
                        }

                        var generatePayRollSingleMapper = new GeneratePayRollSingleMapper();

                        var employee = generatePayRollSingleMapper.ToEntity(reqPaySlip);

                        _taxService.CalculateMonthlyPayslipForEmployee(employee.Salary);
                        employees.Add(employee);
                    }
                }
                
                var monthlyPaySlips = Map.FromEntity(employees);
                await SendAsync(monthlyPaySlips, cancellation: ct);
                return;
            }
            await SendNoContentAsync(ct);
            return;
        }
    }
}
