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
I want to be able to retrieve existing customers")]
    public partial class 取得客戶
    {
        [Scenario]
        public async Task 取得客戶根據回覆的標頭位置()
        {
            await Runner.RunScenarioAsync(
                _ => 給一個成功的新增客戶回覆(),
                _ => 當我根據此回覆的標頭位置查詢(),
                _ => 然後回覆的狀態碼應該是(HttpStatusCode.OK),
                _ => 然後回覆應該包含客戶的詳細資料());
        }

        [Scenario]
        public async Task 取得客戶根據ID()
        {
            await Runner.RunScenarioAsync(
                _ => 給一個成功的新增客戶回覆(),
                _ => 給一個新增成功客戶的ID(),
                _ => 當我根據此ID要求客戶(),
                _ => 然後回覆的狀態碼應該是(HttpStatusCode.OK),
                _ => 然後回覆應該包含客戶的詳細資料());
        }

        [Scenario]
        public async Task 取得不存在的客戶()
        {
            await Runner.RunScenarioAsync(
                _ => 給一個不存在的客戶ID(),
                _ => 當我根據此ID要求客戶(),
                _ => 然後回覆的狀態碼應該是(HttpStatusCode.NotFound));
        }
    }
}