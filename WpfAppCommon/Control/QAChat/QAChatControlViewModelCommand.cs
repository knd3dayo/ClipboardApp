using System.Windows;
using System.Windows.Controls;
using PythonAILib.Model;
using PythonAILib.PythonIF;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using WpfAppCommon.View.QAChat;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;

namespace WpfAppCommon.Control.QAChat {
    public partial class QAChatControlViewModel {

        // チャットを送信するコマンド
        public SimpleDelegateCommand<object> SendChatCommand => new(async (parameter) => {
            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                ChatResult? result = null;
                // プログレスバーを表示
                IsIndeterminate = true;

                // Python処理機能の初期化
                PythonExecutor.Init(ClipboardAppConfig.PythonDllPath);

                await Task.Run(() => {

                    // LangChainChat用。VectorDBItemsを設定
                    List<VectorDBItem> items = [.. SystemVectorDBItems, .. ExternalVectorDBItems];
                    ChatController.VectorDBItems = items;

                    // ImageFilesとImageItemsのImageをChatControllerに設定
                    ChatController.ImageURLs = [];
                    foreach (var item in ImageFiles) {
                        ChatController.ImageURLs.Add(ChatRequest.CreateImageURLFromFilePath(item.ScreenShotImage.ImagePath));
                    }
                    foreach (var item in ImageItems) {
                        ChatController.ImageURLs.Add(ChatRequest.CreateImageURL(item.ClipboardItemImage.ImageBase64));
                    }

                    // OpenAIChat or LangChainChatを実行
                    result = ChatController.ExecuteChat();
                });

                if (result == null) {
                    LogWrapper.Error("チャットの送信に失敗しました。");
                    return;
                }
                // ClipboardItemがある場合はClipboardItemのChatItemsを更新
                if (ClipboardItem != null) {
                    ClipboardItem.ChatItems = [.. ChatHistory];

                }
                // inputTextをクリア
                InputText = "";
                OnPropertyChanged(nameof(ChatHistory));


            } catch (Exception e) {
                LogWrapper.Error($"エラーが発生ました:\nメッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}");
            } finally {
                IsIndeterminate = false;
            }

        });

        // Saveコマンド
        public SimpleDelegateCommand<object> SaveCommand => new((parameter) => {
            // ChatHistoryをClipboardItemに設定
            if (ClipboardItem == null) {
                return;
            }
            ClipboardItem.ChatItems = [.. ChatHistory];
            // ClipboardItemを保存
            ClipboardItem.Save();

            //ChatHistoryItemがある場合は保存
            if (ChatHistoryItem != null) {
                ClipboardItem.CopyTo(ChatHistoryItem);
                ChatHistoryItem.Save();
            }

        });

        // クリアコマンド
        public SimpleDelegateCommand<object> ClearChatCommand => new((parameter) => {
            ChatHistory = [];
            InputText = "";
            // ClipboardItemがある場合は、ChatItemsをクリア
            if (ClipboardItem != null) {
                ClipboardItem.ChatItems = [];
            }
            OnPropertyChanged(nameof(ChatHistory));
        });

