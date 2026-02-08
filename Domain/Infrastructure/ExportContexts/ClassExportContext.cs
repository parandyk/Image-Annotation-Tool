using System.Collections.Generic;
using ImageAnnotationTool.Domain.ExportableObjects;
using ImageAnnotationTool.Enums;

namespace ImageAnnotationTool.Domain.Infrastructure.ExportContexts;

public sealed record ClassExportContext(string OutputDir, 
    ClassFormat Format,
    IEnumerable<ExportableClass> Classes);