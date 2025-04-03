using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using System.Text;

namespace XunitProject
{
    public class OpenApiTests
    {
        [Theory]
        [InlineData("schema.json", "GeneratedTests.cs")]
        public static void GenerateTestCode(string openApiFilePath, string outputFilePath)
        {
            using FileStream stream = File.OpenRead(openApiFilePath);
            var reader = new OpenApiStreamReader();
            OpenApiDocument openApiDocument = reader.Read(stream, out var diagnostic);

            if (diagnostic.Errors.Count > 0)
            {
                Console.WriteLine("Crash. Iterating errors in OpenAPI document:");
                foreach (var error in diagnostic.Errors)
                {
                    Console.WriteLine(error.Message);
                }
                return;
            }
            if (openApiDocument == null)
            {
                Console.WriteLine("Could not load the OpenAPI document.");
                return;
            }

            StringBuilder code = new();

            code.AppendLine("using FluentAssertions;");
            code.AppendLine("using System.Text;");
            code.AppendLine("using System.Text.Json;");
            code.AppendLine("");
            code.AppendLine("namespace XunitProject");
            code.AppendLine("{");

            code.AppendLine("    public class GeneratedTests");
            code.AppendLine("    {");
            code.AppendLine("        private readonly HttpClient _client;");

            code.AppendLine("        public GeneratedTests()");
            code.AppendLine("        {");
            code.AppendLine("            _client = new HttpClient");
            code.AppendLine("            {");
            code.AppendLine("                BaseAddress = new Uri(\"http://localhost:3001\")");
            code.AppendLine("            };");
            code.AppendLine("        }");

            foreach (var path in openApiDocument.Paths)
            {
                foreach (var operation in path.Value.Operations)
                {
                    string domainObjectPlural = path.Key.Replace("/", "");
                    string domainObject = domainObjectPlural[..^1];
                    string firstLetter = domainObject[..1];
                    domainObject = string.Concat(firstLetter.ToUpper(), domainObject.AsSpan(1, domainObject.Length - 1));
                    string methodName = $"{operation.Key}_{domainObjectPlural}";
                    switch (operation.Key.ToString())
                    {
                        case "Get":
                            code.AppendLine($"        [Fact]");
                            code.AppendLine($"        public async Task {methodName}()");
                            code.AppendLine("        {");
                            code.AppendLine($"            var response = await _client.GetAsync(\"{path.Key}\");");
                            code.AppendLine("            response.EnsureSuccessStatusCode();");
                            code.AppendLine("            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);");
                            code.AppendLine("            var content = await response.Content.ReadAsStringAsync();");
                            code.AppendLine("            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };");
                            code.AppendLine($"            var deserialized = JsonSerializer.Deserialize<{domainObject}[]>(content, options);");
                            break;                    
                        case "Post":                  
                            code.AppendLine($"        [Theory]");
                            code.AppendLine($"        [JsonFileData(\"{domainObjectPlural}.json\", typeof({domainObject}))]");
                            code.AppendLine($"        public async Task {methodName}({domainObject} arg)");
                            code.AppendLine("        {");
                            code.AppendLine("            var jsonData = JsonSerializer.Serialize(arg);");
                            code.AppendLine("            var body = new StringContent(jsonData, Encoding.UTF8, \"application/json\");");
                            code.AppendLine($"            var response = await _client.PostAsync(\"{path.Key}\", body);");
                            code.AppendLine("            response.EnsureSuccessStatusCode();");
                            code.AppendLine("            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);");
                            break;                    
                        case "Delete":                
                            code.AppendLine($"        [Theory]");
                            code.AppendLine($"        [JsonFileData(\"{domainObjectPlural}.json\", typeof({domainObject}))]");
                            code.AppendLine($"        public async Task {methodName}({domainObject} arg)");
                            code.AppendLine("        {");
                            code.AppendLine($"            var response = await _client.DeleteAsync($\"{path.Key}/{{arg.Id}}\");");
                            code.AppendLine("            response.EnsureSuccessStatusCode();");
                            break;
                    }
                    code.AppendLine("        }");
                    code.AppendLine("");
                }
            }
            code.AppendLine("    }");
            code.AppendLine("}");

            File.WriteAllText(outputFilePath, code.ToString());
            Console.WriteLine($"Generated code stored at {outputFilePath}");
        }
    }
}
