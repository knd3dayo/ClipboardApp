
using System.IO;
using PythonAILib.Model;

namespace WpfAppCommon.Model {
    public class ClipboardAppConfig {

        // 開発中機能の有効化
        private static Boolean? _enableDevFeatures;
        public static bool EnableDevFeatures {
            get {
                if (_enableDevFeatures == null) {
                    _enableDevFeatures = Properties.Settings.Default.EnableDevFeatures;
                }
                return _enableDevFeatures.Value;
            }
            set {
                _enableDevFeatures = value;
                Properties.Settings.Default.EnableDevFeatures = value;
            }
        }

        // 言語
        private static string? _lang;
        public static string Lang {
            get {
                if (_lang == null) {
                    _lang = Properties.Settings.Default.Lang;
                }
                return _lang;
            }
            set {
                _lang = value;
                Properties.Settings.Default.Lang = value;
            }
        }
        public static string ActualLang {
            get {
                if (string.IsNullOrEmpty(Lang)) {
                    return  System.Globalization.CultureInfo.CurrentUICulture.Name;
                }
                return Lang;
            }
        }

        // このアプリケーションのデータ用のフォルダを取得
        public static string AppDataFolder {
            get {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string appDataFolder = Path.Combine(appDataPath, "ClipboardApp");
                if (!Directory.Exists(appDataFolder)) {
                    Directory.CreateDirectory(appDataFolder);
                }
                return appDataFolder;
            }
        }
        // WpfCommon.Properties.Settingsの値をプロパティとして宣言する。

        // MonitorTargetAppNames
        private static string? _monitorTargetAppNames;
        public static string MonitorTargetAppNames {
            get {
                if (_monitorTargetAppNames == null) {
                    _monitorTargetAppNames = WpfAppCommon.Properties.Settings.Default.MonitorTargetAppNames;
                }
                return _monitorTargetAppNames;
            }
            set {
                _monitorTargetAppNames = value;
                WpfAppCommon.Properties.Settings.Default.MonitorTargetAppNames = value;
            }
        }

        // PythonDllPath
        private static string? _pythonDllPath;
        public static string PythonDllPath {
            get {
                if (_pythonDllPath == null) {
                    _pythonDllPath = WpfAppCommon.Properties.Settings.Default.PythonDllPath;
                }
                return _pythonDllPath;
            }
            set {
                _pythonDllPath = value;
                WpfAppCommon.Properties.Settings.Default.PythonDllPath = value;
            }
        }
        // PythonVenvPath
        private static string? _pythonVenvPath;
        public static string PythonVenvPath {
            get {
                if (_pythonVenvPath == null) {
                    _pythonVenvPath = WpfAppCommon.Properties.Settings.Default.PythonVenvPath;
                }
                return _pythonVenvPath;
            }
            set {
                _pythonVenvPath = value;
                WpfAppCommon.Properties.Settings.Default.PythonVenvPath = value;
            }
        }

        // AzureOpenAI
        private static Boolean? _azureOpenAI;
        public static bool AzureOpenAI {
            get {
                if (_azureOpenAI == null) {
                    _azureOpenAI = WpfAppCommon.Properties.Settings.Default.AzureOpenAI;
                }
                return _azureOpenAI.Value;
            }
            set {
                _azureOpenAI = value;
                WpfAppCommon.Properties.Settings.Default.AzureOpenAI = value;
            }
        }
        // AzureOpenAIEndpoint
        private static string? _azureOpenAIEndpoint;
        public static string AzureOpenAIEndpoint {
            get {
                if (_azureOpenAIEndpoint == null) {
                    _azureOpenAIEndpoint = WpfAppCommon.Properties.Settings.Default.AzureOpenAIEndpoint;
                }
                return _azureOpenAIEndpoint;
            }
            set {
                _azureOpenAIEndpoint = value;
                WpfAppCommon.Properties.Settings.Default.AzureOpenAIEndpoint = value;
            }
        }

