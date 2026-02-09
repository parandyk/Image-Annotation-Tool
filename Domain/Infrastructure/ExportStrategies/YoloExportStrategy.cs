using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageAnnotationTool.Domain.ExportableObjects;
using ImageAnnotationTool.Domain.Infrastructure.ExportContexts;
using ImageAnnotationTool.Enums;

namespace ImageAnnotationTool.Domain.Infrastructure.ExportStrategies;

public class YoloExportStrategy : IAnnotationExportStrategy
{
    public AnnotationFormat Format => AnnotationFormat.Yolo;
    public async Task ExportAsync(AnnotationExportContext context)
    {
        var (datasetDir, imagesDir, labelsDir) = CreateDirectories(context.OutputDir);
        
        await ExportDataset(context.Classes, datasetDir);
        await ExportClasses(context.Classes, datasetDir);
        await ExportAnnotations(context, imagesDir, labelsDir);
    }

    private (string dataset, string images, string labels) CreateDirectories(string outputDir)
    {
        var time = DateTime.Now;
        var datasetDir = outputDir + "dataset_" +
                         time.ToString("yyyy-MM-dd_HH_mm_ss") +
                         Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        
        var imagesDir = Path.Combine(datasetDir, "images");
        var labelsDir = Path.Combine(datasetDir, "labels");
        
        Directory.CreateDirectory(datasetDir);
        Directory.CreateDirectory(imagesDir);
        Directory.CreateDirectory(labelsDir);
        
        return (datasetDir, imagesDir, labelsDir);
    }
        
    private static async Task ExportClasses(IList<ExportableClass> classes, string path)
    {
        var outputPath = Path.Combine(path, "classes.txt");
        await File.WriteAllLinesAsync(outputPath, classes.Select(c => c.Name));
    }

    private static async Task ExportAnnotations(AnnotationExportContext context, string imagesDir, string labelsDir)
    {
        var classMap = context.Classes
            .Select((c, idx) => (c.Name, idx))
            .ToDictionary(x => x.Name, x => x.idx);

        foreach (var image in context.Images)
        {
            var destImgPath = Path.Combine(imagesDir, image.Filename);
            File.Copy(image.Path, destImgPath, overwrite: true);
            
            var lines = new List<string>();
            foreach (var annotation in image.Annotations)
            {
                if (!classMap.TryGetValue(annotation.Label, out int classId))
                    continue;

                var (cx, cy, w, h) = annotation.GetYoloCoordinates();
                
                lines.Add(string.Format(CultureInfo.InvariantCulture, 
                    "{0} {1:F6} {2:F6} {3:F6} {4:F6}",
                    classId, cx, cy, w, h));
            }
            
            var labelFilename = Path.ChangeExtension(image.Filename, ".txt");
            await File.WriteAllLinesAsync(Path.Combine(labelsDir, labelFilename), lines);
        }
    }
    
    private static async Task ExportDataset(IList<ExportableClass> classes, string path)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("path: .");
        sb.AppendLine("train: images");
        sb.AppendLine("val: images");
        sb.AppendLine();
        
        sb.AppendLine($"nc: {classes.Count}");
        sb.AppendLine("names:");
        
        for (int i = 0; i < classes.Count; i++)
        {
            var name = classes[i].Name;
            sb.AppendLine($"  {i}: '{name}'"); 
        }

        var yamlPath = Path.Combine(path, "data.yaml");
        await File.WriteAllTextAsync(yamlPath, sb.ToString());
    }
}