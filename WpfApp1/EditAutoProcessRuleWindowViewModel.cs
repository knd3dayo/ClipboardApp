﻿
using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfApp1
{
    public class EditAutoProcessRuleWindowViewModel : ObservableObject 
    {
        // 自動処理の一覧
        public static ObservableCollection<AutoProcessItem> AutoProcessItems { get; set; } = AutoProcessItem.AutoProcessItems;
        public enum Mode
        {
            Create,
            Edit
        }
        // ルール適用対象のClipboardItemFolder
        private ClipboardItemFolder? _TargetFolder { get; set; }
        public ClipboardItemFolder? TargetFolder {
            get
            {
                return _TargetFolder;
            }
            set
            {
                _TargetFolder = value;
                OnPropertyChanged("TargetFolder");
            }
        }

        // 編集対象の自動処理ルール
        AutoProcessRule? TargetAutoProcessRule { get; set; }

        // 自動処理ルールの名前
        public string RuleName { get; set; } = "";
        // 自動処理が有効かどうか
        private bool _IsAutoProcessRuleEnabled = true;
        public bool IsAutoProcessRuleEnabled
        {
            get
            {
                return _IsAutoProcessRuleEnabled;
            }
            set
            {
                _IsAutoProcessRuleEnabled = value;
                OnPropertyChanged("IsAutoProcessRuleEnabled");
            }
        }

        // 自動処理ルールの条件リスト
        public ObservableCollection<AutoProcessRuleCondition> Conditions { get; set; } = new ObservableCollection<AutoProcessRuleCondition>();
        // 自動処理ルールのアクション
        private AutoProcessItem? _SelectedAutoProcessItem = null;
        public AutoProcessItem? SelectedAutoProcessItem
        {
            get
            {
                return _SelectedAutoProcessItem;
            }
            set
            {
                _SelectedAutoProcessItem = value;
                OnPropertyChanged("SelectedAutoProcessItem");

                // アクションがコピーまたは移動の場合はFolderSelectionPanelEnabledをtrueにする
                if (value?.Type == AutoProcessItem.ActionType.CopyToFolder ||
                                       value?.Type == AutoProcessItem.ActionType.MoveToFolder)
                {
                    FolderSelectionPanelEnabled = true;
                }
                else
                {
                    FolderSelectionPanelEnabled = false;
                }
            }
        }

        // モード
        public Mode CurrentMode { get; set; }

        // すべてのアイテムに対してルールを有効にするかどうか
        public bool IsAllItemsRuleChecked { get; set; } = false;

        // 説明のルールを有効にするかどうか
        public bool IsDescriptionRuleChecked { get; set; } = false;
        public string Description { get; set; } = "";

        // クリップボードの内容のルールを有効にするかどうか
        public bool IsContentRuleChecked { get; set; } = false;
        public string Content { get; set; } = "";

        // ソースアプリケーションのルールを有効にするかどうか
        public bool IsSourceApplicationRuleChecked { get; set; } = false;
        public string SourceApplicationName { get; set; } = "";

        // ソースアプリケーションのタイトルのルールを有効にするかどうか
        public bool IsSourceApplicationTitleRuleChecked { get; set; } = false;
        public string SourceApplicationTitle { get; set; } = "";

        // コピーまたは移動先のフォルダ
        private ClipboardItemFolder? _DestinationFolder = null;
        public ClipboardItemFolder? DestinationFolder
        {
            get
            {
                return _DestinationFolder;
            }
            set
            {
                _DestinationFolder = value;
                OnPropertyChanged("DestinationFolder");
            }
        }

        // アクションがコピーまたは移動の場合にFolderSelectionPanelをEnabledにする
        private bool _FolderSelectionPanelEnabled = false;
        public bool FolderSelectionPanelEnabled
        {
            get
            {
                return _FolderSelectionPanelEnabled;
            }
            set
            {
                _FolderSelectionPanelEnabled = value;
                OnPropertyChanged("FolderSelectionPanelEnabled");
            }
        }

        // 自動処理を更新したあとの処理
        private Action<AutoProcessRule>? _AfterUpdate;

        // 
        // 初期化
        private void Initialize(Mode mode, ClipboardItemFolder? clipboardItemFolder, AutoProcessRule? autoProcessRule, Action<AutoProcessRule> afterUpdate)
        { 
            CurrentMode = mode;
            TargetFolder = clipboardItemFolder;
            TargetAutoProcessRule = autoProcessRule;
            IsAutoProcessRuleEnabled = autoProcessRule?.IsEnabled ?? true;
            _AfterUpdate = afterUpdate;
            //  autoProcessRule?.RuleActionが存在し、AutoProcessItemsの名前が一致するものを選択
            if (autoProcessRule?.RuleAction != null)
            {
                foreach (var item in AutoProcessItems)
                {
                    if (item.Name == autoProcessRule.RuleAction.Name)
                    {
                        SelectedAutoProcessItem = item;
                        break;
                    }
                }
            }
            DestinationFolder = autoProcessRule?.DestinationFolder;


            // autoProcessRuleがNullでない場合は初期化
            if (autoProcessRule != null)
            {
                RuleName = autoProcessRule.RuleName;
                OnPropertyChanged("RuleName");
                Conditions = new ObservableCollection<AutoProcessRuleCondition>(autoProcessRule.Conditions);
                OnPropertyChanged("Conditions");
                SelectedAutoProcessItem = autoProcessRule.RuleAction;
                OnPropertyChanged("SelectedAutoProcessItem");

                foreach (var condition in autoProcessRule.Conditions)
                {
                    switch (condition.Type)
                    {
                        case AutoProcessRuleCondition.ConditionType.AllItems:
                            IsAllItemsRuleChecked = true;
                            OnPropertyChanged("IsAllItemsRuleChecked");
                            break;

                        case AutoProcessRuleCondition.ConditionType.DescriptionContains:
                            IsDescriptionRuleChecked = true;
                            OnPropertyChanged("IsDescriptionRuleChecked");
                            Description = condition.Keyword;
                            OnPropertyChanged("Description");
                            break;
                        case AutoProcessRuleCondition.ConditionType.ContentContains:
                            IsContentRuleChecked = true;
                            OnPropertyChanged("IsContentRuleChecked");
                            Content = condition.Keyword;
                            OnPropertyChanged("Content");
                            break;
                        case AutoProcessRuleCondition.ConditionType.SourceApplicationNameContains:
                            IsSourceApplicationRuleChecked = true;
                            OnPropertyChanged("IsSourceApplicationRuleChecked");
                            SourceApplicationName = condition.Keyword;
                            OnPropertyChanged("SourceApplicationName");
                            break;
                        case AutoProcessRuleCondition.ConditionType.SourceApplicationTitleContains:
                            IsSourceApplicationTitleRuleChecked = true;
                            OnPropertyChanged("IsSourceApplicationTitleRuleChecked");
                            SourceApplicationTitle = condition.Keyword;
                            OnPropertyChanged("SourceApplicationTitle");
                            break;
                    }
                }
            }
        }

        // 初期化 modeがEditの場合
        public void InitializeEdit(ClipboardItemFolder? clipboardItemFolder, AutoProcessRule autoProcessRule, Action<AutoProcessRule> afterUpdate)
        {
            Initialize(Mode.Edit, clipboardItemFolder, autoProcessRule, afterUpdate);

        }
        // 初期化 modeがCreateの場合
        public void InitializeCreate(ClipboardItemFolder? clipboardItemFolder, Action<AutoProcessRule> afterUpdate)
        {
            Initialize(Mode.Create, clipboardItemFolder, null, afterUpdate);
        }

        // OKボタンが押されたときの処理
        public SimpleDelegateCommand OKButtonClickedCommand => new SimpleDelegateCommand(OKButtonClickedCommandExecute);
        public void OKButtonClickedCommandExecute(object parameter)
        {
            // TargetFolderがNullの場合はエラー
            if (TargetFolder == null)
            {
                Tools.Error("フォルダが選択されていません。");
                return;
            }
            // RuleNameが空の場合はエラー
            if (string.IsNullOrEmpty(RuleName))
            {
                Tools.Error("ルール名を入力してください。");
                return;
            }
            // SelectedAutoProcessItemが空の場合はエラー
            if (SelectedAutoProcessItem == null)
            {
                Tools.Error("アクションを選択してください。");
                return;
            }
            // 新規作成
            if (CurrentMode == Mode.Create)
            {
                TargetAutoProcessRule = new AutoProcessRule(RuleName, TargetFolder);
            }
            // 編集
            else
            {
                if (TargetAutoProcessRule == null)
                {
                    Tools.Error("編集対象のルールが見つかりません。");
                    return;
                }
                TargetAutoProcessRule.Conditions.Clear();
                TargetAutoProcessRule.RuleName = RuleName;
            }
            // IsAllItemsRuleCheckedがTrueの場合は条件を追加
            if (IsAllItemsRuleChecked)
            {
                // AllItemsを条件に追加
                TargetAutoProcessRule.Conditions.Add(new AutoProcessRuleCondition(
                                       AutoProcessRuleCondition.ConditionType.AllItems, ""));
            }
            // IsAutoProcessRuleEnabledがTrueの場合はIsEnabledをTrueにする
            TargetAutoProcessRule.IsEnabled = IsAutoProcessRuleEnabled;

            // TargetFolderを設定
            TargetAutoProcessRule.TargetFolder = TargetFolder;

            // IsDescriptionRuleCheckedがTrueの場合は条件を追加
            if (IsDescriptionRuleChecked)
            {
                // Descriptionを条件に追加

                TargetAutoProcessRule.Conditions.Add(new AutoProcessRuleCondition(
                    AutoProcessRuleCondition.ConditionType.DescriptionContains, Description));
            }
            // IsContentRuleCheckedがTrueの場合は条件を追加
            if (IsContentRuleChecked)
            {
                // Contentを条件に追加
                TargetAutoProcessRule.Conditions.Add(new AutoProcessRuleCondition(
                    AutoProcessRuleCondition.ConditionType.ContentContains, Content));
            }
            // IsSourceApplicationRuleCheckedがTrueの場合は条件を追加
            if (IsSourceApplicationRuleChecked)
            {
                // SourceApplicationNameを条件に追加
                TargetAutoProcessRule.Conditions.Add(new AutoProcessRuleCondition(
                    AutoProcessRuleCondition.ConditionType.SourceApplicationNameContains, SourceApplicationName));
            }
            // IsSourceApplicationTitleRuleCheckedがTrueの場合は条件を追加
            if (IsSourceApplicationTitleRuleChecked)
            {
                // SourceApplicationTitleを条件に追加
                TargetAutoProcessRule.Conditions.Add(new AutoProcessRuleCondition(
                    AutoProcessRuleCondition.ConditionType.SourceApplicationTitleContains, SourceApplicationTitle));
            }
            // アクションを追加
            TargetAutoProcessRule.RuleAction = SelectedAutoProcessItem;
            // アクションタイプがCopyToFolderまたは MoveToFolderの場合はDestinationFolderを設定
            if (SelectedAutoProcessItem.Type == AutoProcessItem.ActionType.CopyToFolder ||
                               SelectedAutoProcessItem.Type == AutoProcessItem.ActionType.MoveToFolder)
            {
                if (DestinationFolder == null)
                {
                    Tools.Error("コピーまたは移動先のフォルダを選択してください。");
                    return;
                }
                // TargetFolderとDestinationFolderが同じ場合はエラー
                if (TargetFolder.AbsoluteCollectionName == DestinationFolder.AbsoluteCollectionName)
                {
                    Tools.Error("同じフォルダにはコピーまたは移動できません。");
                    return;
                }
                TargetAutoProcessRule.DestinationFolder = DestinationFolder;
            }
            // 無限ループのチェック処理
            if (AutoProcessRule.CheckInfiniteLoop(TargetAutoProcessRule))
            {
                Tools.Error("コピー/移動処理の無限ループを検出しました。");
                return;
            }

            // 新規作成の場合は追加
            if (CurrentMode == Mode.Create)
            {
                ClipboardDatabaseController.UpsertAutoProcessRule(TargetAutoProcessRule);
            }
            // LiteDBに保存
            ClipboardDatabaseController.UpsertAutoProcessRule(TargetAutoProcessRule);

            // AutoProcessRuleを更新したあとの処理を実行
            _AfterUpdate?.Invoke(TargetAutoProcessRule);

            // ウィンドウを閉じる
            EditAutoProcessRuleWindow.Current?.Close();

        }
        // キャンセルボタンが押されたときの処理
        public SimpleDelegateCommand CancelButtonClickedCommand => new SimpleDelegateCommand(CancelButtonClickedCommandExecute);
        public void CancelButtonClickedCommandExecute(object parameter)
        {
            // ウィンドウを閉じる
            EditAutoProcessRuleWindow.Current?.Close();
        }
        // OnSelectedFolderChanged
        public void OnSelectedFolderChanged(ClipboardItemFolder? folder)
        {
            if (folder == null)
            {
                return;
            }
            // コピーor移動先が同じフォルダの場合はエラー
            if (folder.AbsoluteCollectionName == TargetFolder?.AbsoluteCollectionName)
            {
                System.Windows.MessageBox.Show("同じフォルダにはコピーまたは移動できません。");
                return;
            }
            DestinationFolder = folder;
            folder.IsSelectedOnFolderSelectWindow = true;
            folder.Load();
        }
        // OpenSelectDestinationFolderWindowCommand
        public SimpleDelegateCommand OpenSelectDestinationFolderWindowCommand => new SimpleDelegateCommand(OpenSelectDestinationFolderWindowCommandExecute);
        public void OpenSelectDestinationFolderWindowCommandExecute(object parameter)
        {
            // フォルダが選択されたら、DestinationFolderに設定
            void FolderSelectedAction(ClipboardItemFolder folder)
            {
                DestinationFolder = folder;
            }
            FolderSelectWindow FolderSelectWindow = new FolderSelectWindow();
            FolderSelectWindowViewModel FolderSelectWindowViewModel = ((FolderSelectWindowViewModel)FolderSelectWindow.DataContext);
            FolderSelectWindowViewModel.Initialize(FolderSelectedAction);
            FolderSelectWindow.ShowDialog();
        }

        // OpenSelectTargetFolderWindowCommand
        public SimpleDelegateCommand OpenSelectTargetFolderWindowCommand => new SimpleDelegateCommand(OpenSelectTargetFolderWindowCommandExecute);
        public void OpenSelectTargetFolderWindowCommandExecute(object parameter)
        {
            // フォルダが選択されたら、TargetFolderに設定
            void FolderSelectedAction(ClipboardItemFolder folder)
            {
                TargetFolder = folder;
            }
            FolderSelectWindow FolderSelectWindow = new FolderSelectWindow();
            FolderSelectWindowViewModel FolderSelectWindowViewModel = ((FolderSelectWindowViewModel)FolderSelectWindow.DataContext);
            FolderSelectWindowViewModel.Initialize(FolderSelectedAction);
            FolderSelectWindow.ShowDialog();
        }
    }
}
