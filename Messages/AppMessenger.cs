using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging.Messages;
using ImageAnnotationTool.Models;

namespace ImageAnnotationTool.Messages;


public record AbortOperationMessage { }

public record ErrorOccurredMessage(string? error);
