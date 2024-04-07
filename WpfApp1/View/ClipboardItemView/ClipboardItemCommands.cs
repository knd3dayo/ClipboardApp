﻿using System.Windows;
using WpfApp1.Model;
using WpfApp1.Utils;
using WpfApp1.View.ClipboardItemFolderView;
using WpfApp1.View.TagView;


namespace WpfApp1.View.ClipboardItemView
{
    public class ClipboardItemCommands
    {
        /// <summary>
        /// 選択中のアイテムを削除する処理
        /// 削除後にフォルダ内のアイテムを再読み込む
        /// </summary>
        /// <param name="obj"></param>        
        public static void DeleteSelectedItemCommandExecute(object obj)
        {
            if (MainWindowViewModel.Instance == null)
            {
                Tools.Error("エラーが発生しました。MainWindowViewModelのインスタンスがない");
                return;
            }
            // 選択中のアイテムがない場合は処理をしない
            if (MainWindowViewModel.Instance.SelectedItem == null)
            {
                Tools.Error("エラーが発生しました。選択中のアイテムがない");
                return;
            }
            if (MainWindowViewModel.Instance?.SelectedFolder == null)
            {
                Tools.Error("エラーが発生しました。選択中のフォルダがない");
                return;
            }
            //　削除確認ボタン
            MessageBoxResult result = MessageBox.Show("選択中のアイテムを削除しますか?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                ClipboardItemFolderViewModel clipboardItemFolder = MainWindowViewModel.Instance.SelectedFolder;
                // 選択中のアイテムを削除
                clipboardItemFolder.ClipboardItemFolder.DeleteItem(MainWindowViewModel.Instance.SelectedItem.ClipboardItem);
                // フォルダ内のアイテムを再読み込む
                clipboardItemFolder.Load();
                Tools.Info("削除しました");
            }
        }
        /// <summary>
        /// コンテキストメニューの「編集」をクリックしたときの処理
        /// 編集後にフォルダ内のアイテムを再読み込みする
        /// </summary>
        /// <param name="obj"></param>
        public static void EditItemCommandExecute(object obj)
        {
            // objがClipboardItemViewModelでない場合は処理をしない
            if (obj is not ClipboardItemViewModel)
            {
                Tools.Error("obj is not ClipboardItemViewModel");
                return;
            }
            ClipboardItemViewModel clipboardItemViewModel = (ClipboardItemViewModel)obj;
            EditItemWindow editItemWindow = new EditItemWindow();
            EditItemWindowViewModel editItemWindowViewModel = (EditItemWindowViewModel)editItemWindow.DataContext;
            editItemWindowViewModel.Initialize(clipboardItemViewModel.ClipboardItem, () =>
            {
                // フォルダ内のアイテムを再読み込み
                MainWindowViewModel.Instance?.SelectedFolder?.Load();
                Tools.Info("更新しました");
            });

            editItemWindow.ShowDialog();
        }

        /// <summary>
        /// コンテキストメニューのタグをクリックしたときの処理
        /// 更新後にフォルダ内のアイテムを再読み込みする
        /// </summary>
        /// <param name="obj"></param>
        public static void EditTagCommandExecute(object obj)
        {

            if (obj is not ClipboardItemViewModel)
            {
                Tools.Error("クリップボードアイテムが選択されていません");
                return;
            }
            TagWindow tagWindow = new TagWindow();
            TagWindowViewModel tagWindowViewModel = (TagWindowViewModel)tagWindow.DataContext;
            ClipboardItemViewModel clipboardItemViewModel = (ClipboardItemViewModel)obj;
            tagWindowViewModel.Initialize(clipboardItemViewModel.ClipboardItem, () =>
            {
                // フォルダ内のアイテムを再読み込み
                MainWindowViewModel.Instance?.SelectedFolder?.Load();
                Tools.Info("更新しました");
            });

            tagWindow.ShowDialog();

        }
        /// <summary>
        /// クリップボードアイテムを新規作成する処理
        /// 作成後にフォルダ内のアイテムを再読み込む
        /// </summary>
        /// <param name="obj"></param>
        public static void CreateItemCommandExecute(object obj)
        {
            if (MainWindowViewModel.Instance?.SelectedFolder == null)
            {
                return;
            }
            NewItemWindow newItemWindow = new NewItemWindow();
            NewItemWindowViewModel newItemWindowViewModel = (NewItemWindowViewModel)newItemWindow.DataContext;
            newItemWindowViewModel.clipboardItemFolder = MainWindowViewModel.Instance.SelectedFolder;
            newItemWindow.ShowDialog();
        }

