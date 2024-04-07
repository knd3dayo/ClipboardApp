﻿using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Model;
using WpfApp1.Utils;
using WpfApp1.View.AutoProcessRuleView;

namespace WpfApp1.View.ClipboardItemFolderView
{
    public class FolderEditWindowViewModel : ObservableObject
    {
        // 編集モードか新規子フォルダ作成モードか
        public enum Mode
        {
            Edit,
            CreateChild
        }
        // Windowの名前
        public string WindowName
        {
            get
            {
                return CurrentMode == Mode.Edit ? "フォルダ編集" : "新規フォルダ作成";
            }
        }
        public Mode CurrentMode { get; set; }
        private string _collectionName = "";
        public string CollectionName
        {
            get
            {
                return _collectionName;
            }
            set
            {
                _collectionName = value;
                // プロパティが変更されたことを通知
                OnPropertyChanged("CollectionName");
            }
        }
        private string _displayName = "";
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                _displayName = value;
                // プロパティが変更されたことを通知
                OnPropertyChanged("DisplayName");
            }
        }
        public ClipboardItemFolder? Folder { get; set; }


        // 検索条件を常時適用するかどうか
        private bool _alwaysApplySearchCondition = false;
        public bool AlwaysApplySearchCondition
        {
            get
            {
                return _alwaysApplySearchCondition;
            }
            set
            {
                _alwaysApplySearchCondition = value;
                OnPropertyChanged("AlwaysApplySearchCondition");
            }
        }

        // CollectionNameが編集可能かどうか
        public bool IsCollectionNameEditable
        {
            get
            {
                return CurrentMode == Mode.CreateChild;
            }
        }
        // フォルダ作成後に実行するコマンド
        private Action? _afterUpdate;

        // 起動時の処理
        public void Init(ClipboardItemFolder clipboardItemFolder, Mode mode, Action afterUpdate)
        {
            CurrentMode = mode;
            OnPropertyChanged("IsCollectionNameEditable");
            _afterUpdate = afterUpdate;

            Folder = clipboardItemFolder;
            // 編集モードの場合
            if (CurrentMode == Mode.Edit)
            {
                // CollectionNameを設定
                // AbsoluteCollectionNameを_で分割して最後の要素をCollectionNameに設定
                CollectionName = clipboardItemFolder.AbsoluteCollectionName.Split('_').Last();

                DisplayName = clipboardItemFolder.DisplayName;
            }
            // 新規子フォルダ作成モードの場合
            else if (CurrentMode == Mode.CreateChild)
            {
                // CollectionNameを設定
                CollectionName = "";
                DisplayName = "";
            }

            // Visibilityを更新
            OnPropertyChanged("SearchConditionVisibility");
            OnPropertyChanged("AutoProcessRuleVisibility");


        }
        public SimpleDelegateCommand CreateCommand => new SimpleDelegateCommand(CreateCommandExecute);

        private void CreateCommandExecute(object parameter)
        {

            if (Folder == null)
            {
                Tools.Error("フォルダが指定されていません");
                return;
            }
            // CollectionNameが空の場合はエラー
            if (CollectionName == "")
            {
                Tools.Error("フォルダ名を入力してください");
                return;
            }
            // DisplayNameが空の場合はエラー
            if (DisplayName == "")
            {
                Tools.Error("表示名を入力してください");
                return;
            }
            // CollectionNameが[a-Z$_]以外の場合はエラー
            if (!System.Text.RegularExpressions.Regex.IsMatch(CollectionName, "^[a-zA-Z0-9]+$"))
            {
                Tools.Error("フォルダ名は英文字で入力してください");
                return;
            }

            // 編集モードの場合
            if (CurrentMode == Mode.Edit)
            {
                // DisplayNameを設定
                Folder.DisplayName = DisplayName;

                ClipboardDatabaseController.UpsertFolder(Folder);
            }
            // 新規子フォルダ作成モードの場合
            else if (CurrentMode == Mode.CreateChild)
            {
                // フォルダを作成
                ClipboardItemFolder child = new ClipboardItemFolder(Folder, CollectionName, DisplayName);
                // 親フォルダがSEARCH_ROOT_FOLDERまたはIsSearchFolderの場合
                if (Folder.AbsoluteCollectionName == ClipboardDatabaseController.SEARCH_ROOT_FOLDER_NAME 
                    || Folder.IsSearchFolder)
                {
                    // 子フォルダも検索フォルダにする
                    child.IsSearchFolder = true;
                }

                ClipboardDatabaseController.UpsertFolder(child);
                // 親フォルダと子フォルダの関係を登録
                ClipboardDatabaseController.UpsertFolderRelation(Folder, child);

            }
            // フォルダ作成後に実行するコマンドが設定されている場合
            if (_afterUpdate != null)
            {
                _afterUpdate();
            }

            FolderEditWindow.Current?.Close();
        }

        public SimpleDelegateCommand CancelCommand => new SimpleDelegateCommand(CancelCommandExecute);
        private void CancelCommandExecute(object parameter)
        {
            FolderEditWindow.Current?.Close();

        }

        // OpenListAutoProcessRuleWindowCommand
        public SimpleDelegateCommand OpenListAutoProcessRuleWindowCommand => new SimpleDelegateCommand(OpenListAutoProcessRuleWindowCommandExecute);
        private void OpenListAutoProcessRuleWindowCommandExecute(object parameter)
        {
            if (Folder == null)
            {
                Tools.Error("フォルダが指定されていません");
                return;
            }
            ListAutoProcessRuleWindow ListAutoProcessRuleWindow = new ListAutoProcessRuleWindow();
            ListAutoProcessRuleWindowViewModel ListAutoProcessRuleWindowViewModel = (ListAutoProcessRuleWindowViewModel)ListAutoProcessRuleWindow.DataContext;
            ListAutoProcessRuleWindowViewModel.Initialize(Folder);

            ListAutoProcessRuleWindow.ShowDialog();
        }
        // OpenSelectTargetFolderWindowCommand
        public SimpleDelegateCommand OpenEditSearchConditionWindowCommand => new SimpleDelegateCommand(ClipboardFolderCommands.SearchCommandExecute);
        // 検索条件画面表示ボタンを表示するかどうか
        public Visibility SearchConditionVisibility
        {
            get
            {
                // モードがCreateChildの場合は非表示
                if (CurrentMode == Mode.CreateChild)
                {
                    return Visibility.Collapsed;
                }
                // folderがSelectFolderの場合は表示する
                if (Folder != null && Folder.IsSearchFolder)
                {
                    return Visibility.Visible; 
                }
                return Visibility.Collapsed;
            }
        }
        // 自動処理ルール画面表示ボタンを表示するかどうか
        public Visibility AutoProcessRuleVisibility
        {
            get
            {
                // モードがCreateChildの場合は非表示
                if (CurrentMode == Mode.CreateChild)
                {
                    return Visibility.Collapsed;
                }
                // folderがSelectFolderの場合は表示する
                if (Folder != null && ! Folder.IsSearchFolder)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

    }

}