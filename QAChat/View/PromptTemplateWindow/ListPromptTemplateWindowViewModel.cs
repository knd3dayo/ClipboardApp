﻿using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using QAChat.Model;
using WpfAppCommon.Utils;
using WpfAppCommon.Factory;
using WpfAppCommon;
using WpfAppCommon.Model;
using QAChat.View.RAGWindow;

namespace QAChat.View.PromptTemplateWindow {
    public class ListPromptTemplateWindowViewModel : ObservableObject {

        // プロンプトテンプレートの一覧
        public ObservableCollection<PromptItemViewModel> PromptItems { get; set; } = new ObservableCollection<PromptItemViewModel>();
        // 選択中の自動処理ルール
        private static PromptItemViewModel? _selectedPromptItem;
        public static PromptItemViewModel? SelectedPromptItem {
            get => _selectedPromptItem;
            set {
                _selectedPromptItem = value;
            }
        }

        public enum ActionModeEum {
            Edit,
            Select,
            Exec
        }
        private ActionModeEum ActionMode { get; set; } = ActionModeEum.Edit;
        // モード
        private int _Mode = (int)OpenAIExecutionModeEnum.Normal;
        public int Mode {
            get {
                return _Mode;
            }
            set {
                _Mode = value;
                OnPropertyChanged(nameof(Mode));
            }
        }
        // 実行/選択ボタンの表示
        public Visibility ExecButtonVisibility {
            get {
                // ActionModeがExecまたはSelectの場合は、Visible
                return ActionMode == ActionModeEum.Exec || ActionMode == ActionModeEum.Select ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        // Modeの表示
        public Visibility ModeVisibility {
            get {
                // ActionModeがExecの場合は、Visible
                return ActionMode == ActionModeEum.Exec ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Action<PromptItemViewModel> AfterEdit { get; set; } = (promptItemViewModel) => { };
        private Action<PromptItemViewModel, OpenAIExecutionModeEnum> AfterSelect { get; set; } = (promptItemViewModel, mode) => { };
        // 初期化
        public void Initialize(ActionModeEum actionMode, Action<PromptItemViewModel, OpenAIExecutionModeEnum> afterUpdate) {
            // PromptItemsを更新
            Reload();
            AfterSelect = afterUpdate;
            // ActionModeを設定
            ActionMode = actionMode;

            // TitleとSelectButtonTextを更新
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(SelectButtonText));
            // 実行モードの場合は、実行/選択ボタンを表示する
            OnPropertyChanged(nameof(ExecButtonVisibility));
            // 実行モードの場合は、Modeを表示する
            OnPropertyChanged(nameof(ModeVisibility));

        }

        public void Reload() {
            // PromptItemsを更新
            PromptItems.Clear();
            IClipboardDBController clipboardDBController = ClipboardAppFactory.Instance.GetClipboardDBController();
            foreach(var item in clipboardDBController.GetAllPromptTemplates()) {
                PromptItemViewModel itemViewModel = new PromptItemViewModel(item);
                PromptItems.Add(itemViewModel);
            }
            OnPropertyChanged(nameof(PromptItems));

        }

        public string Title {
            get {
                // ActionModeがExecまたはSelectの場合は、"プロンプトテンプレートを選択"、それ以外は"プロンプトテンプレート一覧"
                return ActionMode == ActionModeEum.Exec || ActionMode == ActionModeEum.Select ? "プロンプトテンプレートを選択" : "プロンプトテンプレート一覧";
            }
        }
        public string SelectButtonText {
            get {
                // ActionModeがExecの場合は、"実行"、それ以外は"選択"
                return ActionMode == ActionModeEum.Exec ? "実行" : "選択";
            }
        }

        public SimpleDelegateCommand EditPromptItemCommand => new((parameter) => {
            if (SelectedPromptItem == null) {
                System.Windows.MessageBox.Show("プロンプトテンプレートが選択されていません。");
                return;
            }
            EditPromptItemWindow window = new EditPromptItemWindow();
            EditPromptItemWindowViewModel editPromptItemWindowViewModel = (EditPromptItemWindowViewModel)window.DataContext;
            editPromptItemWindowViewModel.Initialize(SelectedPromptItem, (PromptItemViewModel) => {
                // PromptItemsを更新
                Reload();
            });
            window.ShowDialog();
        });

        // プロンプトテンプレート処理を追加する処理
        public SimpleDelegateCommand AddPromptItemCommand => new SimpleDelegateCommand(AddPromptItemCommandExecute);

        public void AddPromptItemCommandExecute(object parameter) {
            PromptItemViewModel itemViewModel = new PromptItemViewModel(new PromptItem());
            EditPromptItemWindow window = new EditPromptItemWindow();
            EditPromptItemWindowViewModel editPromptItemWindowViewModel = (EditPromptItemWindowViewModel)window.DataContext;
            editPromptItemWindowViewModel.Initialize(itemViewModel, (PromptItemViewModel) => { 
                // PromptItemsを更新
                Reload();
            });
            window.ShowDialog();
        }
        // プロンプトテンプレートを選択する処理
        public SimpleDelegateCommand SelectPromptItemCommand => new((object parameter) => {
            // 選択されていない場合はメッセージを表示
            if (SelectedPromptItem == null) {
                MessageBox.Show("プロンプトテンプレートが選択されていません。");
                return;
            }
            // Mode からOpenAIExecutionModeEnumに変換
            OpenAIExecutionModeEnum mode = (OpenAIExecutionModeEnum)Mode;
            AfterSelect(SelectedPromptItem, mode);

            // Windowを閉じる
            if (parameter is System.Windows.Window window) {
                window.Close();
            }
        });

        // プロンプトテンプレートを削除する処理
        public SimpleDelegateCommand DeletePromptItemCommand => new(DeletePromptItemCommandExecute);
        public void DeletePromptItemCommandExecute(object parameter) {
            PromptItemViewModel? itemViewModel = SelectedPromptItem;
            if (itemViewModel == null) {
                System.Windows.MessageBox.Show("プロンプトテンプレートが選択されていません。");
                return;
            }
            PromptItem? item = SelectedPromptItem?.PromptItem;
            if (item == null) {
                System.Windows.MessageBox.Show("プロンプトテンプレートが選択されていません。");
                return;
            }
            if (System.Windows.MessageBox.Show($"プロンプトテンプレート{item.Name}を削除しますか？", "確認", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes) {
                return;
            }
            PromptItems.Remove(itemViewModel);
            // LiteDBを更新
            ClipboardAppFactory.Instance.GetClipboardDBController().DeletePromptTemplate(item);
            OnPropertyChanged("PromptItems");
        }

        // CloseCommand
        public SimpleDelegateCommand CloseCommand => new ((parameter) => {
            
            if (parameter is System.Windows.Window window) {
                window.Close();
            }
        });

    }
}
