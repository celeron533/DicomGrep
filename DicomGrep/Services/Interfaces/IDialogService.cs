using System.Windows;

namespace DicomGrep.Services.Interfaces
{
    public interface IDialogService
    {
        MessageBoxResult ShowMessageBox(string messageBoxText, string caption = "Message", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None, MessageBoxResult defaultResult = MessageBoxResult.OK, MessageBoxOptions options = MessageBoxOptions.None);
        void ShowView(Window view);
        bool? ShowViewDialog(Window view);
    }
}