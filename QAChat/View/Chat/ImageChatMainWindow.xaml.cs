using System.Windows;
using PythonAILib.Model.Content;
using QAChat.ViewModel.ImageChat;
using WpfAppCommon.Model;

namespace QAChat.View.Chat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ImageChatMainWindow : Window {
        public ImageChatMainWindow() {
            InitializeComponent();
        }
        public static void OpenMainWindow(ContentItem clipboardItem, Action afterUpdate) {
            ImageChatMainWindow imageEvidenceCheckerWindow = new();
            imageEvidenceCheckerWindow.DataContext = new ImageChatMainWindowViewModel(clipboardItem, afterUpdate);
            imageEvidenceCheckerWindow.Show();
        }
    }
}