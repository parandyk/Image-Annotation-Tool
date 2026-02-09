using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ImageAnnotationTool.Domain.DataTransferObjects.COCO;

public class CocoRootDto
{
    [JsonPropertyName("info")]
    public CocoInfo Info { get; set; } = new();

    [JsonPropertyName("images")]
    public List<CocoImageDto> Images { get; set; } = new();

    [JsonPropertyName("categories")]
    public List<CocoCategoryDto> Categories { get; set; } = new();
    
    [JsonPropertyName("annotations")]
    public List<CocoAnnotationDto> Annotations { get; set; } = new();
}