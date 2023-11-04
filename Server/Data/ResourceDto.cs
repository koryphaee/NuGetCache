using System.Text.Json.Serialization;

namespace Server.Data;

public record ResourceDto
(
    [property: JsonPropertyName("@type")] string Type,
    [property: JsonPropertyName("comment")] string Comment
)
{
    [JsonPropertyName("@id")]
    public required string Id { get; set; }
}