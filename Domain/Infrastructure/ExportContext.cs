using System.Collections.Generic;
using ImageAnnotationTool.Domain.Entities;

namespace ImageAnnotationTool.Domain.Infrastructure;

public sealed record ExportContext(string OutputDir, IReadOnlyDictionary<ClassData, int> ClassMap);