using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.Model.Folders.Browser;
using AIChatExplorer.Model.Folders.FileSystem;
using AIChatExplorer.Model.Item;
using AIChatExplorer.ViewModel.Folders.Clipboard;
using AIChatExplorer.ViewModel.Folders.FileSystem;
using AIChatExplorer.ViewModel.Folders.ShortCut;
using LibPythonAI.Model.Content;
using LibUIPythonAI.ViewModel.Folder;
using PythonAILibUI.ViewModel.Item;

namespace AIChatExplorer.ViewModel.Folders.Browser {
    public class RecentFilesFolderViewModel(FileSystemFolder clipboardItemFolder, ContentItemViewModelCommands commands) : FileSystemFolderViewModel(clipboardItemFolder, commands) {
        // LoadChildrenで再帰読み込みするデフォルトのネストの深さ
        public override int DefaultNextLevel { get; } = 1;

        // -- virtual
        public override ObservableCollection<MenuItem> FolderMenuItems {
            get {
                ShortCutFolderMenu clipboardItemMenu = new(this);
                return clipboardItemMenu.MenuItems;
            }
        }

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public override RecentFilesFolderViewModel CreateChildFolderViewModel(ContentFolderWrapper childFolder) {
            if (childFolder is not RecentFilesFolder) {
                throw new Exception("childFolder is not RecentFilesFolder");
            }
            var childFolderViewModel = new RecentFilesFolderViewModel((RecentFilesFolder)childFolder, Commands) {
                // 親フォルダとして自分自身を設定
                ParentFolderViewModel = this
            };
            return childFolderViewModel;
        }

        // LoadItems
        public override void LoadItems() {
            LoadItems<RecentFilesItem>();
        }

        // LoadChildren
        public override async void LoadChildren(int nestLevel) {
            // ChildrenはメインUIスレッドで更新するため、別のリストに追加してからChildrenに代入する
            List<RecentFilesFolderViewModel> _children = [];

            await Task.Run(() => {
                // RootFolderの場合は、ShortCutFolderを取得
                if (Folder.IsRootFolder) {
                    foreach (var child in Folder.GetChildren<RecentFilesFolder>()) {
                        if (child == null) {
                            continue;
                        }
                        RecentFilesFolderViewModel childViewModel = CreateChildFolderViewModel(child);
                        _children.Add(childViewModel);
                    }
                    return;
                }
                // RootFolder以外の場合は、FileSystemFolderを取得 
                foreach (var child in Folder.GetChildren<FileSystemFolder>()) {
                    if (child == null) {
                        continue;
                    }
                    RecentFilesFolderViewModel childViewModel = CreateChildFolderViewModel(child);
                    // ネストの深さが1以上の場合は、子フォルダの子フォルダも読み込む
                    if (nestLevel > 0) {
                        childViewModel.LoadChildren(nestLevel - 1);
                    }
                    _children.Add(childViewModel);
                }
            });
            Children = new ObservableCollection<ContentFolderViewModel>(_children);
            OnPropertyChanged(nameof(Children));
        }
    }
}

