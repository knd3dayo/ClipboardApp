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
using ClipboardApp.ViewModel;

namespace ClipboardApp.View.AutoProcessRuleView
{
    /// <summary>
    /// ListAutoProcessRuleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ListAutoProcessRuleWindow : Window
    {
        public ListAutoProcessRuleWindow()
        {
            InitializeComponent();
        }
        public static void OpenListAutoProcessRuleWindow(MainWindowViewModel viewModel) {
            ListAutoProcessRuleWindow listAutoProcessRuleWindow = new();
            ListAutoProcessRuleWindowViewModel listAutoProcessRuleWindowViewModel = (ListAutoProcessRuleWindowViewModel)listAutoProcessRuleWindow.DataContext;
            listAutoProcessRuleWindowViewModel.Initialize(viewModel);
            listAutoProcessRuleWindow.ShowDialog();
        }
    }
}
