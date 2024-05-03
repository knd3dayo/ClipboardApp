using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json.Nodes;
using WpfAppCommon.Factory;
using WpfAppCommon.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;

namespace WpfAppCommon.Model {
    public class ClipboardFolder : ObservableObject {

        // アプリ共通の検索条件
        public static SearchRule GlobalSearchCondition { get; set; } = new ();

        //--------------------------------------------------------------------------------
        public static ClipboardFolder RootFolder {
            get {
                IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
                return ClipboardDatabaseController.GetRootFolder();
            }
        }
        public static ClipboardFolder SearchRootFolder {
            get {
                IClipboardDBController clipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
                return clipboardDatabaseController.GetSearchRootFolder();
            }
        }

        // プロパティ
        // LiteDBのID
        public ObjectId Id { get; set; } = ObjectId.Empty;
        // フォルダの表示名
        public string DisplayName { get; set; } = "";
        // フォルダの絶対パス
        public string AbsoluteCollectionName { get; set; } = "";

        // AutoProcessRuleのIdのリスト
        public IEnumerable<int> AutoProcessRuleIds { get; set; } = [];

        // AutoProcessRuleのリスト
        public ObservableCollection<AutoProcessRule> AutoProcessRules {
            get {
                ObservableCollection<AutoProcessRule> autoProcessRules = [.. AutoProcessRuleController.GetAutoProcessRules(this)];
                return autoProcessRules;
            }
        }
        // AddAutoProcessRule
        public void AddAutoProcessRule(AutoProcessRule rule) {
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            ClipboardDatabaseController.UpsertAutoProcessRule(rule);
        }

        // 検索フォルダかどうか
        public bool IsSearchFolder { get; set; } = false;

        // 現在、検索条件を適用中かどうか
        public bool IsApplyingSearchCondition { get; set; } = false;

        // 子フォルダ BSonMapper.GlobalでIgnore設定しているので、LiteDBには保存されない
        public ObservableCollection<ClipboardFolder> Children {
            get {
                ObservableCollection<ClipboardFolder> children = [];
                // AbsoluteCollectionNameが空の場合は空のリストを返す
                if (string.IsNullOrEmpty(AbsoluteCollectionName)) {
                    return children;
                }
                // LiteDBから自分が親となっているフォルダを取得
                IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
                var childrenNames = ClipboardDatabaseController.GetFolderRelations(AbsoluteCollectionName);
                foreach (var childName in childrenNames) {
                    var child = ClipboardDatabaseController.GetFolder(childName);
                    if (child != null) {
                        children.Add(child);
                    }
                }
                return children;
            }
        }
        public void DeleteChild(ClipboardFolder child) {
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            ClipboardDatabaseController.DeleteFolder(child);
        }
        public ClipboardFolder CreateChild(string collectionName, string displayName) {
            ClipboardFolder child = new (this, collectionName, displayName);
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            ClipboardDatabaseController.UpsertFolderRelation(this, child);

            return child;
        }

        // アイテム BSonMapper.GlobalでIgnore設定しているので、LiteDBには保存されない
        public ObservableCollection<ClipboardItem> Items {
            get {
                // AbsoluteCollectionNameが空の場合は空のリストを返す
                if (string.IsNullOrEmpty(AbsoluteCollectionName)) {
                    return [];
                }
                IEnumerable<ClipboardItem> items = [];
                // このフォルダが通常フォルダの場合は、GlobalSearchConditionを適用して取得,
                // 検索フォルダの場合は、SearchConditionを適用して取得
                IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
                // フォルダに検索条件が設定されている場合
                SearchRule? searchConditionRule = SearchRuleController.GetSearchRuleByFolderName(AbsoluteCollectionName);
                if (searchConditionRule != null) {
                    // 検索対象フォルダパスの取得
                    string? targetCollectionName = searchConditionRule.TargetFolder?.AbsoluteCollectionName;
                    if (targetCollectionName != null) {
                        items = ClipboardDatabaseController.SearchItems(targetCollectionName, searchConditionRule.SearchCondition);
                    }
                    // 検索対象フォルダパスがない場合は何もしない。
                } else {
                    // 通常のフォルダの場合で、GlobalSearchConditionが設定されている場合
                    if (GlobalSearchCondition.SearchCondition != null && GlobalSearchCondition.SearchCondition.IsEmpty() == false) {
                        items = ClipboardDatabaseController.SearchItems(AbsoluteCollectionName, GlobalSearchCondition.SearchCondition);

                    } else {
                        // 通常のフォルダの場合で、GlobalSearchConditionが設定されていない場合
                        items = ClipboardDatabaseController.GetItems(AbsoluteCollectionName);
                    }
                }

                ObservableCollection<ClipboardItem> result = [.. items];
                return result;
            }
        }
        //------------
        // 親フォルダのパスと子フォルダ名を連結する。LiteDB用
        private string ConcatenatePath(string parentPath, string childPath) {
            if (string.IsNullOrEmpty(parentPath))
                return childPath;
            if (string.IsNullOrEmpty(childPath))
                return parentPath;
            return parentPath + "_" + childPath;
        }


