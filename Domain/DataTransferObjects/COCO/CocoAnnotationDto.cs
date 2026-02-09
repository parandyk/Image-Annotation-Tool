using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ImageAnnotationTool.Domain.DataTransferObjects.COCO;

public class CocoAnnotationDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("image_id")]
    public int ImageId { get; set; }

    [JsonPropertyName("category_id")]
    public int CategoryId { get; set; }
    
    [JsonPropertyName("bbox")]
    public List<double> Bbox { get; set; }

    [JsonPropertyName("area")]
    public double Area { get; set; }

    [JsonPropertyName("iscrowd")]
    public int IsCrowd { get; set; } = 0;
}