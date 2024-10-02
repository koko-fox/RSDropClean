using System.Text.Json.Serialization;

namespace RSDropClean;

internal class RSItemCategory
{
  [JsonPropertyName("index")]
  public ushort Index { get; set; }

  [JsonPropertyName("name")]
  public string Name { get; set; } = string.Empty;
}
