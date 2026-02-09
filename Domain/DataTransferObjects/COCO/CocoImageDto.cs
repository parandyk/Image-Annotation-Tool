using System.Text.Json.Serialization;

namespace ImageAnnotationTool.Domain.DataTransferObjects.COCO;

public class CocoImageDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("file_name")]
    public string FileName { get; set; }
}