        // OpenAIKey
        private static string? _openAIKey;
        public static string OpenAIKey {
            get {
                if (_openAIKey == null) {
                    _openAIKey = WpfAppCommon.Properties.Settings.Default.OpenAIKey;
                }
                return _openAIKey;
            }
            set {
                _openAIKey = value;
                WpfAppCommon.Properties.Settings.Default.OpenAIKey = value;
            }
        }
        // OpenAICompletionModel
        private static string? _openAICompletionModel;
        public static string OpenAICompletionModel {
            get {
                if (_openAICompletionModel == null) {
                    _openAICompletionModel = WpfAppCommon.Properties.Settings.Default.OpenAICompletionModel;
                }
                return _openAICompletionModel;
            }
            set {
                _openAICompletionModel = value;
                WpfAppCommon.Properties.Settings.Default.OpenAICompletionModel = value;
            }
        }
        // OpenAIEmbeddingModel
        private static string? _openAIEmbeddingModel;
        public static string OpenAIEmbeddingModel {
            get {
                if (_openAIEmbeddingModel == null) {
                    _openAIEmbeddingModel = WpfAppCommon.Properties.Settings.Default.OpenAIEmbeddingModel;
                }
                return _openAIEmbeddingModel;
            }
            set {
                _openAIEmbeddingModel = value;
                WpfAppCommon.Properties.Settings.Default.OpenAIEmbeddingModel = value;
            }
        }
        // OpenAICompletionBaseURL
        private static string? _openAICompletionBaseURL;
        public static string OpenAICompletionBaseURL {
            get {
                if (_openAICompletionBaseURL == null) {
                    _openAICompletionBaseURL = WpfAppCommon.Properties.Settings.Default.OpenAICompletionBaseURL;
                }
                return _openAICompletionBaseURL;
            }
            set {
                _openAICompletionBaseURL = value;
                WpfAppCommon.Properties.Settings.Default.OpenAICompletionBaseURL = value;
            }
        }
        // OpenAIEmbeddingBaseURL
        private static string? _openAIEmbeddingBaseURL;
        public static string OpenAIEmbeddingBaseURL {
            get {
                if (_openAIEmbeddingBaseURL == null) {
                    _openAIEmbeddingBaseURL = WpfAppCommon.Properties.Settings.Default.OpenAIEmbeddingBaseURL;
                }
                return _openAIEmbeddingBaseURL;
            }
            set {
                _openAIEmbeddingBaseURL = value;
                WpfAppCommon.Properties.Settings.Default.OpenAIEmbeddingBaseURL = value;
            }
        }

        // AutoExtractImageWithPyOCR
        private static Boolean? _autoExtractImageWithPyOCR;
        public static bool AutoExtractImageWithPyOCR {
            get {
                if (_autoExtractImageWithPyOCR == null) {
                    _autoExtractImageWithPyOCR = WpfAppCommon.Properties.Settings.Default.AutoExtractImageWithPyOCR;
                }
                return _autoExtractImageWithPyOCR.Value;
            }
            set {
                _autoExtractImageWithPyOCR = value;
                WpfAppCommon.Properties.Settings.Default.AutoExtractImageWithPyOCR = value;
            }
        }

        // EmbeddingWhenExtractingTextFromImage
        private static Boolean? _embeddingWhenExtractingTextFromImage;
        public static bool EmbeddingWhenExtractingTextFromImage {
            get {
                if (_embeddingWhenExtractingTextFromImage == null) {
                    _embeddingWhenExtractingTextFromImage = WpfAppCommon.Properties.Settings.Default.EmbeddingWhenExtractingTextFromImage;
                }
                return _embeddingWhenExtractingTextFromImage.Value;
            }
            set {
                _embeddingWhenExtractingTextFromImage = value;
                WpfAppCommon.Properties.Settings.Default.EmbeddingWhenExtractingTextFromImage = value;
            }
        }

        // SyncClipboardItemAndOSFolder
        private static Boolean? _syncClipboardItemAndOSFolder;
        public static bool SyncClipboardItemAndOSFolder {
            get {
                if (_syncClipboardItemAndOSFolder == null) {
                    _syncClipboardItemAndOSFolder = WpfAppCommon.Properties.Settings.Default.SyncClipboardItemAndOSFolder;
                }
                return _syncClipboardItemAndOSFolder.Value;
            }
            set {
                _syncClipboardItemAndOSFolder = value;
                WpfAppCommon.Properties.Settings.Default.SyncClipboardItemAndOSFolder = value;
            }
        }
        // SyncFolderName
        private static string? _syncFolderName;
        public static string SyncFolderName {
            get {
                if (_syncFolderName == null) {
                    _syncFolderName = WpfAppCommon.Properties.Settings.Default.SyncFolderName;
                }
                return _syncFolderName;
            }
            set {
                _syncFolderName = value;
                WpfAppCommon.Properties.Settings.Default.SyncFolderName = value;
            }
        }


