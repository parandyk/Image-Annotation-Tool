using System.ComponentModel;
using ImageAnnotationTool.Domain.Entities;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IClassProvider : INotifyPropertyChanged
{
    ClassData ActiveClass { get; set; }
    
    // ClassData DefaultClass { get; set; }
}