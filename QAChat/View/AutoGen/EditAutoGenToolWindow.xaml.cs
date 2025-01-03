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
using PythonAILib.Model.AutoGen;
using QAChat.ViewModel.AutoGen;

namespace QAChat.View.AutoGen {
    /// <summary>
    /// EditAutoGenToolWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditAutoGenToolWindow : Window {
        public EditAutoGenToolWindow() {
            InitializeComponent();
        }

        public static void OpenWindow(AutoGenTool autoGenTool, Action afterUpdate) {
            var window = new EditAutoGenToolWindow();
            window.DataContext = new EditAutoGenToolViewModel(autoGenTool, afterUpdate);
            window.ShowDialog();
        }
    }
}
