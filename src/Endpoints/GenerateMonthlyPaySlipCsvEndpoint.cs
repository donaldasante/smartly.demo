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
    public class GenerateMonthlyPaySlipCsvEndpoint : Endpoint<GenerateMonthlyPaySlipReqCsv,GenerateMonthlyPaySlipRespList>
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
        }

        public override async Task HandleAsync(GenerateMonthlyPaySlipReqCsv req,CancellationToken ct)
        {
            if (Files != null && Files.Count > 0)
            {
                var file = Files[0];



                return;
            }
            await SendNoContentAsync();
        }
    }
}
