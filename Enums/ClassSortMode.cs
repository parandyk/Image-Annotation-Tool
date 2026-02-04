using System.ComponentModel;

namespace ImageAnnotationTool.Enums;

public enum ClassSortMode
{
    [Description("Default")]
    None,
    
    [Description("Alphabetically")]
    Alphabetical,
    
    [Description("Alphabetically, reversed")]
    ReversedAlphabetical,
    
    [Description("Oldest")]
    Oldest,
    
    [Description("Newest")]
    Newest,
    
    [Description("#Instances descending")]
    CountDescending,
    
    [Description("#Instances ascending")]
    CountAscending,
}