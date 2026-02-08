namespace ImageAnnotationTool.Domain.DomainCommands;

public interface IUndoableCommand : IDomainCommand
{
    void Undo();
}