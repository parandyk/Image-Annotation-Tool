using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using ImageAnnotationTool.Core;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Enums;
using ImageAnnotationTool.Interfaces;
using ImageAnnotationTool.Messages;
using ImageAnnotationTool.Models;


namespace ImageAnnotationTool.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IAppMessenger _messenger;

    public MainViewModel(IWorkspaceManagerViewModelFactory wmVmFactory,
        ISettingsManagerViewModelFactory smVmFactory,
        IAppMessenger messenger)
    {
        _messenger = messenger;
        _messenger.Register<ErrorOccurredMessage>(this, OnErrorOccurredNotification);

        WorkspaceManagerVM = wmVmFactory.Create();
        SettingsManagerVM = smVmFactory.Create();
    }

    private void OnErrorOccurredNotification(object recipient, ErrorOccurredMessage message)
    {
        ErrorMessagesNotifications?.Clear();
        
        if (message.error is not null)
            ErrorMessagesNotifications?.Add(message.error);
    }
    
    public WorkspaceManagerViewModel WorkspaceManagerVM { get; }
    
    public SettingsManagerViewModel SettingsManagerVM { get; }

    // [ObservableProperty] 
    // private string? _annotationFormat;

    // [ObservableProperty]
    // private bool _hideUnannotated = false;
    //
    // [ObservableProperty]
    // private bool _hideAnnotated = false;
}