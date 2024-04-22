﻿using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.Model;
using ClipboardApp.PythonIF;
using ClipboardApp.Utils;
using ClipboardApp.View.AutoProcessRuleView;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.View.PythonScriptView;
using ClipboardApp.View.StatusMessageView;
using ClipboardApp.View.TagView;
using CommunityToolkit.Mvvm.ComponentModel;


namespace ClipboardApp {

    public class MainWindowViewModel : ObservableObject {
        public static MainWindowViewModel? Instance = null;

        private static MainWindow MainWindow = (MainWindow)Application.Current.MainWindow;

        public MainWindowViewModel() {
            // データベースのチェックポイント処理
            ClipboardDatabaseController.GetClipboardDatabase().Checkpoint();

            // ロギング設定
            Tools.StatusText = StatusText;


            // クリップボードコントローラーの初期化
            // SearchConditionをLiteDBから取得
            SearchConditionRule? searchConditionRule = ClipboardDatabaseController.GetSearchConditionRule(ClipboardDatabaseController.SEARCH_CONDITION_APPLIED_CONDITION_NAME);
            if (searchConditionRule != null) {
                ClipboardItemFolder.GlobalSearchCondition = searchConditionRule;
            }

            // フォルダ階層を再描写する
            ReloadFolder();

            Instance = this;
            // クリップボード監視機能の初期化
            ClipboardController.Init(this);

            // Python処理機能の初期化
            PythonExecutor.Init();

            // バックアップ処理を実施
            BackupController.Init();

            // コンテキストメニューの初期化
            InitContextMenu();


        }
        private void InitContextMenu() {
            // コンテキストメニューの初期化
            ClipboardItemContextMenuItems.Add(new ClipboardAppMenuItem("開く", OpenSelectedItemCommand, "Ctrl+O"));

            ClipboardItemContextMenuItems.Add(new ClipboardAppMenuItem("ファイルとして開く", OpenSelectedItemAsFileCommand, "Ctrl+Shit+O"));
            ClipboardItemContextMenuItems.Add(new ClipboardAppMenuItem("新規ファイルとして開く", OpenSelectedItemAsNewFileCommand, "Ctrl+Shit+Alt+O"));
            ClipboardItemContextMenuItems.Add(new ClipboardAppMenuItem("ピン留め", ChangePinCommand));

            ClipboardItemContextMenuItems.Add(new ClipboardAppMenuItem("コピー", CopyToClipboardCommand, "Ctrl+C"));
            ClipboardItemContextMenuItems.Add(new ClipboardAppMenuItem("削除", DeleteSelectedItemCommand, "Delete"));

            // サブメニュー設定
            ClipboardAppMenuItem pythonMenuItems = new ClipboardAppMenuItem("便利機能", SimpleDelegateCommand.EmptyCommand);
            pythonMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("ファイルのパスを分割", SplitFilePathCommand));
            pythonMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("テキストを抽出", ClipboardItemViewModel.ExtractTextCommand));
            pythonMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("データをマスキング", ClipboardItemViewModel.MaskDataCommand));
            pythonMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("データを整形", FormatTextCommand));
            pythonMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("OpenAIチャット",
                new SimpleDelegateCommand((parameter) => {
                    // 選択中のアイテムがない場合は処理をしない
                    if (SelectedItem == null) {
                        Tools.Error("選択中のアイテムがない");
                        return;
                    }
                    ClipboardItemCommands.OpenAIChatCommandExecute(SelectedItem);
                })));

            // Pythonスクリプト(ユーザー定義)
            foreach (ScriptItem scriptItem in PythonExecutor.ScriptItems) {

                pythonMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem(scriptItem.Description, new SimpleDelegateCommand((parameter) => {
                    ClipboardItemCommands.MenuItemRunPythonScriptCommandExecute(scriptItem);
                })
                ));
            }
            ClipboardItemContextMenuItems.Add(pythonMenuItems);
        }

        // フォルダ階層を再描写する
        public void ReloadFolder() {
            ClipboardItemFolders.Clear();
            ClipboardItemFolders.Add(new ClipboardItemFolderViewModel(ClipboardItemFolder.RootFolder));
            ClipboardItemFolders.Add(new ClipboardItemFolderViewModel(ClipboardItemFolder.SearchRootFolder));
            OnPropertyChanged("ClipboardItemFolders");
        }
        // ClipboardItemを再描写する
        public void ReloadClipboardItems() {
            if (SelectedFolder == null) {
                return;
            }
            SelectedFolder.Load();
            ListBox? listBox = MainWindow.FindName("listBox1") as ListBox;
            // ListBoxの先頭にスクロール
            if (listBox?.Items.Count > 0) {
                listBox?.ScrollIntoView(listBox.Items[0]);

            }
        }

        // クリップボード監視開始終了フラグを反転させる
        public SimpleDelegateCommand ToggleClipboardMonitor => new((parameter) => {
            IsClipboardMonitor = !IsClipboardMonitor;
            OnPropertyChanged("ClipboardMonitorButtonText");
        });

        // クリップボード監視が開始されている場合は「停止」、停止されている場合は「開始」を返す
        public string ClipboardMonitorButtonText {
            get {
                return IsClipboardMonitor ? "停止" : "開始";
            }
        }
        // クリップボード監視を開始、終了するフラグ
        private bool _isClipboardMonitor = false;
        public bool IsClipboardMonitor {
            get {
                return _isClipboardMonitor;
            }
            set {
                _isClipboardMonitor = value;
                OnPropertyChanged("IsClipboardMonitor");
                if (value) {
                    ClipboardController.Start();
                    Tools.Info("クリップボード監視を開始しました");
                } else {
                    ClipboardController.Stop();
                    Tools.Info("クリップボード監視を停止しました");
                }
            }
        }
        // ClipboardItemFolder

        public static ObservableCollection<ClipboardItemFolderViewModel> ClipboardItemFolders { get; set; } = new ObservableCollection<ClipboardItemFolderViewModel>();

        // Cutフラグ
        public bool CutFlag { get; set; } = false;
        // 選択中のアイテム(複数選択)
        public ObservableCollection<ClipboardItemViewModel> SelectedItems { get; set; } = new ObservableCollection<ClipboardItemViewModel>();

        // 選択中のアイテム
        private ClipboardItemViewModel? _selectedItem = null;
        public ClipboardItemViewModel? SelectedItem {
            get {
                return _selectedItem;
            }
            set {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }
        // 選択中のフォルダ
        private ClipboardItemFolderViewModel? _selectedFolder = new ClipboardItemFolderViewModel(ClipboardItemFolder.RootFolder);
        public ClipboardItemFolderViewModel? SelectedFolder {
            get {
                return _selectedFolder;
            }
            set {
                _selectedFolder = value;
                _selectedFolder?.Load();

                OnPropertyChanged("SelectedFolder");
            }
        }
        // Ctrl + C or X が押された時のClipboardItem
        public List<ClipboardItemViewModel> CopiedItems { get; set; } = new List<ClipboardItemViewModel>();

        // Ctrl + C or X  が押された時のClipboardItemFolder
        public ClipboardItemFolderViewModel? CopiedItemFolder { get; set; } = null;

        //-----
        // ClipboardItemContextMenuItems
        //-----
        public ObservableCollection<ClipboardAppMenuItem> ClipboardItemContextMenuItems { get; set; } = new ObservableCollection<ClipboardAppMenuItem>();

        public ObservableCollection<ClipboardAppMenuItem> ClipboardItemFolderContextMenuItems { get; set; } = new ObservableCollection<ClipboardAppMenuItem>();

        // static 

        // ステータスバーのテキスト
        public static StatusText StatusText { get; } = new StatusText();

        //--------------------------------------------------------------------------------
        // コマンド
        //--------------------------------------------------------------------------------
        // フォルダが選択された時の処理
        public SimpleDelegateCommand FolderSelectionChangedCommand => new((parameter) => {
            RoutedEventArgs routedEventArgs = (RoutedEventArgs)parameter;
            TreeView treeView = (TreeView)routedEventArgs.OriginalSource;
            ClipboardItemFolderViewModel clipboardItemFolderViewModel = (ClipboardItemFolderViewModel)treeView.SelectedItem;
            SelectedFolder = clipboardItemFolderViewModel;
        });
        // クリップボードアイテムが選択された時の処理
        public SimpleDelegateCommand ClipboardItemSelectionChangedCommand => new((parameter) => {
            RoutedEventArgs routedEventArgs = (RoutedEventArgs)parameter;
            ListBox listBox = (ListBox)routedEventArgs.OriginalSource;
            ClipboardItemViewModel clipboardItemViewModel = (ClipboardItemViewModel)listBox.SelectedItem;
            SelectedItems.Clear();
            foreach (ClipboardItemViewModel item in listBox.SelectedItems) {
                SelectedItems.Add(item);
            }
            SelectedItem = clipboardItemViewModel;
        });

        // OpenOpenAIWindowCommand メニューの「OpenAIチャット」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand OpenOpenAIWindowCommand => new((parameter) => {
            ClipboardItemCommands.OpenAIChatCommandExecute(null);
        });

        // Ctrl + Q が押された時の処理
        public static SimpleDelegateCommand ExitCommand => new((parameter) => {
            // 終了確認ダイアログを表示。Yesならアプリケーションを終了
            MessageBoxResult result = MessageBox.Show("終了しますか?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {

                Application.Current.Shutdown();
            }
        });

        // Ctrl + F が押された時の処理
        public SimpleDelegateCommand SearchCommand => new((parameter) => {
            // 選択中のフォルダがない場合でも処理をする
            ClipboardFolderCommands.SearchCommandExecute(SelectedFolder);
        });


        // Ctrl + R が押された時の処理
        public SimpleDelegateCommand ReloadCommand => new((parameter) => {
            if (SelectedFolder == null) {
                return;
            }
            ClipboardFolderCommands.ReloadCommandExecute(SelectedFolder);
        });

        // Ctrl + Delete が押された時の処理 選択中のフォルダのアイテムを削除する
        public SimpleDelegateCommand DeleteDisplayedItemCommand => new((parameter) => {
            if (SelectedFolder == null) {
                Tools.Error("フォルダが選択されていません");
                return;
            }
            ClipboardFolderCommands.DeleteDisplayedItemCommandExecute(SelectedFolder);
        });


        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public SimpleDelegateCommand DeleteSelectedItemCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems.Count == 0) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }

            ClipboardItemCommands.DeleteSelectedItemCommandExecute(SelectedFolder, SelectedItems);
        });


        // Ctrl + N が押された時の処理
        public SimpleDelegateCommand CreateItemCommand => new((parameter) => {
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("フォルダが選択されていません");
                return;
            }
            ClipboardItemCommands.CreateItemCommandExecute(SelectedFolder);
        });

        // メニューの「設定」をクリックしたときの処理
        public static SimpleDelegateCommand SettingCommand => new((parameter) => {
            SettingWindow settingWindow = new();
            settingWindow.ShowDialog();
        });

        // ピン留めの切り替え処理 複数アイテム処理可能
        public SimpleDelegateCommand ChangePinCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems == null || SelectedItems.Count == 0) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }
            ClipboardItemCommands.ChangePinCommandExecute(SelectedFolder, SelectedItems);
        });
        // 選択中のアイテムを開く処理 複数アイテム処理不可
        public SimpleDelegateCommand OpenSelectedItemCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItem == null) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }
            ClipboardItemCommands.OpenItemCommandExecute(SelectedFolder, SelectedItem);
        });
        // 選択したアイテムをテキストファイルとして開く処理 複数アイテム処理不可
        public SimpleDelegateCommand OpenSelectedItemAsFileCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItem == null) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            ClipboardItemCommands.OpenSelectedItemAsFileCommandExecute(SelectedItem);
        });

        // 選択したアイテムを新規として開く処理 複数アイテム処理不可
        public SimpleDelegateCommand OpenSelectedItemAsNewFileCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItem == null) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            ClipboardItemCommands.OpenSelectedItemAsNewFileCommandExecute(SelectedItem);
        });

        // Ctrl + X が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand CutItemCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems.Count == 0) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }
            // Cut Flagを立てる
            CutFlag = true;
            // CopiedItemsに選択中のアイテムをセット
            CopiedItems.Clear();
            foreach (ClipboardItemViewModel item in SelectedItems) {
                CopiedItems.Add(item);
            }
            CopiedItemFolder = SelectedFolder;
            Tools.Info("切り取りしました");

        });
        // Ctrl + C が押された時の処理
        public SimpleDelegateCommand CopyToClipboardCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (Instance!.SelectedItem == null) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (Instance!.SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }
            // Cutフラグをもとに戻す
            CutFlag = false;
            // CopiedItemsに選択中のアイテムをセット
            CopiedItems.Clear();
            foreach (ClipboardItemViewModel item in SelectedItems) {
                CopiedItems.Add(item);
            }
            CopiedItemFolder = SelectedFolder;
            try {
                ClipboardController.CopyToClipboard(Instance.SelectedItem.ClipboardItem);
                Tools.Info("コピーしました");

            } catch (Exception e) {
                string message = $"エラーが発生しました。\nメッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}";
                Tools.Error(message);
            }

        });
        // Ctrl + V が押された時の処理
        public SimpleDelegateCommand PasteFromClipboardCommand => new((parameter) => {
            // コピー元のアイテムがない場合は処理をしない
            if (CopiedItems.Count == 0) {
                Tools.Info("コピー元のアイテムがない");
                return;
            }
            // コピー元のフォルダがない場合は処理をしない
            if (CopiedItemFolder == null) {
                Tools.Error("コピー元のフォルダがない");
                return;
            }
            // 貼り付け先のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("貼り付け先のフォルダがない");
                return;
            }
            ClipboardItemCommands.PasteClipboardItemCommandExecute(
                this.CutFlag,
                CopiedItems,
                CopiedItemFolder,
                SelectedFolder
                );
            // Cutフラグをもとに戻す
            CutFlag = false;
            // 貼り付け後にコピー選択中のアイテムをクリア
            CopiedItems.Clear();
            CopiedItemFolder = null;

        });

        // Ctrl + M が押された時の処理
        public SimpleDelegateCommand MergeItemCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems.Count == 0) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }
            ClipboardItemCommands.MergeItemCommandExecute(
                SelectedFolder,
                SelectedItems,
                false
                );
        });
        // Ctrl + Shift + M が押された時の処理
        public SimpleDelegateCommand MergeItemWithHeaderCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems.Count == 0) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }
            ClipboardItemCommands.MergeItemCommandExecute(
                SelectedFolder,
                SelectedItems,
                true
                );
        });


        // メニューの「Pythonスクリプト作成」をクリックしたときの処理
        public SimpleDelegateCommand CreatePythonScriptCommand => new(PythonCommands.CreatePythonScriptCommandExecute);
        // メニューの「Pythonスクリプトを編集」をクリックしたときの処理
        public SimpleDelegateCommand EditPythonScriptCommand => new SimpleDelegateCommand(PythonCommands.EditPythonScriptCommandExecute);
        // メニューの「自動処理ルールを編集」をクリックしたときの処理
        public SimpleDelegateCommand OpenListAutoProcessRuleWindowCommand => new((parameter) => {
            ListAutoProcessRuleWindow ListAutoProcessRuleWindow = new ListAutoProcessRuleWindow();
            ListAutoProcessRuleWindowViewModel ListAutoProcessRuleWindowViewModel = (ListAutoProcessRuleWindowViewModel)ListAutoProcessRuleWindow.DataContext;
            ListAutoProcessRuleWindowViewModel.Initialize();

            ListAutoProcessRuleWindow.ShowDialog();

        });

        // メニューの「タグ編集」をクリックしたときの処理
        public SimpleDelegateCommand OpenTagWindowCommand => new((parameter) => {
            TagWindow tagWindow = new TagWindow();
            tagWindow.ShowDialog();

        });
        // ステータスバーをクリックしたときの処理
        public SimpleDelegateCommand OpenStatusMessageWindowCommand => new((parameter) => {
            StatusMessageWindow statusMessageWindow = new StatusMessageWindow();
            StatusMessageWindowViewModel statusMessageWindowViewModel = (StatusMessageWindowViewModel)statusMessageWindow.DataContext;
            statusMessageWindowViewModel.Initialize();
            statusMessageWindow.ShowDialog();

        });

        // コンテキストメニューの「テキストを整形」の実行用コマンド
        public SimpleDelegateCommand FormatTextCommand => new((parameter) => {
            // 選択中のアイテムを取得
            ClipboardItemViewModel clipboardItemViewModel = Instance!.SelectedItem!;
            if (clipboardItemViewModel == null) {
                return;
            }
            string content = clipboardItemViewModel.ClipboardItem.Content;
            // テキストを整形
            content = AutoProcessCommand.FormatTextCommandExecute(content);
            // 整形したテキストをセット
            clipboardItemViewModel.ClipboardItem.Content = content;
            // LiteDBに保存
            ClipboardDatabaseController.UpsertItem(clipboardItemViewModel.ClipboardItem);
            // 再描写
            ReloadClipboardItems();
        });
        // コンテキストメニューの「ファイルのパスを分割」の実行用コマンド
        public SimpleDelegateCommand SplitFilePathCommand => new((parameter) => {
            // 選択中のアイテムを取得
            if (SelectedItem == null) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            ClipboardItem clipboardItem = SelectedItem.ClipboardItem;
            // ファイルパスを分割
            AutoProcessCommand.SplitFilePathCommandExecute(clipboardItem);
            // LiteDBに保存
            ClipboardDatabaseController.UpsertItem(clipboardItem);
            // 再描写
            ReloadClipboardItems();
        });

    }

}