        // AutoCommit
        private static Boolean? _autoCommit;
        public static bool AutoCommit {
            get {
                if (_autoCommit == null) {
                    _autoCommit = WpfAppCommon.Properties.Settings.Default.AutoCommit;
                }
                return _autoCommit.Value;
            }
            set {
                _autoCommit = value;
                WpfAppCommon.Properties.Settings.Default.AutoCommit = value;
            }
        }

        // AutoMergeItemsBySourceApplicationTitle
        private static Boolean? _autoMergeItemsBySourceApplicationTitle;
        public static bool AutoMergeItemsBySourceApplicationTitle {
            get {
                if (_autoMergeItemsBySourceApplicationTitle == null) {
                    _autoMergeItemsBySourceApplicationTitle = WpfAppCommon.Properties.Settings.Default.AutoMergeItemsBySourceApplicationTitle;
                }
                return _autoMergeItemsBySourceApplicationTitle.Value;
            }
            set {
                _autoMergeItemsBySourceApplicationTitle = value;
                WpfAppCommon.Properties.Settings.Default.AutoMergeItemsBySourceApplicationTitle = value;
            }
        }

        // AutoBackgroundInfo
        private static Boolean? _autoBackgroundInfo;
        public static bool AutoBackgroundInfo {
            get {
                if (_autoBackgroundInfo == null) {
                    _autoBackgroundInfo = WpfAppCommon.Properties.Settings.Default.AutoBackgroundInfo;
                }
                return _autoBackgroundInfo.Value;
            }
            set {
                _autoBackgroundInfo = value;
                WpfAppCommon.Properties.Settings.Default.AutoBackgroundInfo = value;
            }
        }
        // AutoSummary
        private static Boolean? _autoSummary;
        public static bool AutoSummary {
            get {
                if (_autoSummary == null) {
                    _autoSummary = WpfAppCommon.Properties.Settings.Default.AutoSummary;
                }
                return _autoSummary.Value;
            }
            set {
                _autoSummary = value;
                WpfAppCommon.Properties.Settings.Default.AutoSummary = value;
            }
        }

        // AutoDescriptionWithOpenAI
        private static Boolean? _autoDescriptionWithOpenAI;
        public static bool AutoDescriptionWithOpenAI {
            get {
                if (_autoDescriptionWithOpenAI == null) {
                    _autoDescriptionWithOpenAI = WpfAppCommon.Properties.Settings.Default.AutoDescriptionWithOpenAI;
                }
                return _autoDescriptionWithOpenAI.Value;
            }
            set {
                _autoDescriptionWithOpenAI = value;
                WpfAppCommon.Properties.Settings.Default.AutoDescriptionWithOpenAI = value;
            }
        }
        // AutoExtractImageWithOpenAI
        private static Boolean? _autoExtractImageWithOpenAI;
        public static bool AutoExtractImageWithOpenAI {
            get {
                if (_autoExtractImageWithOpenAI == null) {
                    _autoExtractImageWithOpenAI = WpfAppCommon.Properties.Settings.Default.AutoExtractImageWithOpenAI;
                }
                return _autoExtractImageWithOpenAI.Value;
            }
            set {
                _autoExtractImageWithOpenAI = value;
                WpfAppCommon.Properties.Settings.Default.AutoExtractImageWithOpenAI = value;
            }
        }
        // UserMaskedDataInOpenAI
        private static Boolean? _userMaskedDataInOpenAI;
        public static bool UserMaskedDataInOpenAI {
            get {
                if (_userMaskedDataInOpenAI == null) {
                    _userMaskedDataInOpenAI = WpfAppCommon.Properties.Settings.Default.UserMaskedDataInOpenAI;
                }
                return _userMaskedDataInOpenAI.Value;
            }
            set {
                _userMaskedDataInOpenAI = value;
                WpfAppCommon.Properties.Settings.Default.UserMaskedDataInOpenAI = value;
            }
        }

        // AutoEmbedding
        private static Boolean? _autoEmbedding;
        public static bool AutoEmbedding {
            get {
                if (_autoEmbedding == null) {
                    _autoEmbedding = WpfAppCommon.Properties.Settings.Default.AutoEmbedding;
                }
                return _autoEmbedding.Value;
            }
            set {
                _autoEmbedding = value;
                WpfAppCommon.Properties.Settings.Default.AutoEmbedding = value;
            }
        }
        // Embeddingの対象に背景情報を含める
        private static Boolean? _includeBackgroundInfoInEmbedding;

