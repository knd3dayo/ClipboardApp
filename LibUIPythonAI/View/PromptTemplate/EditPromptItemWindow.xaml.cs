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
using LibUIPythonAI.ViewModel.PromptTemplate;

namespace LibUIPythonAI.View.PromptTemplate {
    /// <summary>
    /// EditItemWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditPromptItemWindow : Window
    {
        public EditPromptItemWindow()
        {
            InitializeComponent();
        }
        public static void OpenEditPromptItemWindow(PromptItemViewModel promptItemViewModel, Action<PromptItemViewModel> callback) {
            EditPromptItemWindow editPromptItemWindow = new();
            EditPromptItemWindowViewModel editPromptItemWindowViewModel = new(promptItemViewModel, callback);
            editPromptItemWindow.DataContext = editPromptItemWindowViewModel;
            editPromptItemWindow.ShowDialog();
        }
    }
}
