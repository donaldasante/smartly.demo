using Alba;
using Bogus;
using FluentAssertions;
using Newtonsoft.Json;
using SmartlyDemo.RiotSPA.Contracts.Requests;
using SmartlyDemo.RiotSPA.Contracts.Responses;
using SmartlyDemo.RiotSPA.Validators;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmartlyDemo.RiotSPA.Test.IntegrationTests
{
    public class GenerateMonthlyPaySlipEndpointTests : IClassFixture<WebAppFixture<Program>> 
    {
        private readonly HttpClient _client;
        private readonly IAlbaHost _host;

        private readonly Faker<GenerateMonthlyPaySlipReq> _payrollRequestGenerator = new Faker<GenerateMonthlyPaySlipReq>()
         .RuleFor(x => x.FirstName, faker => faker.Name.FirstName())
         .RuleFor(x => x.Surname, faker => faker.Name.LastName())
         .RuleFor(x => x.AnnualGrossSalary, faker => faker.Random.Decimal(100,1000000))
         .RuleFor(x => x.SuperRatePercentage, faker => faker.Random.Decimal(0M, 50M))
         .RuleFor(x => x.MonthOfTheYear, faker => faker.Date.Month().ToString());

        public GenerateMonthlyPaySlipEndpointTests(WebAppFixture<Program> apiWebFactory)
        {
            _host = apiWebFactory.AlbaHost;
            _client = _host.Server.CreateClient();
        }

        [Theory]
        [InlineData(60050,9, "February", 5004, 919, 4084, 450, "01 February - 28 February")]
        [InlineData(120000,10, "March", 10000, 2543, 7456, 1000, "01 March - 31 March")]
        [InlineData(300000,10, "April", 25000, 8093, 16906, 2500, "01 April - 30 April")]
        [InlineData(400000,5, "May", 33333, 11343, 21990, 1666, "01 May - 31 May")]
        public async Task GenerateMonthlyPaySlipEndpoint_EndToEnd_Tests(
            decimal annualGross,
            decimal superPercentage,
            string month,
            decimal monthlyGrossResult,
            decimal monthlyTaxResult,
            decimal monthlyNetResult,
            decimal superCalcResult,
            string monthResult)
        {
            GenerateMonthlyPaySlipReq request = _payrollRequestGenerator.Generate();
            request.AnnualGrossSalary = annualGross;
            request.SuperRatePercentage = superPercentage;
            request.MonthOfTheYear = month;

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var req = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_client.BaseAddress + "employee/monthlypayslip/view"),
                Content = data,
            };

            var response = await _client.SendAsync(req).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.IsSuccessStatusCode.Should().BeTrue();

            var responseJson = JsonConvert.DeserializeObject<GenerateMonthlyPaySlipResp>((await response.Content.ReadAsStringAsync().ConfigureAwait(false)));

            responseJson.FirstName.Should().Be(request.FirstName);
            responseJson.Surname.Should().Be(request.Surname);
            Math.Floor(responseJson.MonthlyNetSalary).Should().Be(monthlyNetResult);
            Math.Floor(responseJson.MonthlyIncomeTax).Should().Be(monthlyTaxResult);
            Math.Floor(responseJson.MonthlyGrossSalary).Should().Be(monthlyGrossResult);
            Math.Floor(responseJson.SuperRateCalculation).Should().Be(superCalcResult);
            responseJson.PayPeriod.Should().Be(monthResult);
        }
    }
}