using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ImageAnnotationTool.ViewModels;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Services;
using ImageAnnotationTool.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using Avalonia.Platform.Storage;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure.ExportStrategies;
using ImageAnnotationTool.Domain.Infrastructure.SettingsStore;
using ImageAnnotationTool.Domain.Infrastructure.UseCases;
using ImageAnnotationTool.Factories;
using ImageAnnotationTool.Parsers;
using DialogService = HanumanInstitute.MvvmDialogs.Avalonia.DialogService;
using IAnnotationViewModelFactory = ImageAnnotationTool.Factories.IAnnotationViewModelFactory;
using IClassListProvider = ImageAnnotationTool.Domain.Infrastructure.IClassListProvider;
using IImageViewModelFactory = ImageAnnotationTool.Factories.IImageViewModelFactory;

namespace ImageAnnotationTool;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new Views.MainWindow();
            
            var servicesCollection = new ServiceCollection();
            
            servicesCollection.AddSingleton<AnnotationWorkspace>();
            servicesCollection.AddSingleton<IWorkspaceDomainInterface>(sp => sp.GetRequiredService<AnnotationWorkspace>());
            servicesCollection.AddSingleton<IWorkspaceDomainClassInterface>(sp => sp.GetRequiredService<AnnotationWorkspace>());
            servicesCollection.AddSingleton<IWorkspaceDomainImageInterface>(sp => sp.GetRequiredService<AnnotationWorkspace>());
            servicesCollection.AddSingleton<IWorkspaceDomainAnnotationInterface>(sp => sp.GetRequiredService<AnnotationWorkspace>());
            
            servicesCollection.AddSingleton<IDialogService>(sp => new DialogService(new DialogManager(
                    dialogFactory: new DialogFactory()), //.AddFluent()
                viewModelFactory: sp.GetRequiredService));
            servicesCollection.AddTransient<DeleteClassDialogViewModel>();
            servicesCollection.AddTransient<ChangeClassDialogViewModel>();
            servicesCollection.AddSingleton<SettingsDialogViewModel>();
            // servicesCollection.AddSingleton<SettingsDialogViewModel>(sp => sp.GetRequiredService<SettingsDialogViewModel>());
            servicesCollection.AddSingleton<IStorageProvider>(x => mainWindow.StorageProvider);
            
            servicesCollection.AddSingleton<ExportAnnotationsUseCase>();
            servicesCollection.AddSingleton<ExportClassesUseCase>();
            
            servicesCollection.AddSingleton<UseCaseService>();
            servicesCollection.AddSingleton<IUseCaseProvider>(sp => sp.GetRequiredService<UseCaseService>());
            servicesCollection.AddSingleton<IExportUseCaseInterface>(sp => sp.GetRequiredService<UseCaseService>());
            servicesCollection.AddSingleton<IImportUseCaseInterface>(sp => sp.GetRequiredService<UseCaseService>());

            servicesCollection.AddSingleton<IAnnotationExportStrategy, YoloExportStrategy>();
            
            servicesCollection.AddSingleton<AppModeSettings>();
            servicesCollection.AddSingleton<IAppModeSettings>(sp => sp.GetRequiredService<AppModeSettings>());
            servicesCollection.AddSingleton<IAppModeSettingsProvider>(sp => sp.GetRequiredService<AppModeSettings>());
            servicesCollection.AddSingleton<IPersistentAppModeSettings>(sp => sp.GetRequiredService<AppModeSettings>());
            servicesCollection.AddSingleton<ITransientAppModeSettings>(sp => sp.GetRequiredService<AppModeSettings>());
            
            servicesCollection.AddSingleton<RenderingSettings>();
            servicesCollection.AddSingleton<IRenderingSettings>(sp => sp.GetRequiredService<RenderingSettings>());
            servicesCollection.AddSingleton<IRenderingSettingsProvider>(sp => sp.GetRequiredService<RenderingSettings>());
            servicesCollection.AddSingleton<IPersistentRenderingSettings>(sp => sp.GetRequiredService<RenderingSettings>());
            servicesCollection.AddSingleton<ITransientRenderingSettings>(sp => sp.GetRequiredService<RenderingSettings>());
            
            servicesCollection.AddSingleton<InteractionSettings>();
            servicesCollection.AddSingleton<IInteractionSettings>(sp => sp.GetRequiredService<InteractionSettings>());
            servicesCollection.AddSingleton<IInteractionSettingsProvider>(sp => sp.GetRequiredService<InteractionSettings>());
            
            servicesCollection.AddSingleton<NotificationSettings>();
            servicesCollection.AddSingleton<INotificationSettings>(sp => sp.GetRequiredService<NotificationSettings>());
            servicesCollection.AddSingleton<INotificationSettingsProvider>(sp => sp.GetRequiredService<NotificationSettings>());
            
            servicesCollection.AddSingleton<IStatisticsAggregator, StatisticsAggregator>();
            servicesCollection.AddSingleton<IWorkspaceCommandFactory, WorkspaceCommandFactory>();
            servicesCollection.AddSingleton<DialogWrapper>();
            servicesCollection.AddSingleton<IClassDialogWrapper>(sp => sp.GetRequiredService<DialogWrapper>());
            servicesCollection.AddSingleton<IImageDialogWrapper>(sp => sp.GetRequiredService<DialogWrapper>());
            servicesCollection.AddSingleton<ISettingsDialogWrapper>(sp => sp.GetRequiredService<DialogWrapper>());
            servicesCollection.AddSingleton<IAnnotationDialogWrapper>(sp => sp.GetRequiredService<DialogWrapper>());
            servicesCollection.AddSingleton<IDialogWrapper>(sp => sp.GetRequiredService<DialogWrapper>());
            servicesCollection.AddSingleton<ICommandStack, CommandStack>();
            servicesCollection.AddSingleton<ClassDataPolicy>();
            servicesCollection.AddSingleton<IColorAssigner>(sp => sp.GetRequiredService<ClassDataPolicy>());
            servicesCollection.AddSingleton<IClassDataPolicy>(sp => sp.GetRequiredService<ClassDataPolicy>());
            servicesCollection.AddSingleton<ClassStore>();
            servicesCollection.AddSingleton<IClassListProvider>(sp => sp.GetRequiredService<ClassStore>());
            servicesCollection.AddSingleton<IClassProvider>(sp => sp.GetRequiredService<ClassStore>());
            servicesCollection.AddSingleton<IClassStore>(sp => sp.GetRequiredService<ClassStore>());

            servicesCollection.AddSingleton<IAppMessenger, AppMessengerService>();
            servicesCollection.AddSingleton<ISettingsManagerViewModelFactory, SettingsManagerViewModelFactory>();
            servicesCollection.AddSingleton<IWorkspaceManagerViewModelFactory, WorkspaceManagerViewModelFactory>();
            servicesCollection.AddSingleton<IClassManagerViewModelFactory, ClassManagerViewModelFactory>();
            servicesCollection.AddSingleton<IAnnotationViewModelFactory, AnnotationViewModelFactory>();
            servicesCollection.AddSingleton<IImageManagerViewModelFactory, ImageManagerViewModelFactory>();
            servicesCollection.AddSingleton<IImageViewModelFactory, ImageViewModelFactory>();
            servicesCollection.AddSingleton<IFolderParser, FolderParsingService>();
            servicesCollection.AddSingleton<IImageParser, ImageParsingService>();
            servicesCollection.AddSingleton<IClassParser, ClassParsingService>();
            servicesCollection.AddSingleton<IAnnotationParser, AnnotationParsingService>();
            servicesCollection.AddSingleton<IFileParsingProvider, FileParsingService>();
            servicesCollection.AddSingleton<IFileAccessProvider, FileAccessService>();
            servicesCollection.AddSingleton<MainViewModel>();
            
            Services = servicesCollection.BuildServiceProvider();
            var viewModel = Services.GetRequiredService<MainViewModel>();
            
            mainWindow.DataContext = viewModel;
            desktop.MainWindow = mainWindow;
            
            GC.KeepAlive(typeof(DialogService));
            
            var dialogWrapper = Services.GetRequiredService<IDialogWrapper>();
            dialogWrapper.SetDefaultOwner(viewModel);
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    public new static App? Current => Application.Current as App;

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
    /// </summary>
    public IServiceProvider? Services { get; private set; }
}