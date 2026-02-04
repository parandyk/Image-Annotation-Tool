using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Annotations;

public class ChangeAnchoringAnnotationCommand : IUndoableCommand
{
    private readonly Annotation _annotation;
    private readonly bool _initialAnchoring;
    private readonly bool _newAnchoring;

    public ChangeAnchoringAnnotationCommand(Annotation annotation, bool? newAnchoring)
    {
        _annotation = annotation;
        _initialAnchoring = annotation.IsAnchored;
        if (newAnchoring == null)
        {
            _newAnchoring = !annotation.IsAnchored;
        }
        else
        {
            _newAnchoring = newAnchoring.Value;
        }
    }
    
    public void Execute()
    {
        _annotation.ChangeAnchored(_newAnchoring);
    }

    public void Undo()
    {
        _annotation.ChangeAnchored(_initialAnchoring);
    }
}