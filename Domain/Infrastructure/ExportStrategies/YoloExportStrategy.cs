using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure.ExportContexts;
using ImageAnnotationTool.Enums;

namespace ImageAnnotationTool.Domain.Infrastructure.ExportStrategies;

public class YoloExportStrategy : IAnnotationExportStrategy
{
    public AnnotationFormat Format => AnnotationFormat.Yolo;
    public async Task ExportAsync(AnnotationExportContext context)
    {
        var time = DateTime.Now;
        var datasetDir = context.OutputDir + "dataset_" +
                         time.ToString("yyyy-MM-dd_HH_mm_ss") +
                         Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        
        var imagesDir = Path.Combine(datasetDir, "images");
        var labelsDir = Path.Combine(datasetDir, "labels");
        
        Directory.CreateDirectory(datasetDir);
        Directory.CreateDirectory(imagesDir);
        Directory.CreateDirectory(labelsDir);
        
        await File.WriteAllLinesAsync(Path.Combine(datasetDir, "classes.txt"), 
            context.Classes.Select(c => c.Name));
        
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
                
                // lines.Add(string.Format(
                //     $"{classId} {cx:F6} {cy:F6} {w:F6} {h:F6}"));
            }
            
            var labelFilename = Path.ChangeExtension(image.Filename, ".txt");
            await File.WriteAllLinesAsync(Path.Combine(labelsDir, labelFilename), lines);
        }
    }
}