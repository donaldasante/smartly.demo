using Bogus;
using FastEndpoints;
using FluentAssertions;
using Newtonsoft.Json;
using SmartlyDemo.RiotSPA.Contracts.Requests;
using SmartlyDemo.RiotSPA.Contracts.Responses;
using SmartlyDemo.RiotSPA.Endpoints;
using SmartlyDemo.RiotSPA.Validators;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmartlyDemo.RiotSPA.Test.IntegrationTests
{
    public class GenerateMonthlyPaySlipValidatorTests : IClassFixture<ApiWebFactory<Program>> 
    {
        private readonly ApiWebFactory<Program> _apiWebFactory;
        private readonly HttpClient _client;

        private readonly Faker<GenerateMonthlyPaySlipReq> _payrollRequestGenerator = new Faker<GenerateMonthlyPaySlipReq>()
         .RuleFor(x => x.FirstName, faker => faker.Name.FirstName())
         .RuleFor(x => x.Surname, faker => faker.Name.LastName())
         .RuleFor(x => x.AnnualGrossSalary, faker => faker.Random.Decimal(100,1000000))
         .RuleFor(x => x.SuperRatePercentage, faker => faker.Random.Decimal(0M, 0.5M))
         .RuleFor(x => x.MonthOfTheYear, faker => faker.Date.Month().ToString());

        public GenerateMonthlyPaySlipValidatorTests(ApiWebFactory<Program> apiWebFactory)
        {
            _apiWebFactory = apiWebFactory;
            _client = _apiWebFactory.CreateClient();
        }

        [Fact]
        public async Task GenerateMonthlyPaySlipReq_Validation_On_FirstName_Null_Or_Empty_Should_Fail()
        {

            GenerateMonthlyPaySlipReq request = _payrollRequestGenerator.Generate();
            request.FirstName = null;
            await validate(request, false, 2);

            request.FirstName = "";
            await validate(request, false, 2);

            request.FirstName = "Test";
            await validate(request, true, 0);
        }

        [Fact]
        public async Task GenerateMonthlyPaySlipReq_Validation_On_Surname_Null_Or_Empty_Should_Fail()
        {
            GenerateMonthlyPaySlipReq request = _payrollRequestGenerator.Generate();
            request.Surname = null;
            await validate(request, false, 2);

            request.Surname = "";
            await validate(request, false, 2);

            request.Surname = "Test";
            await validate(request, true, 0);
        }

        [Fact]
        public async Task GenerateMonthlyPaySlipReq_Validation_On_AnnualGrossSalary()
        {
            GenerateMonthlyPaySlipReq request = _payrollRequestGenerator.Generate();
            request.AnnualGrossSalary = 0;
            await validate(request, false, 1);

            request.AnnualGrossSalary = 12000;
            await validate(request, true, 0);
        }

        [Fact]
        public async Task GenerateMonthlyPaySlipReq_Validation_On_SuperRatePercentage()
        {
            GenerateMonthlyPaySlipReq request = _payrollRequestGenerator.Generate();
            request.SuperRatePercentage = 0.51M;
            await validate(request, false, 1);


            request.SuperRatePercentage = 0.1906M;
            await validate(request, true, 0);
        }

        [Fact]
        public async Task GenerateMonthlyPaySlipReq_Validation_On_MonthOfTheYear()
        {
            GenerateMonthlyPaySlipReq request = _payrollRequestGenerator.Generate();

            request.MonthOfTheYear = "";
            await validate(request, false, 2);

            request.MonthOfTheYear = "Blah";
            await validate(request, false, 1);

            request.MonthOfTheYear = "January";
            await validate(request, true, 0);

            request.MonthOfTheYear = " February  ";
            await validate(request, true, 0);
        }


        private async Task validate(GenerateMonthlyPaySlipReq req, bool isValid, int errorCount)
        {
            var generateMonthlyPayslipValidator = new GenerateMonthlyPaySlipValidator();

            var validatorResult = await generateMonthlyPayslipValidator.ValidateAsync(req).ConfigureAwait(false);

            validatorResult.IsValid.Should().Be(isValid);
            validatorResult.Errors.Count.Should().Be(errorCount);
        }
    }
}