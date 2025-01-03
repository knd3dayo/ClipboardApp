using System.Windows;
using PythonAILib.Model.Content;
using PythonAILib.Model.Search;
using QAChat.ViewModel.Search;

namespace QAChat.View.Search {
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class SearchWindow : Window {
        public SearchWindow() {
            InitializeComponent();
        }

        public static void OpenSearchWindow(SearchRule searchConditionRule,
            ContentFolder searchFolder, bool isSearchFolder, Action afterUpdate) {
            SearchWindow searchWindow = new() {
                DataContext = new SearchWindowViewModel(searchConditionRule, searchFolder, isSearchFolder, afterUpdate)
            };
            searchWindow.ShowDialog();
        }

    }
}
