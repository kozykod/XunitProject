using FluentAssertions;
using System.Text;
using System.Text.Json;

namespace XunitProject
{
    public class GeneratedApiTests
    {
        private readonly HttpClient _client;
        public GeneratedApiTests()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:3001")
            };
        }
        [Fact]
        public async Task Get_tables()
        {
            var response = await _client.GetAsync("/tables");
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var deserialized = JsonSerializer.Deserialize<Table[]>(content, options);
            deserialized.Should().BeOfType<Table[]>();
            deserialized.Should().HaveCount(2);
        }

        [Theory]
        [JsonFileData("tables.json", typeof(Table))]
        public async Task Post_tables(Table arg)
        {
            var jsonData = JsonSerializer.Serialize(arg);
            var body = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/tables", body);
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        //[Fact]
        //public async Task Get_fields()
        //{
        //    var response = await _client.GetAsync("/fields");
        //    response.EnsureSuccessStatusCode();
        //    response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        //    var content = await response.Content.ReadAsStringAsync();
        //    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        //    var deserialized = JsonSerializer.Deserialize<Field[]>(content, options);
        //}

        //[Theory]
        //[JsonFileData("fields.json", typeof(Field))]
        //public async Task Post_fields(Field arg)
        //{
        //    var jsonData = JsonSerializer.Serialize(arg);
        //    var body = new StringContent(jsonData, Encoding.UTF8, "application/json");
        //    var response = await _client.PostAsync("/fields", body);
        //    response.EnsureSuccessStatusCode();
        //    response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        //}

        //[Theory]
        //[JsonFileData("fields.json", typeof(Field))]
        //public async Task Delete_fields(Field arg)
        //{
        //    var response = await _client.DeleteAsync($"/fields/{arg.Id}");
        //    response.EnsureSuccessStatusCode();
        //}

        //[Theory]
        //[JsonFileData("records.json", typeof(Record))]
        //public async Task Post_records(Record arg)
        //{
        //    var jsonData = JsonSerializer.Serialize(arg);
        //    var body = new StringContent(jsonData, Encoding.UTF8, "application/json");
        //    var response = await _client.PostAsync("/records", body);
        //    response.EnsureSuccessStatusCode();
        //    response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        //}

        //[Theory]
        //[JsonFileData("records.json", typeof(Record))]
        //public async Task Delete_records(Record arg)
        //{
        //    var response = await _client.DeleteAsync($"/records/{arg.Id}");
        //    response.EnsureSuccessStatusCode();
        //}

    }
}
