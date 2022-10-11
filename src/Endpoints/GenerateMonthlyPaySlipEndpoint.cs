using Microsoft.AspNetCore.Http;
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
            Routes("/employee/monthlypayslip/view");
            Description(b => b
                .Accepts<GenerateMonthlyPaySlipReq>("application/json")
                .Produces<GenerateMonthlyPaySlipResp>(200, "application/json+custom")
                .Produces<ErrorResponse>(400, "application/json+problem")
                .ProducesProblemFE<InternalErrorResponse>(500));
            Summary(s => {
                s.Summary = "Generates new payslip information";
                s.Description = "Generates new payslip information given employee information";
                s.ExampleRequest = new GenerateMonthlyPaySlipReq {  };
                s.Responses[200] = "payslip generation successful";
                s.Responses[400] = "Bad request. Check parameters";
                s.Responses[500] = "Internal Server Error";
            });

            AllowAnonymous();
        }

        public override async Task HandleAsync(GenerateMonthlyPaySlipReq req,CancellationToken ct)
        {
            _logger.LogDebug("Generating Payroll");

            Employee employee = Map.ToEntity (req);

            _taxService.CalculateMonthlyPayslipForEmployee(employee.Salary);

            Response = Map.FromEntity(employee);
            await SendAsync(Response, cancellation: ct);
        }
    }
}
