using WpfAppCommon.Utils;
using WpfAppCommon.Model;
using System.Windows;
using WpfAppCommon.Control.Settings;

namespace ClipboardApp.View.AutoProcessRuleView
{
    public partial class ListAutoProcessRuleWindowViewModel : MyWindowViewModel {


        // 優先順位を上げる処理
        public SimpleDelegateCommand<string> ChangePriorityCommand => new((parameter) => {
            if (SelectedAutoProcessRule == null) {
                System.Windows.MessageBox.Show("自動処理ルールが選択されていません。");
                return;
            }
            if (parameter == "down") {
                AutoProcessRule.DownPriority(SelectedAutoProcessRule);
            } else {
                AutoProcessRule.UpPriority(SelectedAutoProcessRule);
            }
            // AutoProcessRulesを更新
            AutoProcessRules = [.. AutoProcessRule.GetAllAutoProcessRules()];
            OnPropertyChanged(nameof(AutoProcessRules));
        });

        public SimpleDelegateCommand<object> EditAutoProcessRuleCommand => new((parameter) => {
            // AutoProcessRuleが更新された後の処理
            void AutoProcessRuleUpdated(AutoProcessRule rule) {
                // AutoProcessRulesを更新
                AutoProcessRules = [.. AutoProcessRule.GetAllAutoProcessRules()];
                OnPropertyChanged(nameof(AutoProcessRules));
            }
            // debug
            if (SelectedAutoProcessRule == null) {
                System.Windows.MessageBox.Show("自動処理ルールが選択されていません。");
                return;
            }
            EditAutoProcessRuleWindow.OpenEditAutoProcessRuleWindow(EditAutoProcessRuleWindowViewModel.Mode.Edit, _mainWindowViewModel, SelectedAutoProcessRule, AutoProcessRuleUpdated);
        });

        // 自動処理を追加する処理
        public SimpleDelegateCommand <object> AddAutoProcessRuleCommand => new((parameter) => {
            // AutoProcessRuleが更新された後の処理
            void AutoProcessRuleUpdated(AutoProcessRule rule) {
                // InstanceがNullの場合は処理を終了
                // AutoProcessRulesを更新
                AutoProcessRules = [.. AutoProcessRule.GetAllAutoProcessRules()];
                OnPropertyChanged(nameof(AutoProcessRules));
            }
            EditAutoProcessRuleWindow.OpenEditAutoProcessRuleWindow(EditAutoProcessRuleWindowViewModel.Mode.Create, _mainWindowViewModel, null, AutoProcessRuleUpdated);
        });

        // 自動処理を削除する処理
        public SimpleDelegateCommand<object> DeleteAutoProcessRuleCommand => new((parameter) => {
            AutoProcessRule? rule = SelectedAutoProcessRule;
            if (rule == null) {
                System.Windows.MessageBox.Show("自動処理ルールが選択されていません。");
                return;
            }
            if (System.Windows.MessageBox.Show($"自動処理ルール{rule.RuleName}を削除しますか？", "確認", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes) {
                return;
            }
            AutoProcessRules.Remove(rule);
            // LiteDBを更新
            rule.Delete();
            OnPropertyChanged(nameof(AutoProcessRules));
        });

        // SaveSystemCommonSettingCommand
        public SimpleDelegateCommand<object> SaveSystemCommonSettingCommand => new((parameter) => {
            if (SettingUserControlViewModel.Save()) {
                LogWrapper.Info("システム共通設定を保存しました。");
            } else {
                LogWrapper.Warn("システム共通設定の変更はありません。");
            }
        });
        // CloseCommand
        public SimpleDelegateCommand<Window> CloseCommand => new ((window) => {
            window.Close();
        });
    }
}
