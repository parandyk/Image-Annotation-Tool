using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Annotations;

public class ChangeVisibilityAnnotationCommand : IUndoableCommand
{
    private readonly Annotation _annotation;
    private readonly bool _initialVisibility;
    private readonly bool _newVisibility;

    public ChangeVisibilityAnnotationCommand(Annotation annotation, bool? newVisibility)
    {
        _annotation = annotation;
        _initialVisibility = annotation.IsVisible;
        if (newVisibility == null)
        {
            _newVisibility = !annotation.IsVisible;
        }
        else
        {
            _newVisibility = newVisibility.Value;
        }
    }
    
    public void Execute()
    {
        _annotation.ChangeVisibility(_newVisibility);
    }

    public void Undo()
    {
        _annotation.ChangeVisibility(_initialVisibility);
    }
}