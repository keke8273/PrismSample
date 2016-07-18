using System.Windows;

namespace QBR.Infrastructure.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Shows the message box.
        /// </summary>
        /// <param name="messageBoxText">The message box text.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <param name="icon">The icon.</param>
        /// <returns></returns>
        MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon);

        /// <summary>
        /// Shows the folder browser dialog.
        /// </summary>
        /// <returns></returns>
        string ShowFolderBrowserDialog();

        /// <summary>
        /// Shows the open file dialog.
        /// </summary>
        /// <param name="fileFilter">The file filter.</param>
        /// <param name="multiSelect">if set to <c>true</c> [multi select].</param>
        /// <param name="readOnlyChecked">if set to <c>true</c> [read only checked].</param>
        /// <param name="showReadOnly">if set to <c>true</c> [show read only].</param>
        /// <returns></returns>
        string[] ShowOpenFileDialog(string fileFilter, bool multiSelect = false, bool readOnlyChecked = false,
                                  bool showReadOnly = false);
    }
}
