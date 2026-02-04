using System.Collections.Generic;

namespace ImageAnnotationTool.Domain.Infrastructure;

public class CompositeCommand : IUndoableCommand //TODO
{
    private readonly List<IUndoableCommand> _commands;
    
    public void Execute()
    {
        throw new System.NotImplementedException();
    }

    public void Undo()
    {
        throw new System.NotImplementedException();
    }
}