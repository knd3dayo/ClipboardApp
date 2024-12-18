using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using ClipboardApp.Factory;
using ClipboardApp.Model;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Search;
using ClipboardApp.Utils;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.VectorDBView;
using ClipboardApp.ViewModel;
using ClipboardApp.ViewModel.Folder;
using ClipboardApp.ViewModel.Search;
using QAChat;
using QAChat.Control;
using QAChat.Resource;
using WpfAppCommon.Control.Editor;


namespace ClipboardApp {
    public partial class MainWindowViewModel : ClipboardAppViewModelBase {
        public MainWindowViewModel() { }
        public void Init() {

            ActiveInstance = this;

            // ProgressIndicatorの表示更新用のアクションをセット
            UpdateProgressCircleVisibility = (visible) => {
                IsIndeterminate = visible;
            };
            ClipboardAppPythonAILibConfigParams configParams = new();
            PythonAILibManager.Init(configParams);
            QAChatManager.Init(configParams);
            // フォルダの初期化
            InitClipboardFolders();

            // データベースのチェックポイント処理
            ClipboardAppFactory.Instance.GetClipboardDBController().GetDatabase().Checkpoint();

            // DBのバックアップの取得
            BackupController.BackupNow();

            // ClipboardControllerのOnClipboardChangedに処理をセット
            ClipboardController.OnClipboardChanged = (e) => {
                // CopiedItemsをクリア
                CopiedObjects.Clear();
            };
        }

        private void InitClipboardFolders() {
            RootFolderViewModel = new ClipboardFolderViewModel(ClipboardFolder.RootFolder);
            ImageCheckRootFolderViewModel = new ImageCheckFolderViewModel(ClipboardFolder.ImageCheckRootFolder);
            SearchRootFolderViewModel = new SearchFolderViewModel(ClipboardFolder.SearchRootFolder);
            ChatRootFolderViewModel = new ChatFolderViewModel(ClipboardFolder.ChatRootFolder);
            ClipboardItemFolders.Add(RootFolderViewModel);
            ClipboardItemFolders.Add(SearchRootFolderViewModel);
            ClipboardItemFolders.Add(ChatRootFolderViewModel);
            ClipboardItemFolders.Add(ImageCheckRootFolderViewModel);

            OnPropertyChanged(nameof(ClipboardItemFolders));
        }

        public static MainWindowViewModel ActiveInstance { get; set; } = new MainWindowViewModel();

        // RootFolderのClipboardViewModel
        // Null非許容を無視
        [AllowNull]
        public ClipboardFolderViewModel RootFolderViewModel { get; private set; }

        // 画像チェックフォルダのClipboardViewModel
        // Null非許容を無視
        [AllowNull]
        public ImageCheckFolderViewModel ImageCheckRootFolderViewModel { get; private set; }

        // 検索フォルダのClipboardViewModel
        // Null非許容を無視
        [AllowNull]
        public SearchFolderViewModel SearchRootFolderViewModel { get; private set; }

        // チャットフォルダのClipboardViewModel
        // Null非許容を無視
        [AllowNull]
        public ChatFolderViewModel ChatRootFolderViewModel { get; private set; }


        // ClipboardController
        public static ClipboardController ClipboardController { get; } = new();

        // プログレスインジケータ表示更新用のアクション
        // 
        public static Action<bool> UpdateProgressCircleVisibility { get; set; } = (visible) => { };

        // Progress Indicatorの表示フラグ
        private bool _IsIndeterminate = false;
        public bool IsIndeterminate {
            get {
                return _IsIndeterminate;
            }
            set {
                _IsIndeterminate = value;
                OnPropertyChanged(nameof(IsIndeterminate));
            }
        }

        // クリップボード監視が開始されている場合は「停止」、停止されている場合は「開始」を返す
        public string ClipboardMonitorButtonText {
            get {
                return IsClipboardMonitor ? StringResources.StopClipboardWatch : StringResources.StartClipboardWatch;
            }
        }
        // クリップボード監視を開始、終了するフラグ
        public bool IsClipboardMonitor { get; set; } = false;

