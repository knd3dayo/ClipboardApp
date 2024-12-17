using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using QAChat.Model;
using QAChat.Resource;
using QAChat.View.Folder;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.Folder {

    public class FolderSelectWindowViewModel : QAChatViewModelBase {

        public FolderSelectWindowViewModel(ContentFolderViewModel rootFolderViewModel, Action<ContentFolderViewModel> _FolderSelectedAction) {

            FolderSelectedAction = _FolderSelectedAction;
            if (rootFolderViewModel == null) {
                return;
            }
            RootFolders.Add(rootFolderViewModel);
        }

        // フォルダツリーのルート
        public ObservableCollection<ContentFolderViewModel> RootFolders { get; set; } = [];

        // フォルダ選択時のAction
        public Action<ContentFolderViewModel>? FolderSelectedAction { get; set; }

        // 選択されたフォルダ
        public ContentFolderViewModel? SelectedFolder { get; set; }


        private string _selectedFolderAbsoluteCollectionName = "";
        public string SelectedFolderAbsoluteCollectionName {
            get {
                return _selectedFolderAbsoluteCollectionName;
            }
            set {
                _selectedFolderAbsoluteCollectionName = value;
                OnPropertyChanged(nameof(SelectedFolderAbsoluteCollectionName));
            }
        }
        public SimpleDelegateCommand<FolderSelectWindow> SelectFolderCommand => new((folderSelectWindow) => {

            if (SelectedFolder == null) {
                LogWrapper.Warn(CommonStringResources.Instance.SelectedFolderNotFound);
                return;
            }
            FolderSelectedAction?.Invoke(SelectedFolder);
            // Windowを閉じる
            folderSelectWindow.Close();

        });

        public SimpleDelegateCommand<RoutedEventArgs> FolderSelectionChangedCommand => new((routedEventArgs) => {
            TreeView treeView = (TreeView)routedEventArgs.OriginalSource;
            ContentFolderViewModel clipboardItemFolderViewModel = (ContentFolderViewModel)treeView.SelectedItem;

            SelectedFolder = clipboardItemFolderViewModel;
            SelectedFolderAbsoluteCollectionName = clipboardItemFolderViewModel.FolderPath;
            SelectedFolder.LoadFolderCommand.Execute(null);

        });
    }
}