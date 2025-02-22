using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.Model.Folders.Clipboard;
using ClipboardApp.Model.Item;
using ClipboardApp.Model.Main;
using ClipboardApp.ViewModel.Content;
using ClipboardApp.ViewModel.Main;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.Folder;
using LibUIPythonAI.View.Item;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;
using PythonAILib.Model.Content;
using PythonAILib.Model.File;
using PythonAILibUI.ViewModel.Item;
using WpfAppCommon.Utils;
using LibPythonAI.Utils.Common;


namespace ClipboardApp.ViewModel.Folders.Clipboard {
    public class ClipboardFolderViewModel(ContentFolderWrapper clipboardItemFolder, ContentItemViewModelCommands commands) : ContentFolderViewModel(clipboardItemFolder, commands) {
        public override ClipboardItemViewModel CreateItemViewModel(ContentItemWrapper item) {
            return new ClipboardItemViewModel(this, item);
        }

        // RootFolderのViewModelを取得する
        public override ContentFolderViewModel GetRootFolderViewModel() {
            return MainWindowViewModel.Instance.RootFolderViewModelContainer.RootFolderViewModel;
        }


        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public override ClipboardFolderViewModel CreateChildFolderViewModel(ContentFolderWrapper childFolder) {
            var childFolderViewModel = new ClipboardFolderViewModel(childFolder, Commands) {
                // 親フォルダとして自分自身を設定
                ParentFolderViewModel = this
            };
            return childFolderViewModel;
        }

        // -- virtual
        public override ObservableCollection<MenuItem> FolderMenuItems {
            get {
                ClipboardFolderMenu clipboardItemMenu = new(this);
                return clipboardItemMenu.MenuItems;
            }
        }

        // フォルダ作成コマンドの実装
        public override void CreateFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            // 子フォルダを作成する
            ClipboardFolder childFolder = (ClipboardFolder)Folder.CreateChild("");
            ClipboardFolderViewModel childFolderViewModel = new(childFolder, Commands);

            FolderEditWindow.OpenFolderEditWindow(childFolderViewModel, afterUpdate);

        }

        public override void LoadFolderExecute(Action beforeAction, Action afterAction) {
            beforeAction();
            Task.Run(() => {
                LoadChildren(DefaultNextLevel);
                LoadItems();
            }).ContinueWith((task) => {
                afterAction();
            });
        }
        // LoadChildren
        // 子フォルダを読み込む。nestLevelはネストの深さを指定する。1以上の値を指定すると、子フォルダの子フォルダも読み込む
        // 0を指定すると、子フォルダの子フォルダは読み込まない
        protected override void LoadChildren(int nestLevel = 5) {
            // ChildrenはメインUIスレッドで更新するため、別のリストに追加してからChildrenに代入する
            List<ContentFolderViewModel> _children = [];
            foreach (var child in Folder.GetChildren()) {
                if (child == null) {
                    continue;
                }
                ClipboardFolderViewModel childViewModel = CreateChildFolderViewModel(child);
                // ネストの深さが1以上の場合は、子フォルダの子フォルダも読み込む
                if (nestLevel > 0) {
                    childViewModel.LoadChildren(nestLevel - 1);
                }
                _children.Add(childViewModel);
            }
            MainUITask.Run(() => {
                Children = new ObservableCollection<ContentFolderViewModel>(_children);
                OnPropertyChanged(nameof(Children));
            });
        }

        // LoadItems
        protected override void LoadItems() {
            // ClipboardItemFolder.Itemsは別スレッドで実行
            List<ContentItemWrapper> _items = Folder.GetItems();
            MainUITask.Run(() => {
                Items.Clear();
                foreach (ContentItemWrapper item in _items) {
                    Items.Add(CreateItemViewModel(item));
                }
            });
        }

