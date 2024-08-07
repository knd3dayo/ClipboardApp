using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ClipboardApp.View.SearchView;
using ClipboardApp.ViewModel;

namespace ClipboardApp.View.TagView
{
    /// <summary>
    /// TagSearchWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TagSearchWindow : Window {
        public TagSearchWindow() {
            InitializeComponent();
        }

        public static void OpenTagSearchWindow(Action<string, bool> afterUpdate) {
            TagSearchWindow tagSearchWindow = new TagSearchWindow();
            TagSearchWindowViewModel searchWindowViewModel = (TagSearchWindowViewModel)tagSearchWindow.DataContext;
            searchWindowViewModel.Initialize(afterUpdate);
            tagSearchWindow.Show();
        }
    }
}
