namespace ImageAnnotationTool.Domain.Infrastructure.SettingsStore;

public interface INotificationSettings : INotificationSettingsProvider
{
    void ChangeSuppressionLocalClassInstanceDeletionDialogs(bool state);
    void ChangeSuppressionGlobalClassInstanceDeletionDialogs(bool state);
    void ChangeSuppressionAnnotationDeletionDialogs(bool state);
    // void ChangeSuppressionLocalClassInstanceSwapDialogs(bool state);
    // void ChangeSuppressionGlobalClassInstanceSwapDialogs(bool state);
    void ChangeSuppressionClassDeletionDialogs(bool state);
    void ChangeSuppressionImageDeletionDialogs(bool state);
}