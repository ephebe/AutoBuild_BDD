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
    public partial class 刪除客戶
    {
        [Scenario]
        public async Task 刪除一個客戶()
        {
            await Runner.RunScenarioAsync(
                _ => 給一個已存在的客戶ID(),
                _ => 當要求根據此ID刪除客戶(),
                _ => 然後回覆碼應該是(HttpStatusCode.OK),
                _ => 當要取得此ID的客戶時(),
                _ => 然後回覆碼應該是(HttpStatusCode.NotFound));
        }

        [Scenario]
        public async Task 刪除不存在的客戶()
        {
            await Runner.RunScenarioAsync(
                _ => 給一個不存在的客戶ID(),
                _ => 當要求根據此ID刪除客戶(),
                _ => 然後回覆碼應該是(HttpStatusCode.NotFound));
        }
    }
}