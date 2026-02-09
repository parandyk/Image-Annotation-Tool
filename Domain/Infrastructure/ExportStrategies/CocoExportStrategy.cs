using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ImageAnnotationTool.Domain.DataTransferObjects.COCO;
using ImageAnnotationTool.Domain.ExportableObjects;
using ImageAnnotationTool.Domain.Infrastructure.ExportContexts;
using ImageAnnotationTool.Enums;

namespace ImageAnnotationTool.Domain.Infrastructure.ExportStrategies;

public class CocoExportStrategy : IAnnotationExportStrategy
{
    public AnnotationFormat Format => AnnotationFormat.Coco;
    
    public async Task ExportAsync(AnnotationExportContext context)
    {
        var datasetDir = CreateDatasetDirectory(context.OutputDir);
        
        int annotationIdCounter = 1; 
        int imageIdCounter = 1;
        int catIdCounter = 1;
        
        var root = new CocoRootDto
        {
            Info = new CocoInfo
            {
                Description = $"Export of {context.Images.Count()} images",
                Version = "1.0", 
                Year = DateTime.Now.Year,
                DateCreated = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                // Contributor = Environment.UserName
            }
        };

        var categoryMap = new Dictionary<string, int>();

        foreach (var classSnapshot in context.Classes)
        {
            var className = classSnapshot.Name;
            root.Categories.Add(
                new CocoCategoryDto
                {
                    Id = catIdCounter, 
                    Name = className
                });
            
            categoryMap[className] = catIdCounter;
            catIdCounter++;
        }
        
        foreach (var image in context.Images)
        {
            var imgId = imageIdCounter++;
            
            var imgDto = new CocoImageDto
            {
                Id = imgId, 
                FileName = image.Filename,
                Width = image.Width,
                Height = image.Height
            };
            
            root.Images.Add(imgDto);
            
            foreach (var annotation in image.Annotations)
            {
                var coordinates = annotation.GetCocoCoordinates();
                List<double> coordinateList = [coordinates.x, coordinates.y, coordinates.w, coordinates.h];
                
                var annDto = new CocoAnnotationDto
                {
                    Id = annotationIdCounter++,
                    ImageId = imgId,
                    CategoryId = categoryMap[annotation.Label],
                    Bbox = coordinateList,
                    Area = annotation.Area,
                };
                
                root.Annotations.Add(annDto);
            }
        }
    
        var options = new JsonSerializerOptions 
        { 
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var jsonPath = Path.Combine(datasetDir, "instances_default.json");
        
        await using var stream = File.Create(jsonPath);
        await JsonSerializer.SerializeAsync(stream, root, options);
    }
        
    private string CreateDatasetDirectory(string outputDir)
    {
        var time = DateTime.Now;
        var datasetDir = outputDir + "dataset_" +
                         time.ToString("yyyy-MM-dd_HH_mm_ss") +
                         Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();

        Directory.CreateDirectory(datasetDir);
        return datasetDir;
    }
}