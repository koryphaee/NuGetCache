namespace Server.Data;

public record UpstreamInfo
(
    string ManifestJson,
    string Segment,
    string OriginalUrl
);