        /// <summary>
        ///  フォルダ編集コマンド
        ///  フォルダ編集ウィンドウを表示する処理
        ///  フォルダ編集後に実行するコマンドが設定されている場合は、実行する.
        /// </summary>
        /// <param name="parameter"></param>
        public override void EditFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            FolderEditWindow.OpenFolderEditWindow(folderViewModel, afterUpdate);
        }

        public override void CreateItemCommandExecute() {
            ClipboardItem clipboardItem = new(Folder.Id);
            ContentItemViewModel ItemViewModel = new ClipboardItemViewModel(this, clipboardItem);


            // MainWindowViewModelのTabItemを追加する
            EditItemControl editItemControl = EditItemControl.CreateEditItemControl(this, ItemViewModel,
                () => {
                    // フォルダ内のアイテムを再読み込み
                    LoadFolderCommand.Execute();
                    LogWrapper.Info(StringResources.Edited);
                });

            AppTabContainer container = new("New Item", editItemControl);

            // UserControlをクローズする場合の処理を設定
            editItemControl.SetCloseUserControl(() => {
                MainWindowViewModel.Instance.RemoveTabItem(container);
            });

            MainWindowViewModel.Instance.AddTabItem(container);


        }

        public virtual void PasteClipboardItemCommandExecute(ClipboardController.CutFlagEnum CutFlag,
            IEnumerable<object> items, ClipboardFolderViewModel toFolder) {
            foreach (var item in items) {
                if (item is ClipboardItemViewModel itemViewModel) {
                    ContentItemWrapper clipboardItem = itemViewModel.ContentItem;
                    if (CutFlag == ClipboardController.CutFlagEnum.Item) {
                        // Cutフラグが立っている場合はコピー元のアイテムを削除する
                        clipboardItem.MoveToFolder(toFolder.Folder);
                    } else {
                        clipboardItem.CopyToFolder(toFolder.Folder);
                    }
                }
                if (item is ClipboardFolderViewModel folderViewModel) {
                    ContentFolderWrapper folder = folderViewModel.Folder;
                    if (CutFlag == ClipboardController.CutFlagEnum.Folder) {
                        // Cutフラグが立っている場合はコピー元のフォルダを削除する
                        folder.MoveTo(toFolder.Folder);
                        // 元のフォルダの親フォルダを再読み込み
                        folderViewModel.ParentFolderViewModel?.LoadFolderCommand.Execute();
                    }
                }

            }
            toFolder.LoadFolderCommand.Execute();

            LogWrapper.Info(StringResources.Pasted);
        }

        // -----------------------------------------------------------------------------------
        #region プログレスインジケーター表示の処理


        #endregion


        public override SimpleDelegateCommand<object> LoadFolderCommand => new((parameter) => {
            LoadFolderExecute(
                () => {
                    Commands.UpdateIndeterminate(true);
                },
                () => {
                    MainUITask.Run(() => {
                        Commands.UpdateIndeterminate(false);
                        UpdateStatusText();
                    });
                });
        });


        // Ctrl + Delete が押された時の処理 選択中のフォルダのアイテムを削除する
        public SimpleDelegateCommand<object> DeleteDisplayedItemCommand => new((parameter) => {
            DeleteDisplayedItemCommandExecute(() => {
                Commands.UpdateIndeterminate(true);
            }, () => {
                // 全ての削除処理が終了した後、後続処理を実行
                // フォルダ内のアイテムを再読み込む
                MainUITask.Run(() => {
                    LoadFolderCommand.Execute();
                });
                LogWrapper.Info(CommonStringResources.Instance.Deleted);
                Commands.UpdateIndeterminate(false);
            });
        });


        // ExtractTextCommand
        public SimpleDelegateCommand<object> ExtractTextCommand => new((parameter) => {
            // ContentTypes.Files, ContentTypes.Imageのアイテムを取得
            var itemViewModels = Items.Where(x => x.ContentItem.ContentType == ContentTypes.ContentItemTypes.Files || x.ContentItem.ContentType == ContentTypes.ContentItemTypes.Files);
            Commands.ExtractTextCommand.Execute(MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel?.SelectedItems);

        });


        //
    }
}
