using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageAnnotationTool.Domain.Infrastructure.ExportContexts;
using ImageAnnotationTool.Enums;

namespace ImageAnnotationTool.Domain.Infrastructure.ExportStrategies;

public class TxtExportStrategy : IClassExportStrategy
{
    public ClassFormat Format => ClassFormat.Txt;
    public async Task ExportAsync(ClassExportContext context)
    {
        var time = DateTime.Now;
        var classesDir = context.OutputDir + "classes_" +
                         time.ToString("yyyy-MM-dd_HH_mm_ss") +
                         Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        
        Directory.CreateDirectory(classesDir);
        
        await File.WriteAllLinesAsync(Path.Combine(classesDir, "classes.txt"), 
            context.Classes.Select(c => c.Name));
    }
}