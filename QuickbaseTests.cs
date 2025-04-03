using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace XunitProject
{
    public class QuickbaseTests
    {
        private readonly HttpClient _client;
        const string appId = "buzrmit2x";

        public QuickbaseTests()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://api.quickbase.com")
            };
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("QB-USER-TOKEN", "capxbb_rjwv_0_buztwwfd95hjzwbigb86mcshjknp");
            _client.DefaultRequestHeaders.Add("QB-Realm-Hostname", "antongeorgiev");
        }

        [Fact]
        public async Task GetTables_ExistingApp_ShouldReturnTablesForApp()
        { 
            var response = await _client.GetAsync($"/v1/tables?appId={appId}");
            var content = await response.Content.ReadAsStringAsync();

            var actual = JToken.Parse(content);
            actual.Should().BeOfType<JArray>();

            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            foreach (var obj in actual)
            {
                var table = JsonSerializer.Deserialize<Table>(obj.ToString(), opts);
                table.Should().BeOfType<Table>();

                foreach (var property in table.GetType().GetProperties())
                {
                    property.GetValue(table).Should().NotBeNull();
                }
            }
        }

        private async Task<int> GetTablesCount()
        {
            var getTables = await _client.GetAsync($"/v1/tables?appId={appId}");
            var tables = await getTables.Content.ReadAsStringAsync();
            return JToken.Parse(tables).Count();
        }

        private async Task<HttpResponseMessage> PostTable(Table table)
        {
            var jsonData = JsonSerializer.Serialize(table);
            var body = new StringContent(jsonData, Encoding.UTF8, "application/json");
            return await _client.PostAsync($"/v1/tables?appId={appId}", body);
        }

        private async Task<Table> CreateTable(Table table)
        {
            var post = await PostTable(table);
            post.EnsureSuccessStatusCode();
            post.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var content = await post.Content.ReadAsStringAsync();
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var created = JsonSerializer.Deserialize<Table>(content, opts);
            created.Should().NotBeNull();
            created.Should().BeOfType<Table>();
            created.Id.Should().NotBeNull();

            return created;
        }

        private async Task RemoveTable(Table table)
        {
            var remove = await _client.DeleteAsync($"/v1/tables/{table.Id}?appId={appId}");
            remove.EnsureSuccessStatusCode();
        }

        [Theory]
        [JsonFileData("tables.json", "unique", typeof(Table))]
        public async Task PostTable_Unique_ShouldResultInSuccess(Table table)
        {
            var tablesCount = await GetTablesCount();
            var created = await CreateTable(table);

            var updatedTablesCount = await GetTablesCount();
            updatedTablesCount.Should().Be(tablesCount+1);

            var dupe = await PostTable(table);
            dupe.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            await RemoveTable(created);

            updatedTablesCount = await GetTablesCount();
            updatedTablesCount.Should().Be(tablesCount);
        }

        [Theory]
        [JsonFileData("tables.json", "invalid", typeof(Table))]
        public async Task PostTable_InvalidData_ShouldResultInError(Table table)
        {
            var post = await PostTable(table);
            post.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Theory]
        [JsonFileData("tables.json", "data type", typeof(Table))]
        public async Task PostTable_DataTypeValidation_ShouldResultInError(Table table)
        {
            var post = await PostTable(table);
            post.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Theory]
        [JsonFileData("tables.json", "local", typeof(Table))]
        public async Task PostTable_ContentType_ShouldResultInError(Table table)
        {
            var jsonData = JsonSerializer.Serialize(table);
            var body = new StringContent(jsonData, Encoding.UTF8, "text/plain");
            var post = await _client.PostAsync($"/v1/tables?appId={appId}", body);
            post.StatusCode.Should().Be(System.Net.HttpStatusCode.UnsupportedMediaType);
        }

        [Theory]
        [JsonFileData("tables.json", "local", typeof(Table))]
        public async Task PostTable_Authorization_ShouldResultInUnauthorized(Table table)
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var post = await PostTable(table);
            post.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

    }
}
