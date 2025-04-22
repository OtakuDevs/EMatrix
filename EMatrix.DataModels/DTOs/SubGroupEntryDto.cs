namespace EMatrix.DataModels.DTOs;

using System.Text.Json.Serialization;

public class SubGroupEntryDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}

