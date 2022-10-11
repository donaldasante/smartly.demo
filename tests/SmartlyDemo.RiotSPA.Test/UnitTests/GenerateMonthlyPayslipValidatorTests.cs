using Alba;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
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

namespace SmartlyDemo.RiotSPA.Test.UnitTests
{
    public class GenerateMonthlyPayslipValidatorTests
    {
        private readonly HttpClient _client;
        private readonly IAlbaHost _host;

        private readonly Faker<GenerateMonthlyPaySlipReq> _payrollRequestGenerator = new Faker<GenerateMonthlyPaySlipReq>()
         .RuleFor(x => x.FirstName, faker => faker.Name.FirstName())
         .RuleFor(x => x.Surname, faker => faker.Name.LastName())
         .RuleFor(x => x.AnnualGrossSalary, faker => faker.Random.Decimal(100,1000000))
         .RuleFor(x => x.SuperRatePercentage, faker => faker.Random.Decimal(0M, 50M))
         .RuleFor(x => x.MonthOfTheYear, faker => faker.Date.Month().ToString());

        public GenerateMonthlyPayslipValidatorTests()
        {

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

            request.MonthOfTheYear = "";
            await Validate(request, false, 2);

            request.MonthOfTheYear = "Blah";
            await Validate(request, false, 1);

            request.MonthOfTheYear = "January";
            await Validate(request, true, 0);

            request.MonthOfTheYear = " February  ";
            await Validate(request, true, 0);
        }


        private async Task Validate(GenerateMonthlyPaySlipReq req, bool isValid, int errorCount)
        {
            var generateMonthlyPayslipValidator = new GenerateMonthlyPaySlipValidator();

            var validatorResult = await generateMonthlyPayslipValidator.ValidateAsync(req).ConfigureAwait(false);

            validatorResult.IsValid.Should().Be(isValid);
            validatorResult.Errors.Count.Should().Be(errorCount);
        }
    }
}