        public static bool IncludeBackgroundInfoInEmbedding {
            get {
                if (_includeBackgroundInfoInEmbedding == null) {
                    _includeBackgroundInfoInEmbedding = WpfAppCommon.Properties.Settings.Default.IncludeBackgroundInfoInEmbedding;
                }
                return _includeBackgroundInfoInEmbedding.Value;
            }
            set {
                _includeBackgroundInfoInEmbedding = value;
                WpfAppCommon.Properties.Settings.Default.IncludeBackgroundInfoInEmbedding = value;
            }
        }


        // AutoFileExtract
        private static Boolean? _autoFileExtract;
        public static bool AutoFileExtract {
            get {
                if (_autoFileExtract == null) {
                    _autoFileExtract = WpfAppCommon.Properties.Settings.Default.AutoFileExtract;
                }
                return _autoFileExtract.Value;
            }
            set {
                _autoFileExtract = value;
                WpfAppCommon.Properties.Settings.Default.AutoFileExtract = value;
            }
        }
        // BackupGeneration
        private static int _backupGeneration = -1;
        public static int BackupGeneration {
            get {
                if (_backupGeneration == -1) {
                    _backupGeneration = WpfAppCommon.Properties.Settings.Default.BackupGeneration;
                }
                return _backupGeneration;
            }
            set {
                _backupGeneration = value;
                WpfAppCommon.Properties.Settings.Default.BackupGeneration = value;
            }
        }



        // TextWrapping
        private static bool _textWrapping = false;
        public static System.Windows.TextWrapping TextWrapping {
            get {
                _textWrapping = WpfAppCommon.Properties.Settings.Default.TextWrapping;
                return _textWrapping ? System.Windows.TextWrapping.Wrap : System.Windows.TextWrapping.NoWrap;
            }
            set {
                _textWrapping = value == System.Windows.TextWrapping.Wrap;
                WpfAppCommon.Properties.Settings.Default.TextWrapping = _textWrapping;
            }
        }


        // PreviewMode
        private static Boolean? _previewMode;
        public static bool PreviewMode {
            get {
                if (_previewMode == null) {
                    _previewMode = WpfAppCommon.Properties.Settings.Default.PreviewMode;
                }
                return _previewMode.Value;
            }
            set {
                _previewMode = value;
                WpfAppCommon.Properties.Settings.Default.PreviewMode = value;
            }
        }

        // IgnoreLineCount
        private static int _ignoreLineCount = -1;
        public static int IgnoreLineCount {
            get {
                if (_ignoreLineCount == -1) {
                    _ignoreLineCount = WpfAppCommon.Properties.Settings.Default.IgnoreLineCount;
                }
                return _ignoreLineCount;
            }
            set {
                _ignoreLineCount = value;
                WpfAppCommon.Properties.Settings.Default.IgnoreLineCount = value;
            }
        }
        #region 開発中機能関連の設定
        // UseSpacy
        private static Boolean? _useSpacy;
        public static bool UseSpacy {
            get {
                if (_useSpacy == null) {
                    _useSpacy = WpfAppCommon.Properties.Settings.Default.UseSpacy;
                }
                return _useSpacy.Value;
            }
            set {
                _useSpacy = value;
                WpfAppCommon.Properties.Settings.Default.UseSpacy = value;
            }
        }
        // SpacyModel
        private static string? _spacyModel;
        public static string SpacyModel {
            get {
                if (_spacyModel == null) {
                    _spacyModel = WpfAppCommon.Properties.Settings.Default.SpacyModel;
                }
                return _spacyModel;
            }
            set {
                _spacyModel = value;
                WpfAppCommon.Properties.Settings.Default.SpacyModel = value;
            }
        }
        // TesseractExePath
        private static string? _tesseractExePath;
        public static string TesseractExePath {
            get {
                if (_tesseractExePath == null) {
                    _tesseractExePath = WpfAppCommon.Properties.Settings.Default.TesseractExePath;
                }
                return _tesseractExePath;
            }
            set {
                _tesseractExePath = value;
                WpfAppCommon.Properties.Settings.Default.TesseractExePath = value;
            }
        }

