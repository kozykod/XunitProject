using Newtonsoft.Json.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace XunitProject
{
    public class JsonFileDataAttribute : DataAttribute
    {
        private readonly string _filePath;
        private readonly string _propertyName;
        private readonly Type _dataType;

        /// <summary>
        /// Load data from a JSON file as the data source for a theory
        /// </summary>
        /// <param name="filePath">The absolute or relative path to the JSON file to load</param>
        /// <param name="dataType">The Type to deserialize the data to.</param>
        public JsonFileDataAttribute(string filePath, Type dataType)
            : this(filePath, "", dataType) { }

        /// <summary>
        /// Load data from a JSON file as the data source for a theory
        /// </summary>
        /// <param name="filePath">The absolute or relative path to the JSON file to load</param>
        /// <param name="propertyName">The name of the property on the JSON file that contains the data for the test</param>
        /// <param name="dataType">The Type to deserialize the data to.</param>
        public JsonFileDataAttribute(string filePath, string propertyName, Type dataType)
        {
            _filePath = filePath;
            _propertyName = propertyName;
            _dataType = dataType;
        }

        /// <inheritDoc />
        public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
        {
            ArgumentNullException.ThrowIfNull(testMethod);

            var path = Path.IsPathRooted(_filePath)
                ? _filePath
                : Path.GetRelativePath(Directory.GetCurrentDirectory(), _filePath);

            if (!File.Exists(path))
            {
                throw new ArgumentException($"Could not find file at path: {path}");
            }

            var fileData = File.ReadAllText(path);

            JToken? dataToUse;
            if (string.IsNullOrEmpty(_propertyName))
            {
                dataToUse = JToken.Parse(fileData); // Whole file is the data
            }
            else
            {
                var allData = JObject.Parse(fileData); // Only use the specified property
                dataToUse = allData[_propertyName];
            }

            if (dataToUse is JArray array)
            {
                foreach (var item in array)
                {
                    if (item is JObject obj && _dataType != null)
                    {
                        // Convert each JObject in the array to a POCO, if _dataType is provided
                        object? poco = obj.ToObject(_dataType);
                        yield return new object?[] { poco }; // Wrap the POCO in an object[]
                    }
                    else if (item is JObject jo)
                    {
                        // Convert each JObject in the array to an object[]
                        yield return jo.Values().Select(v => v.ToObject<object>()).ToArray();
                    }
                    else
                    {
                        // The array contains non-object elements
                        throw new InvalidDataException($"Expected an array of objects in JSON file {_filePath}, but found element of type {item.Type}.");
                    }

                }
            }
            else if (dataToUse is JObject singleObject)
            {
                if (_dataType != null)
                {
                    object? poco = singleObject.ToObject(_dataType);
                    yield return new object?[] { poco };
                }
                else
                {
                    yield return singleObject.Values().Select(v => v.ToObject<object>()).ToArray();
                }

            }
            else
            {
                throw new InvalidDataException($"Expected a JSON array or object at the root or property '{_propertyName}' in JSON file {_filePath}.");
            }
        }
    }
}
