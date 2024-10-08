using System.Windows;
using ClipboardApp.ViewModel;

namespace QAChat.View.TagView {
    /// <summary>
    /// TagSearchWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TagSearchWindow : Window {
        public TagSearchWindow() {
            InitializeComponent();
        }

        public static void OpenTagSearchWindow(Action<string, bool> afterUpdate) {
            TagSearchWindow tagSearchWindow = new TagSearchWindow();
            TagSearchWindowViewModel searchWindowViewModel =new (afterUpdate);
            tagSearchWindow.DataContext = searchWindowViewModel;
            tagSearchWindow.Show();
        }
    }
}