        // Windows通知監視が開始されている場合は「停止」、停止されている場合は「開始」を返す
        public string WindowsNotificationMonitorButtonText {
            get {
                return IsWindowsNotificationMonitor ? StringResources.StopNotificationWatch : StringResources.StartNotificationWatch;
            }
        }
        // Windows通知監視が開始、終了するフラグ
        public bool IsWindowsNotificationMonitor { get; set; } = false;

        // ClipboardFolder

        public ObservableCollection<ClipboardFolderViewModel> ClipboardItemFolders { get; set; } = [];

        // Cutフラグ
        public enum CutFlagEnum {
            None,
            Item,
            Folder
        }
        public CutFlagEnum CutFlag { get; set; } = CutFlagEnum.None;

        // 選択中のアイテム(複数選択)
        private ObservableCollection<ClipboardItemViewModel> _selectedItems = [];
        public ObservableCollection<ClipboardItemViewModel> SelectedItems {
            get {
                return _selectedItems;

            }
            set {
                _selectedItems = value;

                OnPropertyChanged(nameof(SelectedItems));
            }
        }

        // 選択中のアイテム
        private ClipboardItemViewModel? _selectedItem;
        public ClipboardItemViewModel? SelectedItem {
            get {
                return _selectedItem;
            }
            set {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        // 選択中のフォルダ
        private ClipboardFolderViewModel? _selectedFolder;
        public ClipboardFolderViewModel? SelectedFolder {
            get {

                return _selectedFolder;
            }
            set {
                _selectedFolder = value;
                OnPropertyChanged(nameof(SelectedFolder));
            }
        }
        /// <summary>
        /// コピーされたアイテム
        /// </summary>
        // Ctrl + C or X が押された時のClipboardItem or ClipboardFolder
        public List<object> CopiedObjects { get; set; } = [];

        /// <summary>
        /// コピーされたアイテムのフォルダ
        /// </summary>
        // Ctrl + C or X  が押された時のClipboardItemFolder
        private ClipboardFolderViewModel? _copiedFolder;
        public ClipboardFolderViewModel? CopiedFolder {
            get {
                return _copiedFolder;
            }
            set {
                _copiedFolder = value;
                OnPropertyChanged(nameof(CopiedFolder));
            }
        }

        // テキストを右端で折り返すかどうか
        public bool TextWrapping {
            get {
                return ClipboardAppConfig.Instance.TextWrapping == System.Windows.TextWrapping.Wrap;
            }
            set {
                if (value) {
                    ClipboardAppConfig.Instance.TextWrapping = System.Windows.TextWrapping.Wrap;
                } else {
                    ClipboardAppConfig.Instance.TextWrapping = System.Windows.TextWrapping.NoWrap;
                }
                // Save
                ClipboardAppConfig.Instance.Save();
                OnPropertyChanged(nameof(TextWrapping));
                if (TextWrapping) {
                    CommonViewModelBase.TextWrappingMode = MyTextBox.TextWrappingModeEnum.WrapWithThreshold;
                } else {
                    if (value) {
                        CommonViewModelBase.TextWrappingMode = MyTextBox.TextWrappingModeEnum.Wrap;
                    } else {
                        CommonViewModelBase.TextWrappingMode = MyTextBox.TextWrappingModeEnum.NoWrap;
                    }
                }
            }
        }
        // AutoTextWrapping
        public bool AutoTextWrapping {
            get {
                return ClipboardAppConfig.Instance.AutoTextWrapping;
            }
            set {
                ClipboardAppConfig.Instance.AutoTextWrapping = value;
                // Save
                ClipboardAppConfig.Instance.Save();
                OnPropertyChanged(nameof(AutoTextWrapping));
                // CommonViewModelBaseのTextWrappingModeを更新
                if (value) {
                    CommonViewModelBase.TextWrappingMode = MyTextBox.TextWrappingModeEnum.WrapWithThreshold;
                } else {
                    if (TextWrapping) {
                        CommonViewModelBase.TextWrappingMode = MyTextBox.TextWrappingModeEnum.Wrap;
                    } else { 
                        CommonViewModelBase.TextWrappingMode = MyTextBox.TextWrappingModeEnum.NoWrap;
                    }
                }
            }
        }

        // プレビューモード　プレビューを表示するかどうか
        public static Visibility PreviewModeVisibility {
            get {
                return ClipboardAppConfig.Instance.PreviewMode ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        //　プレビューモード表示するかどうか
        public bool PreviewMode {
            get {
                return ClipboardAppConfig.Instance.PreviewMode;
            }
            set {
                ClipboardAppConfig.Instance.PreviewMode = value;
                // Save
                ClipboardAppConfig.Instance.Save();

                OnPropertyChanged(nameof(PreviewMode));
                OnPropertyChanged(nameof(PreviewModeVisibility));
                // アプリケーション再起動後に反映されるようにメッセージを表示
                MessageBox.Show(StringResources.DisplayModeWillChangeWhenYouRestartTheApplication, StringResources.Information, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 外部からプロパティの変更を通知する
        /// </summary>
        /// <param name="propertyName"></param>
        public void NotifyPropertyChanged(string propertyName) {
            OnPropertyChanged(propertyName);
        }

        // 開発中の機能を有効にするかどうか
        public bool EnableDevFeatures {
            get {
                return ClipboardAppConfig.Instance.EnableDevFeatures;
            }
            set {
                ClipboardAppConfig.Instance.EnableDevFeatures = value;
                // Save
                ClipboardAppConfig.Instance.Save();
                OnPropertyChanged(nameof(EnableDevFeatures));
                OnPropertyChanged(nameof(EnableDevFeaturesVisibility));
            }
        }
        // 開発中機能の表示
        public Visibility EnableDevFeaturesVisibility {
            get {
                return ClipboardAppConfig.Instance.EnableDevFeatures ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public static QAChatStartupProps CreateQAChatStartupProps(ClipboardItem clipboardItem) {

            SearchRule rule = ClipboardFolder.GlobalSearchCondition.Copy();

            QAChatStartupProps props = new(clipboardItem) {
                // フォルダ選択アクション
                SelectVectorDBItemAction = (vectorDBItems) => {
                    SelectVectorDBWindow.OpenSelectVectorDBWindow(ActiveInstance.RootFolderViewModel, true, (selectedItems) => {
                        foreach (var item in selectedItems) {
                            vectorDBItems.Add(item);
                        }
                    });

                },
                // Saveアクション
                SaveCommand = (item, saveChatHistory) => {
                    clipboardItem = (ClipboardItem)item;
                    // ClipboardItemを保存
                    clipboardItem.Save();
                    if (saveChatHistory) {
                        // チャット履歴用のItemの設定
                        ClipboardFolder chatFolder = MainWindowViewModel.ActiveInstance.ChatRootFolderViewModel.ClipboardItemFolder;
                        ClipboardItem chatHistoryItem = new(chatFolder.Id);
                        clipboardItem.CopyTo(chatHistoryItem);
                        // タイトルを日付 + 元のタイトルにする
                        chatHistoryItem.Description = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Chat";
                        if (!string.IsNullOrEmpty(clipboardItem.Description)) {
                            chatHistoryItem.Description += " " + clipboardItem.Description;
                        }
                        chatHistoryItem.Save();
                    }

                },
                // ExportChatアクション
                ExportChatCommand = (chatHistory) => {
                    ClipboardFolderViewModel? folderViewModel = MainWindowViewModel.ActiveInstance.SelectedFolder ?? MainWindowViewModel.ActiveInstance.RootFolderViewModel;

                    FolderSelectWindow.OpenFolderSelectWindow(folderViewModel, (folder) => {
                        ClipboardItem chatHistoryItem = new(folder.ClipboardItemFolder.Id);
                        // タイトルを日付 + 元のタイトルにする
                        chatHistoryItem.Description = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Chat";
                        if (!string.IsNullOrEmpty(clipboardItem.Description)) {
                            chatHistoryItem.Description += " " + clipboardItem.Description;
                        }
                        // chatHistoryItemの内容をテキスト化
                        string chatHistoryText = "";
                        foreach (var item in chatHistory) {
                            chatHistoryText += $"--- {item.Role} ---\n";
                            chatHistoryText += item.ContentWithSources + "\n\n";
                        }
                        chatHistoryItem.Content = chatHistoryText;
                        chatHistoryItem.Save();

                    });

                }
            };

            return props;
        }

    }

}
