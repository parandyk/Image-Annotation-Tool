using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Classes;

public class ChangeVisibilityClassCommand : IUndoableCommand
{
    private readonly ClassData _classData;
    private readonly bool _initialVisibility;
    private readonly bool _newVisibility;

    public ChangeVisibilityClassCommand(ClassData classData, bool? newVisibility)
    {
        _classData = classData;
        
        _initialVisibility = _classData.IsVisible;
        if (newVisibility == null)
        {
            _newVisibility = !_classData.IsVisible;
        }
        else
        {
            _newVisibility = newVisibility.Value;
        }
    }
    
    public void Execute()
    {
        _classData.ChangeVisibility(_newVisibility);
    }

    public void Undo()
    {
        _classData.ChangeVisibility(_initialVisibility);
    }
}