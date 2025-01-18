using System.IO;
using System.Net.Http;
using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.Folder;
using PythonAILib.Model.Search;
using PythonAILib.Model.VectorDB;
using PythonAILib.PythonIF;
using PythonAILib.Resource;
using PythonAILib.Utils.Common;

namespace PythonAILib.Model.Content {
    public class ContentFolder {

        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.NewObjectId();

        // フォルダの種類
        public FolderTypeEnum FolderType { get; set; } = FolderTypeEnum.Normal;

        // プロパティ
        // 親フォルダのID
        public ObjectId ParentId { get; set; } = ObjectId.Empty;

        // ルートフォルダか否か
        public bool IsRootFolder { get; set; } = false;

        public List<VectorSearchProperty> ReferenceVectorSearchProperties { get; set; } = [];

        private VectorDBItem MainVectorDBItem {
            get {
                var item = VectorDBItem.GetDefaultVectorDB();
                item.Description = Description;
                item.FolderId = Id.ToString();

                return item;
            }
        }

        public VectorSearchProperty GetMainVectorSearchProperty() {

            VectorSearchProperty searchProperty = new(MainVectorDBItem) {
                FolderId = Id
            };
            return searchProperty;
        }

        public List<VectorSearchProperty> GetVectorSearchProperties() {
            List<VectorSearchProperty> searchProperties = new();
            searchProperties.Add(GetMainVectorSearchProperty());
            // ReferenceVectorDBItemsに設定されたVectorDBItemを取得
            foreach (var item in ReferenceVectorSearchProperties) {
                searchProperties.Add(item);
            }
            return searchProperties;
        }

        // AutoProcessを有効にするかどうか
        public bool IsAutoProcessEnabled { get; set; } = false;

        // フォルダの絶対パス ファイルシステム用
        public virtual string FolderPath {
            get {
                // 親フォルダを取得
                var parentFolder = GetParent();
                if (parentFolder == null) {
                    return FolderName;
                }
                return $"{parentFolder.FolderPath}/{FolderName}";
            }           
        }

        //　フォルダ名
        public virtual string FolderName { get; set; } = "";
        // Description
        public virtual string Description { get; set; } = "";

        // Idを指定してフォルダを取得
        public static T? GetFolderById<T>(ObjectId id) where T : ContentFolder {
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T>();
            return collection.FindById(id);
        }
        // 子フォルダ
        public virtual List<T> GetChildren<T>() where T : ContentFolder {
            // DBからParentIDが自分のIDのものを取得
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T>();
            var folders = collection.Find(x => x.ParentId == Id).OrderBy(x => x.FolderName);
            return folders.Cast<T>().ToList();
        }

        // 親フォルダ
        public virtual ContentFolder? GetParent() {
            return GetParent<ContentFolder>();
        }

        protected virtual T? GetParent<T>() where T : ContentFolder {
            if (ParentId == ObjectId.Empty) {
                return null;
            }
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T>();
            return collection.FindById(ParentId);
        }


        // フォルダを削除
        protected virtual void DeleteFolder<T1, T2>(T1 folder) where T1 : ContentFolder where T2 : ContentItem {
            // folderの子フォルダを再帰的に削除
            foreach (var child in folder.GetChildren<T1>()) {
                if (child != null) {
                    DeleteFolder<T1, T2>(child);
                }
            }
            // folderのアイテムを削除
            var items = PythonAILibManager.Instance.DataFactory.GetItemCollection<T2>().Find(x => x.CollectionId == folder.Id);
            foreach (var item in items) {
                item.Delete();
            }
            // folderを削除
            var folderCollection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T1>();
            folderCollection.Delete(folder.Id);
        }

        // フォルダを移動する
        public virtual void MoveTo(ContentFolder toFolder) {
            // 自分自身を移動
            ParentId = toFolder.Id;
            Save<ContentFolder, ContentItem>();
        }
        // 名前を変更
        public virtual void Rename(string newName) {
            FolderName = newName;
            Save<ContentFolder, ContentItem>();
        }

