using System.Text.Json.Serialization;

namespace Server.Data;

public record ContextDto
(
    [property: JsonPropertyName("vocab")] string Vocab,
    [property: JsonPropertyName("comment")] string Comment
);