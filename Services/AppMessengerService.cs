using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using ImageAnnotationTool.Interfaces;
using ImageAnnotationTool.Messages;
using ImageAnnotationTool.Models;

namespace ImageAnnotationTool.Services;

public sealed class AppMessengerService : IAppMessenger
{
    public static AppMessengerService Instance { get; } = new();

    public void SendAbortOperationRequest()
    {
        WeakReferenceMessenger.Default.Send(new AbortOperationMessage());
    }
    
    public void SendErrorOccurredNotification(string? error)
    {
        WeakReferenceMessenger.Default.Send(new ErrorOccurredMessage(error));
    }

    public void Register<TMessage>(object recipient, MessageHandler<object, TMessage> handler) 
        where TMessage : class
    {
        WeakReferenceMessenger.Default.Register(recipient, handler);
    }
}