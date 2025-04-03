using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace XunitProject
{
    public class MockoonTests
    {
        private readonly HttpClient _client;

        public MockoonTests()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:3001")
            };
        }

        [Fact]
        public async Task GetTables_ShouldReturnMockoonDefaults()
        {
            var response = await _client.GetAsync("/tables");
            var content = await response.Content.ReadAsStringAsync();
            
            var actual = JToken.Parse(content);
            actual.Should().BeOfType(typeof(JArray));
            actual.Should().HaveCount(2);
            
            foreach (var item in actual)
            {
                var obj = item.ToObject<Table>();
                obj.Should().BeOfType<Table>();
            }
        }

        const string exampleTable = @"{
              ""name"": ""Example Table"",
              ""created"": ""2020-03-27T18:34:40Z"",
              ""updated"": ""2020-04-03T19:12:40Z"",
              ""alias"": ""_dbid_example_table"",
              ""description"": ""Table as an example."",
              ""id"": ""bpqe82s0"",
              ""nextRecordId"": 1,
              ""nextFieldId"": 1,
              ""defaultSortFieldId"": 2,
              ""defaultSortOrder"": ""ASC"",
              ""keyFieldId"": 3,
              ""singleRecordName"": ""Example Record"",
              ""pluralRecordName"": ""Example Records"",
              ""sizeLimit"": ""150 MB"",
              ""spaceUsed"": ""17 MB"",
              ""spaceRemaining"": ""133 MB""
            }";

        /// <summary>
        /// Mockoon defaults given Quickbase schema.
        /// </summary>
        [Theory]
        [JsonFileData("tables.json", "Quickbase", typeof(Table))]
        public async Task GetTableById_ShouldReturnDefaultTable(Table anyTable)
        {
            var response = await _client.GetAsync($"/tables/{anyTable.Id}");
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNull().And.NotBeEmpty();
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            Table? actualTable = JsonSerializer.Deserialize<Table>(content, opts);

            actualTable.Should().NotBeEquivalentTo(anyTable);

            var expectedTable = JsonSerializer.Deserialize<Table>(exampleTable, opts);
            actualTable.Should().BeEquivalentTo(expectedTable);
        }


        [Fact]
        public async Task GetApi_ShouldReturnNotFound()
        {
            var response = await _client.GetAsync("/api");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

    }
}
