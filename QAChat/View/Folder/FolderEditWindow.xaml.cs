using System.Windows;
using QAChat.ViewModel.Folder;

namespace QAChat.View.Folder {
    /// <summary>
    /// FolderEditWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class FolderEditWindow : Window {
        public FolderEditWindow() {
            InitializeComponent();
        }
        public static void OpenFolderEditWindow(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            FolderEditWindow folderEditWindow = new() {
                DataContext = new FolderEditWindowViewModel(folderViewModel, afterUpdate)
            };
            folderEditWindow.ShowDialog();
        }
    }
}