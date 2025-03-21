using LibPythonAI.Model.Content;
using LibPythonAI.Utils.Common;
using PythonAILib.Model.File;

namespace PythonAILib.Model.AutoProcess {
    public class AutoProcessRuleCondition {


        public AutoProcessRuleConditionEntity Entity { get; set; }
        public AutoProcessRuleCondition(AutoProcessRuleConditionEntity entity) {
            Entity = entity;
        }

        public enum ConditionTypeEnum {
            AllItems,
            DescriptionContains,
            ContentContains,
            SourceApplicationNameContains,
            SourceApplicationTitleContains,
            SourceApplicationPathContains,
            ContentTypeIs,

        }
        // 条件のキーワード
        public string Keyword {
            get {
                return Entity.Keyword;
            }
            set {
                Entity.Keyword = value;
            }
        }

        // 条件の種類
        public ConditionTypeEnum Type {
            get {
                return Entity.ConditionType;
            }
            set {
                Entity.ConditionType = value;
            }
        }

        // アイテムのタイプ種類のリスト
        public List<ContentTypes.ContentItemTypes> ContentTypes {
            get {
                return Entity.ContentTypes;
            }
            set {
                Entity.ContentTypes = value;
            }
        }


        public ConditionTypeEnum ConditionType {
            get {
                return Entity.ConditionType;
            }
            set {
                Entity.ConditionType = value;
            }
        }

        public int MinLineCount {
            get {
                return Entity.MinLineCount;
            }
            set {
                Entity.MinLineCount = value;
            }
        }

        public int MaxLineCount {
            get {
                return Entity.MaxLineCount;
            }
            set {
                Entity.MaxLineCount = value;
            }
        }




        //ClipboardItemのDescriptionが指定したキーワードを含むかどうか
        public bool IsDescriptionContains(ContentItemWrapper clipboardItem, string keyword) {
            // DescriptionがNullの場合はFalseを返す
            if (clipboardItem.Description == null) {
                return false;
            }
            LogWrapper.Info("Description:" + clipboardItem.Description);
            LogWrapper.Info("Keyword:" + keyword);
            LogWrapper.Info("Contains:" + clipboardItem.Description.Contains(keyword));

            return clipboardItem.Description.Contains(keyword);

        }
        //ClipboardItemのContentが指定したキーワードを含むかどうか
        public bool IsContentContains(ContentItemWrapper clipboardItem, string keyword) {
            // ContentがNullの場合はFalseを返す
            if (clipboardItem.Content == null) {
                return false;
            }
            return clipboardItem.Content.Contains(keyword);
        }
        // ClipboardItemのSourceApplicationNameが指定したキーワードを含むかどうか
        public bool IsSourceApplicationNameContains(ContentItemWrapper clipboardItem, string keyword) {
            // SourceApplicationNameがnullの場合は、falseを返す
            if (clipboardItem.SourceApplicationName == null) {
                return false;
            }
            return clipboardItem.SourceApplicationName.Contains(keyword);
        }
        // ClipboardItemのSourceApplicationTitleが指定したキーワードを含むかどうか
        public bool IsSourceApplicationTitleContains(ContentItemWrapper clipboardItem, string keyword) {
            // SourceApplicationTitleがnullの場合は、falseを返す
            if (clipboardItem.SourceApplicationTitle == null) {
                return false;
            }
            return clipboardItem.SourceApplicationTitle.Contains(keyword);
        }
        // ClipboardItemのSourceApplicationPathが指定したキーワードを含むかどうか
        public bool IsSourceApplicationPathContains(ContentItemWrapper clipboardItem, string keyword) {
            // SourceApplicationPathがnullの場合は、falseを返す
            if (clipboardItem.SourceApplicationPath == null) {
                return false;
            }
            return clipboardItem.SourceApplicationPath != null && clipboardItem.SourceApplicationPath.Contains(keyword);
        }

        // ClipboardItemのContentの行数が指定した行数以上かどうか
        public bool IsContentLineCountOver(ContentItemWrapper clipboardItem) {
            // MinLineCountが-1の場合はTrueを返す
            if (MinLineCount == -1) {
                return true;
            }
            // ContentがNullの場合はFalseを返す
            if (clipboardItem.Content == null) {
                return false;
            }
            return clipboardItem.Content.Split('\n').Length >= MinLineCount;
        }
        // ClipboardItemのContentの行数が指定した行数以下かどうか
        public bool IsContentLineCountUnder(ContentItemWrapper clipboardItem) {
            // MaxLineCountが-1の場合はTrueを返す
            if (MaxLineCount == -1) {
                return true;
            }
            // ContentがNullの場合はFalseを返す
            if (clipboardItem.Content == null) {
                return false;
            }
            return clipboardItem.Content.Split('\n').Length <= MaxLineCount;
        }

        // ConditionTypeに対応する関数を実行してBoolを返す
        // ★TODO SearchConditionと共通化する
        public bool CheckCondition(ContentItemWrapper clipboardItem) {
            return Type switch {
                ConditionTypeEnum.DescriptionContains => IsDescriptionContains(clipboardItem, Keyword),
                ConditionTypeEnum.ContentContains => IsContentContains(clipboardItem, Keyword),
                ConditionTypeEnum.SourceApplicationNameContains => IsSourceApplicationNameContains(clipboardItem, Keyword),
                ConditionTypeEnum.SourceApplicationTitleContains => IsSourceApplicationTitleContains(clipboardItem, Keyword),
                ConditionTypeEnum.SourceApplicationPathContains => IsSourceApplicationPathContains(clipboardItem, Keyword),
                ConditionTypeEnum.ContentTypeIs => CheckContentTypeIs(clipboardItem),
                _ => false,
            };
        }

        // ContentTypeIsの条件にマッチするかどうか
        public bool CheckContentTypeIs(ContentItemWrapper clipboardItem) {
            if (ContentTypes.Contains(clipboardItem.ContentType) == false) {
                return false;
            }
            if (clipboardItem.ContentType == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text) {
                return IsContentLineCountOver(clipboardItem) && IsContentLineCountUnder(clipboardItem);
            }
            return true;
        }

    }

}
