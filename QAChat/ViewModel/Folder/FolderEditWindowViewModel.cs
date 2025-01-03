using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using PythonAILib.Model.VectorDB;
using QAChat.Model;
using QAChat.View.VectorDB;
using QAChat.ViewModel.VectorDB;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.Folder {
    public class FolderEditWindowViewModel : QAChatViewModelBase {

        // 起動時の処理
        public FolderEditWindowViewModel(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            AfterUpdate = afterUpdate;
            FolderViewModel = folderViewModel;


            OnPropertyChanged(nameof(FolderViewModel));
            OnPropertyChanged(nameof(SelectedVectorDBItem));
        }

        public ContentFolderViewModel FolderViewModel { get; set; }
        // フォルダ作成後に実行するコマンド
        private Action AfterUpdate { get; set; }

        // 検索条件を常時適用するかどうか
        private bool _alwaysApplySearchCondition = false;
        public bool AlwaysApplySearchCondition {
            get {
                return _alwaysApplySearchCondition;
            }
            set {
                _alwaysApplySearchCondition = value;
                OnPropertyChanged(nameof(AlwaysApplySearchCondition));
            }
        }

        public ObservableCollection<VectorDBItem> VectorDBItems {
            get {
                return [.. VectorDBItem.GetVectorDBItems()];
            }
        }
        public VectorDBItem? SelectedVectorDBItem {
            get {
                return FolderViewModel.Folder.MainVectorDBItem;
            }
            set {
                if (value == null) {
                    return;
                }
                FolderViewModel.Folder.MainVectorDBItem = value;
                OnPropertyChanged(nameof(SelectedVectorDBItem));
            }
        }

        private int _selectedTabIndex = 0;
        public int SelectedTabIndex {
            get {
                return _selectedTabIndex;
            }
            set {
                _selectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
                // ボタンの表示を更新
                OnPropertyChanged(nameof(VectorDBItemButtonVisibility));
            }
        }

        public Visibility VectorDBItemButtonVisibility => Tools.BoolToVisibility(SelectedTabIndex != 0);

        public bool SaveVectorDBEditable => FolderViewModel.Folder.IsRootFolder;

        public SimpleDelegateCommand<Window> CreateCommand => new((window) => {
            // フォルダ名が空の場合はエラー
            if (FolderViewModel.FolderName == "") {
                LogWrapper.Error(StringResources.EnterFolderName);
                return;
            }
            //　説明がない場合はエラー
            if (FolderViewModel.Description == "") {
                LogWrapper.Error(StringResources.EnterDescription);
                return;
            }

            FolderViewModel.SaveFolderCommand.Execute(null);
            // フォルダ作成後に実行するコマンドが設定されている場合
            AfterUpdate?.Invoke();
            // ウィンドウを閉じる
            window.Close();
        });

        // VectorDBSelectionChangedCommand
        public SimpleDelegateCommand<RoutedEventArgs> VectorDBSelectionChangedCommand => new((routedEventArgs) => {
            if (routedEventArgs.OriginalSource is ComboBox comboBox) {
                // 選択されたComboBoxItemのIndexを取得
                int index = comboBox.SelectedIndex;
                SelectedVectorDBItem = VectorDBItems[index];
            }
        });

    }

}
