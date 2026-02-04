using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ImageAnnotationTool.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    protected ViewModelBase()
    {
        ErrorMessagesNotifications = new ObservableCollection<string>();
    }
    
    [ObservableProperty] 
    private ObservableCollection<string>? _errorMessagesNotifications;
}
