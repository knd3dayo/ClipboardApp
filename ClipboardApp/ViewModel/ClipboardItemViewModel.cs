using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.VectorSearchView;
using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILib.Model;
using QAChat.View.VectorDBWindow;
using QAChat.ViewModel;
using WpfAppCommon;
using WpfAppCommon.Control.QAChat;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel {
    public partial class ClipboardItemViewModel : ObservableObject {

        // コンストラクタ
        public ClipboardItemViewModel(ClipboardFolderViewModel folderViewModel, ClipboardItem clipboardItem) {
            ClipboardItem = clipboardItem;
            FolderViewModel = folderViewModel;

            OnPropertyChanged(nameof(Content));
            OnPropertyChanged(nameof(Images));
            OnPropertyChanged(nameof(Files));
            OnPropertyChanged(nameof(ThumbnailImages));

        }
        // StringResources
        public CommonStringResources StringResources { get; } = CommonStringResources.Instance;

        // ClipboardItem
        public ClipboardItem ClipboardItem { get; }
        // FolderViewModel
        public ClipboardFolderViewModel FolderViewModel { get; set; }

        // Context Menu

        public ObservableCollection<MenuItem> MenuItems {
            get {
                return FolderViewModel.CreateItemContextMenuItems(this);
            }
        }

        // Content
        public string Content {
            get {
                return ClipboardItem.Content;
            }
            set {
                ClipboardItem.Content = value;
                OnPropertyChanged(nameof(Content));
            }
        }
        // BackgroundInfo
        public string BackgroundInfo {
            get {
                return ClipboardItem.BackgroundInfo;
            }
            set {
                ClipboardItem.BackgroundInfo = value;
                OnPropertyChanged(nameof(BackgroundInfo));
            }
        }
        public string Summary {
            get {
                return ClipboardItem.Summary;
            }
            set {
                ClipboardItem.Summary = value;
                OnPropertyChanged(nameof(Summary));
            }
        }

        // Image
        public ObservableCollection<ImageSource> Images {
            get {
                ObservableCollection<ImageSource> imageSources = [];
                foreach (var clipboardItemImage in ClipboardItem.ClipboardItemImages) {
                    BitmapImage? bitmapImage = clipboardItemImage.BitmapImage;
                    if (bitmapImage != null) {
                        imageSources.Add(bitmapImage);
                    }
                }
                return imageSources;
            }

        }
        // Files
        public ObservableCollection<ClipboardItemFile> Files {
            get {
                return [.. ClipboardItem.ClipboardItemFiles];
            }
        }

        // ThumbnailImage
        public ObservableCollection<ImageSource> ThumbnailImages {
            get {
                ObservableCollection<ImageSource> imageSources = new();
                foreach (var clipboardItemImage in ClipboardItem.ClipboardItemImages) {
                    BitmapImage? bitmapImage = clipboardItemImage.ThumbnailBitmapImage;
                    if (bitmapImage != null) {
                        imageSources.Add(bitmapImage);
                    }
                }
                return imageSources;
            }
        }

        // Description
        public string Description {
            get {
                return ClipboardItem.Description;
            }
            set {
                ClipboardItem.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        // Tags
        public HashSet<string> Tags {
            get {
                return ClipboardItem.Tags;
            }
            set {
                ClipboardItem.Tags = value;
                OnPropertyChanged(nameof(Tags));
            }
        }

        public string ToolTipString {
            get {
                string result = "";
                if (string.IsNullOrEmpty(ClipboardItem.Description) == false) {
                    result += DescriptionText + "\n";
                }
                result += HeaderText + "\n" + ClipboardItem.Content;
                return result;
            }
        }


        // GUI関連
        // 説明が空かつタグが空の場合はCollapsed,それ以外はVisible
        public Visibility DescriptionVisibility {
            get {
                if (string.IsNullOrEmpty(ClipboardItem.Description) && ClipboardItem.Tags.Count() == 0) {
                    return Visibility.Collapsed;
                } else {
                    return Visibility.Visible;
                }
            }
        }
        // 分類がFileの場合はVisible,それ以外はCollapsed
        public Visibility FileVisibility {
            get {
                if (ClipboardItem.ContentType == ClipboardContentTypes.Files) {
                    return Visibility.Visible;
                } else {
                    return Visibility.Collapsed;
                }
            }
        }
        // 分類がTextの場合はVisible,それ以外はCollapsed
        public Visibility TextVisibility {
            get {
                if (ClipboardItem.ContentType == ClipboardContentTypes.Text) {
                    return Visibility.Visible;
                } else {
                    return Visibility.Collapsed;
                }
            }
        }
        // BackgroundInfoが空の場合はCollapsed,それ以外はVisible
        public Visibility BackgroundInfoVisibility {
            get {
                if (string.IsNullOrEmpty(ClipboardItem.BackgroundInfo)) {
                    return Visibility.Collapsed;
                } else {
                    return Visibility.Visible;
                }
            }
        }
        // サマリーが空の場合はCollapsed,それ以外はVisible
        public Visibility SummaryVisibility {
            get {
                if (string.IsNullOrEmpty(ClipboardItem.Summary)) {
                    return Visibility.Collapsed;
                } else {
                    return Visibility.Visible;
                }
            }
        }

        public string DescriptionText {
            get {
                string result = "";
                if (string.IsNullOrEmpty(ClipboardItem.Description) == false) {
                    result += ClipboardItem.Description;
                }
                return result;
            }
        }
        public string TagsText {
            get {
                return string.Join(",", ClipboardItem.Tags);
            }
        }

        public string SourceApplicationTitleText {
            get {
                return ClipboardItem.SourceApplicationTitle;
            }
            set {
                ClipboardItem.SourceApplicationTitle = value;
                OnPropertyChanged(nameof(SourceApplicationTitleText));
            }
        }

        // 表示用の文字列
        public string? HeaderText {
            get {
                return ClipboardItem.HeaderText;
            }
        }
        public string UpdatedAtString {
            get {
                return ClipboardItem.UpdatedAtString;
            }
        }

        public string ContentTypeString {
            get {
                return ClipboardItem.ContentTypeString;
            }
        }

        // IsPinned
        public bool IsPinned {
            get {
                return ClipboardItem.IsPinned;
            }
            set {
                ClipboardItem.IsPinned = value;
                // 保存
                ClipboardItem.Save();
                OnPropertyChanged(nameof(IsPinned));
            }
        }
        // ContentType
        public ClipboardContentTypes ContentType {
            get {
                return ClipboardItem.ContentType;
            }
        }

        // MergeItems
        public void MergeItems(List<ClipboardItemViewModel> itemViewModels, bool mergeWithHeader, Action<ActionMessage>? action) {
            List<ClipboardItem> items = [];
            foreach (var itemViewModel in itemViewModels) {
                items.Add(itemViewModel.ClipboardItem);
            }
            ClipboardItem.MergeItems(items, mergeWithHeader, action);
        }


        // Copy
        public ClipboardItemViewModel Copy() {
            return new ClipboardItemViewModel(FolderViewModel, ClipboardItem.Copy());
        }

        public void UpdateTagList(ObservableCollection<TagItemViewModel> tagList) {
            // TagListのチェックを反映
            foreach (var item in tagList) {
                if (item.IsChecked) {
                    Tags.Add(item.Tag);
                } else {
                    Tags.Remove(item.Tag);
                }
            }
            // DBに反映
            SaveClipboardItemCommand.Execute(true);
        }

        // アイテム保存
        public SimpleDelegateCommand<bool> SaveClipboardItemCommand => new(ClipboardItem.Save);

        // Delete
        public SimpleDelegateCommand<ClipboardItemViewModel> DeleteItemCommand => new((obj) => {
            ClipboardItem.Delete();
        });

        public SimpleDelegateCommand<object> OpenFolderCommand => new((parameter) => {
            // ContentTypeがFileの場合のみフォルダを開く
            if (ContentType != ClipboardContentTypes.Files) {
                LogWrapper.Error(StringResources.CannotOpenFolderForNonFileContent);
                return;
            }
            // Process.Startでフォルダを開く
            foreach (var item in ClipboardItem.ClipboardItemFiles) {
                string? folderPath = item.FolderName;
                if (folderPath != null) {
                    var p = new Process {
                        StartInfo = new ProcessStartInfo(folderPath) {
                            UseShellExecute = true
                        }
                    };
                    p.Start();
                }
            }
        });

        // コンテキストメニューの「テキストを抽出」の実行用コマンド
        public SimpleDelegateCommand<object> ExtractTextCommand => new((parameter) => {
            if (ContentType != ClipboardContentTypes.Files) {
                LogWrapper.Error(StringResources.CannotExtractTextForNonFileContent);
                return;
            }
            ClipboardItem.ExtractTextCommandExecute(ClipboardItem);
            // 保存
            SaveClipboardItemCommand.Execute(true);
        });
        // OpenAI Chatを開くコマンド
        public SimpleDelegateCommand<object> OpenOpenAIChatWindowCommand => new((parameter) => {

            SearchRule rule = ClipboardFolder.GlobalSearchCondition.Copy();

            QAChatStartupProps qAChatStartupProps = new(FolderViewModel.ClipboardItemFolder, ClipboardItem, false) {

                // ベクトルDBアイテムを開くアクション
                OpenVectorDBItemAction = (vectorDBItem) => {
                    VectorDBItemViewModel vectorDBItemViewModel = new(vectorDBItem);
                    EditVectorDBWindow.OpenEditVectorDBWindow(vectorDBItemViewModel, (model) => { });
                },
                // ベクトルDBアイテムを選択するアクション
                SelectVectorDBItemsAction = (vectorDBItems) => {
                    ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, (selectedItem) => {
                        vectorDBItems.Add(selectedItem);
                    });
                },
                // フォルダ選択アクション
                SelectFolderAction = (vectorDBItems) => {
                    if (MainWindowViewModel.ActiveInstance == null) {
                        LogWrapper.Error(StringResources.MainWindowViewModelIsNull);
                        return;
                    }
                    FolderSelectWindow.OpenFolderSelectWindow(MainWindowViewModel.ActiveInstance.RootFolderViewModel, (folderViewModel) => {
                        vectorDBItems.Add(folderViewModel.ClipboardItemFolder.GetVectorDBItem());
                    });
                },
                // 選択中のクリップボードアイテムを取得するアクション
                GetSelectedClipboardItemImageFunction = () => {
                    List<ClipboardItemImage> images = [];
                    var selectedItems = MainWindowViewModel.ActiveInstance?.SelectedItems;
                    if (selectedItems == null) {
                        return images;
                    }
                    foreach (ClipboardItemViewModel selectedItem in selectedItems) {
                        selectedItem.ClipboardItem.ClipboardItemImages.ForEach((image) => {
                            images.Add(image);
                        });
                    }
                    return images;
                }

            };
            QAChat.MainWindow.OpenOpenAIChatWindow(qAChatStartupProps);

        });

        // ファイルを開くコマンド
        public SimpleDelegateCommand<object> OpenFileCommand => new((obj) => {
            // 選択中のアイテムを開く
            ClipboardAppFactory.Instance.GetClipboardProcessController().OpenClipboardItemFile(ClipboardItem, false);
        });

        // ファイルを新規ファイルとして開くコマンド
        public SimpleDelegateCommand<object> OpenFileAsNewFileCommand => new((obj) => {
            // 選択中のアイテムを開く
            ClipboardAppFactory.Instance.GetClipboardProcessController().OpenClipboardItemFile(ClipboardItem, true);
        });

        // 背景情報を生成するコマンド
        public SimpleDelegateCommand<object> GenerateBackgroundInfoCommand => new(async (obj) => {
            LogWrapper.Info(StringResources.GenerateBackgroundInformation);
            await Task.Run(() => {
                ClipboardItem.CreateAutoBackgroundInfo(this.ClipboardItem);
                // 保存
                SaveClipboardItemCommand.Execute(false);
            });
            LogWrapper.Info(StringResources.GeneratedBackgroundInformation);

        });
        // サマリーを生成するコマンド
        public SimpleDelegateCommand<object> GenerateSummaryCommand => new(async (obj) => {
            LogWrapper.Info(StringResources.GenerateSummary2);
            await Task.Run(() => {
                ClipboardItem.CreateAutoSummary(this.ClipboardItem);
                // 保存
                SaveClipboardItemCommand.Execute(false);
            });
            LogWrapper.Info(StringResources.GeneratedSummary);

        });
        // ベクトルを生成するコマンド
        public SimpleDelegateCommand<object> GenerateVectorCommand => new(async (obj) => {
            LogWrapper.Info(StringResources.GenerateVector2);
            await Task.Run(() => {
                ClipboardItem.UpdateEmbedding();
                // 保存
                SaveClipboardItemCommand.Execute(false);
            });
            LogWrapper.Info(StringResources.GeneratedVector);

        });
        // ベクトル検索を実行するコマンド
        public SimpleDelegateCommand<object> VectorSearchCommand => new(async (obj) => {
            // ClipboardItemを元にベクトル検索を実行
            List<VectorSearchResult> vectorSearchResults = [];
            await Task.Run(() => {
                // ベクトル検索を実行
                vectorSearchResults.AddRange(ClipboardItem.VectorSearchCommandExecute(ClipboardItem));
            });
            // ベクトル検索結果ウィンドウを開く
            VectorSearchResultWindow.OpenVectorSearchResultWindow(vectorSearchResults);

        });


        // 画像を開くコマンド
        public SimpleDelegateCommand<object> OpenImageCommand => new((obj) => {
            // 選択中のアイテムを開く
            ClipboardAppFactory.Instance.GetClipboardProcessController().OpenClipboardItemImage(ClipboardItem);
        });

        // テキストをファイルとして開くコマンド
        public SimpleDelegateCommand<object> OpenContentAsFileCommand => new((obj) => {
            try {
                // 選択中のアイテムを開く
                ClipboardAppFactory.Instance.GetClipboardProcessController().OpenClipboardItemContent(ClipboardItem);
            } catch (Exception e) {
                LogWrapper.Error(e.Message);
            }
        });

        // 画像からイメージを抽出するコマンド
        public SimpleDelegateCommand<object> ExtractTextFromImageWithOpenAICommand => new((obj) => {
            if (ClipboardItem.ContentType != ClipboardContentTypes.Image) {
                throw new Exception(StringResources.CannotExtractTextForNonImageContent);
            }
            ChatRequest.ExtractTextFromImage(ClipboardAppConfig.CreateOpenAIProperties(), ClipboardItem.ClipboardItemImages.Select(image => image.ImageBase64).ToList());

        });
        // ピン留めの切り替えコマンド
        public SimpleDelegateCommand<object> ChangePinCommand => new((obj) => {
            IsPinned = !IsPinned;
            // ピン留めの時は更新日時を変更しない
            SaveClipboardItemCommand.Execute(false);
        });
    }
}