        //--------------------------------------------------------------------------------
        // コンストラクタ
        public ClipboardFolder() {
        }

        public ClipboardFolder(ClipboardFolder? parent, string collectionName, string displayName) {
            if (parent == null) {
                AbsoluteCollectionName = collectionName;
            } else {
                AbsoluteCollectionName = ConcatenatePath(parent.AbsoluteCollectionName, collectionName);
            }
            DisplayName = displayName;

        }
        public ClipboardFolder(string collectionName, string displayName) : this(null, collectionName, displayName) {

        }
        //--------------------------------------------------------------------------------
        // 自分自身を保存
        public void Save() {
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            ClipboardDatabaseController.UpsertFolder(this);
        }
        // アイテムを追加する処理
        public ClipboardItem AddItem(ClipboardItem item, Action<ActionMessage> actionMessage) {
            // AbsoluteCollectionNameを設定
            item.CollectionName = AbsoluteCollectionName;

            // 自動処理を適用
            ClipboardItem? result = ApplyAutoProcess(item, actionMessage);

            if (result == null) {
                // 自動処理で削除または移動された場合は何もしない
                actionMessage(ActionMessage.Info("自動処理でアイテムが削除または移動されました"));
                return item;
            }
            // 保存
            result.Save();
            // Itemsに追加
            Items.Add(result);
            // 通知
            actionMessage(ActionMessage.Info("アイテムを追加しました"));
            return item;
        }
        // ClipboardItemを削除
        public void DeleteItem(ClipboardItem item) {
            // LiteDBに保存
            item.Delete();
        }
        // Delete
        public void Delete() {
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            ClipboardDatabaseController.DeleteFolder(this);
        }


        // 自動処理を適用する処理
        public ClipboardItem? ApplyAutoProcess(ClipboardItem clipboardItem, Action<ActionMessage> action) {
            ClipboardItem? result = clipboardItem;
            // AutoProcessRulesを取得
            var AutoProcessRules = AutoProcessRuleController.GetAutoProcessRules(this);
            foreach (var rule in AutoProcessRules) {
                action(ActionMessage.Info("自動処理を適用します " + rule.GetDescriptionString()));
                result = rule.RunAction(result);
                // resultがNullの場合は処理を中断
                if (result == null) {
                    action(ActionMessage.Info("自動処理でアイテムが削除されました"));
                    return null;
                }
            }
            return result;
        }

        // フォルダ内のアイテムをJSON形式でExport
        public void ExportItemsToJson(string directoryPath) {
            JsonArray jsonArray = [];
            foreach (ClipboardItem item in Items) {
                jsonArray.Add(ClipboardItem.ToJson(item));
            }
            string jsonString = jsonArray.ToString();
            string fileName = DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + this.AbsoluteCollectionName + ".json";

            File.WriteAllText(Path.Combine(directoryPath, fileName), jsonString);

        }

