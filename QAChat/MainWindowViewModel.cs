using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using PythonAILib.Model;
using QAChat.Model;
using QAChat.View.PromptTemplateWindow;
using WpfAppCommon.Control.QAChat;
using WpfAppCommon.Control.Settings;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat {
    public partial class MainWindowViewModel : MyWindowViewModel {

        public class QAChatStartupProps {
            public QAChatStartupProps(ClipboardFolder clipboardFolder, ClipboardItem clipboardItem, bool isStartFromInternalApp) {
                ClipboardFolder = clipboardFolder;
                ClipboardItem = clipboardItem;
                IsStartFromInternalApp = isStartFromInternalApp;
            }

            public ClipboardFolder ClipboardFolder { get; set; }
            public ClipboardItem ClipboardItem { get; set; }


            // 内部アプリから起動されたか否か
            public bool IsStartFromInternalApp { get; set; }

            public Action<Action<List<ClipboardItem>>> SearchWindowAction = (action) => { };

            public Action<Action<List<ClipboardItem>>> ContentTextFromClipboardItemsAction = (action) => { };

            public Action<ClipboardItem> OpenClipboardItemAction = (clipboardItem) => { };

            public Action<VectorDBItem> OpenVectorDBItemAction = (vectorDBItem) => { };

        }


        // OnActivatedAction
        public override void OnActivatedAction() {
            if (ClipboardFolder == null) {
                return;
            }
            // StatusText.Readyにフォルダ名を設定
            Tools.StatusText.ReadyText = $"フォルダ名:[{ClipboardFolder?.FolderName}]";
            // StatusText.Textにフォルダ名を設定
            Tools.StatusText.Text = $"フォルダ名:[{ClipboardFolder?.FolderName}]";
        }
        private ClipboardFolder? _ClipboardFolder;
        public ClipboardFolder? ClipboardFolder {
            get {
                return _ClipboardFolder;
            }
            set {
                _ClipboardFolder = value;
                OnPropertyChanged(nameof(ClipboardFolder));
            }
        }
        public Action<ClipboardItem> OpenClipboardItemAction { 
            get {
                return QAChatControlViewModel.OpenClipboardItemAction;
            }
            set {
                QAChatControlViewModel.OpenClipboardItemAction = value;
                OnPropertyChanged(nameof(OpenClipboardItemAction));
            }
        }
        // OpenVectorDBItemAction
        public Action<VectorDBItem> OpenVectorDBItemAction {
            get {
                return QAChatControlViewModel.OpenVectorDBItemAction;
            }
            set {
                QAChatControlViewModel.OpenVectorDBItemAction = value;
                OnPropertyChanged(nameof(OpenVectorDBItemAction));
            }
        }

        //初期化
        public void Initialize(QAChatStartupProps props) {
            // PythonAILibのLogWrapperのログ出力設定
            PythonAILib.Utils.LogWrapper.SetActions(LogWrapper.Info, LogWrapper.Warn, LogWrapper.Error);
            QAChatControlViewModel.Initialize(props.ClipboardItem, PromptTemplateCommandExecute);
            IsStartFromInternalApp = props.IsStartFromInternalApp;
            ShowSearchWindowAction = props.SearchWindowAction;
            SetContentTextFromClipboardItemsAction = props.ContentTextFromClipboardItemsAction;
            OpenClipboardItemAction = props.OpenClipboardItemAction;
            OpenVectorDBItemAction = props.OpenVectorDBItemAction;

        }
        public Action<Action<List<ClipboardItem>>> ShowSearchWindowAction {
            get {
                return QAChatControlViewModel.ShowSearchWindowAction;
            }
            set {
                QAChatControlViewModel.ShowSearchWindowAction = value;
                OnPropertyChanged(nameof(ShowSearchWindowAction));
            }
        }
        // フォルダ内のClipboardItemを選択するアクション
        // ★ Actionにしなくてもいいかも
        public Action<Action<List<ClipboardItem>>> SetContentTextFromClipboardItemsAction {
            get {
                return QAChatControlViewModel.SetContentTextFromClipboardItemsAction;
            }
            set {
                QAChatControlViewModel.SetContentTextFromClipboardItemsAction = value;
                OnPropertyChanged(nameof(SetContentTextFromClipboardItemsAction));
            }
        }


        // 選択中のフォルダの全てのClipboardItem
        public ObservableCollection<ClipboardItem> ClipboardItems {
            get {
                return QAChatControlViewModel.ClipboardItems;
            }
            set {
                QAChatControlViewModel.ClipboardItems = value;
                OnPropertyChanged(nameof(ClipboardItems));
            }
        }


        // QAChatControlのViewModel
        public QAChatControlViewModel QAChatControlViewModel { get; set; } = new();

        // 内部から起動されたか否か
        private bool isStartFromInternalApp = true;
        public bool IsStartFromInternalApp {
            get {
                return isStartFromInternalApp;
            }
            set {
                isStartFromInternalApp = value;
                OnPropertyChanged(nameof(IsStartFromInternalApp));
            }
        }

        // 設定画面を開くコマンド
        public SimpleDelegateCommand<object> SettingCommand => new((parameter) => {
            // SettingUserControlを生成してWindowを表示する。
            SettingsUserControl settingsControl = new();
            Window window = new() {
                SizeToContent = SizeToContent.Height,
                Title = CommonStringResources.Instance.SettingWindowTitle,
                Content = settingsControl
            };
            window.ShowDialog();
        }

        );

        private void PromptTemplateCommandExecute(object parameter) {
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum.Select, (promptTemplateWindowViewModel, Mode) => {
                QAChatControlViewModel.PromptText = promptTemplateWindowViewModel.PromptItem.Prompt;

            });
        }

    }
}
