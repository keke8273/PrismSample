using System.Windows;
using System.Windows.Forms;
using QBR.Infrastructure.Interfaces;
using MessageBox = System.Windows.MessageBox;

namespace QBR.Shell.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class DialogService : IDialogService
    {
        #region Member Variables
        #endregion

        #region Constructors
        #endregion

        #region Properties
        #endregion

        #region Functions
        #endregion

        #region Enums
        #endregion

        /// <summary>
        /// Shows the message box.
        /// </summary>
        /// <param name="messageBoxText">The message box text.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <param name="icon">The icon.</param>
        /// <returns></returns>
        public MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            return MessageBox.Show(messageBoxText, caption, button, icon);
        }

        /// <summary>
        /// Shows the folder browser dialog.
        /// </summary>
        /// <returns></returns>
        public string ShowFolderBrowserDialog()
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            var selectedPath = string.Empty;

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                selectedPath = folderBrowserDialog.SelectedPath;
            }

            return selectedPath;
        }

        /// <summary>
        /// Shows the open file dialog.
        /// </summary>
        /// <param name="fileFilter">The file filter.</param>
        /// <param name="multiSelect">if set to <c>true</c> [multi select].</param>
        /// <param name="readOnlyChecked">if set to <c>true</c> [read only checked].</param>
        /// <param name="showReadOnly">if set to <c>true</c> [show read only].</param>
        /// <returns></returns>
        public string[] ShowOpenFileDialog(string fileFilter, bool multiSelect = false, bool readOnlyChecked = false,
                                         bool showReadOnly = false)
        {
            var openFileDialog = new OpenFileDialog()
                {
                    Filter = fileFilter,
                    Multiselect = multiSelect,
                    ReadOnlyChecked = readOnlyChecked,
                    ShowReadOnly = showReadOnly
                };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileNames;
            }

            return new string[]{};
        }
    }
}