using System.Net;
using System.Threading.Tasks;
using CustomerApi.ErrorHandling;
using CustomerApi.Models;
using LightBDD.Framework;
using LightBDD.Framework.Parameters;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;

namespace CustomerApi.ServiceTests.Features
{
    [FeatureDescription(
@"In order to manage customers database
As an Api client
I want to be able to add new customers")]
    public partial class 新增客戶 : FeatureFixture
    {
        [Scenario]
        public async Task 創建一個新的客戶()
        {
            await Runner.RunScenarioAsync(
                _ => 給一個正確的創建客戶的要求內容(),
                _ => 當我要求客戶的創建(),
                _ => 然後回覆應該有狀態碼(HttpStatusCode.Created),
                _ => 然後回覆應該有客戶內容(),
                _ => 然後回覆的標頭應該有查詢的網址(),
                _ => 然後已建立的客戶應包含特定的客戶資料(),
                _ => 然後已建立的客戶應包含客戶的ID());
        }

        [Scenario]
        public async Task 創建客戶但沒內容是不被允許()
        {
            await Runner.RunScenarioAsync(
                _ => 給一個創建客戶的要求但沒有資料內容(),
                _ => 當我要求客戶的創建(),
                _ => 然後回覆應該有狀態碼(HttpStatusCode.BadRequest),
                _ => 然後回覆應該包含錯誤內容(Table.ExpectData(
                    new Error(ErrorCodes.ValidationError, "The Email field is required."),
                    new Error(ErrorCodes.ValidationError, "The FirstName field is required."),
                    new Error(ErrorCodes.ValidationError, "The LastName field is required."))));
        }

        [Scenario]
        public async Task 創建的客戶的Email與現存的相同是不被允許的()
        {
            await Runner.RunScenarioAsync(
                _ => 給一個已存在的客戶(),
                _ => 給一個創建客戶的要求但Email與一個現存客戶相同(),
                _ => 當我要求客戶的創建(),
                _ => 然後回覆應該有狀態碼(HttpStatusCode.BadRequest),
                _ => 然後回覆應該包含錯誤內容(Table.ExpectData(new Error(ErrorCodes.EmailInUse, "Email already in use."))));
        }
    }
}