        // Ctrl + V が押された時の処理
        public static void PasteFromClipboardCommandExecute(object obj)
        {
            MainWindowViewModel? Instance = MainWindowViewModel.Instance;
            if (Instance == null)
            {
                Tools.Error("Instanceがありません");
                return;
            }
            PasteClipboardItemCommandExecute(
                Instance,
                Instance.CopiedItem, Instance.CopiedItemFolder, Instance.SelectedFolder);
        }

        /// <summary>
        /// Ctrl + V が押された時の処理
        /// コピー中のアイテムを選択中のフォルダにコピー/移動する
        /// 貼り付け後にフォルダ内のアイテムを再読み込む
        /// 
        /// </summary>
        /// <param name="Instance"></param>
        /// <param name="item"></param>
        /// <param name="fromFolder"></param>
        /// <param name="toFolder"></param>
        /// <returns></returns>
        public static ClipboardItemViewModel? PasteClipboardItemCommandExecute(
            MainWindowViewModel Instance,
            ClipboardItemViewModel? item, ClipboardItemFolderViewModel? fromFolder, ClipboardItemFolderViewModel? toFolder)
        {
            if (item == null)
            {
                Tools.Error("アイテムがありません");
                return item;
            }
            if (Instance == null)
            {
                Tools.Error("Instanceがありません");
                return item;
            }
            if (toFolder == null)
            {
                Tools.Error("コピー/移動先のフォルダが選択されていません");
                return item;
            }
            if (fromFolder == null)
            {
                Tools.Error("コピー/移動元のフォルダがありません");
                return item;
            }
            try
            {
                ClipboardItem newItem = item.ClipboardItem.Copy();
                toFolder.ClipboardItemFolder.AddItem(newItem);
                // Cutフラグが立っている場合はコピー元のアイテムを削除する
                if (Instance.CutFlag)
                {

                    fromFolder.ClipboardItemFolder.DeleteItem(item.ClipboardItem);
                    Instance.CutFlag = false;
                }
                // フォルダ内のアイテムを再読み込み
                MainWindowViewModel.Instance?.SelectedFolder?.Load();

                return new ClipboardItemViewModel(newItem);

            }
            catch (Exception e)
            {
                string message = string.Format("エラーが発生しました。\nメッセージ:\n{0]\nスタックトレース:\n[1]", e.Message, e.StackTrace);
                Tools.ShowMessage(message);
                return item;
            }
        }

        /// <summary>
        /// Ctrl + M が押された時の処理
        /// コピー中のアイテムを選択中のアイテムにマージする
        /// マージ後にフォルダ内のアイテムを再読み込む
        /// </summary>
        /// <param name="parameter"></param>
        public static void MergeItemCommandExecute(object parameter)
        {
            MergeItemCommandExecuteImpl(false);
        }
        /// <summary>
        ///  Ctrl + Shift + M が押された時の処理
        ///  コピー中のアイテムを選択中のアイテムにマージする
        ///  ヘッダー情報も本文にマージする
        ///  マージ後にフォルダ内のアイテムを再読み込む
        /// </summary>
        /// <param name="parameter"></param>
        public static void MergeItemWithHeaderCommandExecute(object parameter)
        {
            MergeItemCommandExecuteImpl(true);
        }

