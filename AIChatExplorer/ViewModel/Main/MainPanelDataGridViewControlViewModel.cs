using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using AIChatExplorer.ViewModel.Content;
using AIChatExplorer.ViewModel.Folders.Clipboard;
using CommunityToolkit.Mvvm.ComponentModel;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;
using PythonAILib.Resources;

namespace AIChatExplorer.ViewModel.Main {
    public class MainPanelDataGridViewControlViewModel(AppItemViewModelCommands commands) : ObservableObject {

        private AppItemViewModelCommands Commands { get; set; } = commands;


        public Action<bool> UpdateIndeterminateAction { get; set; } = (isIndeterminate) => { };

        private ContentFolderViewModel? _selectedFolder;
        public ContentFolderViewModel? SelectedFolder {
            get {
                return _selectedFolder;
            }
            set {
                _selectedFolder = value;
                OnPropertyChanged(nameof(SelectedFolder));
            }
        }

        // 選択中のアイテム(複数選択)
        private ObservableCollection<ContentItemViewModel> _selectedItems = [];
        public ObservableCollection<ContentItemViewModel> SelectedItems {
            get {
                return _selectedItems;

            }
            set {
                _selectedItems = value;

                OnPropertyChanged(nameof(SelectedItems));
            }
        }
        public ContentItemViewModel? SelectedItem {
            get {
                // SelectedItemsの最後のアイテムを返す
                if (SelectedItems.Count > 0) {
                    return SelectedItems[^1];
                }
                return null;
            }
        }

        // クリップボードアイテムが選択された時の処理
        // ListBoxで、SelectionChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> ClipboardItemSelectionChangedCommand => new((routedEventArgs) => {

            // DataGridの場合
            if (routedEventArgs.OriginalSource is DataGrid dataGrid) {
                // 前回選択していたTabIndexを取得
                int lastSelectedTabIndex = SelectedItem?.SelectedTabIndex ?? 0;

                if (dataGrid.SelectedItem is ContentItemViewModel clipboardItemViewModel) {
                    // SelectedItemsをMainWindowViewModelにセット
                    SelectedItems.Clear();
                    foreach (ContentItemViewModel item in dataGrid.SelectedItems) {
                        SelectedItems.Add(item);
                    }
                    // SelectedTabIndexを更新する処理
                    if (SelectedItem != null) {
                        SelectedItem.SelectedTabIndex = lastSelectedTabIndex;
                        /**
                         * Task.Run(() => {
                            SelectedItem.ContentItem.Load(() => { }, () => {
                                MainUITask.Run(() => {
                                    OnPropertyChanged(nameof(SelectedItem));
                                });
                            });
                        });
                        **/
                        OnPropertyChanged(nameof(SelectedItem));
                    }
                }
            }

        });

        // ピン留めの切り替え処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> ChangePinCommand => new((parameter) => {

            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems.Count == 0) {
                LogWrapper.Error(PythonAILibStringResources.Instance.NoItemSelected);
                return;
            }
            foreach (ClipboardItemViewModel clipboardItemViewModel in SelectedItems) {
                Commands.ChangePinCommand.Execute();
            }
        });

        #region クリップボードアイテムのコンテキストメニューのInputBinding用のコマンド
        // 選択したアイテムをテキストファイルとして開く処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> OpenContentAsFileCommand => new((parameter) => {
            Commands.OpenContentAsFileCommand.Execute(this.SelectedItem);
        });

        // ベクトルを生成する処理 複数アイテム処理可
        public SimpleDelegateCommand<object> GenerateVectorCommand => new((parameter) => {
            Commands.GenerateVectorCommand.Execute(this.SelectedItems);
        });


        // ベクトル検索を実行する処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> VectorSearchCommand => new((parameter) => {
            Commands.VectorSearchCommand.Execute(SelectedItem);
        });

        #endregion

        #region クリップボードアイテムのInputBinding用のコマンド
        // Ctrl + Delete が押された時の処理 選択中のフォルダのアイテムを削除する
        public SimpleDelegateCommand<object> DeleteDisplayedItemCommand => new((parameter) => {
            SelectedFolder?.DeleteDisplayedItemCommand.Execute();
        });

        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public SimpleDelegateCommand<object> DeleteItemCommand => new((parameter) => {
            Commands.DeleteItemsCommand.Execute(this.SelectedItems);
        });

        // Ctrl + X が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> CutItemCommand => new((parameter) => {
            Commands.CutItemCommand.Execute(this.SelectedItems);
        });

        // Ctrl + C が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> CopyItemCommand => new((parameter) => {
            Commands.CopyToClipboardCommandExecute(this.SelectedItems);
        });

        // 選択中のアイテムを開く処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> OpenSelectedItemCommand => new((parameter) => {
            Commands.OpenItemCommand.Execute(this.SelectedItem);

        });


        #endregion



    }
}
