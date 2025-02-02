using System.Windows.Controls;
using LibUIPythonAI.ViewModel.Common;

namespace LibUIPythonAI.View.Common{
    /// <summary>
    /// MyStatusBar.xaml の相互作用ロジック
    /// </summary>
    public partial class MyStatusBar : UserControl {
        public MyStatusBar() {
            // DataContextにViewModelを設定
            MyStatusBarViewModel myStatusBarViewModel = new();
            DataContext = myStatusBarViewModel;
            InitializeComponent();

        }
    }
}
