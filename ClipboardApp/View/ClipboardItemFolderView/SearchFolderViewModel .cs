using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.View.SearchView;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.ClipboardItemFolderView {
    public class SearchFolderViewModel(MainWindowViewModel mainWindowViewModel, ClipboardFolder clipboardItemFolder) : ClipboardFolderViewModel(mainWindowViewModel, clipboardItemFolder) {
        public override ObservableCollection<MenuItem> MenuItems {
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

        // LoadChildren
        public override void LoadChildren() {
            Children.Clear();
            foreach (var child in ClipboardItemFolder.Children) {
                if (child == null) {
                    continue;
                }
                Children.Add(new SearchFolderViewModel(MainWindowViewModel, child));
            }

        }
        // LoadItems
        public override void LoadItems() {
            Items.Clear();
            foreach (ClipboardItem item in ClipboardItemFolder.Items) {
                Items.Add(new ClipboardItemViewModel(item));
            }
        }



        public override SimpleDelegateCommand<ClipboardFolderViewModel> CreateFolderCommand => new((folderViewModel) => {

            // 子フォルダを作成
            ClipboardFolder clipboardFolder = ClipboardItemFolder.CreateChild("新規フォルダ");

            // 検索フォルダの親フォルダにこのフォルダを追加

            SearchFolderViewModel searchFolderViewModel = new(MainWindowViewModel, clipboardFolder);
            SearchRule? searchConditionRule = new() {
                Type = SearchRule.SearchType.SearchFolder,
                SearchFolder = clipboardFolder
            };

            SearchWindow.OpenSearchWindow(searchConditionRule, searchFolderViewModel, true, () => {
                // 保存と再読み込み
                searchFolderViewModel.Save();
                // 親フォルダを保存
                folderViewModel.Save();
                folderViewModel.Load();

            });

        });


        public override SimpleDelegateCommand<ClipboardFolderViewModel> EditFolderCommand => new((parameter) => {


            SearchRule? searchConditionRule = SearchRuleController.GetSearchRuleByFolder(this.ClipboardItemFolder);
            searchConditionRule ??= new() {
                Type = SearchRule.SearchType.SearchFolder,
                SearchFolder = this.ClipboardItemFolder
            };

            SearchWindow.OpenSearchWindow(searchConditionRule, this, true, () => {
                // 保存と再読み込み
                this.Save();
                this.Load();

            });

        });

        public override void PasteClipboardItemCommandExecute(bool CutFlag, IEnumerable<ClipboardItemViewModel> items, ClipboardFolderViewModel fromFolder, ClipboardFolderViewModel toFolder) {
            // 検索フォルダには貼り付け不可

        }
        public override void MergeItemCommandExecute(ClipboardFolderViewModel folderViewModel, Collection<ClipboardItemViewModel> selectedItems, bool mergeWithHeader) {
            // 検索フォルダにはマージ不可
        }


    }
}

