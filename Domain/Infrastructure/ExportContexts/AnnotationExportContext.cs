using System;
using System.Collections.Generic;
using ImageAnnotationTool.Domain.ExportableObjects;
using ImageAnnotationTool.Enums;

namespace ImageAnnotationTool.Domain.Infrastructure.ExportContexts;

public sealed record AnnotationExportContext(string OutputDir,
    AnnotationFormat Format,
    IEnumerable<ExportableImage> Images,
    IList<ExportableClass> Classes);