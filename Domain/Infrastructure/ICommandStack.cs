using System;
using ImageAnnotationTool.Domain.DomainCommands;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface ICommandStack
{
    // Stack<IUndoableCommand> UndoStack { get; }
    // Stack<IUndoableCommand> RedoStack { get; }

    event Action? StacksChanged;
    bool UndoStackNotEmpty { get; }
    bool RedoStackNotEmpty { get; }
    void Execute(IPersistentCommand cmd);
    void Execute(IUndoableCommand cmd);
    void Undo();
    void Redo();
}