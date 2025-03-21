using System.Windows;
using LibUIPythonAI.ViewModel.Tag;
using LibPythonAI.Model.Content;

namespace LibUIPythonAI.View.Tag
{
    /// <summary>
    /// Tag.xaml の相互作用ロジック
    /// </summary>
    public partial class TagWindow : Window {
        public TagWindow() {
            InitializeComponent();
        }
        public static void OpenTagWindow(ContentItemWrapper? contentItem, Action action) {
            TagWindow tagWindow = new();
            TagWindowViewModel tagWindowViewModel =new(contentItem, action);
            tagWindow.DataContext = tagWindowViewModel;
            tagWindow.ShowDialog();
        }
    }
}
