using System.Collections.ObjectModel;
using System.Windows;
using ClipboardApp.View.PythonScriptView.PythonScriptView;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.PythonScriptView {
    public class ListPythonScriptWindowViewModel : MyWindowViewModel {

        public enum ActionModeEnum {
            Edit,
            Select,
            Exec
        }
        private ActionModeEnum ActionMode { get; set; } = ActionModeEnum.Edit;

        public static ObservableCollection<ScriptItem> ScriptItems { get; } = ScriptItem.ScriptItems;

        private ScriptItem? _selectedScriptItem;

        public ScriptItem? SelectedScriptItem {
            get {
                return _selectedScriptItem;
            }
            set {
                _selectedScriptItem = value;
                OnPropertyChanged(nameof(ScriptItem));
            }
        }

        public string Title {
            get {
                return ActionMode == ActionModeEnum.Exec ? "Pythonスクリプトを選択" : "Pythonスクリプト一覧";
            }
        }

        private Action<ScriptItem> afterSelect = (scriptItem) => { };

        // ExecモードまたはSelectモード時は、実行ボタンを表示する。
        public Visibility ExecButtonVisibility {
            get {
                return (ActionMode == ActionModeEnum.Exec || ActionMode == ActionModeEnum.Select)? Visibility.Visible : Visibility.Collapsed;
            }
        }
        // Selectボタンのテキスト Selectモード時は「選択」、Execモード時は「実行」
        public string SelectButtonText {
            get {
                return ActionMode == ActionModeEnum.Exec ? "実行" : "選択";
            }
        }


        public void Initialize(ActionModeEnum actionModeEnum ,Action<ScriptItem> afterSelect) {
            ActionMode = actionModeEnum;
            this.afterSelect = afterSelect;

            ScriptItems.Clear();
            foreach (var item in ClipboardAppFactory.Instance.GetClipboardDBController().GetScriptItems()) {
                ScriptItems.Add(item);
            }
            OnPropertyChanged(nameof(ScriptItems));
            OnPropertyChanged(nameof(ExecButtonVisibility));
        }


        // Scriptを新規作成するときの処理
        public static SimpleDelegateCommand CreateScriptItemCommand => new(PythonCommands.CreateScriptCommandExecute);

        // Scriptを編集するときの処理
        public SimpleDelegateCommand EditScriptItemCommand => new((parameter) => {
            if (SelectedScriptItem == null) {
                Tools.Error("スクリプトを選択してください");
                return;
            }
            PythonCommands.EditScriptItemCommandExecute(SelectedScriptItem);
        });
        // Scriptを削除したときの処理
        public SimpleDelegateCommand DeleteScriptCommand => new((parameter) => {
            if (SelectedScriptItem == null) {
                Tools.Error("スクリプトを選択してください");
                return;
            }
            ScriptItem.DeleteScriptItem(SelectedScriptItem);
            ScriptItems.Remove(SelectedScriptItem);
            OnPropertyChanged(nameof(ScriptItems));
        });

        // 選択ボタンを押したときの処理
        public SimpleDelegateCommand SelectScriptItemCommand => new((parameter) => {
            if (_selectedScriptItem == null) {
                Tools.Error("スクリプトを選択してください");
                return;
            }
            // Actionを実行
            afterSelect(_selectedScriptItem);
            // ウィンドウを閉じる
            if (parameter is ListPythonScriptWindow selectScriptWindow) {
                selectScriptWindow.Close();

            }
        });
        // キャンセルボタンを押したときの処理
        public SimpleDelegateCommand CloseCommand => new((parameter) => {
            // ウィンドウを閉じる
            if (parameter is ListPythonScriptWindow selectScriptWindow) {
                selectScriptWindow.Close();
            }

        });

    }
}
