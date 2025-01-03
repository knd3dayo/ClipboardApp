using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using PythonAILib.Model.Folder;
using PythonAILib.Model.Prompt;
using QAChat.Model;
using QAChat.Resource;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.Folder {
    public class ExportImportWindowViewModel(ContentFolderViewModel ClipboardFolderViewModel, Action AfterUpdate) : QAChatViewModelBase {


        // ImportItems
        public ObservableCollection<ExportImportItem> ImportItems { get; set; } = CreateImportItems();

        // ExportItems
        public ObservableCollection<ExportImportItem> ExportItems { get; set; } = CreateExportItems();

        private static ObservableCollection<ExportImportItem> CreateImportItems() {
            return [
                new ExportImportItem("Title", CommonStringResources.Instance.Title, true, false),
                new ExportImportItem("Text", CommonStringResources.Instance.Text, true, false),
            ];
        }

        private static ObservableCollection<ExportImportItem> CreateExportItems() {
            // PromptItemの設定 出力タイプがテキストコンテンツのものを取得
            List<PromptItem> promptItems = PromptItem.GetPromptItems().Where(item => item.PromptResultType == PromptResultTypeEnum.TextContent).ToList();

            ObservableCollection<ExportImportItem> items = [
                new ExportImportItem("Title", CommonStringResources.Instance.Title, true, false),
                new ExportImportItem("Text", CommonStringResources.Instance.Text, true, false),
            ];
            foreach (PromptItem promptItem in promptItems) {
                items.Add(new ExportImportItem(promptItem.Name, promptItem.Description, false, true));
            }
            return items;
        }


        public int SelectedIndex { get; set; }

        // 選択したファイル名
        public string SelectedFileName { get; set; } = "";

        // インポート時に自動処理を実行
        public bool IsAutoProcessEnabled { get; set; } = false;


        public SimpleDelegateCommand<Window> OKCommand => new((window) => {

            IsIndeterminate = true;
            // 選択されたインデックスによって処理を分岐
            Task.Run(() => {
                switch (SelectedIndex) {
                    case 0:
                        // エクスポート処理
                        ClipboardFolderViewModel.Folder.ExportToExcel(SelectedFileName, [.. ExportItems]);
                        break;
                    case 1:
                        // インポート処理
                        ClipboardFolderViewModel.Folder.ImportFromExcel(SelectedFileName, [.. ImportItems], IsAutoProcessEnabled);
                        break;
                    default:
                        break;
                }
                MainUITask.Run(() => {
                    IsIndeterminate = false;
                    AfterUpdate();
                    window.Close();
                });
            });
        });

        public SimpleDelegateCommand<object> SelectExportFileCommand => new((obj) => {
            // SelectedFileNameが空の場合はデフォルトのファイル名を設定
            if (SelectedFileName == "") {
                SelectedFileName = DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + ClipboardFolderViewModel.Folder.Id.ToString() + ".xlsx";
                OnPropertyChanged(nameof(SelectedFileName));
            }

            //ファイルダイアログを表示


            using var dialog = new CommonOpenFileDialog() {
                Title = CommonStringResources.Instance.SelectFilePlease,
                DefaultFileName = SelectedFileName,
                InitialDirectory = @".",
            };
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            if (dialog.ShowDialog(window) != CommonFileDialogResult.Ok) {
                return;
            } else {
                SelectedFileName = dialog.FileName;
                OnPropertyChanged(nameof(SelectedFileName));
            }
        });

        public SimpleDelegateCommand<object> SelectImportFileCommand => new((obj) => {
            //ファイルダイアログを表示
            using var dialog = new CommonOpenFileDialog() {
                Title = CommonStringResources.Instance.SelectFilePlease,
                InitialDirectory = @".",
                DefaultExtension = ".xlsx",
            };
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            if (dialog.ShowDialog(window) != CommonFileDialogResult.Ok) {
                return;
            } else {
                SelectedFileName = dialog.FileName;
                OnPropertyChanged(nameof(SelectedFileName));
            }
        });

    }
}
