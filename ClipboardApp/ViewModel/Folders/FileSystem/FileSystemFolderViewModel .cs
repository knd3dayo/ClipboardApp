using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.Model;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Item;
using ClipboardApp.ViewModel.Content;
using ClipboardApp.ViewModel.Folders.Clipboard;
using NetOffice.OutlookApi;
using PythonAILib.Model.Content;
using PythonAILib.Model.Folder;
using QAChat.ViewModel.Folder;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel.Folders.FileSystem
{
    public class FileSystemFolderViewModel(FileSystemFolder clipboardItemFolder) : ClipboardFolderViewModel(clipboardItemFolder)
    {
        // LoadChildrenで再帰読み込みするデフォルトのネストの深さ
        public override int DefaultNextLevel { get; } = 0;

        // -- virtual
        public override ObservableCollection<MenuItem> FolderMenuItems
        {
            get
            {
                FileSystemFolderMenu clipboardItemMenu = new(this);
                return clipboardItemMenu.MenuItems;
            }
        }

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public override FileSystemFolderViewModel CreateChildFolderViewModel(ClipboardFolder childFolder)
        {
            if (childFolder is not FileSystemFolder)
            {
                throw new System.Exception("childFolder is not FileSystemFolder");
            }
            var childFolderViewModel = new FileSystemFolderViewModel((FileSystemFolder)childFolder)
            {
                // 親フォルダとして自分自身を設定
                ParentFolderViewModel = this
            };
            return childFolderViewModel;
        }



        // LoadChildren
        // 子フォルダを読み込む。nestLevelはネストの深さを指定する。1以上の値を指定すると、子フォルダの子フォルダも読み込む
        // 0を指定すると、子フォルダの子フォルダは読み込まない
        public override async void LoadChildren(int nestLevel = 0)
        {
            try
            {
                UpdateIndeterminate(true);
                // ChildrenはメインUIスレッドで更新するため、別のリストに追加してからChildrenに代入する
                List<ContentFolderViewModel> _children = [];

                await Task.Run(() =>
                {
                    foreach (var child in Folder.GetChildren<FileSystemFolder>())
                    {
                        if (child == null)
                        {
                            continue;
                        }
                        FileSystemFolderViewModel childViewModel = CreateChildFolderViewModel(child);
                        // ネストの深さが1以上の場合は、子フォルダの子フォルダも読み込む
                        if (nestLevel > 0)
                        {
                            childViewModel.LoadChildren(nestLevel - 1);
                        }
                        _children.Add(childViewModel);
                    }
                });
                Children = new ObservableCollection<ContentFolderViewModel>(_children);
                OnPropertyChanged(nameof(Children));
            }
            finally
            {
                UpdateIndeterminate(false);
            }


        }
        // LoadItems
        public override async void LoadItems()
        {
            Items.Clear();
            // ClipboardItemFolder.Itemsは別スレッドで実行
            List<FileSystemItem> _items = [];
            try
            {
                UpdateIndeterminate(true);
                await Task.Run(() =>
                {
                    _items = Folder.GetItems<FileSystemItem>();
                });
                foreach (FileSystemItem item in _items)
                {
                    Items.Add(CreateItemViewModel(item));
                }
            }
            finally
            {
                UpdateIndeterminate(false);
            }
        }
        public static SimpleDelegateCommand<FileSystemFolderViewModel> SyncItemCommand => new(async (folderViewModel) =>
        {
            try
            {
                FileSystemFolder folder = (FileSystemFolder)folderViewModel.Folder;
                folderViewModel.UpdateIndeterminate(true);
                await Task.Run(() =>
                {
                    folder.SyncItems();
                });
            }
            finally
            {
                folderViewModel.UpdateIndeterminate(false);
            }
        });

        // ショートカット登録コマンド
        public static SimpleDelegateCommand<ClipboardFolderViewModel> CreateShortCutCommand => new((folderViewModel) =>
        {
            // ショートカット登録
            // ShortCutRootFolderを取得
            FileSystemFolder shortCutRootFolder = FolderManager.ShortcutRootFolder;
            // ショートカットフォルダを作成
            ShortCutFolder subFolder = new()
            {
                FolderType = FolderTypeEnum.ShortCut,
                Description = folderViewModel.FolderName,
                FolderName = folderViewModel.FolderName,
                ParentId = shortCutRootFolder.Id,
                FileSystemFolderPath = folderViewModel.FolderPath
            };
            subFolder.Save();
        });

    }
}

