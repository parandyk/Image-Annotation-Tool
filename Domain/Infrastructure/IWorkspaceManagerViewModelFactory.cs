using ImageAnnotationTool.ViewModels;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IWorkspaceManagerViewModelFactory
{
    WorkspaceManagerViewModel Create();
}