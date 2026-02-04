namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IUndoableCommand : IDomainCommand
{
    void Undo();
}