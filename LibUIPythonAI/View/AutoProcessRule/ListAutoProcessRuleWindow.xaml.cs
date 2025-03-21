using System.Collections.ObjectModel;
using System.Windows;
using LibUIPythonAI.ViewModel.AutoProcess;
using LibUIPythonAI.ViewModel.Folder;

namespace LibUIPythonAI.View.AutoProcessRule {
    /// <summary>
    /// ListAutoProcessRuleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ListAutoProcessRuleWindow : Window {
        public ListAutoProcessRuleWindow() {
            InitializeComponent();
        }
        public static void OpenListAutoProcessRuleWindow(ObservableCollection<ContentFolderViewModel> viewModel) {
            ListAutoProcessRuleWindow listAutoProcessRuleWindow = new();
            listAutoProcessRuleWindow.DataContext = new ListAutoProcessRuleWindowViewModel(viewModel);
            listAutoProcessRuleWindow.ShowDialog();
        }
    }
}
