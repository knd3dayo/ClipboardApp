using System.Collections.ObjectModel;
using System.Windows;
using PythonAILib.Model.VectorDB;
using QAChat.Model;
using QAChat.View.VectorDBWindow;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.VectorDBWindow {
    /// <summary>
    /// RAGのドキュメントソースとなるGitリポジトリ、作業ディレクトリを管理するためのウィンドウのViewModel
    /// </summary>
    public class ListVectorDBWindowViewModel : QAChatViewModelBase {

        public ListVectorDBWindowViewModel(ActionModeEnum mode, Action<VectorDBItem> callBackup) {

            this.mode = mode;
            this.callBackup = callBackup;
            LoadVectorItemsCommand.Execute();

        }

        public enum ActionModeEnum {
            Edit,
            Select,
        }
        // VectorDBItemのリスト
        public ObservableCollection<VectorDBItemViewModel> VectorDBItems { get; set; } = [];

        private ActionModeEnum mode;
        Action<VectorDBItem>? callBackup;


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


        // 選択ボタンの表示可否
        public Visibility SelectModeVisibility {
            get {
                if (mode == ActionModeEnum.Select) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        // VectorDBItemのロード
        public SimpleDelegateCommand<object> LoadVectorItemsCommand => new((parameter) => {
            // VectorDBItemのリストを初期化
            VectorDBItems.Clear();
            var items = PythonAILibManager.Instance?.DataFactory.GetVectorDBItems();
            if (items == null) {
                return;
            }
            if (!IsShowSystemCommonVectorDB) {
                items = items.Where(item => !item.IsSystem && item.Name != VectorDBItem.SystemCommonVectorDBName);
            }
            foreach (var item in items) {
                VectorDBItems.Add(new VectorDBItemViewModel(item));
            }
            OnPropertyChanged(nameof(VectorDBItems));
        });

        // VectorDB Sourceの追加
        public SimpleDelegateCommand<object> AddVectorDBCommand => new((parameter) => {
            // SelectVectorDBItemを設定
            var item = PythonAILibManager.Instance?.DataFactory.CreateVectorDBItem();
            if (item == null) {
                return;
            }
            SelectedVectorDBItem = new VectorDBItemViewModel(item);
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
            if (SelectedVectorDBItem == null) {
                LogWrapper.Error(StringResources.SelectVectorDBPlease);
                return;
            }
            callBackup?.Invoke(SelectedVectorDBItem.Item);
            // Windowを閉じる
            window.Close();
        });
    }
}
