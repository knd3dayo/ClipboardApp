using System.Collections.ObjectModel;
using System.Windows;
using LibUIPythonAI.ViewModel.Item;
using MergeChat.ViewModel;
using LibUIPythonAI.ViewModel.Folder;

namespace MergeChat.View {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MergeChatMainWindow : Window {
        public MergeChatMainWindow() {
            InitializeComponent();
        }

        public static void OpenWindow(ContentFolderViewModel folderViewModel, ObservableCollection<ContentItemViewModel> selectedItems) {
            MergeChat.View.MergeChatMainWindow openAIChatWindow = new();
            MergeChatWindowViewModel mainWindowViewModel = new(folderViewModel, selectedItems);
            openAIChatWindow.DataContext = mainWindowViewModel;

            openAIChatWindow.Show();
        }
    }
}