        // AutoTag
        private static Boolean? _autoTag;
        public static bool AutoTag {
            get {
                if (_autoTag == null) {
                    _autoTag = WpfAppCommon.Properties.Settings.Default.AutoTag;
                }
                return _autoTag.Value;
            }
            set {
                _autoTag = value;
                WpfAppCommon.Properties.Settings.Default.AutoTag = value;
            }
        }

        // AutoDescription
        private static Boolean? _autoDescription;
        public static bool AutoDescription {
            get {
                if (_autoDescription == null) {
                    _autoDescription = WpfAppCommon.Properties.Settings.Default.AutoDescription;
                }
                return _autoDescription.Value;
            }
            set {
                _autoDescription = value;
                WpfAppCommon.Properties.Settings.Default.AutoDescription = value;
            }
        }

        // AnalyzeJapaneseSentence 日本語文章の解析
        private static Boolean? _analyzeJapaneseSentence;
        public static bool AnalyzeJapaneseSentence {
            get {
                if (_analyzeJapaneseSentence == null) {
                    _analyzeJapaneseSentence = WpfAppCommon.Properties.Settings.Default.AnalyzeJapaneseSentence;
                }
                return _analyzeJapaneseSentence.Value;
            }
            set {
                _analyzeJapaneseSentence = value;
                WpfAppCommon.Properties.Settings.Default.AnalyzeJapaneseSentence = value;
            }
        }
        // 自動的にQAを生成する
        private static Boolean? _autoGenerateQA;
        public static bool AutoGenerateQA {
            get {
                if (_autoGenerateQA == null) {
                    _autoGenerateQA = WpfAppCommon.Properties.Settings.Default.AutoGenerateQA;
                }
                return _autoGenerateQA.Value;
            }
            set {
                _autoGenerateQA = value;
                WpfAppCommon.Properties.Settings.Default.AutoGenerateQA = value;
            }
        }

        // 自動的にIssuesを生成する
        private static Boolean? _autoGenerateIssues;
        public static bool AutoGenerateIssues {
            get {
                if (_autoGenerateIssues == null) {
                    _autoGenerateIssues = WpfAppCommon.Properties.Settings.Default.AutoGenerateIssues;
                }
                return _autoGenerateIssues.Value;
            }
            set {
                _autoGenerateIssues = value;
                WpfAppCommon.Properties.Settings.Default.AutoGenerateIssues = value;
            }
        }


        #endregion


        public static void Save() {
            WpfAppCommon.Properties.Settings.Default.Save();

        }
        public static void Reload() {
            Properties.Settings.Default.Reload();
        }
        public static  OpenAIProperties CreateOpenAIProperties() {
            OpenAIProperties openAIProperties = new() {
                OpenAIKey = OpenAIKey,
                OpenAICompletionModel = OpenAICompletionModel,
                OpenAIEmbeddingModel = OpenAIEmbeddingModel,
                AzureOpenAI = AzureOpenAI,
            };

            if (string.IsNullOrEmpty(WpfAppCommon.Properties.Settings.Default.AzureOpenAIEndpoint) == false) {
                openAIProperties.AzureOpenAIEndpoint = WpfAppCommon.Properties.Settings.Default.AzureOpenAIEndpoint;
            }

            if (WpfAppCommon.Properties.Settings.Default.OpenAICompletionBaseURL != "") {
                openAIProperties.OpenAICompletionBaseURL = WpfAppCommon.Properties.Settings.Default.OpenAICompletionBaseURL;
            }
            if (WpfAppCommon.Properties.Settings.Default.OpenAIEmbeddingBaseURL != "") {
                openAIProperties.OpenAIEmbeddingBaseURL = WpfAppCommon.Properties.Settings.Default.OpenAIEmbeddingBaseURL;
            }
            return openAIProperties;
        }

    }

    public class MiscConfig {

        private static DateTime? _windowsNotificationLastCheckedTime;

        public static DateTime WindowsNotificationLastCheckedTime {
            get {
                if (_windowsNotificationLastCheckedTime == null) {
                    _windowsNotificationLastCheckedTime = Properties.Misc.Default.WindowsNotificationLastCheckedTime;
                }
                return _windowsNotificationLastCheckedTime == null ? DateTime.MinValue.ToUniversalTime() : _windowsNotificationLastCheckedTime.Value;
            }
            set {
                _windowsNotificationLastCheckedTime = value;
                Properties.Misc.Default.WindowsNotificationLastCheckedTime = value;
            }
        }
        public static void Save() {
            Properties.Misc.Default.Save();
        }


    }
}