        //exportしたJSONファイルをインポート
        public void ImportItemsFromJson(string json, Action<ActionMessage> action) {
            JsonNode? node = JsonNode.Parse(json);
            if (node == null) {
                action(ActionMessage.Error("JSON文字列をパースできませんでした"));
                return;
            }
            JsonArray? jsonArray = node as JsonArray;
            if (jsonArray == null) {
                action(ActionMessage.Error("JSON文字列をパースできませんでした"));
                return;
            }

            // Itemsをクリア
            Items.Clear();

            foreach (JsonValue? jsonValue in jsonArray.Cast<JsonValue?>()) {
                if (jsonValue == null) {
                    continue;
                }
                string jsonString = jsonValue.ToString();
                ClipboardItem? item = ClipboardItem.FromJson(jsonString, action);
                if (item == null) {
                    continue;
                }
                // Itemsに追加
                Items.Add(item);
                //保存
                item.Save();
            }

        }
        // 指定されたフォルダの中のSourceApplicationTitleが一致するアイテムをマージするコマンド
        public void MergeItemsBySourceApplicationTitleCommandExecute(ClipboardItem newItem) {
            // NewItemのSourceApplicationTitleが空の場合は何もしない
            if (string.IsNullOrEmpty(newItem.SourceApplicationTitle)) {
                return;
            }
            if (Items.Count == 0) {
                return;
            }
            List<ClipboardItem> sameTitleItems = [];
            // マージ先のアイテムのうち、SourceApplicationTitleが一致するアイテムを取得
            foreach (var item in Items) {
                if (newItem.SourceApplicationTitle == item.SourceApplicationTitle) {
                    // TypeがTextのアイテムのみマージ
                    if (item.ContentType == ClipboardContentTypes.Text) {
                        sameTitleItems.Add(item);
                    }
                }
            }
            // mergeFromItemsが空の場合は、newItemをそのまま返す。
            if (sameTitleItems.Count == 0) {
                return;
            }
            // マージ元のアイテムをマージ先(更新時間が一番古いもの)のアイテムにマージ
            ClipboardItem mergeToItem = Items.Last();
            // sameTitleItemsの1から最後までをマージ元のアイテムとする
            sameTitleItems.RemoveAt(sameTitleItems.Count - 1);

            // sameTitleItemsに、newItemを追加
            sameTitleItems.Insert(0, newItem);
            // マージ元のアイテムをマージ先のアイテムにマージ

            mergeToItem.MergeItems(sameTitleItems, false, Tools.DefaultAction);
            // newItemにマージしたアイテムをコピー
            mergeToItem.CopyTo(newItem);
            // マージしたアイテムを削除
            foreach (var mergedItem in sameTitleItems) {
                DeleteItem(mergedItem);
            }
            // mergedItemを削除
            DeleteItem(mergeToItem);
        }
        // 指定されたフォルダの全アイテムをマージするコマンド
        public void MergeItemsCommandExecute(ClipboardItem item) {
            if (Items.Count == 0) {
                return;
            }

            // マージ元のアイテム
            List<ClipboardItem> mergedFromItems = [];
            for (int i = Items.Count - 1; i > 0; i--) {
                // TypeがTextのアイテムのみマージ
                if (Items[i].ContentType == ClipboardContentTypes.Text) {
                    mergedFromItems.Add(Items[i]);
                }
            }
            // 先頭に引数のアイテムを追加
            mergedFromItems.Insert(0, item);
            // mergeToItemを取得(更新時間が一番古いアイテム)
            ClipboardItem mergeToItem = mergedFromItems.Last();
            // mergedFromItemsから、mergeToItemを削除
            mergedFromItems.RemoveAt(mergedFromItems.Count - 1);

            // マージ元のアイテムをマージ先のアイテムにマージ
            mergeToItem.MergeItems(mergedFromItems, false, Tools.DefaultAction);

            // マージ先アイテムを、newItemにコピー
            mergeToItem.CopyTo(item);

            // マージしたアイテムを削除
            foreach (var mergedItem in mergedFromItems) {
                DeleteItem(mergedItem);
            }
            // マージ先アイテムを削除
            DeleteItem(mergeToItem);

        }


    }
}