using NSwag;
using NSwag.CodeGeneration.CSharp;

namespace XunitProject
{
    public class NSwagTests
    {
        [Theory]
        [InlineData("schema.json", "NSwagGenerated.cs")]
        public async Task GenerateClientCode(string openApiJsonPath, string outputFilePath)
        {
            var path = Path.IsPathRooted(openApiJsonPath)
                ? openApiJsonPath
                : Path.GetRelativePath(Directory.GetCurrentDirectory(), openApiJsonPath);

            if (!File.Exists(path))
            {
                throw new ArgumentException($"Could not find file at path: {path}");
            }

            var fileData = File.ReadAllText(path);
            OpenApiDocument document = await OpenApiDocument.FromJsonAsync(fileData);

            var settings = new CSharpClientGeneratorSettings
            {
                ClassName = "QuickbaseClient",
                CSharpGeneratorSettings =
                {
                    Namespace = "XunitProject",
                    GenerateDataAnnotations = true
                }
            };
            var generator = new CSharpClientGenerator(document, settings);

            var clientCode = generator.GenerateFile();
            File.WriteAllText(outputFilePath, clientCode);
        }
    }
}