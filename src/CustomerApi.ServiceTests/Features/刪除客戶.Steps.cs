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
    public partial class 刪除客戶 : FeatureFixture
    {
        private readonly HttpClient _client;
        private State<Guid> _customerId;
        private State<HttpResponseMessage> _response;

        public 刪除客戶()
        {
            _client = TestServer.GetClient();
        }

        private async Task 給一個已存在的客戶ID()
        {
            var request = new CreateCustomerRequest
            {
                Email = $"{Guid.NewGuid()}@mail.com",
                FirstName = "Joe",
                LastName = "Kennedy"
            };
            var response = await _client.CreateCustomer(request);
            var customer = await response.EnsureSuccessStatusCode().DeserializeAsync<Customer>();
            _customerId = customer.Id;
        }

        private async Task 給一個不存在的客戶ID()
        {
            _customerId = Guid.NewGuid();
        }

        private async Task 當要求根據此ID刪除客戶()
        {
            _response = await _client.DeleteCustomerById(_customerId.GetValue());
        }

        private async Task 當要取得此ID的客戶時()
        {
            _response = await _client.GetCustomerById(_customerId);
        }

        private async Task 然後回覆碼應該是(HttpStatusCode code)
        {
            Assert.Equal(code, _response.GetValue().StatusCode);
        }
    }
}