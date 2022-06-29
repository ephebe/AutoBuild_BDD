using CustomerApi.Models;
using LightBDD.Framework;
using LightBDD.XUnit2;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CustomerApi.ServiceTests.Features
{
    public partial class 取得客戶 : FeatureFixture
    {
        private readonly HttpClient _client;
        private State<HttpResponseMessage> _customerCreationResponse;
        private State<HttpResponseMessage> _response;
        private State<CreateCustomerRequest> _customerCreationRequest;
        private State<Guid> _customerId;

        public 取得客戶()
        {
            _client = TestServer.GetClient();
        }

        private async Task 給一個成功的新增客戶回覆()
        {
            _customerCreationRequest = new CreateCustomerRequest
            {
                Email = $"{Guid.NewGuid()}@mail.com",
                FirstName = "Joe",
                LastName = "Smith"
            };
            _customerCreationResponse = await _client.CreateCustomer(_customerCreationRequest);
            _customerCreationResponse.GetValue().EnsureSuccessStatusCode();
        }

        private async Task 給一個新增成功客戶的ID()
        {
            var customer = await _customerCreationResponse.GetValue().DeserializeAsync<Customer>();
            _customerId = customer.Id;
        }

        private async Task 給一個不存在的客戶ID()
        {
            _customerId = Guid.NewGuid();
        }

        private async Task 當我根據此ID要求客戶()
        {
            _response = await _client.GetCustomerById(_customerId);
        }

        private async Task 當我根據此回覆的標頭位置查詢()
        {
            _response = await _client.GetAsync(_customerCreationResponse.GetValue().Headers.Location);
        }

        private async Task 然後回覆的狀態碼應該是(HttpStatusCode code)
        {
            Assert.Equal(code, _response.GetValue().StatusCode);
        }

        private async Task 然後回覆應該包含客戶的詳細資料()
        {
            var actual = await _response.GetValue().DeserializeAsync<Customer>();
            var expected = _customerCreationRequest.GetValue();
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.FirstName, actual.FirstName);
            Assert.Equal(expected.LastName, actual.LastName);
        }
    }
}