        public static void MergeItemCommandExecuteImpl(bool mergeWithHeader)
        {

            MainWindowViewModel? Instance = MainWindowViewModel.Instance;
            if (Instance == null)
            {
                Tools.Error("Instanceがありません");
                return;
            }
            if (Instance.SelectedItem == null)
            {
                Tools.Error("選択中のアイテムがありません");
                return;
            }
            if (Instance.CopiedItem == null)
            {
                Tools.Error("コピーされたアイテムがありません");
                return;
            }
            ClipboardItem fromItem = Instance.CopiedItem.ClipboardItem;
            ClipboardItem toItem = Instance.SelectedItem.ClipboardItem;

            try
            {
                ClipboardItem newItem = toItem.Merge(fromItem, mergeWithHeader);
                // ClipboardItemをLiteDBに保存
                ClipboardDatabaseController.UpsertItem(newItem);
                // コピー元のアイテムを削除
                ClipboardDatabaseController.DeleteItem(fromItem);

                // フォルダ内のアイテムを再読み込み
                Instance.SelectedFolder?.Load();
                Tools.Info("マージしました");

            }
            catch (Exception e)
            {
                string message = string.Format("エラーが発生しました。\nメッセージ:\n{0]\nスタックトレース:\n[1]", e.Message, e.StackTrace);
                Tools.Error(message);
            }

        }

        // Ctrl + X が押された時の処理

        public static void CutItemCommandExecute(object obj)
        {
            // Cutフラグを立てる
            if (MainWindowViewModel.Instance == null)
            {
                Tools.Error("エラーが発生しました。MainWindowViewModelのインスタンスがない");
                return;
            }
            if (MainWindowViewModel.Instance.SelectedItem == null)
            {
                Tools.Error("エラーが発生しました。選択中のアイテムがない");
                return;
            }
            if (MainWindowViewModel.Instance.SelectedFolder == null)
            {
                Tools.Error("エラーが発生しました。選択中のフォルダがない");
                return;
            }
            MainWindowViewModel.Instance.CutFlag = true;
            try
            {
                MainWindowViewModel.Instance.CopiedItem = MainWindowViewModel.Instance.SelectedItem;
                MainWindowViewModel.Instance.CopiedItemFolder = MainWindowViewModel.Instance.SelectedFolder;
                // ClipboardController.CopyToClipboard(MainWindowViewModel.Instance.SelectedItem);
                MainWindowViewModel.Instance.UpdateStatusText("切り取りました");

            }
            catch (Exception e)
            {
                string message = $"エラーが発生しました。\nメッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}";
                Tools.Error(message);
            }
        }
        // Ctrl+Cで実行するコマンド
        public static void CopyToClipboardCommandExecute(object obj)
        {
            MainWindowViewModel? Instance = MainWindowViewModel.Instance;
            StatusText? StatusText = MainWindowViewModel.StatusText;

            if (Instance == null)
            {
                Tools.Error("エラーが発生しました。MainWindowViewModelのインスタンスがない");
                return;
            }
            if (Instance.SelectedItem == null)
            {
                Tools.Error("エラーが発生しました。選択中のアイテムがない");
                return;
            }
            // Cutフラグをもとに戻す
            Instance.CutFlag = false;
            try
            {
                Instance.CopiedItem = Instance.SelectedItem;
                Instance.CopiedItemFolder = Instance.SelectedFolder;
                // ClipboardController.CopyToClipboard(Instance.SelectedItem);
                MainWindowViewModel.Instance?.UpdateStatusText("クリップボードにコピーしました");

            }
            catch (Exception e)
            {
                string message = $"エラーが発生しました。\nメッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}";
                Tools.Error(message);
            }
        }


        // 選択中のアイテムを開く処理
        public static void OpenSelectedItemCommandExecute(object obj)
        {
            if (MainWindowViewModel.Instance == null)
            {
                return;
            }
            if (MainWindowViewModel.Instance.SelectedItem == null)
            {
                return;
            }
            try
            {
                // 選択中のアイテムを開く
                ClipboardProcessController.OpenItem(MainWindowViewModel.Instance.SelectedItem.ClipboardItem);
            }
            catch (ThisApplicationException e)
            {
                Tools.Error(e.Message);
            }

        }
        // 選択中のアイテムを新規として開く処理
        public static void OpenSelectedItemAsNewCommandExecute(object obj)
        {
            if (MainWindowViewModel.Instance == null)
            {
                return;
            }
            if (MainWindowViewModel.Instance.SelectedItem == null)
            {
                return;
            }
            try
            {
                // 選択中のアイテムを新規として開く
                ClipboardProcessController.OpenItem(MainWindowViewModel.Instance.SelectedItem.ClipboardItem, true);
            }
            catch (ThisApplicationException e)
            {
                Tools.Error(e.Message);
            }

        }


    }

}