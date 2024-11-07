using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;
using ClipboardApp.Factory;
using ClipboardApp.Model.Search;
using LiteDB;
using PythonAILib.Model;
using PythonAILib.Model.Content;
using PythonAILib.Model.File;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.VectorDB;
using PythonAILib.PythonIF;
using QAChat;
using QAChat.Resource;
using WK.Libraries.SharpClipboardNS;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace ClipboardApp.Model.Folder {
    public partial class ClipboardFolder : ContentFolder {

        public static readonly string CLIPBOARD_ROOT_FOLDER_NAME = CommonStringResources.Instance.Clipboard;
        public static readonly string SEARCH_ROOT_FOLDER_NAME = CommonStringResources.Instance.SearchFolder;
        public static readonly string CHAT_ROOT_FOLDER_NAME = CommonStringResources.Instance.ChatHistory;
        public static readonly string IMAGECHECK_ROOT_FOLDER_NAME = CommonStringResources.Instance.ImageChat;


        //--------------------------------------------------------------------------------
        // コンストラクタ
        public ClipboardFolder() { }

        protected ClipboardFolder(ClipboardFolder? parent, string folderName) {

            ParentId = parent?.Id ?? ObjectId.Empty;
            FolderName = folderName;
            // 親フォルダがnullの場合は、FolderTypeをNormalに設定
            FolderType = parent?.FolderType ?? FolderTypeEnum.Normal;
            // 親フォルダのAutoProcessEnabledを継承
            IsAutoProcessEnabled = parent?.IsAutoProcessEnabled ?? true;

        }
        // 言語変更時にルートフォルダ名を変更する
        public static void ChangeRootFolderNames(CommonStringResources toRes) {
            // ClipboardRootFolder
            ClipboardFolder? clipboardRootFolder = ClipboardAppFactory.Instance.GetClipboardDBController().GetRootFolderByType(FolderTypeEnum.Normal);
            if (clipboardRootFolder != null) {
                clipboardRootFolder.FolderName = toRes.Clipboard;
                clipboardRootFolder.Save();
            }
            // SearchRootFolder
            ClipboardFolder? searchRootFolder = ClipboardAppFactory.Instance.GetClipboardDBController().GetRootFolderByType(FolderTypeEnum.Search);
            if (searchRootFolder != null) {
                searchRootFolder.FolderName = toRes.SearchFolder;
                searchRootFolder.Save();
            }
            // ChatRootFolder
            ClipboardFolder? chatRootFolder = ClipboardAppFactory.Instance.GetClipboardDBController().GetRootFolderByType(FolderTypeEnum.Chat);
            if (chatRootFolder != null) {
                chatRootFolder.FolderName = toRes.ChatHistory;
                chatRootFolder.Save();
            }
            // ImageCheckRootFolder
            ClipboardFolder? imageCheckRootFolder = ClipboardAppFactory.Instance.GetClipboardDBController().GetRootFolderByType(FolderTypeEnum.ImageCheck);
            if (imageCheckRootFolder != null) {
                imageCheckRootFolder.FolderName = toRes.ImageChat;
                imageCheckRootFolder.Save();
            }
        }

        // フォルダの種類
        public FolderTypeEnum FolderType { get; set; } = FolderTypeEnum.Normal;

        // ルートフォルダか否か
        public bool IsRootFolder { get; set; } = false;

        // AutoProcessを有効にするかどうか
        public bool IsAutoProcessEnabled { get; set; } = true;

        // AutoProcessRuleのIdのリスト
        public List<ObjectId> AutoProcessRuleIds { get; set; } = [];
        // フォルダの参照用のベクトルDBのリストに含めるかどうかを示すプロパティ名を変更
        public bool IncludeInReferenceVectorDBItems { get; set; } = true;

        // フォルダの絶対パス ファイルシステム用
        public override string FolderPath {
            get {
                ClipboardFolder? parent = (ClipboardFolder?)ClipboardAppFactory.Instance.GetClipboardDBController().GetFolder(ParentId);
                if (parent == null) {
                    return FolderName;
                }
                return Tools.ConcatenateFileSystemPath(parent.FolderPath, FolderName);
            }
        }

        // 子フォルダ　LiteDBには保存しない。
        [BsonIgnore]
        public List<ClipboardFolder> Children {
            get {
                // DBからParentIDが自分のIDのものを取得
                return ClipboardAppFactory.Instance.GetClipboardDBController().GetFoldersByParentId(Id);
            }
        }

        // アイテム LiteDBには保存しない。
        [BsonIgnore]
        public List<ClipboardItem> Items {
            get {
                if (FolderType == FolderTypeEnum.Search) {
                    return GetSearchFolderItems();
                }
                return GetNormalFolderItems();
            }
        }

        // アプリ共通の検索条件
        public static SearchRule GlobalSearchCondition { get; set; } = new();

        //--------------------------------------------------------------------------------
        public static ClipboardFolder RootFolder {
            get {
                ClipboardFolder? rootFolder = ClipboardAppFactory.Instance.GetClipboardDBController().GetRootFolderByType(FolderTypeEnum.Normal);
                if (rootFolder == null) {
                    rootFolder = new() {
                        FolderName = CLIPBOARD_ROOT_FOLDER_NAME,
                        IsRootFolder = true,
                        IsAutoProcessEnabled = true,
                        FolderType = FolderTypeEnum.Normal
                    };
                    rootFolder.Save();
                }
                // 既にRootFolder作成済みの環境のための措置
                rootFolder.IsRootFolder = true;
                return rootFolder;
            }
        }

        public static ClipboardFolder SearchRootFolder {
            get {
                ClipboardFolder? searchRootFolder = ClipboardAppFactory.Instance.GetClipboardDBController().GetRootFolderByType(FolderTypeEnum.Search);
                if (searchRootFolder == null) {
                    searchRootFolder = new ClipboardFolder {
                        FolderName = SEARCH_ROOT_FOLDER_NAME,
                        FolderType = FolderTypeEnum.Search,
                        IsRootFolder = true,
                        // 自動処理を無効にする
                        IsAutoProcessEnabled = false
                    };
                    searchRootFolder.Save();
                }
                // 既にSearchRootFolder作成済みの環境のための措置
                searchRootFolder.IsRootFolder = true;
                searchRootFolder.FolderType = FolderTypeEnum.Search;
                return searchRootFolder;
            }
        }

        public static ClipboardFolder ChatRootFolder {
            get {
                ClipboardFolder? chatRootFolder = ClipboardAppFactory.Instance.GetClipboardDBController().GetRootFolderByType(FolderTypeEnum.Chat);
                if (chatRootFolder == null) {
                    chatRootFolder = new ClipboardFolder {
                        FolderName = CHAT_ROOT_FOLDER_NAME,
                        FolderType = FolderTypeEnum.Chat,
                        IsRootFolder = true,
                        // 自動処理を無効にする
                        IsAutoProcessEnabled = false
                    };
                    chatRootFolder.Save();
                }
                // 既にSearchRootFolder作成済みの環境のための措置
                chatRootFolder.IsRootFolder = true;
                return chatRootFolder;
            }
        }

        private List<ClipboardItem> GetNormalFolderItems() {
            List<ClipboardItem> _items = [];
            // このフォルダが通常フォルダの場合は、GlobalSearchConditionを適用して取得,
            // 検索フォルダの場合は、SearchConditionを適用して取得
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // 通常のフォルダの場合で、GlobalSearchConditionが設定されている場合
            if (GlobalSearchCondition.SearchCondition != null && GlobalSearchCondition.SearchCondition.IsEmpty() == false) {
                _items = [.. ClipboardDatabaseController.SearchItems(this, GlobalSearchCondition.SearchCondition)];

            } else {
                // 通常のフォルダの場合で、GlobalSearchConditionが設定されていない場合
                _items = [.. ClipboardDatabaseController.GetItems(this)];
            }
            return _items;
        }

        private List<ClipboardItem> GetSearchFolderItems() {
            List<ClipboardItem> _items = [];
            // このフォルダが通常フォルダの場合は、GlobalSearchConditionを適用して取得,
            // 検索フォルダの場合は、SearchConditionを適用して取得
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // フォルダに検索条件が設定されている場合
            SearchRule? searchConditionRule = SearchRuleController.GetSearchRuleByFolder(this);
            if (searchConditionRule != null && searchConditionRule.TargetFolder != null) {
                // 検索対象フォルダのアイテムを検索する。
                _items = [.. ClipboardDatabaseController.SearchItems(searchConditionRule.TargetFolder, searchConditionRule.SearchCondition)];

            }
            // 検索対象フォルダパスがない場合は何もしない。
            return _items;
        }

        // 子フォルダ BSonMapper.GlobalでIgnore設定しているので、LiteDBには保存されない
        public void DeleteChild(ClipboardFolder child) {
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            ClipboardDatabaseController.DeleteFolder(child);
        }

        public ClipboardFolder CreateChild(string folderName) {
            ClipboardFolder child = new(this, folderName);
            return child;
        }

        //--------------------------------------------------------------------------------
        // ReferenceVectorDBItemsからVectorDBItemを削除
        public void RemoveVectorDBItem(VectorDBItem vectorDBItem) {
            List<VectorDBItem> existingItems = new(ReferenceVectorDBItems.Where(x => x.Name == vectorDBItem.Name && x.CollectionName == vectorDBItem.CollectionName));
            foreach (var item in existingItems) {
                ReferenceVectorDBItems.Remove(item);
            }
        }
        // ReferenceVectorDBItemsにVectorDBItemを追加
        public void AddVectorDBItem(VectorDBItem vectorDBItem) {
            var existingItems = ReferenceVectorDBItems.FirstOrDefault(x => x.Name == vectorDBItem.Name && x.CollectionName == vectorDBItem.CollectionName);
            if (existingItems == null) {
                ReferenceVectorDBItems.Add(vectorDBItem);
            }
        }


        // 自分自身を保存
        public override void Save() {
            // IncludeInReferenceVectorDBItemsがTrueの場合は、ReferenceVectorDBItemsに自分自身を追加
            if (IncludeInReferenceVectorDBItems) {
                AddVectorDBItem(GetVectorDBItem());
            } else {
                // IncludeInReferenceVectorDBItemsがFalseの場合は、ReferenceVectorDBItemsから自分自身を削除
                RemoveVectorDBItem(GetVectorDBItem());
            }

            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            ClipboardDatabaseController.UpsertFolder(this);

            // ItemsのIsReferenceVectorDBItemsSyncedをFalseに設定
            foreach (var item in Items) {
                item.IsReferenceVectorDBItemsSynced = false;
                item.Save(false);
            }

        }
        // Delete
        public void Delete() {
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            ClipboardDatabaseController.DeleteFolder(this);
        }

        // アイテムを追加する処理
        public ClipboardItem AddItem(ClipboardItem item) {
            // 検索フォルダの場合は何もしない
            if (FolderType == FolderTypeEnum.Search) {
                return item;
            }

            // CollectionNameを設定
            item.CollectionId = Id;

            // ReferenceVectorDBItemsを設定
            item.ReferenceVectorDBItems = ReferenceVectorDBItems;

            // 自動処理を適用
            ClipboardItem? result = item;
            if (IsAutoProcessEnabled) {
                result = item.ApplyAutoProcess();
            }

            if (result == null) {
                // 自動処理で削除または移動された場合は何もしない
                LogWrapper.Info(CommonStringResources.Instance.ItemsDeletedOrMovedByAutoProcessing);
                return item;
            }
            // 保存
            result.Save();
            // Itemsに追加
            Items.Add(result);
            // 通知
            LogWrapper.Info(CommonStringResources.Instance.AddedItems);
            return item;
        }

        // ClipboardItemを削除
        public void DeleteItem(ClipboardItem item) {
            // 検索フォルダの場合は何もしない
            if (FolderType == FolderTypeEnum.Search) {
                return;
            }

            // LiteDBに保存
            item.Delete();
        }

        // フォルダ内のアイテムをJSON形式でExport
        public void ExportItemsToJson(string fileName) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            var options = jsonSerializerOptions;
            string jsonString = System.Text.Json.JsonSerializer.Serialize(Items, options);

            File.WriteAllText(fileName, jsonString);

        }

        //exportしたJSONファイルをインポート
        public void ImportItemsFromJson(string json) {
            JsonNode? node = JsonNode.Parse(json);
            if (node == null) {
                LogWrapper.Error(CommonStringResources.Instance.FailedToParseJSONString);
                return;
            }
            JsonArray? jsonArray = node as JsonArray;
            if (jsonArray == null) {
                LogWrapper.Error(CommonStringResources.Instance.FailedToParseJSONString);
                return;
            }

            // Itemsをクリア
            Items.Clear();

            foreach (JsonObject? jsonValue in jsonArray.Cast<JsonObject?>()) {
                if (jsonValue == null) {
                    continue;
                }
                string jsonString = jsonValue.ToString();
                ClipboardItem? item = ClipboardItem.FromJson(jsonString);

                if (item == null) {
                    continue;
                }
                item.CollectionId = Id;
                // Itemsに追加
                Items.Add(item);
                //保存
                item.Save();
            }

        }

        // 指定されたフォルダの中のSourceApplicationTitleが一致するアイテムをマージするコマンド
        public void MergeItemsBySourceApplicationTitleCommandExecute(ClipboardItem newItem) {

            // SourceApplicationNameが空の場合は何もしない
            if (string.IsNullOrEmpty(newItem.SourceApplicationName)) {
                return;
            }
            // NewItemのSourceApplicationTitleが空の場合は何もしない
            if (string.IsNullOrEmpty(newItem.SourceApplicationTitle)) {
                return;
            }
            if (Items.Count == 0) {
                return;
            }
            List<ClipboardItem> sameTitleItems = [];
            // マージ先のアイテムのうち、SourceApplicationTitleとSourceApplicationNameが一致するアイテムを取得
            foreach (var item in Items) {
                if (newItem.SourceApplicationTitle == item.SourceApplicationTitle
                    && newItem.SourceApplicationName == item.SourceApplicationName) {
                    // TypeがTextのアイテムのみマージ
                    if (item.ContentType == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text) {
                        sameTitleItems.Add(item);
                    }
                }
            }
            newItem.MergeItems(sameTitleItems);
        }

        // 指定されたフォルダの全アイテムをマージするコマンド
        public void MergeItems(ClipboardItem item) {
            item.MergeItems(Items);

        }
        // --- Export/Import
        public void ExportToExcel(string fileName, bool exportTitle, bool exportText, bool exportBackgroundInfo, bool exportSummary) {
            // PythonNetの処理を呼び出す。
            List<List<string>> data = [];
            // ClipboardItemのリスト要素毎に処理を行う
            foreach (var item in Items) {
                List<string> row = [];
                if (exportTitle) {
                    row.Add(item.Description);
                }
                if (exportText) {
                    row.Add(item.Content);
                }
                if (exportBackgroundInfo) {
                    row.Add(item.BackgroundInfo);
                }
                if (exportSummary) {
                    row.Add(item.Summary);
                }
                data.Add(row);
            }
            CommonDataTable dataTable = new(data);

            PythonExecutor.PythonAIFunctions.ExportToExcel(fileName, dataTable);

        }

        public void ImportFromExcel(string fileName, bool executeAutoProcess, bool importTitle, bool importText) {
            // importTitle, importTextがFalseの場合は何もしない
            if (importTitle == false && importText == false) {
                return;
            }

            // PythonNetの処理を呼び出す。
            CommonDataTable data = PythonExecutor.PythonAIFunctions.ImportFromExcel(fileName);
            if (data == null) {
                return;
            }

            foreach (var row in data.Rows) {
                ClipboardItem item = new(Id);
                // importTitleと、importTextがTrueの場合は、row[0]をTitle、row[1]をContentに設定。Row.Countが足りない場合は空文字を設定
                if (importTitle && importText) {
                    item.Description = row.Count > 0 ? row[0] : "";
                    item.Content = row.Count > 1 ? row[1] : "";
                } else if (importTitle) {
                    item.Description = row.Count > 0 ? row[0] : "";
                } else if (importText) {
                    item.Content = row.Count > 0 ? row[0] : "";
                }
                item.Save();
                if (executeAutoProcess) {
                    // システム共通自動処理を適用
                    ProcessClipboardItem(item, (processedItem) => {
                        // 自動処理後のアイテムを保存
                        item.Save();
                    });
                }
            }
        }

        // SystemCommonVectorDBを取得する。
        public VectorDBItem GetVectorDBItem() {
            return ClipboardAppVectorDBItem.GetFolderVectorDBItem(this);
        }

        // フォルダに設定されたVectorDBのコレクションを削除
        public void DeleteVectorDBCollection() {
            PythonAILibManager libManager = PythonAILibManager.Instance;

            VectorDBItem vectorDBItem = GetVectorDBItem();
            PythonExecutor.PythonAIFunctions.DeleteVectorDBCollection(libManager.ConfigParams.GetOpenAIProperties(), vectorDBItem);
        }
        // フォルダに設定されたVectorDBのインデックスを更新
        public void RefreshVectorDBCollection() {
            // ベクトルを全削除
            DeleteVectorDBCollection();
            // ベクトルを再作成
            // フォルダ内のアイテムを取得して、ベクトルを作成
            foreach (var item in Items) {
                item.UpdateEmbedding();
            }
        }

        #region システムのクリップボードへ貼り付けられたアイテムに関連する処理
        public void ProcessClipboardItem(ClipboardChangedEventArgs e, Action<ClipboardItem> _afterClipboardChanged) {

            // Get the cut/copied text.
            List<ClipboardItem> items = CreateClipboardItem(this, e);

            foreach (var item in items) {
                // Process clipboard item
                ProcessClipboardItem(item, _afterClipboardChanged);
            }
        }

        /// Process clipboard item 
        /// <summary>
        /// Process clipboard item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="_afterClipboardChanged"></param>
        public static void ProcessClipboardItem(ClipboardItem item, Action<ClipboardItem> _afterClipboardChanged) {

            // Execute in a separate thread
            Task.Run(() => {
                StatusText statusText = Tools.StatusText;
                MainUITask.Run(() => {
                    statusText.InProgressText = CommonStringResources.Instance.AutoProcessing;
                    statusText.IsInProgress = true;
                });
                try {
                    // Apply automatic processing
                    Task<ClipboardItem?> updatedItemTask = ApplyAutoAction(item);
                    if (updatedItemTask.Result == null) {
                        // If the item is ignored, return
                        return;
                    }
                    // Notify the completion of processing
                    _afterClipboardChanged(updatedItemTask.Result);

                } catch (Exception ex) {
                    LogWrapper.Error($"{CommonStringResources.Instance.AddItemFailed}\n{ex.Message}\n{ex.StackTrace}");

                } finally {
                    MainUITask.Run(() => {
                        statusText.IsInProgress = false;
                    });
                }
            });
        }


        /// Create ContentItem
        public static List<ClipboardItem> CreateClipboardItem(
            ClipboardFolder clipboardFolder, ClipboardChangedEventArgs e) {

            List<ClipboardItem> result = [];

            PythonAILib.Model.File.ContentTypes.ContentItemTypes contentTypes = PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text;
            if (e.ContentType == SharpClipboard.ContentTypes.Text) {
                contentTypes = PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text;
            } else if (e.ContentType == SharpClipboard.ContentTypes.Files) {
                contentTypes = PythonAILib.Model.File.ContentTypes.ContentItemTypes.Files;
            } else if (e.ContentType == SharpClipboard.ContentTypes.Image) {
                contentTypes = PythonAILib.Model.File.ContentTypes.ContentItemTypes.Image;
            } else if (e.ContentType == SharpClipboard.ContentTypes.Other) {
                return result;
            } else {
                return result;
            }

            // If ContentType is Text, set text data
            if (contentTypes == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text) {
                ClipboardItem item = new(clipboardFolder.Id) {
                    ContentType = contentTypes
                };
                SetApplicationInfo(item, e);
                item.Content = (string)e.Content;
                result.Add(item);
                return result;
            }

            // If ContentType is BitmapImage, set image data
            if (contentTypes == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Image) {
                ClipboardItem item = new(clipboardFolder.Id) {
                    ContentType = contentTypes
                };
                SetApplicationInfo(item, e);
                System.Drawing.Image image = (System.Drawing.Image)e.Content;
                // byte
                item.CachedBase64String = PythonAILib.Model.File.ContentTypes.GetBase64StringFromImage(image);
                result.Add(item);
                return result;
            }

            // If ContentType is Files, set file data
            if (contentTypes == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Files) {
                string[] files = (string[])e.Content;

                // Get the cut/copied file/files.
                for (int i = 0; i < files.Length; i++) {
                    ClipboardItem item = new(clipboardFolder.Id) {
                        ContentType = contentTypes
                    };
                    SetApplicationInfo(item, e);
                    item.FilePath = files[i];
                    item.LastModified = new System.IO.FileInfo(item.FilePath).LastWriteTime.Ticks;
                    result.Add(item);
                }
                return result;
            }
            return result;
        }

        /// <summary>
        /// Set application information from ClipboardChangedEventArgs
        /// </summary>
        /// <param name="item"></param>
        /// <param name="sender"></param>
        private static void SetApplicationInfo(ClipboardItem item, ClipboardChangedEventArgs sender) {
            item.SourceApplicationName = sender.SourceApplication.Name;
            item.SourceApplicationTitle = sender.SourceApplication.Title;
            item.SourceApplicationID = sender.SourceApplication.ID;
            item.SourceApplicationPath = sender.SourceApplication.Path;
        }

        /// <summary>
        /// Apply automatic processing
        /// </summary>
        /// <param name="item"></param>
        /// <param name="image"></param>
        private static async Task<ClipboardItem?> ApplyAutoAction(ClipboardItem item) {
            // ★TODO Implement processing based on automatic processing rules.
            // 指定した行数以下のテキストアイテムは無視
            int lineCount = item.Content.Split('\n').Length;
            if (item.ContentType == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text && lineCount <= ClipboardAppConfig.Instance.IgnoreLineCount) {
                return null;
            }

            // ★TODO Implement processing based on automatic processing rules.
            // If AutoMergeItemsBySourceApplicationTitle is set, automatically merge items
            if (ClipboardAppConfig.Instance.AutoMergeItemsBySourceApplicationTitle) {
                LogWrapper.Info(CommonStringResources.Instance.AutoMerge);
                RootFolder.MergeItemsBySourceApplicationTitleCommandExecute(item);
            }
            // If AutoFileExtract is set, extract files
            if (ClipboardAppConfig.Instance.AutoFileExtract && item.ContentType == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Files) {
                string text = PythonExecutor.PythonAIFunctions.ExtractFileToText(item.FilePath);
                item.Content += "\n" + text;
            }
            if (item.IsImage() && item.Image != null) {
                // ★TODO Implement processing based on automatic processing rules.
                // If AutoExtractImageWithPyOCR is set, perform OCR
                if (ClipboardAppConfig.Instance.AutoExtractImageWithPyOCR) {
                    string extractImageText = PythonExecutor.PythonMiscFunctions.ExtractTextFromImage(item.Image, ClipboardAppConfig.Instance.TesseractExePath);
                    item.Content += "\n" + extractImageText;
                    LogWrapper.Info(CommonStringResources.Instance.OCR);

                } else if (ClipboardAppConfig.Instance.AutoExtractImageWithOpenAI) {

                    LogWrapper.Info(CommonStringResources.Instance.AutoExtractImageText);
                    item.ExtractImageWithOpenAI();
                }
            }

            // ★TODO Implement processing based on automatic processing rules.
            var task1 = Task.Run(() => {
                // If AUTO_TAG is set, automatically set the tags
                if (ClipboardAppConfig.Instance.AutoTag) {
                    LogWrapper.Info(CommonStringResources.Instance.AutoSetTag);
                    ClipboardItem.CreateAutoTags(item);
                }
            });
            var task2 = Task.Run(() => {
                // If AUTO_DESCRIPTION is set, automatically set the Description
                if (ClipboardAppConfig.Instance.AutoDescription) {
                    LogWrapper.Info(CommonStringResources.Instance.AutoSetTitle);
                    ClipboardItem.CreateAutoTitle(item);

                } else if (ClipboardAppConfig.Instance.AutoDescriptionWithOpenAI) {

                    LogWrapper.Info(CommonStringResources.Instance.AutoSetTitle);
                    item.CreateAutoTitleWithOpenAI();
                }
            });
            var task3 = Task.Run(() => {
                // 背景情報
                if (ClipboardAppConfig.Instance.AutoBackgroundInfo) {
                    LogWrapper.Info(CommonStringResources.Instance.AutoSetBackgroundInfo);
                    item.CreateAutoBackgroundInfo();
                }
            });
            var task4 = Task.Run(() => {
                // サマリー
                if (ClipboardAppConfig.Instance.AutoSummary) {
                    LogWrapper.Info(CommonStringResources.Instance.AutoCreateSummary);
                    item.CreateChatResult(PromptItem.SystemDefinedPromptNames.SummaryGeneration.ToString());
                }
            });
            var task5 = Task.Run(() => {
                // Tasks
                if (ClipboardAppConfig.Instance.AutoGenerateTasks) {
                    LogWrapper.Info(CommonStringResources.Instance.AutoCreateTaskList);
                    item.CreateChatResult(PromptItem.SystemDefinedPromptNames.TasksGeneration.ToString());
                }
            });
            var task6 = Task.Run(() => {
                // Tasks
                if (ClipboardAppConfig.Instance.AutoDocumentReliabilityCheck) {
                    LogWrapper.Info(CommonStringResources.Instance.AutoCheckDocumentReliability);
                    item.CheckDocumentReliability();
                }
            });

            await Task.WhenAll(task1, task2, task3, task4, task5, task6);

            return item;
        }
        #endregion
    }
}

