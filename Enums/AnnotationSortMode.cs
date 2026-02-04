using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ImageAnnotationTool.Enums;

public enum AnnotationSortMode
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
    
    [Description("Largest first")]
    LargestFirst,
    
    [Description("Smallest first")]
    SmallestFirst
}