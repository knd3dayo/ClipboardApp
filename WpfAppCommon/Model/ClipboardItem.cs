using System.Drawing;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LiteDB;
using QAChat.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace WpfAppCommon.Model {
    public enum ClipboardContentTypes {
        Text,
        Files,
        Image,
        Unknown
    }
    public class ClipboardItem {
        // コンストラクタ
        public ClipboardItem() {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        // プロパティ

        public ObjectId? Id { get; set; }

        public string? CollectionName { get; set; }

        // 生成日時
        public DateTime CreatedAt { get; set; }

        // 更新日時
        public DateTime UpdatedAt { get; set; }

        // クリップボードの内容
        public string Content { get; set; } = "";

        //　画像イメージのObjectId
        public ObjectId? ImageObjectId { get; set; }

        // ファイルのObjectId
        public ObjectId? FileObjectId { get; set; }


        // 画像イメージ
        public ClipboardItemImage? ClipboardItemImage {
            get {
                if (ImageObjectId == null) {
                    return new ClipboardItemImage();
                }
                return ClipboardAppFactory.Instance.GetClipboardDBController().GetItemImage(ImageObjectId);
            }
            set {
                if (value == null) {
                    //Imageを削除
                    if (ClipboardItemImage != null) {
                        ClipboardItemImage.Delete();
                    }
                    ImageObjectId = null;
                    return;
                }
                //Imageを保存
                value.Save();
                ImageObjectId = value.Id;
            }
        }

        // ファイル
        public ClipboardItemFile? ClipboardItemFile {
            get {
                if (FileObjectId == null) {
                    return null;
                }
                return ClipboardAppFactory.Instance.GetClipboardDBController().GetItemFile(FileObjectId);
            }
            set {
                if (value == null) {
                    //Fileを削除
                    ClipboardItemFile?.Delete();
                    FileObjectId = null;
                    return;
                }
                //Fileを保存
                value.Save();
                FileObjectId = value.Id;
            }
        }

        // クリップボードの内容の種類
        public ClipboardContentTypes ContentType { get; set; }

        //Tags
        public HashSet<string> Tags { get; set; } = [];

        //説明
        public string Description { get; set; } = "";

        //　貼り付け元のアプリケーション名
        public string SourceApplicationName { get; set; } = "";
        //　貼り付け元のアプリケーションのタイトル
        public string SourceApplicationTitle { get; set; } = "";
        //　貼り付け元のアプリケーションのID
        public int? SourceApplicationID { get; set; }
        //　貼り付け元のアプリケーションのパス
        public string? SourceApplicationPath { get; set; }
        // ピン留め
        public bool IsPinned { get; set; }


        // -------------------------------------------------------------------
        // インスタンスメソッド
        // -------------------------------------------------------------------

        public ClipboardItem Copy() {
            ClipboardItem newItem = new();
            CopyTo(newItem);
            return newItem;

        }
        public void CopyTo(ClipboardItem newItem) {
            newItem.UpdatedAt = UpdatedAt;
            newItem.Content = Content;
            newItem.ContentType = ContentType;
            newItem.SourceApplicationName = SourceApplicationName;
            newItem.SourceApplicationTitle = SourceApplicationTitle;
            newItem.SourceApplicationID = SourceApplicationID;
            newItem.SourceApplicationPath = SourceApplicationPath;
            newItem.Tags = new HashSet<string>(Tags);
            newItem.Description = Description;

            //-- 画像がある場合はコピー
            if (ImageObjectId != null) {
                ClipboardItemImage newImage = new ();
                newImage.ImageBase64 = ClipboardItemImage?.ImageBase64 ?? string.Empty;
                newItem.ClipboardItemImage = newImage;
            }
            //-- ファイルがある場合はコピー
            if (FileObjectId != null) {
                ClipboardItemFile newFile = new(ClipboardItemFile?.FileName ?? string.Empty);
                newFile.ExtractedText = ClipboardItemFile?.ExtractedText ?? string.Empty;
                newItem.ClipboardItemFile = newFile;
            }
        }

        public void MergeItems(List<ClipboardItem> items, bool mergeWithHeader, Action<ActionMessage>? action) {
            if (ContentType != ClipboardContentTypes.Text) {
                action?.Invoke(ActionMessage.Error("Text以外のアイテムへのマージはできません"));
                return;
            }
            string mergeText = "\n";
            // 現在の時刻をYYYY/MM/DD HH:MM:SS形式で取得
            mergeText += "---\n";

            foreach (var item in items) {

                // Itemの種別がText以外が含まれている場合はマージしない
                if (item.ContentType != ClipboardContentTypes.Text) {
                    action?.Invoke(ActionMessage.Error("Text以外のアイテムが含まれているアイテムはマージできません"));
                    return;
                }
            }
            foreach (var item in items) {
                if (mergeWithHeader) {
                    // 説明がある場合は追加
                    if (Description != "") {
                        mergeText += item.Description + "\n";
                    }
                    // mergeTextにHeaderを追加
                    mergeText += item.HeaderText + "\n";
                }
                // Contentを追加
                mergeText += item.Content + "\n";

            }
            // mergeTextをContentに追加
            Content += mergeText;

            // Tagsのマージ。重複を除外して追加
            Tags.UnionWith(items.SelectMany(item => item.Tags));
        }

        // タグ表示用の文字列
        public string TagsString() {
            return string.Join(",", Tags);
        }

        public void SetApplicationInfo(ClipboardChangedEventArgs sender) {
            SourceApplicationName = sender.SourceApplication.Name;
            SourceApplicationTitle = sender.SourceApplication.Title;
            SourceApplicationID = sender.SourceApplication.ID;
            SourceApplicationPath = sender.SourceApplication.Path;
        }

        public string? HeaderText {
            get {
                string header1 = "";
                // 更新日時文字列を追加
                header1 += "[更新日時]" + UpdatedAt.ToString("yyyy/MM/dd HH:mm:ss") + "\n";
                // 作成日時文字列を追加
                header1 += "[作成日時]" + CreatedAt.ToString("yyyy/MM/dd HH:mm:ss") + "\n";
                // 貼り付け元のアプリケーション名を追加
                header1 += "[ソースアプリ名]" + SourceApplicationName + "\n";
                // 貼り付け元のアプリケーションのタイトルを追加
                header1 += "[ソースタイトル]" + SourceApplicationTitle + "\n";
                // Tags
                header1 += "[タグ]" + TagsString() + "\n";
                // ピン留め中かどうか
                if (IsPinned) {
                    header1 += "[ピン留めしてます]\n";
                }

                if (ContentType == ClipboardContentTypes.Text) {
                    return header1 + "[種類]Text";
                } else if (ContentType == ClipboardContentTypes.Files) {
                    return header1 + "[種類]File";
                } else if (ContentType == ClipboardContentTypes.Image) {
                    return header1 + "[種類]Image";
                } else {
                    return header1 + "[種類]Unknown";
                }
            }
        }
        //--------------------------------------------------------------------------------
        // staticメソッド
        //--------------------------------------------------------------------------------

        // ClipboardItemをJSON文字列に変換する
        public static string ToJson(ClipboardItem item) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            var options = jsonSerializerOptions;
            return System.Text.Json.JsonSerializer.Serialize(item, options);
        }
        // JSON文字列をClipboardItemに変換する
        public static ClipboardItem? FromJson(string json, Action<ActionMessage> action) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            var options = jsonSerializerOptions;
            ClipboardItem? item = System.Text.Json.JsonSerializer.Deserialize<ClipboardItem>(json, options);
            if (item == null) {
                action(ActionMessage.Error("JSON文字列をClipboardItemに変換できませんでした"));
                return null;
            }

            return item;

        }

        // 自分自身をDBに保存する
        public void Save(bool updateModifiedTime = true) {
            ClipboardAppFactory.Instance.GetClipboardDBController().UpsertItem(this);
        }
        // 自分自身をDBから削除する
        public void Delete() {
            ClipboardAppFactory.Instance.GetClipboardDBController().DeleteItem(this);
            // DBから削除したらIdをnullにする
            Id = null;
        }

        public static void CreateAutoDescription(ClipboardItem item) {
            string updatedAtString = item.UpdatedAt.ToString("yyyy/MM/dd HH:mm:ss");
            // Textの場合
            if (item.ContentType == ClipboardContentTypes.Text) {
                item.Description = $"{updatedAtString} {item.SourceApplicationName} {item.SourceApplicationTitle}";
            }
            // Fileの場合
            else if (item.ContentType == ClipboardContentTypes.Files) {
                item.Description = $"{updatedAtString} {item.SourceApplicationName} {item.SourceApplicationTitle}";
                // Contentのサイズが50文字以上の場合は先頭20文字 + ... + 最後の30文字をDescriptionに設定
                if (item.Content.Length > 20) {
                    item.Description += " ファイル：" + item.Content[..20] + "..." + item.Content[^30..];
                } else {
                    item.Description += " ファイル：" + item.Content;
                }
            }
        }

        // 自動処理でファイルパスをフォルダとファイル名に分割するコマンド
        public void SplitFilePathCommandExecute() {

            if (this.ContentType != ClipboardContentTypes.Files) {
                throw new ThisApplicationException("ファイル以外のコンテンツはファイルパスを分割できません");
            }
            
            string? path = ClipboardItemFile?.FileName;

            if (string.IsNullOrEmpty(path) == false) {
                // ファイルパスをフォルダ名とファイル名に分割
                string? folderPath = Path.GetDirectoryName(path) ?? throw new ThisApplicationException("フォルダパスが取得できません");
                string? fileName = Path.GetFileName(path);
                this.Content = folderPath + "\n" + fileName;
                // ContentTypeをTextに変更
                this.ContentType = ClipboardContentTypes.Text;
                // StatusTextにメッセージを表示
                Tools.Info("ファイルパスをフォルダ名とファイル名に分割しました");
            }
        }

        // 自動でタグを付与するコマンド
        public static void CreateAutoTags(ClipboardItem item) {
            // PythonでItem.ContentからEntityを抽出
            string spacyModel = WpfAppCommon.Properties.Settings.Default.SpacyModel;
            HashSet<string> entities = PythonExecutor.PythonFunctions.ExtractEntity(spacyModel, item.Content);
            foreach (var entity in entities) {
                // LiteDBにタグを追加
                TagItem tagItem = new() { Tag = entity };
                ClipboardAppFactory.Instance.GetClipboardDBController().UpsertTag(tagItem);
                // タグを追加
                item.Tags.Add(entity);
            }

        }
        // 自動処理でテキストを抽出」を実行するコマンド
        public static ClipboardItem ExtractTextCommandExecute(ClipboardItem clipboardItem) {

            if (clipboardItem.ContentType != ClipboardContentTypes.Files) {
                throw new ThisApplicationException("ファイル以外のコンテンツはテキストを抽出できません");
            }
            ClipboardItemFile? clipboardItemFile = clipboardItem.ClipboardItemFile;
            if (clipboardItemFile == null) {
                throw new ThisApplicationException("ファイルが取得できません");
            }
            string path = clipboardItemFile.FileName;
            if (string.IsNullOrEmpty(path)) {
                throw new ThisApplicationException("ファイルパスが取得できません");
            }
            string text = PythonExecutor.PythonFunctions.ExtractText(path);
            clipboardItemFile.ExtractedText = text;

            Tools.Info($"{path}のテキストを抽出しました");

            return clipboardItem;

        }

        // 自動処理でデータをマスキング」を実行するコマンド
        public ClipboardItem MaskDataCommandExecute() {

            if (this.ContentType != ClipboardContentTypes.Text) {
                throw new ThisApplicationException("テキスト以外のコンテンツはマスキングできません");
            }
            string spacyModel = WpfAppCommon.Properties.Settings.Default.SpacyModel;
            string result = PythonExecutor.PythonFunctions.GetMaskedString(spacyModel, this.Content);
            this.Content = result;

            Tools.Info("データをマスキングしました");
            return this;
        }

        public static string CovertMaskedDataToOriginalData(MaskedData? maskedData, string maskedText) {
            if (maskedData == null) {
                return maskedText;
            }
            // マスキングデータをもとに戻す
            string result = maskedText;
            foreach (var entity in maskedData.Entities) {
                // ステータスバーにメッセージを表示
                Tools.Info($"マスキングデータをもとに戻します: {entity.Before} -> {entity.After}\n");
                result = result.Replace(entity.After, entity.Before);
            }
            return result;
        }

        private static ChatItem CreateMaskedDataSystemMessage() {
            ChatItem chatItem
                = new(ChatItem.SystemRole,
                "このチャットではマスキングデータ(MASKED_...)を使用している場合があります。" +
                "マスキングデータの文字列はそのままにしてください");
            return chatItem;
        }

        public static string FormatTextCommandExecute(string text) {
            string prompt = "次の文章はWindowsのクリップボードから取得した文章です。これを整形してください。重複した内容がある場合は削除してください。\n";

            // ChatCommandExecuteを実行
            prompt += "処理対象の文章\n-----------\n" + text;
            // Vector DBのアイテムを取得
            IEnumerable<VectorDBItem> vectorDBItems = VectorDBItem.GetEnabledItems();

            ChatResult result = PythonExecutor.PythonFunctions.LangChainChat(prompt, [], vectorDBItems);

            return result.Response;

        }
        // 画像からイメージを抽出するコマンド
        public static ClipboardItem ExtractTextFromImageCommandExecute(ClipboardItem clipboardItem) {
            if (clipboardItem.ContentType != ClipboardContentTypes.Image) {
                throw new ThisApplicationException("画像以外のコンテンツはテキストを抽出できません");
            }
            Image? image = clipboardItem.ClipboardItemImage?.GetImage();
            if (image == null) {
                throw new ThisApplicationException("画像が取得できません");
            }
            string text = PythonExecutor.PythonFunctions.ExtractTextFromImage(image, ClipboardAppConfig.TesseractExePath);
            clipboardItem.Content = text;

            return clipboardItem;
        }
    }

}
