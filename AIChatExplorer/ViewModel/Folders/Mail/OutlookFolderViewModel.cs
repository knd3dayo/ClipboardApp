using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.Model.Folders.Clipboard;
using AIChatExplorer.Model.Folders.Outlook;
using AIChatExplorer.Model.Item;
using AIChatExplorer.ViewModel.Folders.Clipboard;
using LibPythonAI.Model.Content;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel.Folder;
using PythonAILibUI.ViewModel.Item;

namespace AIChatExplorer.ViewModel.Folders.Mail {
    public class OutlookFolderViewModel(ContentFolderWrapper clipboardItemFolder, ContentItemViewModelCommands commands) : ClipboardFolderViewModel(clipboardItemFolder, commands) {
        // LoadChildrenで再帰読み込みするデフォルトのネストの深さ
        public override int DefaultNextLevel { get; } = 1;

        // -- virtual
        public override ObservableCollection<MenuItem> FolderMenuItems {
            get {
                OutlookFolderMenu clipboardItemMenu = new(this);
                return clipboardItemMenu.MenuItems;
            }
        }

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public override OutlookFolderViewModel CreateChildFolderViewModel(ContentFolderWrapper childFolder) {
            if (childFolder is not OutlookFolder) {
                throw new Exception("childFolder is not OutlookFolder");
            }
            var childFolderViewModel = new OutlookFolderViewModel(childFolder, Commands) {
                // 親フォルダとして自分自身を設定
                ParentFolderViewModel = this
            };
            return childFolderViewModel;
        }


        // LoadItems
        public override void LoadItems() {
            LoadItems<OutlookItem>();
        }

        // LoadChildren
        public override void LoadChildren(int nestLevel) {
            LoadChildren<OutlookFolderViewModel, OutlookFolder>(nestLevel);
        }
        public static SimpleDelegateCommand<OutlookFolderViewModel> SyncItemCommand => new(async (folderViewModel) => {
            try {
                OutlookFolder folder = (OutlookFolder)folderViewModel.Folder;
                folderViewModel.UpdateIndeterminate(true);
                await Task.Run(() => {
                    folder.SyncItems();
                });
            } finally {
                folderViewModel.UpdateIndeterminate(false);
            }
            folderViewModel.LoadItems();

        });

    }
}