        public virtual void Save() {
            Save<ContentFolder, ContentItem>();
        }

        // 保存
        protected virtual void Save<T1, T2>() where T1 : ContentFolder where T2 : ContentItem {

            IDataFactory dataFactory = PythonAILibManager.Instance.DataFactory;
            dataFactory.GetFolderCollection<T1>().Upsert((T1)this);
            // ItemsのIsReferenceVectorDBItemsSyncedをFalseに設定
            foreach (var item in GetItems<T2>()) {
                item.IsReferenceVectorDBItemsSynced = false;
                item.Save(false);
            }
        }
        // 削除
        public virtual void Delete() {
            Delete<ContentFolder, ContentItem>();
        }

        protected virtual void Delete<T1, T2>() where T1 : ContentFolder where T2 : ContentItem {
            DeleteFolder<T1, T2>((T1)this);
        }

        public virtual List<T> GetItems<T>() where T : ContentItem {
            var collection = PythonAILibManager.Instance.DataFactory.GetItemCollection<T>();
            var items = collection.Find(x => x.CollectionId == Id).OrderByDescending(x => x.UpdatedAt);
            return items.Cast<T>().ToList();
        }



        #region ベクトル検索
        // ReferenceVectorDBItemsからVectorDBItemを削除
        public void RemoveVectorSearchProperty(VectorSearchProperty vectorDBItem) {
            List<VectorSearchProperty> existingItems = new(ReferenceVectorSearchProperties.Where(x => x.VectorDBItemId == vectorDBItem.VectorDBItemId && x.FolderId == vectorDBItem.FolderId));
            foreach (var item in existingItems) {
                ReferenceVectorSearchProperties.Remove(item);
            }
        }
        // ReferenceVectorDBItemsにVectorDBItemを追加
        public void AddVectorSearchProperty(VectorSearchProperty vectorDBItem) {
            var existingItems = ReferenceVectorSearchProperties.FirstOrDefault(x => x.VectorDBItemId == vectorDBItem.VectorDBItemId);
            if (existingItems == null) {
                ReferenceVectorSearchProperties.Add(vectorDBItem);
            }
        }
        // フォルダに設定されたVectorDBのコレクションを削除
        public void DeleteVectorDBCollection() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();

            ChatRequestContext chatRequestContext = new() {
                OpenAIProperties = openAIProperties,
                VectorSearchProperties = [GetMainVectorSearchProperty()]
            };

            PythonExecutor.PythonAIFunctions.DeleteVectorDBCollection(chatRequestContext);
        }

        // フォルダに設定されたVectorDBのインデックスを更新
        public void RefreshVectorDBCollection<T>() where T : ContentItem {
            // ベクトルを全削除
            DeleteVectorDBCollection();
            // ベクトルを再作成
            // フォルダ内のアイテムを取得して、ベクトルを作成
            foreach (var item in GetItems<T>()) {
                ContentItemCommands.UpdateEmbedding(item);
                // Save
                item.Save();
            }
        }

        #endregion
        public virtual ContentFolder CreateChild(string folderName) {
            ContentFolder folder = new() {
                ParentId = Id,
                FolderName = folderName,
            };
            return folder;
        }

        public virtual void AddItem(ContentItem item) {
            // CollectionNameを設定
            item.CollectionId = Id;
            // 保存
            item.Save();
            // 通知
            LogWrapper.Info(PythonAILibStringResources.Instance.AddedItems);

        }
        #region 検索
        // ClipboardItemを検索する。
        public IEnumerable<ContentItem> SearchItems(SearchCondition searchCondition) {
            // 結果を格納するIEnumerable<ContentItem>を作成
            IEnumerable<ContentItem> result = [];
            // 検索条件が空の場合は、結果を返す
            if (searchCondition.IsEmpty()) {
                return result;
            }

            // folder内のアイテムを保持するコレクションを取得
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetItemCollection<ContentItem>();
            var clipboardItems = collection.Find(x => x.CollectionId == this.Id).OrderByDescending(x => x.UpdatedAt);
            // Filterの結果を結果に追加
            result = Filter(clipboardItems, searchCondition);

            // サブフォルダを含む場合は、対象フォルダとそのサブフォルダを検索
            if (searchCondition.IsIncludeSubFolder) {
                // 対象フォルダの子フォルダを取得
                var folderCollection = libManager.DataFactory.GetFolderCollection<ContentFolder>();
                var childFolders = folderCollection.Find(x => x.ParentId == this.Id).OrderBy(x => x.FolderName);
                foreach (var childFolder in childFolders) {
                    // サブフォルダのアイテムを検索
                    var subFolderResult = childFolder.SearchItems(searchCondition);
                    // Filterの結果を結果に追加
                    result = result.Concat(subFolderResult);
                }
            }
            return result;
        }

