using Alba;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using SmartlyDemo.RiotSPA.Contracts.Requests;
using SmartlyDemo.RiotSPA.Contracts.Responses;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FastEndpoints;
using Xunit;

namespace SmartlyDemo.RiotSPA.Test.IntegrationTests
{
    public class GenerateMonthlyPayslipValidatorTests: IClassFixture<WebAppFixture<Program>> 
    {
        private readonly HttpClient _client;
        private readonly IAlbaHost _host;

        private readonly Faker<GenerateMonthlyPaySlipReq> _payrollRequestGenerator = new Faker<GenerateMonthlyPaySlipReq>()
         .RuleFor(x => x.FirstName, faker => faker.Name.FirstName())
         .RuleFor(x => x.Surname, faker => faker.Name.LastName())
         .RuleFor(x => x.AnnualGrossSalary, faker => faker.Random.Decimal(100,1000000))
         .RuleFor(x => x.SuperRatePercentage, faker => faker.Random.Decimal(0M, 50M))
         .RuleFor(x => x.MonthOfTheYear, faker => faker.Date.Month().ToString());

        public GenerateMonthlyPayslipValidatorTests(WebAppFixture<Program> apiWebFactory)
        {
            _host = apiWebFactory.AlbaHost;
            _client = _host.Server.CreateClient();
        }

        [Fact]
        public async Task GenerateMonthlyPaySlipReq_Validation_On_FirstName_Null_Or_Empty_Should_Fail()
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            GenerateMonthlyPaySlipReq request = _payrollRequestGenerator.Generate();
            request.FirstName = null;
            await Validate(request, false, 2);

            request.FirstName = "";
            await Validate(request, false, 2);

            request.FirstName = "Test";
            await Validate(request, true, 0);
        }

        [Fact]
        public async Task GenerateMonthlyPaySlipReq_Validation_On_Surname_Null_Or_Empty_Should_Fail()
        {
            GenerateMonthlyPaySlipReq request = _payrollRequestGenerator.Generate();
            request.Surname = null;
            await Validate(request, false, 2);

            request.Surname = "";
            await Validate(request, false, 2);

            request.Surname = "Test";
            await Validate(request, true, 0);
        }

        [Fact]
        public async Task GenerateMonthlyPaySlipReq_Validation_On_AnnualGrossSalary()
        {
            GenerateMonthlyPaySlipReq request = _payrollRequestGenerator.Generate();
            request.AnnualGrossSalary = 0;
            await Validate(request, false, 1);

            request.AnnualGrossSalary = 12000;
            await Validate(request, true, 0);
        }

        [Fact]
        public async Task GenerateMonthlyPaySlipReq_Validation_On_SuperRatePercentage()
        {
            GenerateMonthlyPaySlipReq request = _payrollRequestGenerator.Generate();
            request.SuperRatePercentage = 51;
            await Validate(request, false, 1);


            request.SuperRatePercentage = 19.06M;
            await Validate(request, true, 0);
        }

        [Fact]
        public async Task GenerateMonthlyPaySlipReq_Validation_On_MonthOfTheYear()
        {
            GenerateMonthlyPaySlipReq request = _payrollRequestGenerator.Generate();
            

            request.MonthOfTheYear = " February  ";
            await Validate(request, true, 0);
        }


        private async Task Validate(GenerateMonthlyPaySlipReq req, bool isValid, int errorCount)
        {

            var json = JsonConvert.SerializeObject(req);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var httpReq = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_client.BaseAddress + "employee/monthlypayslip/view"),
                Content = data,
            };
            
            var response = await _client.SendAsync(httpReq).ConfigureAwait(false);
            
            if (errorCount > 0)
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);  
            else
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.IsSuccessStatusCode.Should().Be(isValid);
            

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                ErrorResponse errorResponse= JsonConvert.DeserializeObject<ErrorResponse>((await response.Content.ReadAsStringAsync().ConfigureAwait(false)));
                errorResponse.Errors.Values.FirstOrDefault()?.Count.Should().Be(errorCount);
            }
            else
            {
                GenerateMonthlyPaySlipResp responseObject= JsonConvert.DeserializeObject<GenerateMonthlyPaySlipResp>((await response.Content.ReadAsStringAsync().ConfigureAwait(false)));
                responseObject.FirstName.Should().Be(req.FirstName);
                responseObject.Surname.Should().Be(req.Surname);
            }
        }
    }
}