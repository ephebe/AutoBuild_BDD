using CustomerApi.Models;
using LightBDD.Framework;
using LightBDD.Framework.Parameters;
using LightBDD.Framework.Scenarios;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CustomerApi.ServiceTests.Features
{
    public partial class Adding_customers
    {
        private readonly HttpClient _client;
        private State<CreateCustomerRequest> _createCustomerRequest;
        private State<HttpResponseMessage> _response;
        private State<Customer> _createdCustomer;

        public Adding_customers()
        {
            _client = TestServer.GetClient();
        }

        private async Task 給一個正確的創建客戶的要求內容()
        {
            _createCustomerRequest = new CreateCustomerRequest
            {
                Email = $"{Guid.NewGuid()}@mymail.com",
                FirstName = "John",
                LastName = "Smith"
            };
        }

        private async Task 給一個創建客戶的要求但Email與一個現存客戶相同()
        {
            _createCustomerRequest = new CreateCustomerRequest
            {
                Email = _createCustomerRequest.GetValue().Email,
                FirstName = "Bob",
                LastName = "Johnson"
            };
        }

        private async Task<CompositeStep> 給一個已存在的客戶()
        {
            return CompositeStep.DefineNew()
                .AddAsyncSteps(
                    _ => 給一個正確的創建客戶的要求內容(),
                    _ => 當我要求客戶的創建(),
                    _ => 然後回覆應該有狀態碼(HttpStatusCode.Created))
                .Build();
        }

        private async Task 給一個創建客戶的要求但沒有資料內容()
        {
            _createCustomerRequest = new CreateCustomerRequest();
        }

        private async Task 當我要求客戶的創建()
        {
            _response = await _client.CreateCustomer(_createCustomerRequest.GetValue());
        }

        private async Task 然後已建立的客戶應包含客戶的ID()
        {
            Assert.NotEqual(Guid.Empty, _createdCustomer.GetValue().Id);
        }

        private async Task 然後已建立的客戶應包含特定的客戶資料()
        {
            var request = _createCustomerRequest.GetValue();
            var customer = _createdCustomer.GetValue();

            Assert.Equal(request.Email, customer.Email);
            Assert.Equal(request.FirstName, customer.FirstName);
            Assert.Equal(request.LastName, customer.LastName);
        }

        private async Task 然後回覆應該有狀態碼(HttpStatusCode code)
        {
            Assert.Equal(code, _response.GetValue().StatusCode);
        }

        private async Task 然後回覆應該有客戶內容()
        {
            _createdCustomer = await _response.GetValue().DeserializeAsync<Customer>();
        }

        private async Task 然後回覆應該包含錯誤內容(VerifiableDataTable<Error> errors)
        {
            var actual = await _response.GetValue().DeserializeAsync<Errors>();
            errors.SetActual(actual.Items.OrderBy(x => x.Message));
        }

        private async Task 然後回覆的標頭應該有查詢的網址()
        {
            Assert.NotNull(_response.GetValue().Headers.Location);
        }
    }
}