        public IEnumerable<ContentItem> Filter(IEnumerable<ContentItem> liteCollection, SearchCondition searchCondition) {
            if (searchCondition.IsEmpty()) {
                return liteCollection;
            }

            var results = liteCollection;
            // SearchConditionの内容に従ってフィルタリング
            if (string.IsNullOrEmpty(searchCondition.Description) == false) {
                if (searchCondition.ExcludeDescription) {
                    results = results.Where(x => x.Description != null && x.Description.Contains(searchCondition.Description) == false);
                } else {
                    results = results.Where(x => x.Description != null && x.Description.Contains(searchCondition.Description));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.Content) == false) {
                if (searchCondition.ExcludeContent) {
                    results = results.Where(x => x.Content != null && x.Content.Contains(searchCondition.Content) == false);
                } else {
                    results = results.Where(x => x.Content != null && x.Content.Contains(searchCondition.Content));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.Tags) == false) {
                if (searchCondition.ExcludeTags) {
                    results = results.Where(x => x.Tags != null && x.Tags.Contains(searchCondition.Tags) == false);
                } else {
                    results = results.Where(x => x.Tags != null && x.Tags.Contains(searchCondition.Tags));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.SourceApplicationName) == false) {
                if (searchCondition.ExcludeSourceApplicationName) {
                    results = results.Where(x => x.SourceApplicationName != null && x.SourceApplicationName.Contains(searchCondition.SourceApplicationName) == false);
                } else {
                    results = results.Where(x => x.SourceApplicationName != null && x.SourceApplicationName.Contains(searchCondition.SourceApplicationName));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.SourceApplicationTitle) == false) {
                if (searchCondition.ExcludeSourceApplicationTitle) {
                    results = results.Where(x => x.SourceApplicationTitle != null && x.SourceApplicationTitle.Contains(searchCondition.SourceApplicationTitle) == false);
                } else {
                    results = results.Where(x => x.SourceApplicationTitle != null && x.SourceApplicationTitle.Contains(searchCondition.SourceApplicationTitle));
                }
            }
            if (searchCondition.EnableStartTime) {
                results = results.Where(x => x.CreatedAt > searchCondition.StartTime);
            }
            if (searchCondition.EnableEndTime) {
                results = results.Where(x => x.CreatedAt < searchCondition.EndTime);
            }
            results = results.OrderByDescending(x => x.UpdatedAt);

            return results;
        }
        #endregion
        // --- Export/Import
        public void ExportToExcel(string fileName, List<ExportImportItem> items) {
            // PythonNetの処理を呼び出す。
            List<List<string>> data = [];
            // ClipboardItemのリスト要素毎に処理を行う
            foreach (var clipboardItem in GetItems<ContentItem>()) {
                List<string> row = [];
                bool exportTitle = items.FirstOrDefault(x => x.Name == "Title")?.IsChecked ?? false;
                if (exportTitle) {
                    row.Add(clipboardItem.Description);
                }
                bool exportText = items.FirstOrDefault(x => x.Name == "Text")?.IsChecked ?? false;
                if (exportText) {
                    row.Add(clipboardItem.Content);
                }
                // SourcePath
                bool exportSourcePath = items.FirstOrDefault(x => x.Name == "SourcePath")?.IsChecked ?? false;
                if (exportSourcePath) {
                    row.Add(clipboardItem.SourcePath);
                }

                // PromptItemのリスト要素毎に処理を行う
                foreach (var promptItem in items.Where(x => x.IsPromptItem)) {
                    if (promptItem.IsChecked) {
                        string promptResult = clipboardItem.PromptChatResult.GetTextContent(promptItem.Name);
                        row.Add(promptResult);
                    }
                }

                data.Add(row);
            }
            CommonDataTable dataTable = new(data);

            PythonExecutor.PythonAIFunctions.ExportToExcel(fileName, dataTable);

        }

