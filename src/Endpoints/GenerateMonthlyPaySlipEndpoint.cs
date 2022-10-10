using Microsoft.Extensions.Logging;
using SmartlyDemo.RiotSPA.Contracts.Requests;
using SmartlyDemo.RiotSPA.Contracts.Responses;
using SmartlyDemo.RiotSPA.Domain.Interface;
using SmartlyDemo.RiotSPA.Domain.Model.Employee;
using SmartlyDemo.RiotSPA.Mappers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmartlyDemo.RiotSPA.Endpoints
{
    public class GenerateMonthlyPaySlipEndpoint : Endpoint<GenerateMonthlyPaySlipReq, GenerateMonthlyPaySlipResp, GeneratePayRollSingleMapper>
    {

        private readonly ILogger<GenerateMonthlyPaySlipEndpoint> _logger;
        private readonly ITaxService _taxService;


        public GenerateMonthlyPaySlipEndpoint(
            ILogger<GenerateMonthlyPaySlipEndpoint> logger,
            ITaxService taxService
            )
            
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _taxService = taxService ?? throw new ArgumentNullException(nameof(taxService));
        }

        public override void Configure()
        {
            Verbs(Http.GET);
            Routes("/api/employee/monthlypayslip");
            AllowAnonymous();
        }

        public override async Task HandleAsync(GenerateMonthlyPaySlipReq req,CancellationToken ct)
        {
            _logger.LogDebug("Generating Payroll");

            Employee employee = new Employee(
                firstName: req.FirstName,
                surname: req.Surname);


            employee.SetSalaryDetails(
                annualGrossSalary: req.AnnualGrossSalary,
                superRatePercentage: req.SuperRatePercentage,
                monthOfTheYear:req.MonthOfTheYear);

            _taxService.CalculateMonthlyPayslipForEmployee(employee.Salary);

            Response = Map.FromEntity(employee);
            await SendAsync(Response, cancellation: ct);
        }
    }
}
