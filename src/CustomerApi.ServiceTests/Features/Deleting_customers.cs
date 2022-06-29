using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using System.Net;
using System.Threading.Tasks;

namespace CustomerApi.ServiceTests.Features
{
    [FeatureDescription(
@"In order to manage customers database
As an Api client
I want to be able to delete existing customers")]
    public partial class Deleting_customers
    {
        [Scenario]
        public async Task 刪除客戶()
        {
            await Runner.RunScenarioAsync(
                _ => 給一個已存在的客戶ID(),
                _ => 當要求根據此ID刪除客戶(),
                _ => 然後回覆碼應該是OK(HttpStatusCode.OK),
                _ => When_I_request_the_customer_by_this_Id(),
                _ => 然後回覆碼應該是OK(HttpStatusCode.NotFound));
        }

        [Scenario]
        public async Task Deleting_nonexistent_customer()
        {
            await Runner.RunScenarioAsync(
                _ => Given_an_Id_of_nonexistent_customer(),
                _ => 當要求根據此ID刪除客戶(),
                _ => 然後回覆碼應該是OK(HttpStatusCode.NotFound));
        }
    }
}