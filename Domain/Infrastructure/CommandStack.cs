using System;
using System.Collections.Generic;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ImageAnnotationTool.Domain.DomainCommands;

namespace ImageAnnotationTool.Domain.Infrastructure;

public class CommandStack : ObservableObject, ICommandStack
{
    private readonly Stack<IUndoableCommand> _undoStack = new Stack<IUndoableCommand>();
    private readonly Stack<IUndoableCommand> _redoStack = new Stack<IUndoableCommand>();
    
    public event Action? StacksChanged;

    public bool UndoStackNotEmpty => _undoStack.Count != 0;
    public bool RedoStackNotEmpty => _redoStack.Count != 0;

    public void Execute(IPersistentCommand cmd)
    {
        cmd.Execute();
    }
    
    public void Execute(IUndoableCommand cmd)
    {
        cmd.Execute();
        _undoStack.Push(cmd);
        _redoStack.Clear();
        
        ChangeInStacks();
    }

    public void Undo()
    {
        if (_undoStack.Count == 0)
            return;
        
        var cmd = _undoStack.Pop();
        cmd.Undo();
        _redoStack.Push(cmd);
        
        ChangeInStacks();
    }

    public void Redo()
    {
        if (_redoStack.Count == 0)
            return;
        
        var cmd = _redoStack.Pop();
        cmd.Execute();
        _undoStack.Push(cmd);

        ChangeInStacks();
    }

    private void ChangeInStacks()
    {
        OnPropertyChanged(nameof(UndoStackNotEmpty));
        OnPropertyChanged(nameof(RedoStackNotEmpty));
        
        StacksChanged?.Invoke();
    }
}