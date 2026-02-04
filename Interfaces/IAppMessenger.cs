using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using ImageAnnotationTool.Models;

namespace ImageAnnotationTool.Interfaces;

public interface IAppMessenger
{
    void SendAbortOperationRequest();
    
    void SendErrorOccurredNotification(string? eventText);

    void Register<TMessage>(object recipient, MessageHandler<object, TMessage> handler)
        where TMessage : class;
}