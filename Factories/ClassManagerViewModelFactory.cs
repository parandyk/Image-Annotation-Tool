using HanumanInstitute.MvvmDialogs;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Interfaces;
using ClassManagerViewModel = ImageAnnotationTool.ViewModels.ClassManagerViewModel;
using MainViewModel = ImageAnnotationTool.ViewModels.MainViewModel;

namespace ImageAnnotationTool.Factories;

public class ClassManagerViewModelFactory : IClassManagerViewModelFactory
{
    private readonly IClassStore _classStore;
    private readonly IStatisticsAggregator _statisticsAggregator;
    private readonly INotificationSettings _notificationSettings;
    private readonly IAppMessenger _appMessenger;
    private readonly IDialogWrapper _dialogWrapper;
    
    public ClassManagerViewModelFactory(IClassStore classStore,
        IStatisticsAggregator statisticsAggregator,
        INotificationSettings notificationSettings,
        IAppMessenger appMessenger,
        IDialogWrapper dialogWrapper)
    {
        _classStore = classStore;
        _statisticsAggregator = statisticsAggregator;
        _notificationSettings = notificationSettings;
        _appMessenger = appMessenger;
        _dialogWrapper = dialogWrapper;
    }
    
    public ClassManagerViewModel Create()
    {
        return new ClassManagerViewModel(
            _classStore, 
            _statisticsAggregator,
            _notificationSettings,
            _dialogWrapper,
            _appMessenger);
    }
}