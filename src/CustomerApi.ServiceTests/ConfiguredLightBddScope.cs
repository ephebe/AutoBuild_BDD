using CustomerApi.ServiceTests;
using LightBDD.Core.Configuration;
using LightBDD.XUnit2;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Reporting.Formatters;


[assembly: ClassCollectionBehavior(AllowTestParallelization = true)]
[assembly: ConfiguredLightBddScope]

namespace CustomerApi.ServiceTests
{
    internal class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            // some example customization of report writers
            configuration
                .ReportWritersConfiguration()
                .AddFileWriter<HtmlReportFormatter>(".\\output\\FeaturesReport.html");
        }

        protected override void OnSetUp()
        {
            TestServer.Initialize();
        }

        protected override void OnTearDown()
        {
            TestServer.Dispose();
        }
    }
}
