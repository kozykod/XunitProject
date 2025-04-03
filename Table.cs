using System.Text.Json.Serialization;

namespace XunitProject
{
    public class Table
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("created")]
        public string Created { get; set; }

        [JsonPropertyName("updated")]
        public string Updated { get; set; }

        [JsonPropertyName("alias")]
        public string Alias { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("nextRecordId")]
        public int NextRecordId { get; set; }

        [JsonPropertyName("nextFieldId")]
        public int NextFieldId { get; set; }

        [JsonPropertyName("defaultSortFieldId")]
        public int DefaultSortFieldId { get; set; }

        [JsonPropertyName("defaultSortOrder")]
        public string DefaultSortOrder { get; set; }

        [JsonPropertyName("keyFieldId")]
        public int KeyFieldId { get; set; }

        [JsonPropertyName("singleRecordName")]
        public string SingleRecordName { get; set; }

        [JsonPropertyName("pluralRecordName")]
        public string PluralRecordName { get; set; }

        [JsonPropertyName("sizeLimit")]
        public string SizeLimit { get; set; }

        [JsonPropertyName("spaceUsed")]
        public string SpaceUsed { get; set; }

        [JsonPropertyName("spaceRemaining")]
        public string SpaceRemaining { get; set; }
    }

}
