using Alba;
using Bogus;
using FastEndpoints;
using FluentAssertions;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using SmartlyDemo.RiotSPA.Contracts.Requests;
using SmartlyDemo.RiotSPA.Contracts.Responses;
using SmartlyDemo.RiotSPA.Endpoints;
using SmartlyDemo.RiotSPA.Validators;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmartlyDemo.RiotSPA.Test.IntegrationTests
{
    public class GenerateMonthlyPaySlipCsvEndpointTests : IClassFixture<WebAppFixture<Program>> 
    {
        private readonly HttpClient _client;
        private readonly IAlbaHost _host;

        public GenerateMonthlyPaySlipCsvEndpointTests(WebAppFixture<Program> apiWebFactory)
        {
            _host = apiWebFactory.AlbaHost;
            _client = _host.Server.CreateClient();
        }

        [Fact]
        public async Task GenerateMonthlyPaySlipEndpoint_Csv_EndToEnd_Test_Invalid_Data_Should_Fail()
        {
            HttpResponseMessage response;
            using (var file = File.OpenRead(@"TestData\test_data_invalid_pay_period.csv"))
            using (var content = new StreamContent(file))
            using (var formData = new MultipartFormDataContent())
            {
                new FileExtensionContentTypeProvider().TryGetContentType(file.Name, out var contentType);
                content.Headers.ContentType = new MediaTypeHeaderValue(contentType??"text/csv");

                formData.Add(content, "files", "test_data_invalid_pay_period.csv");

                response = await _client.PostAsync("employee/monthlypayslip/csv", formData);

            }

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.IsSuccessStatusCode.Should().BeFalse();
            
            var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            responseJson.Contains("CSV Import failed for").Should().BeTrue();
        }


        [Fact]
        public async Task GenerateMonthlyPaySlipEndpoint_Csv_EndToEnd_Test_Valid_Data_Should_Pass()
        {
            HttpResponseMessage response;
            using (var file = File.OpenRead(@"TestData\test_data_valid.csv"))
            using (var content = new StreamContent(file))
            using (var formData = new MultipartFormDataContent())
            {
                new FileExtensionContentTypeProvider().TryGetContentType(file.Name, out var contentType);
                content.Headers.ContentType = new MediaTypeHeaderValue(contentType ?? "text/csv");

                formData.Add(content, "files", "test_data_valid.csv");
                response = await _client.PostAsync("employee/monthlypayslip/csv", formData);

            }

           response.StatusCode.Should().Be(HttpStatusCode.OK);
           response.IsSuccessStatusCode.Should().BeTrue();
           var generateMonthlyPaySlipRespList = JsonConvert.DeserializeObject<GenerateMonthlyPaySlipRespList>((await response.Content.ReadAsStringAsync().ConfigureAwait(false)));
           generateMonthlyPaySlipRespList.MonthlyPaySlips.Count.Should().Be(4);
        }

    }
}