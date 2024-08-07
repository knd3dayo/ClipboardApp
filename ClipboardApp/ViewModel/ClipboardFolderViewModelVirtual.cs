using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.ClipboardItemView;
using Microsoft.WindowsAPICodePack.Dialogs;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel {
    public partial class ClipboardFolderViewModel {

        // -- virtual
        public virtual ObservableCollection<MenuItem> MenuItems {
            get {
                // MenuItemのリストを作成
                ObservableCollection<MenuItem> menuItems = [];
                // 新規作成
                MenuItem createMenuItem = new();
                createMenuItem.Header = StringResources.Create;
                createMenuItem.Command = CreateFolderCommand;
                createMenuItem.CommandParameter = this;
                menuItems.Add(createMenuItem);

                // 編集
                MenuItem editMenuItem = new();
                editMenuItem.Header = StringResources.Edit;
                editMenuItem.Command = EditFolderCommand;
                editMenuItem.IsEnabled = IsEditVisible;
                editMenuItem.CommandParameter = this;
                menuItems.Add(editMenuItem);

                // 削除
                MenuItem deleteMenuItem = new();
                deleteMenuItem.Header = StringResources.Delete;
                deleteMenuItem.Command = DeleteFolderCommand;
                deleteMenuItem.IsEnabled = IsDeleteVisible;
                deleteMenuItem.CommandParameter = this;
                menuItems.Add(deleteMenuItem);

                // インポート    
                MenuItem importMenuItem = new();
                importMenuItem.Header = StringResources.Import;
                importMenuItem.Command = ImportItemsToFolderCommand;
                importMenuItem.CommandParameter = this;
                menuItems.Add(importMenuItem);

                // エクスポート
                MenuItem exportMenuItem = new();
                exportMenuItem.Header = StringResources.Export;
                exportMenuItem.Command = ExportItemsFromFolderCommand;
                exportMenuItem.CommandParameter = this;
                menuItems.Add(exportMenuItem);

                return menuItems;

            }
        }

        // Itemのコンテキストメニュー
        public virtual ObservableCollection<MenuItem> ItemContextMenuItems {
            get {
                // MenuItemのリストを作成
                ObservableCollection<MenuItem> menuItems = [];
                if (MainWindowViewModel.ActiveInstance == null) {
                    return menuItems;
                }
                // 開く
                MenuItem createMenuItem = new();
                createMenuItem.Header = "開く";
                createMenuItem.Command = MainWindowViewModel.ActiveInstance.OpenSelectedItemCommand;
                createMenuItem.CommandParameter = this;
                createMenuItem.InputGestureText = "Ctrl+O";
                menuItems.Add(createMenuItem);

                // ファイルとして開く
                MenuItem openContentAsFileMenuItem = new();
                openContentAsFileMenuItem.Header = "ファイルとして開く";
                openContentAsFileMenuItem.Command = MainWindowViewModel.ActiveInstance.OpenContentAsFileCommand;
                openContentAsFileMenuItem.CommandParameter = this;
                openContentAsFileMenuItem.InputGestureText = "Ctrl+Shit+O";

                // 背景情報生成
                MenuItem generateBackgroundInfoMenuItem = new();
                generateBackgroundInfoMenuItem.Header = StringResources.GenerateBackgroundInfo;
                generateBackgroundInfoMenuItem.Command = MainWindowViewModel.ActiveInstance.GenerateBackgroundInfoCommand;
                generateBackgroundInfoMenuItem.CommandParameter = this;
                menuItems.Add(generateBackgroundInfoMenuItem);

                // サマリーを生成
                MenuItem generateSummaryMenuItem = new();
                generateSummaryMenuItem.Header = StringResources.GenerateSummary;
                generateSummaryMenuItem.Command = MainWindowViewModel.ActiveInstance.GenerateSummaryCommand;
                generateSummaryMenuItem.CommandParameter = this;
                menuItems.Add(generateSummaryMenuItem);

                // ベクトル検索
                MenuItem vectorSearchMenuItem = new();
                vectorSearchMenuItem.Header = StringResources.VectorSearch;
                vectorSearchMenuItem.Command = MainWindowViewModel.ActiveInstance.VectorSearchCommand;
                vectorSearchMenuItem.CommandParameter = this;
                menuItems.Add(vectorSearchMenuItem);

                // ピン留め
                MenuItem pinnedStateChangeMenuItem = new();
                pinnedStateChangeMenuItem.Header = "ピン留め";
                pinnedStateChangeMenuItem.Command = MainWindowViewModel.ActiveInstance.ChangePinCommand;
                pinnedStateChangeMenuItem.CommandParameter = this;
                menuItems.Add(pinnedStateChangeMenuItem);

                // コピー
                MenuItem copyMenuItem = new();
                copyMenuItem.Header = "コピー";
                copyMenuItem.Command = MainWindowViewModel.ActiveInstance.CopyItemCommand;
                copyMenuItem.CommandParameter = this;
                copyMenuItem.InputGestureText = "Ctrl+C";
                menuItems.Add(copyMenuItem);

                // 削除
                MenuItem deleteMnuItem = new();
                deleteMnuItem.Header = "削除";
                deleteMnuItem.Command = MainWindowViewModel.ActiveInstance.DeleteSelectedItemCommand;
                deleteMnuItem.CommandParameter = this;
                deleteMnuItem.InputGestureText = "Delete";
                menuItems.Add(deleteMnuItem);

                return menuItems;

            }
        }


        // LoadChildren
        public virtual void LoadChildren() {
            Children.Clear();
            foreach (var child in ClipboardItemFolder.Children) {
                if (child == null) {
                    continue;
                }
                Children.Add(new ClipboardFolderViewModel(MainWindowViewModel, child));
            }

        }
        // LoadItems
        public virtual void LoadItems() {
            Items.Clear();
            foreach (ClipboardItem item in ClipboardItemFolder.Items) {
                Items.Add(new ClipboardItemViewModel(this, item));
            }
        }


        // フォルダ作成コマンドの実装
        public virtual void CreateFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {
            // 子フォルダを作成する
            ClipboardFolder childFolder = ClipboardItemFolder.CreateChild("");
            childFolder.FolderType = ClipboardFolder.FolderTypeEnum.Normal;
            ClipboardFolderViewModel childFolderViewModel = new(MainWindowViewModel, childFolder);

            FolderEditWindow.OpenFolderEditWindow(childFolderViewModel, afterUpdate);

        }

        /// <summary>
        ///  フォルダ編集コマンド
        ///  フォルダ編集ウィンドウを表示する処理
        ///  フォルダ編集後に実行するコマンドが設定されている場合は、実行する.
        /// </summary>
        /// <param name="parameter"></param>
        public virtual void EditFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {

            FolderEditWindow.OpenFolderEditWindow(folderViewModel, afterUpdate);

        }

        public virtual void CreateItemCommandExecute() {
            EditItemWindow.OpenEditItemWindow(this, null, () => {
                // フォルダ内のアイテムを再読み込み
                LoadFolderCommand.Execute();
                LogWrapper.Info("追加しました");
            });
        }
        public virtual void OpenItemCommandExecute(ClipboardItemViewModel item) {
            EditItemWindow.OpenEditItemWindow(this, item, () => {
                // フォルダ内のアイテムを再読み込み
                LoadFolderCommand.Execute();
                LogWrapper.Info("編集しました");
            });
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

        public virtual void PasteClipboardItemCommandExecute(bool CutFlag,
            IEnumerable<ClipboardItemViewModel> items, ClipboardFolderViewModel fromFolder, ClipboardFolderViewModel toFolder) {
            foreach (var item in items) {
                ClipboardItemViewModel newItem = item.Copy();
                toFolder.AddItemCommand.Execute(newItem);
                // Cutフラグが立っている場合はコピー元のアイテムを削除する
                if (CutFlag) {

                    fromFolder.DeleteItemCommand.Execute(item);
                }
            }
            // フォルダ内のアイテムを再読み込み
            toFolder.LoadFolderCommand.Execute();
            LogWrapper.Info("貼り付けました");
        }

        public virtual void MergeItemCommandExecute(
            ClipboardFolderViewModel folderViewModel, Collection<ClipboardItemViewModel> selectedItems, bool mergeWithHeader) {

            if (selectedItems.Count < 2) {
                LogWrapper.Error("マージするアイテムを2つ選択してください");
                return;
            }
            // マージ先のアイテム。SelectedItems[0]がマージ先
            if (selectedItems[0] is not ClipboardItemViewModel toItemViewModel) {
                LogWrapper.Error("マージ先のアイテムが選択されていません");
                return;
            }
            List<ClipboardItemViewModel> fromItemsViewModel = [];
            try {
                // toItemにSelectedItems[1]からCount - 1までのアイテムをマージする
                for (int i = 1; i < selectedItems.Count; i++) {
                    if (selectedItems[i] is not ClipboardItemViewModel fromItemModelView) {
                        LogWrapper.Error("マージ元のアイテムが選択されていません");
                        return;
                    }
                    fromItemsViewModel.Add(fromItemModelView);
                }
                toItemViewModel.MergeItems(fromItemsViewModel, mergeWithHeader, Tools.DefaultAction);

                // ClipboardItemをLiteDBに保存
                toItemViewModel.SaveClipboardItemCommand.Execute(true);
                // コピー元のアイテムを削除
                foreach (var fromItem in fromItemsViewModel) {
                    fromItem.DeleteClipboardItemCommand.Execute();
                }

                // フォルダ内のアイテムを再読み込み
                folderViewModel.LoadFolderCommand.Execute();
                LogWrapper.Info("マージしました");

            } catch (Exception e) {
                string message = $"エラーが発生しました。\nメッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}";
                LogWrapper.Error(message);
            }

        }


    }
}
