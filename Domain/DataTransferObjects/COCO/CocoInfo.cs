using System;
using System.Text.Json.Serialization;

namespace ImageAnnotationTool.Domain.DataTransferObjects.COCO;

public class CocoInfo
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("url")]
    public string Url { get; set; } = "";

    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0";

    [JsonPropertyName("year")]
    public int Year { get; set; } = DateTime.Now.Year;

    [JsonPropertyName("contributor")]
    public string Contributor { get; set; } = "";

    [JsonPropertyName("date_created")]
    public string DateCreated { get; set; } = DateTime.Now.ToString("yyyy/MM/dd");
}