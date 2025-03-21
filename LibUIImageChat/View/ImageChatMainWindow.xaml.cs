using System.Windows;
using LibPythonAI.Model.Content;
using LibUIImageChat.ViewModel;

namespace LibUIImageChat.View {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ImageChatWindow : Window {
        public ImageChatWindow() {
            InitializeComponent();
        }
        public static void OpenMainWindow(ContentItemWrapper clipboardItem, Action afterUpdate) {
            ImageChatWindow imageEvidenceCheckerWindow = new();
            imageEvidenceCheckerWindow.DataContext = new ImageChatWindowViewModel(clipboardItem, afterUpdate);
            imageEvidenceCheckerWindow.Show();
        }
    }
}