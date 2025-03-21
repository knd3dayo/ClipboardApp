using System.Collections.ObjectModel;
using System.Windows;
using LibPythonAI.Data;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.VectorDB;
using LibUIPythonAI.ViewModel.Folder;

namespace LibUIPythonAI.ViewModel.VectorDB {
    /// <summary>
    /// RAGのドキュメントソースとなるGitリポジトリ、作業ディレクトリを管理するためのウィンドウのViewModel
    /// </summary>
    public class ListVectorDBWindowViewModel : ChatViewModelBase {

        public ListVectorDBWindowViewModel(ActionModeEnum mode, ObservableCollection<ContentFolderViewModel> rootFolderViewModels, Action<VectorDBProperty> callBackup) {

            this.mode = mode;
            this.callBackup = callBackup;

            LoadVectorItemsCommand.Execute();
            FolderSelectWindowViewModel = new(rootFolderViewModels, (selectedFolder, finished) => {
                FolderViewModel = selectedFolder;
            });

        }

        public enum ActionModeEnum {
            Edit,
            Select,
        }
        // VectorDBItemのリスト
        public ObservableCollection<VectorDBItemViewModel> VectorDBItems { get; set; } = [];

        private ActionModeEnum mode;
        Action<VectorDBProperty>? callBackup;


        // 選択中のVectorDBItem
        private VectorDBItemViewModel? selectedVectorDBItem;
        public VectorDBItemViewModel? SelectedVectorDBItem {
            get {
                return selectedVectorDBItem;
            }
            set {
                selectedVectorDBItem = value;
                OnPropertyChanged(nameof(SelectedVectorDBItem));
            }
        }
        // システム用のVectorDBItemを表示するか否か
        private bool isShowSystemCommonVectorDB;
        public bool IsShowSystemCommonVectorDB {
            get {
                return isShowSystemCommonVectorDB;
            }
            set {
                isShowSystemCommonVectorDB = value;
                OnPropertyChanged(nameof(IsShowSystemCommonVectorDB));
                // リストを更新
                LoadVectorItemsCommand.Execute();

            }
        }

        // FolderSelectWindowViewModel
        public FolderSelectWindowViewModel FolderSelectWindowViewModel { get; set; }
        // ContentFolderViewModel 
        public ContentFolderViewModel? FolderViewModel { get; set; }

        // 選択ボタンの表示可否
        public Visibility SelectModeVisibility => Tools.BoolToVisibility(mode == ActionModeEnum.Select);

        // SelectedTabIndex
        private int selectedTabIndex;
        public int SelectedTabIndex {
            get {
                return selectedTabIndex;
            }
            set {
                selectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
            }
        }


        // VectorDBItemのロード
        public SimpleDelegateCommand<object> LoadVectorItemsCommand => new((parameter) => {
            // VectorDBItemのリストを初期化
            VectorDBItems.Clear();
            foreach (var item in VectorDBItem.GetVectorDBItems(IsShowSystemCommonVectorDB)) {
                VectorDBItems.Add(new VectorDBItemViewModel(item));
            }
            OnPropertyChanged(nameof(VectorDBItems));
        });

        // VectorDB Sourceの追加
        public SimpleDelegateCommand<object> AddVectorDBCommand => new((parameter) => {
            SelectedVectorDBItem = new VectorDBItemViewModel(new VectorDBItem(new VectorDBItemEntity()));
            // ベクトルDBの編集Windowを開く
            EditVectorDBWindow.OpenEditVectorDBWindow(SelectedVectorDBItem, (afterUpdate) => {
                // リストを更新
                LoadVectorItemsCommand.Execute();
            });

        });
        // Vector DB編集
        public SimpleDelegateCommand<object> EditVectorDBCommand => new((parameter) => {
            if (SelectedVectorDBItem == null) {
                LogWrapper.Error(StringResources.SelectVectorDBToEdit);
                return;
            }
            // ベクトルDBの編集Windowを開く
            EditVectorDBWindow.OpenEditVectorDBWindow(SelectedVectorDBItem, (afterUpdate) => {

                // リストを更新
                LoadVectorItemsCommand.Execute();
            });

        });
        // DeleteVectorDBCommand
        public SimpleDelegateCommand<object> DeleteVectorDBCommand => new((parameter) => {
            if (SelectedVectorDBItem == null) {
                LogWrapper.Error(StringResources.SelectVectorDBToDelete);
                return;
            }
            // 確認ダイアログを表示
            MessageBoxResult result = MessageBox.Show(StringResources.ConfirmDeleteSelectedVectorDB, StringResources.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {

                // 削除
                SelectedVectorDBItem.Item.Delete();

                // リストを更新
                LoadVectorItemsCommand.Execute();
            }
        });

        // SelectCommand
        public SimpleDelegateCommand<Window> SelectCommand => new((window) => {
            // SelectedTabIndexが0の場合は、選択したVectorDBItemを返す
            if (SelectedTabIndex == 0) {
                if (SelectedVectorDBItem == null) {
                    LogWrapper.Error(StringResources.SelectVectorDBPlease);
                    return;
                }
                VectorDBPropertyEntity? entity = new() { VectorDBItemId = SelectedVectorDBItem.Item.Id };
                callBackup?.Invoke(new VectorDBProperty(entity));
            }
            // SelectedTabIndexが1の場合は、選択したFolderのVectorDBItemを返す
            else if (SelectedTabIndex == 1) {
                VectorDBProperty? item = FolderViewModel?.Folder.GetMainVectorSearchProperty();
                if (item == null) {
                    LogWrapper.Error(StringResources.SelectVectorDBPlease);
                    return;
                }
                callBackup?.Invoke(item);
            }
            // Windowを閉じる
            window.Close();
        });

    }
}
