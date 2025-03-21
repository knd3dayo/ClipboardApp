using System.Windows;
using LibUIPythonAI.ViewModel.Folder;

namespace LibUIPythonAI.View.Folder {
    /// <summary>
    /// ExportImportWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ExportImportWindow : Window {
        public ExportImportWindow() {
            InitializeComponent();
        }

        public static void OpenExportImportFolderWindow(ContentFolderViewModel clibpboardFolderViewModel, Action afterUpdate) {

            ExportImportWindow exportImportWindow = new() {
                DataContext = new ExportImportWindowViewModel(clibpboardFolderViewModel, afterUpdate)
            };
            exportImportWindow.ShowDialog();
        }
    }
}
