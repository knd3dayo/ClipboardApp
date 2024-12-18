using System.Windows;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Search;
using ClipboardApp.ViewModel;
using ClipboardApp.ViewModel.Search;

namespace ClipboardApp.View.SearchView
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class SearchWindow : Window {
        public SearchWindow() {
            InitializeComponent();
        }

        public static void OpenSearchWindow(SearchRule searchConditionRule,
            ClipboardFolder searchFolder, bool isSearchFolder, Action afterUpdate) {
            SearchWindow searchWindow = new() {
                DataContext = new SearchWindowViewModel(searchConditionRule, searchFolder, isSearchFolder, afterUpdate)
            };
            searchWindow.ShowDialog();
        }

    }
}
