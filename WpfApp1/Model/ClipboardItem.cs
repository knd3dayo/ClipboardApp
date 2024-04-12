﻿using static WK.Libraries.SharpClipboardNS.SharpClipboard;
using WK.Libraries.SharpClipboardNS;
using LiteDB;
using System.Text.Json.Nodes;
using WpfApp1.Utils;

namespace WpfApp1.Model
{
    public class ClipboardItem
    {
        public ObjectId? Id { get; set; }

        public string? CollectionName { get; set; }

        // 生成日時
        public DateTime CreatedAt { get; set; }

        // 更新日時
        public DateTime UpdatedAt { get; set; }

        // クリップボードの内容
        public string Content { get; set; } = "";
        // クリップボードの内容の種類
        public ContentTypes ContentType { get; set; }

        //Tags
        public List<string> Tags { get; set; } = new List<string>();

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


        public ClipboardItem()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        public ClipboardItem Copy()
        {
            ClipboardItem newItem = new ClipboardItem();
            newItem.CreatedAt = CreatedAt;
            newItem.UpdatedAt = UpdatedAt;
            newItem.Content = Content;
            newItem.ContentType = ContentType;
            newItem.SourceApplicationName = SourceApplicationName;
            newItem.SourceApplicationTitle = SourceApplicationTitle;
            newItem.SourceApplicationID = SourceApplicationID;
            newItem.SourceApplicationPath = SourceApplicationPath;
            newItem.Tags = new List<string>(Tags);
            newItem.Description = Description;
            return newItem;

        }
        // このオブジェクトをJSONに変換
        public JsonNode ToJsonNode() {
            JsonNode jsonNode = new JsonObject();
            jsonNode["content"] = Content;
            jsonNode["type"] = ContentType.ToString();
            JsonArray tagsJsonArray = new JsonArray();
            foreach (var tag in Tags) {
                tagsJsonArray.Add(tag);
            }
            jsonNode["tags"] = tagsJsonArray;
            jsonNode["description"] = Description;
            jsonNode["sourceApplicationName"] = SourceApplicationName;
            jsonNode["sourceApplicationTitle"] = SourceApplicationTitle;
            jsonNode["sourceApplicationID"] = SourceApplicationID;
            jsonNode["sourceApplicationPath"] = SourceApplicationPath;
            return jsonNode;
        }
        // JSONからオブジェクトを生成
        public void FromJsonNode(JsonNode jsonNode) {
            if (jsonNode == null) { 
                Tools.Warn("ClipboardItem.FromJsonNode: jsonNode is null");
                return;
            }
            JsonNode? item = jsonNode["item"];
            if (item == null) {
                Tools.Warn("ClipboardItem.FromJsonNode: item is null");
                return;
            }

            JsonNode? contentNode = item["content"];
            if (contentNode != null) {
                var value = contentNode.GetValue<string>();
                if (value != null) {
                    Content = value;
                }
            }
            JsonNode? typeNode = item["type"];
            if (typeNode != null) {
                var value = typeNode.GetValue<string>();
                if (value != null) {
                    ContentType = (ContentTypes)System.Enum.Parse(typeof(ContentTypes), value);
                }
            }
            JsonArray? tagsNode = item["tags"]?.AsArray();
            if (tagsNode != null) {
                Tags.Clear();
                foreach (var tagNode in tagsNode) {
                    var value = tagNode?.GetValue<string>();
                    if (value != null) {
                        Tags.Add(value);
                    }
                }
            }
            JsonNode? descriptionNode = item["description"];
            if (descriptionNode != null) {
                var value = descriptionNode.GetValue<string>();
                if (value != null) {
                    Description = value;
                }
            }
            JsonNode? sourceApplicationNameNode = item["sourceApplicationName"];
            if (sourceApplicationNameNode != null) {
                var value = sourceApplicationNameNode.GetValue<string>();
                if (value != null) {
                    SourceApplicationName = value;
                }
            }
            JsonNode? sourceApplicationTitleNode = item["sourceApplicationTitle"];
            if (sourceApplicationTitleNode != null) {
                var value = sourceApplicationTitleNode.GetValue<string>();
                if (value != null) {
                    SourceApplicationTitle = value;
                }
            }
            JsonNode? sourceApplicationIDNode = item["sourceApplicationID"];
            if (sourceApplicationIDNode != null) {
                var value = sourceApplicationIDNode.GetValue<int>();
                SourceApplicationID = value;
            }
            JsonNode? sourceApplicationPathNode = item["sourceApplicationPath"];
            if (sourceApplicationPathNode != null) {
                var value = sourceApplicationPathNode.GetValue<string>();
                if (value != null) {
                    SourceApplicationPath = value;
                }
            }

        }

        public ClipboardItem MergeItems(List<ClipboardItem> items, bool mergeWithHeader) 
        {
            if (this.ContentType != SharpClipboard.ContentTypes.Text) {

                throw new Utils.ThisApplicationException("Text以外のアイテムへのマージはできません");
            }
            string mergeText = "\n";
            // 現在の時刻をYYYY/MM/DD HH:MM:SS形式で取得
            mergeText += "--- マージ時刻 " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "\n";

            foreach ( var item in items) {

                // Itemの種別がFileが含まれている場合はマージしない
                if (item.ContentType != ContentTypes.Files && item.ContentType != ContentTypes.Text) {
                    throw new Utils.ThisApplicationException("TextまたはFile以外のアイテムのマージはできません");
                }
            }
            foreach (var item in items) {
                if (mergeWithHeader)
                {
                    // 説明がある場合は追加
                    if (Description != "")
                    {
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

            return this;
        }

        // タグ表示用の文字列
        public string TagsString()
        {
            return string.Join(",", Tags);
        }

        public void SetApplicationInfo(ClipboardChangedEventArgs sender)
        {
            SourceApplicationName = sender.SourceApplication.Name;
            SourceApplicationTitle = sender.SourceApplication.Title;
            SourceApplicationID = sender.SourceApplication.ID;
            SourceApplicationPath = sender.SourceApplication.Path;
        }

        public string? HeaderText
        {
            get
            {
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


                if (ContentType == SharpClipboard.ContentTypes.Text)
                {
                    return header1 + "[種類]Text";
                }
                else if (ContentType == SharpClipboard.ContentTypes.Files)
                {
                    return header1 + "[種類]File";
                }
                else
                {
                    return header1 + " [種類]Unknown";
                }
            }
        }

    }

}