        public virtual void ImportFromExcel(string fileName, List<ExportImportItem> items, bool executeAutoProcess) {

            // PythonNetの処理を呼び出す。
            CommonDataTable data = PythonExecutor.PythonAIFunctions.ImportFromExcel(fileName);
            if (data == null) {
                return;
            }
            bool importTitle = items.FirstOrDefault(x => x.Name == "Title")?.IsChecked ?? false;
            bool importText = items.FirstOrDefault(x => x.Name == "Text")?.IsChecked ?? false;

            foreach (var row in data.Rows) {
                ContentItem item = new(Id);
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
            }
        }

        public virtual void ImportFromURLList(string filePath, bool executeAutoProcess) {

            // filePathのファイルが存在しない場合は何もしない
            if (System.IO.File.Exists(filePath) == false) {
                LogWrapper.Error(PythonAILibStringResources.Instance.FileNotFound);
                return;
            }
            // ファイルからすべての行を読み込んでリストに格納します
            List<string> lines = new List<string>(System.IO.File.ReadAllLines(filePath));
            Task[] tasks = [];

            foreach (var line in lines) {
                // URLを取得
                string url = line;
                // URLからデータを取得して一時ファイルに保存
                // Task.Runを使用して非同期操作を開始します
                Task task = Task.Run(async () => {
                    string tempFilePath = Path.GetTempFileName();
                    try {
                        string data = PythonExecutor.PythonAIFunctions.ExtractWebPage(url);
                        // 一時ファイルのパスを取得します

                        // データを一時ファイルに書き込みます
                        await System.IO.File.WriteAllTextAsync(tempFilePath, data);
                        // 成功メッセージを表示します
                        Console.WriteLine($"データは一時ファイルに保存されました: {tempFilePath}");
                        // 一時ファイルからテキスト抽出
                        string text = PythonExecutor.PythonAIFunctions.ExtractFileToText(tempFilePath);

                        // アイテムの作成
                        ContentItem item = new(Id) {
                            Content = text,
                            Description = url,
                            SourcePath = url
                        };
                        item.Save();

                    } catch (Exception ex) {
                        LogWrapper.Warn($"エラーが発生しました: {ex.Message}");
                    } finally {
                        // 一時ファイルを削除します
                        System.IO.File.Delete(tempFilePath);
                    }
                });
                tasks.Append(task);
            }
            // すべてのタスクが完了するまで待機します
            Task.WaitAll(tasks);

        }
        public virtual void ImportItemsFromJson(string json) { }

        // 指定されたフォルダの全アイテムをマージするコマンド
        public void MergeItems(ContentItem item) {
            item.MergeItems(GetItems<ContentItem>());
        }
        // 指定されたフォルダの中のSourceApplicationTitleが一致するアイテムをマージするコマンド
        public void MergeItemsBySourceApplicationTitleCommandExecute(ContentItem newItem) {
            // SourceApplicationNameが空の場合は何もしない
            if (string.IsNullOrEmpty(newItem.SourceApplicationName)) {
                return;
            }
            // NewItemのSourceApplicationTitleが空の場合は何もしない
            if (string.IsNullOrEmpty(newItem.SourceApplicationTitle)) {
                return;
            }
            List<ContentItem> items = GetItems<ContentItem>();
            if (items.Count == 0) {
                return;
            }
            List<ContentItem> sameTitleItems = [];
            // マージ先のアイテムのうち、SourceApplicationTitleとSourceApplicationNameが一致するアイテムを取得
            foreach (var item in items) {
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

    }
}
