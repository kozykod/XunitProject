using NBomber.CSharp;

namespace XunitProject
{
    public class NBomberTests
    {
        [Theory]
        [InlineData("tables")]
        [InlineData("fields")]
        public void GetTables_WithLoadSimulation(string endpoint)
        {
            using var httpClient = new HttpClient();

            var scenario = Scenario.Create("fetch_list", async context =>
            {
                var response = await httpClient.GetAsync($"http://localhost:3001/{endpoint}");

                return response.IsSuccessStatusCode
                    ? NBomber.CSharp.Response.Ok()
                    : NBomber.CSharp.Response.Fail();
            })
            .WithoutWarmUp()
            .WithLoadSimulations(
                Simulation.Inject(rate: 100,
                                  interval: TimeSpan.FromSeconds(1),
                                  during: TimeSpan.FromSeconds(3))
            );

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();
        }
    }
}