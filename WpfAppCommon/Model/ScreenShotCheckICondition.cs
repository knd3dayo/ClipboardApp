using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using WpfAppCommon.Utils;
using static WpfAppCommon.Model.ScreenShotCheckICondition;

namespace WpfAppCommon.Model {

    public class ScreenShotCheckResults {
        // 結果のEnum
        public enum ResultEnum {
            Empty,
            OK,
            NG,
            Unknown,
        }

        [JsonPropertyName("results")]
        public List<ScreenShotCheckICondition> Results { get; set; } = [];
        // 結果
        private ResultEnum _result = ResultEnum.Empty;
        [JsonIgnore]
        public ResultEnum Result {
            get {
                return _result;
            }
            set {
                _result = value;
            }
        }
        // 結果の文字列
        [JsonPropertyName("result_string")]
        public string ResultString {
            get {
                return _result switch {
                    ResultEnum.Empty => "",
                    ResultEnum.OK => "OK",
                    ResultEnum.NG => "NG",
                    ResultEnum.Unknown => "Unknown",
                    _ => "",
                };

            }
            set {
                _result = value switch {
                    "OK" => ResultEnum.OK,
                    "NG" => ResultEnum.NG,
                    "Unknown" => ResultEnum.Unknown,
                    _ => ResultEnum.Empty,
                };
            }
        }
        // 画像データ内の実際の設定値
        private string _actualValue = "";
        [JsonPropertyName("actual_value")]
        public string ActualValue {
            get {
                return _actualValue;
            }
            set {
                _actualValue = value;
            }
        }


        public static List<ScreenShotCheckICondition> FromJson(string json) {
            List<ScreenShotCheckICondition> result = [];
            var jsonObject = JsonSerializer.Deserialize<ScreenShotCheckResults>(json);
            if (jsonObject == null) {
                return result;
            }
            var results = jsonObject.Results;
            foreach (var item in results) {
                result.Add(item);
            }
            return result;

        }
    }
    public class CheckTypes {

        public enum CheckTypeEnum {
            [Description("等しい")]
            Equal,
            [Description("等しくない")]
            NotEqual,
            [Description("含む")]
            Include,
            [Description("含まない")]
            NotInclude,
            [Description("開始している")]
            StartWith,
            [Description("開始していない")]
            NotStartWith,
            [Description("終わっている")]
            EndWith,
            [Description("終わっていない")]
            NotEndWith,
            [Description("空である")]
            Empty,
            [Description("チェックボックス")]
            CheckBox,
        }

        public CheckTypeEnum CheckType { get; set; } = CheckTypeEnum.Equal;
        public string CheckTypeString {
            get {
                return EnumDescription.GetEnumDescription<CheckTypeEnum>(CheckType);
            }
        }
        public static ObservableCollection<CheckTypes> CheckTypeList {
            get {
                ObservableCollection<CheckTypes> result = [];
                foreach (CheckTypeEnum checkType in Enum.GetValues(typeof(CheckTypeEnum))) {
                    result.Add(new CheckTypes() {
                        CheckType = checkType,
                    });
                }
                return result;
            }
        }

    }

    public class ScreenShotCheckICondition {

        // 設定項目
        private string _settingItem = "";
        [JsonPropertyName("setting_item")]
        public string SettingItem {
            get {
                return _settingItem;
            }
            set {
                _settingItem = value;
            }
        }
        // 設定値
        private string _settingValue = "";
        [JsonPropertyName("setting_value")]
        public string SettingValue {
            get {
                return _settingValue;
            }
            set {
                _settingValue = value;
            }
        }
        // チェック内容
        private CheckTypes? _checkType;
        [JsonPropertyName("check_type")]
        public CheckTypes? CheckType {
            get {
                return _checkType;
            }
            set {
                _checkType = value;
            }
        }

        public string ToPromptString() {
            string result = $"{SettingItem}の値は{SettingValue}である";
            if (CheckType == null) {
                return result;
            }
            // CheckType.CheckTypeがEqualの場合
            if (CheckType.CheckType == CheckTypes.CheckTypeEnum.Equal) {
                result = $"{SettingItem}の値は{SettingValue}である";
            }
            // CheckType.CheckTypeがNotEqualの場合
            if (CheckType.CheckType == CheckTypes.CheckTypeEnum.NotEqual) {
                result = $"{SettingItem}の値は{SettingValue}でない";
            }
            // CheckType.CheckTypeがIncludeの場合
            if (CheckType.CheckType == CheckTypes.CheckTypeEnum.Include) {
                result = $"{SettingItem}の値に{SettingValue}が含まれている";
            }
            // CheckType.CheckTypeがNotIncludeの場合
            if (CheckType.CheckType == CheckTypes.CheckTypeEnum.NotInclude) {
                result = $"{SettingItem}の値に{SettingValue}が含まれていない";
            }
            // CheckType.CheckTypeがStartWithの場合
            if (CheckType.CheckType == CheckTypes.CheckTypeEnum.StartWith) {
                result = $"{SettingItem}の値が{SettingValue}で始まっている";
            }
            // CheckType.CheckTypeがNotStartWithの場合
            if (CheckType.CheckType == CheckTypes.CheckTypeEnum.NotStartWith) {
                result = $"{SettingItem}の値が{SettingValue}で始まっていない";
            }
            // CheckType.CheckTypeがEndWithの場合
            if (CheckType.CheckType == CheckTypes.CheckTypeEnum.EndWith) {
                result = $"{SettingItem}の値が{SettingValue}で終わっている";
            }
            // CheckType.CheckTypeがNotEndWithの場合
            if (CheckType.CheckType == CheckTypes.CheckTypeEnum.NotEndWith) {
                result = $"{SettingItem}の値が{SettingValue}で終わっていない";
            }
            // CheckType.CheckTypeがEmptyの場合
            if (CheckType.CheckType == CheckTypes.CheckTypeEnum.Empty) {
                result = $"{SettingItem}の値が空である";
            }
            // CheckType.CheckTypeがCheckBoxの場合
            if (CheckType.CheckType == CheckTypes.CheckTypeEnum.CheckBox) {
                result = $"{SettingItem}のチェックボックスが{SettingValue}になっている";
            }
            return result;
        }

        // ScreenShotCheckItemのIEnumerableをJSONに変換する
        public static string ToJson(IEnumerable<ScreenShotCheckICondition> items) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            return System.Text.Json.JsonSerializer.Serialize(items, options);
        }
        // JSONをScreenShotCheckItemのListに変換する
        public static List<ScreenShotCheckICondition> FromJson(string json) {
            List<ScreenShotCheckICondition> result = [];
            var jsonObject = JsonSerializer.Deserialize<List<ScreenShotCheckICondition>>(json);
            if (jsonObject == null) {
                return result;
            }
            foreach (var item in jsonObject) {
                result.Add(item);
            }
            return result;

        }
    }
}
