using System;
using System.Data;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Classes;

public class ModifyColorClassCommand : IUndoableCommand
{
    private readonly ClassData _classData;
    private readonly string _newClassHexColor;
    private readonly string _initialClassHexColor;
    
    public ModifyColorClassCommand(IClassDataPolicy policy, ClassData classData, string newClassHexColor)
    {
        if (!policy.IsColorValid(newClassHexColor))
        {
            throw new ConstraintException("Provided color is invalid.", new ArgumentOutOfRangeException(nameof(newClassHexColor)));
        }
        
        _classData = classData;
        _initialClassHexColor = classData.HexColor;
        _newClassHexColor = newClassHexColor;
    }
    
    public void Execute()
    {
        _classData.ChangeColor(_newClassHexColor);
    }

    public void Undo()
    {
        _classData.ChangeColor(_initialClassHexColor);
    }
}