﻿using System.Collections.ObjectModel;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.Views.ClipboardItemView;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp {
    public class ClipboardItemFolderContextMenuItems : ObservableCollection<ClipboardAppMenuItem> {

        private MainWindowViewModel _mainWindowViewModel;

        public ClipboardItemFolderContextMenuItems(MainWindowViewModel mainWindowViewModel) {
            _mainWindowViewModel = mainWindowViewModel;
            InitContextMenu();
        }


        private void InitContextMenu() {
            // コンテキストメニューの初期化
            this.Add(new ClipboardAppMenuItem("開く", _mainWindowViewModel.OpenSelectedItemCommand, "Ctrl+O"));

            this.Add(new ClipboardAppMenuItem("ファイルとして開く", _mainWindowViewModel.OpenSelectedItemAsFileCommand, "Ctrl+Shit+O"));
            this.Add(new ClipboardAppMenuItem("新規ファイルとして開く", _mainWindowViewModel.OpenSelectedItemAsNewFileCommand, "Ctrl+Shit+Alt+O"));
            this.Add(new ClipboardAppMenuItem("ピン留め", _mainWindowViewModel.ChangePinCommand));

            this.Add(new ClipboardAppMenuItem("コピー", _mainWindowViewModel.CopyToClipboardCommand, "Ctrl+C"));
            this.Add(new ClipboardAppMenuItem("削除", _mainWindowViewModel.DeleteSelectedItemCommand, "Delete"));

            // サブメニュー設定
            ClipboardAppMenuItem utilityMenuItems = new ClipboardAppMenuItem("便利機能", SimpleDelegateCommand.EmptyCommand);
            ClipboardAppMenuItem basicUtilityMenuItems = new ClipboardAppMenuItem("基本機能", SimpleDelegateCommand.EmptyCommand);

            basicUtilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("ファイルのパスを分割", _mainWindowViewModel.SplitFilePathCommand));
            basicUtilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("テキストを抽出", ClipboardItemViewModel.ExtractTextCommand));
            basicUtilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("データをマスキング", ClipboardItemViewModel.MaskDataCommand));
            basicUtilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("データを整形", _mainWindowViewModel.FormatTextCommand));

            utilityMenuItems.SubMenuItems.Add(basicUtilityMenuItems);

            // AI関連のメニューを追加
            ClipboardAppMenuItem aiUtilityMenuItems
                = new ClipboardAppMenuItem("OpenAI", SimpleDelegateCommand.EmptyCommand);

            aiUtilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("OpenAIチャット",
                new SimpleDelegateCommand((parameter) => {
                    ClipboardItemCommands.OpenOpenAIChatWindowExecute(_mainWindowViewModel.SelectedItem);
                })));

            aiUtilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("プロンプトテンプレートを実行",
                    new SimpleDelegateCommand((parameter) => {
                        ClipboardItemCommands.OpenAIChatCommandExecute(_mainWindowViewModel.SelectedItem);
                    })));

            // 便利機能にAI関連のメニューを追加
            utilityMenuItems.SubMenuItems.Add(aiUtilityMenuItems);

            // ユーザー定義のPythonスクリプトをメニューに追加
            ClipboardAppMenuItem userDefinedPythonScriptsMenu
                = new ClipboardAppMenuItem("ユーザー定義Pythonスクリプト", SimpleDelegateCommand.EmptyCommand);


            // Pythonスクリプト(ユーザー定義)
            foreach (ScriptItem scriptItem in ScriptItem.ScriptItems) {

                userDefinedPythonScriptsMenu.SubMenuItems.Add(new ClipboardAppMenuItem(scriptItem.Description, new SimpleDelegateCommand((parameter) => {
                    if (parameter is not ClipboardItemViewModel itemViewModel) {
                        return;
                    }
                    ClipboardItemCommands.MenuItemRunPythonScriptCommandExecute(scriptItem, itemViewModel);
                })
                ));
            }

            utilityMenuItems.SubMenuItems.Add(userDefinedPythonScriptsMenu);
            this.Add(utilityMenuItems);


        }
    }
}
