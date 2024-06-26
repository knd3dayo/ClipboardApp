using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAppCommon.Model;

namespace ImageChat {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }
        public static void OpenMainWindow(ClipboardItem? clipboardItem, bool isStartFromInternalApp) {
            ImageChat.MainWindow imageEvidenceCheckerWindow = new();
            ImageChat.MainWindowViewModel imageEvidenceCheckerWindowViewModel = (ImageChat.MainWindowViewModel)imageEvidenceCheckerWindow.DataContext;
            // Initialize
            imageEvidenceCheckerWindowViewModel.Initialize(clipboardItem, isStartFromInternalApp);

            imageEvidenceCheckerWindow.Show();
        }
    }
}