        // モードが変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> ModeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            int index = comboBox.SelectedIndex;
            ChatController.ChatMode = (OpenAIExecutionModeEnum)index;
            // ModeがRAGの場合は、VectorDBItemを取得
            ExternalVectorDBItems = [];
            if (ChatController.ChatMode == OpenAIExecutionModeEnum.RAG) {
                VectorDBItem? item = ClipboardFolder?.GetVectorDBItem();
                if (item != null) {
                    ExternalVectorDBItems.Add(item);
                }
            }
            // VectorDBItemVisibilityを更新
            OnPropertyChanged(nameof(VectorDBItemVisibility));

        });
        // Tabが変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> TabSelectionChangedCommand => new((routedEventArgs) => {
            if (routedEventArgs.OriginalSource is TabControl tabControl) {
                // タブが変更されたときの処理
                if (tabControl.SelectedIndex == 1) {
                    // プレビュータブが選択された場合、プレビューテキストを更新
                    OnPropertyChanged(nameof(PreviewText));
                } 
                if (tabControl.SelectedIndex == 2) {
                    // プレビュー(JSON)タブが選択された場合、プレビューJSONを更新
                    OnPropertyChanged(nameof(PreviewJson));
                }


            }
        });

        // プロンプトテンプレート画面を開くコマンド
        public SimpleDelegateCommand<object> PromptTemplateCommand => new((parameter) => { 

            PromptTemplateCommandExecute(parameter);
        });

        // Closeコマンド
        public SimpleDelegateCommand<Window?> CloseCommand => new((window) => {

            window?.Close();
        });

        // Ctrl + Aを一回をしたら行選択、二回をしたら全選択
        public SimpleDelegateCommand<TextBox> SelectTextCommand => new((textBox) => {

            // テキスト選択
            TextSelector.SelectText(textBox);
            return;
        });

        // 選択中のテキストをプロセスとして実行
        public SimpleDelegateCommand<TextBox> ExecuteSelectedTextCommand => new((textbox) => {

            // 選択中のテキストをプロセスとして実行
            TextSelector.ExecuteSelectedText(textbox);
        });

        // チャットアイテムを編集するコマンド
        public SimpleDelegateCommand<ChatItem>  OpenChatItemCommand => new((chatItem) => {
            EditChatItemWindow.OpenEditChatItemWindow(chatItem);
        });

        // ベクトルDB(フォルダ)をリストから削除するコマンド
        public SimpleDelegateCommand<object> RemoveVectorDBItemCommand => new((parameter) => {
            if (SelectedSystemVectorDBItem != null) {
                SystemVectorDBItems.Remove(SelectedSystemVectorDBItem);
            }
            OnPropertyChanged(nameof(SystemVectorDBItems));
        });
        // ベクトルDB(フォルダ)を追加するコマンド
        public SimpleDelegateCommand<object> AddVectorDBItemFolderCommand => new((parameter) => {
            // フォルダを選択
            QAChatStartupProps?.SelectFolderAction(SystemVectorDBItems);
            OnPropertyChanged(nameof(SystemVectorDBItems));
        });


        // 選択したVectorDBItemの編集画面を開くコマンド
        public SimpleDelegateCommand<object> OpenExternalVectorDBItemCommand => new((parameter) => {
            QAChatStartupProps?.SelectVectorDBItemsAction(ExternalVectorDBItems);
        });

        // 選択したVectorDBItemをリストから削除するコマンド
        public SimpleDelegateCommand<object> RemoveExternalVectorDBItemCommand => new((parameter) => {
            if (SelectedExternalVectorDBItem != null) {
                ExternalVectorDBItems.Remove(SelectedExternalVectorDBItem);
            }
            OnPropertyChanged(nameof(ExternalVectorDBItems));
        });

        // 画像選択コマンド SelectImageFileCommand
        public SimpleDelegateCommand<Window> SelectImageFileCommand => new((window) => {

            //ファイルダイアログを表示
            // 画像ファイルを選択して画像ファイル名一覧に追加
            CommonOpenFileDialog dialog = new() {
                Title = "画像ファイルを選択してください",
                InitialDirectory = lastSelectedImageFolder,
                Multiselect = true,
                Filters = {
                    new CommonFileDialogFilter("画像ファイル", "*.png;*.jpg;*.jpeg;*.bmp;*.gif"),
                    new CommonFileDialogFilter("すべてのファイル", "*.*"),
                }
            };
            if (dialog.ShowDialog() != CommonFileDialogResult.Ok) {
                return;
            } else {
                foreach (string filePath in dialog.FileNames) {
                    // filePathをフォルダ名とファイル名に分割してフォルダ名を取得
                    string? folderPath = Path.GetDirectoryName(filePath);
                    if (folderPath != null) {
                        lastSelectedImageFolder = folderPath;
                    }
                    // ScreenShotImageを生成してImageFilesに追加
                    ScreenShotImage image = new() {
                        ImagePath = filePath
                    };
                    // 画像ファイル名一覧に画像ファイル名を追加
                    ImageFiles.Add(new ScreenShotImageViewModel(this, image));
                }
                OnPropertyChanged(nameof(ImageFiles));
            }
            window.Activate();

        });



        // クリップボードの画像アイテムを追加
        public SimpleDelegateCommand<Window> PasteImageItemCommand => new((window) => {
            // 選択中のClipboardItemを取得
            List<ClipboardItemImage>? images = QAChatStartupProps?.GetSelectedClipboardItemImageFunction();
            if (images == null) {
                return;
            }
            foreach (ClipboardItemImage image in images) {
                ImageItems.Add(new ClipboardItemImageViewModel(this, image));
            }
        });


    }

}
