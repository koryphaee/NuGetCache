using System.Text.Json.Serialization;

namespace Server.Data;

public record ManifestDto
(
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("resources")] ResourceDto[] Resources,
    [property: JsonPropertyName("@context")] ContextDto ContextDto
);