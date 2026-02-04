using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ImageAnnotationTool.Enums;

public enum ImageSortMode
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
    
    [Description("Largest first")]
    LargestFirst,
    
    [Description("Smallest first")]
    SmallestFirst,
    
    [Description("Most annotations first")]
    MostAnnotations,
    
    [Description("Fewest annotations first")]
    FewestAnnotations,
    
    [Description("Most distinct classes first")]
    MostTotalClasses,
    
    [Description("Fewest distinct classes first")]
    FewestTotalClasses,
    
    [Description("Extensions alphabetically")]
    ExtensionAlphabetical,
    
    [Description("Extensions alphabetically, reversed")]
    ExtensionReversedAlphabetical
}