using System;
using System.Collections.Generic;
using System.ComponentModel;
using ImageAnnotationTool.Domain.Entities;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IStatisticsAggregator : INotifyPropertyChanged
{
    // int AnnotationTotalCount { get; }
    // int ClassTotalCount { get; }
    // int ImageTotalCount { get; }
    
    bool? GlobalClassVisibility { get; }
    bool? GlobalAnnotationVisibility { get; }
    bool? GlobalAnnotationAnchoring { get; }
    
    IReadOnlyDictionary<Guid, int> AnnotationsPerImageCounts { get; }
    IReadOnlyDictionary<Guid, int> ClassesPerImageCounts { get; }
    IReadOnlyDictionary<Guid, int> AnnotationsPerClassCounts { get; }
    IReadOnlyDictionary<Guid, IReadOnlyDictionary<Guid, int>> SpecificClassPerImageCounts { get; }
}