﻿using System.Windows;


namespace WpfApp1
{
    public class ClipboardItemCommands
    {
        // 選択中のアイテムを削除する処理
        public static void DeleteSelectedItemCommandExecute(object obj)
        {
            if (MainWindowViewModel.Instance == null)
            {
                return;
            }
            // 選択中のアイテムがない場合は処理をしない
            if (MainWindowViewModel.Instance.SelectedItem == null)
            {
                return;
            }
            if (MainWindowViewModel.Instance?.SelectedFolder == null)
            {
                return;
            }
            //　削除確認ボタン
            MessageBoxResult result = System.Windows.MessageBox.Show("選択中のアイテムを削除しますか?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                ClipboardItemFolder clipboardItemFolder = MainWindowViewModel.Instance.SelectedFolder;
                // 選択中のアイテムを削除
                clipboardItemFolder.DeleteItem(MainWindowViewModel.Instance.SelectedItem);
                MainWindowViewModel.Instance.UpdateStatusText("削除しました");
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
                ClipboardController.OpenItem(MainWindowViewModel.Instance.SelectedItem);
            }
            catch (ThisApplicationException e)
            {
                Tools.ShowMessage(e.Message);
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
                ClipboardController.OpenItem(MainWindowViewModel.Instance.SelectedItem, true);
            }
            catch (ThisApplicationException e)
            {
                Tools.ShowMessage(e.Message);
            }

        }
        // コンテキストメニューの「編集」をクリックしたときの処理
        public static void EditItemCommandExecute(object obj)
        {
            if (obj is ClipboardItem)
            {
                EditItemWindow editItemWindow = new EditItemWindow();
                EditItemWindowViewModel editItemWindowViewModel = (EditItemWindowViewModel)editItemWindow.DataContext;
                editItemWindowViewModel.ClipboardItem = (ClipboardItem)obj;

                editItemWindow.ShowDialog();
            }
        }

        // コンテキストメニューのタグをクリックしたときの処理
        public static void EditTagCommandExecute(object obj)
        {
            TagWindow tagWindow = new TagWindow();
            if (tagWindow.DataContext != null)
            {
                TagWindowViewModel tagWindowViewModel = (TagWindowViewModel)tagWindow.DataContext;
                tagWindowViewModel.ClipboardItem = (ClipboardItem)obj;
            }

            tagWindow.ShowDialog();

        }
        public static void CreateItemCommandExecute(object obj)
        {
            if (MainWindowViewModel.Instance?.SelectedFolder == null)
            {
                return;
            }
            NewItemWindow newItemWindow = new NewItemWindow();
            NewItemWindowViewModel newItemWindowViewModel = ((NewItemWindowViewModel)newItemWindow.DataContext);
            newItemWindowViewModel.clipboardItemFolder = MainWindowViewModel.Instance.SelectedFolder;
            newItemWindow.ShowDialog();
        }

        // Ctrl + X が押された時の処理

        public static void CutItemCommandExecute(object obj)
        {
            // Cutフラグを立てる
            if (MainWindowViewModel.Instance == null)
            {
                Tools.ShowMessage("エラーが発生しました。MainWindowViewModelのインスタンスがない");
                return;
            }
            if (MainWindowViewModel.Instance.SelectedItem == null)
            {
                Tools.ShowMessage("エラーが発生しました。選択中のアイテムがない");
                return;
            }
            if (MainWindowViewModel.Instance.SelectedFolder == null)
            {
                Tools.ShowMessage("エラーが発生しました。選択中のフォルダがない");
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
                Tools.ShowMessage(message);
            }
        }
        // Ctrl+Cで実行するコマンド
        public static void CopyToClipboardCommandExecute(object obj)
        {
            MainWindowViewModel? Instance = MainWindowViewModel.Instance;
            StatusText? StatusText = MainWindowViewModel.StatusText;

            if (Instance == null)
            {
                Tools.ShowMessage("エラーが発生しました。MainWindowViewModelのインスタンスがない");
                return;
            }
            if (Instance.SelectedItem == null)
            {
                Tools.ShowMessage("エラーが発生しました。選択中のアイテムがない");
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
                Tools.ShowMessage(message);
            }
        }

        // Ctrl + V が押された時の処理
        public static void PasteFromClipboardCommandExecute(object obj)
        {
            MainWindowViewModel? Instance = MainWindowViewModel.Instance;
            StatusText? StatusText = MainWindowViewModel.StatusText;

            if (Instance == null)
            {
                return;
            }
            if (Instance.CopiedItem == null)
            {
                Tools.ShowMessage("コピーされたアイテムがありません");
                return;
            }
            if (Instance.SelectedFolder == null)
            {
                Tools.ShowMessage("フォルダが選択されていません");
                return;
            }
            if (Instance.CopiedItemFolder == null)
            {
                Tools.ShowMessage("コピーされたフォルダがありません");
                return;
            }
            try
            {
                // 自動処理が設定されている場合は処理を行う
                ClipboardItem clipboardItem;
                if (Instance.SelectedFolder.AutoProcessItems.Count() > 0)
                {
                    clipboardItem = Instance.SelectedFolder.ApplyAutoProcess(Instance.CopiedItem);

                }else
                {
                    clipboardItem = (ClipboardItem)Instance.CopiedItem;
                }
                clipboardItem.CopyTo(Instance.SelectedFolder);
                if (Instance.CutFlag)
                {

                    Instance.CopiedItemFolder.DeleteItem(Instance.CopiedItem);
                    Instance.CutFlag = false;
                }
                Instance.UpdateStatusText("貼り付けました");   

            }
            catch (Exception e)
            {
                string message = string.Format("エラーが発生しました。\nメッセージ:\n{0]\nスタックトレース:\n[1]", e.Message, e.StackTrace);
                Tools.ShowMessage(message);
            }
        }
    }

}
