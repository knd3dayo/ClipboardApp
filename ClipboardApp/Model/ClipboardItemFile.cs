using ClipboardApp.Factory;
using LiteDB;
using PythonAILib.Model.Content;
using PythonAILib.Model.File;
using PythonAILib.Model.VectorDB;
using PythonAILib.PythonIF;

namespace ClipboardApp.Model {
    public class ClipboardItemFile : ContentAttachedItem {

        public static ClipboardItemFile Create(ClipboardItem clipboardItem, string filePath) {
            ClipboardItemFile itemFile = new() {
                ClipboardItem = clipboardItem,
                FilePath = filePath,
                LastModified = new System.IO.FileInfo(filePath).LastWriteTime.Ticks
            };
            // キャッシュを更新
            itemFile.UpdateCache();
            return itemFile;
        }
        public static ClipboardItemFile Create(ClipboardItem clipboardItem, System.Drawing.Image image) {
            ClipboardItemFile itemFile = new() {
                ClipboardItem = clipboardItem,
                CachedBase64String = ContentTypes.GetBase64StringFromImage(image)
            };
            return itemFile;
        }
        public static ClipboardItemFile CreateFromBase64(ClipboardItem clipboardItem, string base64string) {
            ClipboardItemFile itemFile = new() {
                ClipboardItem = clipboardItem,
                CachedBase64String = base64string
            };
            return itemFile;
        }


        [BsonIgnore]
        public ClipboardItem? ClipboardItem { get; set; }

        // 削除
        public override void Delete() {
            ClipboardAppFactory.Instance.GetClipboardDBController().DeleteAttachedItem(this);
            // クリップボードアイテムとファイルを同期する
            if (ClipboardAppConfig.Instance.SyncClipboardItemAndOSFolder) {
                // SyncFolderName/フォルダ名/ファイル名を削除する
                string syncFolderName = ClipboardAppConfig.Instance.SyncFolderName;

                string syncFolder = System.IO.Path.Combine(syncFolderName, ClipboardItem?.FolderPath ?? "");
                string syncFilePath = System.IO.Path.Combine(syncFolder, FileName);
                if (System.IO.File.Exists(syncFilePath)) {
                    System.IO.File.Delete(syncFilePath);
                }
                // 自動コミットが有効の場合はGitにコミット
                if (ClipboardAppConfig.Instance.AutoCommit) {
                    ClipboardItem?.GitCommit(syncFilePath);
                }
            }
        }

        // 保存
        public override void Save() {

            ClipboardAppFactory.Instance.GetClipboardDBController().UpsertAttachedItem(this);
            // クリップボードアイテムとファイルを同期する
            if (ClipboardAppConfig.Instance.SyncClipboardItemAndOSFolder) {
                if (FilePath == null) {
                    throw new Exception("FilePath is null");
                }
                // SyncFolderName/フォルダ名/ファイル名にファイルを保存する
                string syncFolderName = ClipboardAppConfig.Instance.SyncFolderName;
                string syncFolder = System.IO.Path.Combine(syncFolderName, ClipboardItem?.FolderPath ?? "");
                string syncFilePath = System.IO.Path.Combine(syncFolder, FileName);
                if (!System.IO.Directory.Exists(syncFolder)) {
                    System.IO.Directory.CreateDirectory(syncFolder);
                }
                if (System.IO.File.Exists(FilePath)) {
                    System.IO.File.Copy(FilePath, syncFilePath, true);
                }
                // 自動コミットが有効の場合はGitにコミット
                if (ClipboardAppConfig.Instance.AutoCommit) {
                    ClipboardItem?.GitCommit(syncFilePath);
                }
            }
        }

        // Embeddingを更新する
        public void UpdateEmbedding() {
            if (ClipboardItem == null) {
                throw new Exception("ClipboardItem is null");
            }

            if (IsImage()) {
                // 画像からテキスト抽出
                ImageInfo imageInfo = new(VectorDBUpdateMode.update, Id.ToString(), ExtractedText, Base64String);
                // VectorDBItemを取得
                VectorDBItem folderVectorDBItem = ClipboardAppVectorDBItem.GetFolderVectorDBItem(ClipboardItem.GetFolder());
                // Embeddingを保存
                folderVectorDBItem.UpdateIndex(imageInfo);
            } else {
                ContentInfo contentInfo = new(VectorDBUpdateMode.update, Id.ToString(), ExtractedText);
                // VectorDBItemを取得
                VectorDBItem folderVectorDBItem = ClipboardAppVectorDBItem.GetFolderVectorDBItem(ClipboardItem.GetFolder());
                // Embeddingを保存
                folderVectorDBItem.UpdateIndex(contentInfo);
            }
        }
    }
}
