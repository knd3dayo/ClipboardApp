using System.Windows;
using QAChat.Control;
using QAChat.ViewModel;

namespace QAChat.View.QAChatMain {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class QAChatMainWindow : Window {
        public QAChatMainWindow() {
            InitializeComponent();
        }

        public static void OpenOpenAIChatWindow(QAChatStartupProps props) {
            QAChat.View.QAChatMain.QAChatMainWindow openAIChatWindow = new();
            MainWindowViewModel mainWindowViewModel = new(props);
            openAIChatWindow.DataContext = mainWindowViewModel;

            openAIChatWindow.Show();
        }
    }
}