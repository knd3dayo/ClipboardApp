using System.IO;
using PythonAILib.Common;

namespace AIChatExplorer.ViewModel.Settings {
    public class AIChatExplorerConfig {

        private static AIChatExplorerConfig? _instance;
        public static AIChatExplorerConfig Instance {
            get {
                if (_instance == null) {
                    _instance = new AIChatExplorerConfig();
                }
                return _instance;
            }
        }

        // 開発中機能の有効化
        private bool? _enableDevFeatures;
        public bool EnableDevFeatures {
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
        private string? _lang;
        public string Lang {
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
        public string ActualLang {
            get {
                if (string.IsNullOrEmpty(Lang)) {
                    return System.Globalization.CultureInfo.CurrentUICulture.Name;
                }
                return Lang;
            }
        }

        // このアプリケーションのデータ用のフォルダを取得
        public string AppDataFolder {
            get {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string appDataFolder = Path.Combine(appDataPath, "AIChatExplorer");
                if (!Directory.Exists(appDataFolder)) {
                    Directory.CreateDirectory(appDataFolder);
                }
                return appDataFolder;
            }
        }
        // WpfCommon.Properties.Settingsの値をプロパティとして宣言する。

        // MonitorTargetAppNames
        private string? _monitorTargetAppNames;
        public string MonitorTargetAppNames {
            get {
                if (_monitorTargetAppNames == null) {
                    _monitorTargetAppNames = Properties.Settings.Default.MonitorTargetAppNames;
                }
                return _monitorTargetAppNames;
            }
            set {
                _monitorTargetAppNames = value;
                Properties.Settings.Default.MonitorTargetAppNames = value;
            }
        }

        // PythonDllPath
        private string? _pythonDllPath;
        public string PythonDllPath {
            get {
                if (_pythonDllPath == null) {
                    _pythonDllPath = Properties.Settings.Default.PythonDllPath;
                }
                return _pythonDllPath;
            }
            set {
                _pythonDllPath = value;
                Properties.Settings.Default.PythonDllPath = value;
            }
        }
        // PythonVenvPath
        private string? _pythonVenvPath;
        public string PythonVenvPath {
            get {
                if (_pythonVenvPath == null) {
                    _pythonVenvPath = Properties.Settings.Default.PythonVenvPath;
                }
                return _pythonVenvPath;
            }
            set {
                _pythonVenvPath = value;
                Properties.Settings.Default.PythonVenvPath = value;
            }
        }

        // AzureOpenAI
        private bool? _azureOpenAI;
        public bool AzureOpenAI {
            get {
                if (_azureOpenAI == null) {
                    _azureOpenAI = Properties.Settings.Default.AzureOpenAI;
                }
                return _azureOpenAI.Value;
            }
            set {
                _azureOpenAI = value;
                Properties.Settings.Default.AzureOpenAI = value;
            }
        }

        // AzureOpenAIAPIVersion
        private string? _azureOpenAIAPIVersion;
        public string AzureOpenAIAPIVersion {
            get {
                if (_azureOpenAIAPIVersion == null) {
                    _azureOpenAIAPIVersion = Properties.Settings.Default.AzureOpenAIAPIVersion;
                }
                return _azureOpenAIAPIVersion;
            }
            set {
                _azureOpenAIAPIVersion = value;
                Properties.Settings.Default.AzureOpenAIAPIVersion = value;
            }
        }

        // AzureOpenAIEndpoint
        private string? _azureOpenAIEndpoint;
        public string AzureOpenAIEndpoint {
            get {
                if (_azureOpenAIEndpoint == null) {
                    _azureOpenAIEndpoint = Properties.Settings.Default.AzureOpenAIEndpoint;
                }
                return _azureOpenAIEndpoint;
            }
            set {
                _azureOpenAIEndpoint = value;
                Properties.Settings.Default.AzureOpenAIEndpoint = value;
            }
        }

        // OpenAIKey
        private string? _openAIKey;
        public string OpenAIKey {
            get {
                if (_openAIKey == null) {
                    _openAIKey = Properties.Settings.Default.OpenAIKey;
                }
                return _openAIKey;
            }
            set {
                _openAIKey = value;
                Properties.Settings.Default.OpenAIKey = value;
            }
        }
        // OpenAICompletionModel
        private string? _openAICompletionModel;
        public string OpenAICompletionModel {
            get {
                if (_openAICompletionModel == null) {
                    _openAICompletionModel = Properties.Settings.Default.OpenAICompletionModel;
                }
                return _openAICompletionModel;
            }
            set {
                _openAICompletionModel = value;
                Properties.Settings.Default.OpenAICompletionModel = value;
            }
        }
        // OpenAIEmbeddingModel
        private string? _openAIEmbeddingModel;
        public string OpenAIEmbeddingModel {
            get {
                if (_openAIEmbeddingModel == null) {
                    _openAIEmbeddingModel = Properties.Settings.Default.OpenAIEmbeddingModel;
                }
                return _openAIEmbeddingModel;
            }
            set {
                _openAIEmbeddingModel = value;
                Properties.Settings.Default.OpenAIEmbeddingModel = value;
            }
        }
        // OpenAIBaseURL
        private string? _openAIBaseURL;
        public string OpenAIBaseURL {
            get {
                if (_openAIBaseURL == null) {
                    _openAIBaseURL = Properties.Settings.Default.OpenAIBaseURL;
                }
                return _openAIBaseURL;
            }
            set {
                _openAIBaseURL = value;
                Properties.Settings.Default.OpenAIBaseURL = value;
            }
        }

        // AutoExtractImageWithPyOCR
        private bool? _autoExtractImageWithPyOCR;
        public bool AutoExtractImageWithPyOCR {
            get {
                if (_autoExtractImageWithPyOCR == null) {
                    _autoExtractImageWithPyOCR = Properties.Settings.Default.AutoExtractImageWithPyOCR;
                }
                return _autoExtractImageWithPyOCR.Value;
            }
            set {
                _autoExtractImageWithPyOCR = value;
                Properties.Settings.Default.AutoExtractImageWithPyOCR = value;
            }
        }

        // EmbeddingWhenExtractingTextFromImage
        private bool? _embeddingWhenExtractingTextFromImage;
        public bool EmbeddingWhenExtractingTextFromImage {
            get {
                if (_embeddingWhenExtractingTextFromImage == null) {
                    _embeddingWhenExtractingTextFromImage = Properties.Settings.Default.EmbeddingWhenExtractingTextFromImage;
                }
                return _embeddingWhenExtractingTextFromImage.Value;
            }
            set {
                _embeddingWhenExtractingTextFromImage = value;
                Properties.Settings.Default.EmbeddingWhenExtractingTextFromImage = value;
            }
        }

        // AutoBackgroundInfo
        private bool? _autoBackgroundInfo;
        public bool AutoBackgroundInfo {
            get {
                if (_autoBackgroundInfo == null) {
                    _autoBackgroundInfo = Properties.Settings.Default.AutoBackgroundInfo;
                }
                return _autoBackgroundInfo.Value;
            }
            set {
                _autoBackgroundInfo = value;
                Properties.Settings.Default.AutoBackgroundInfo = value;
            }
        }
        // AutoSummary
        private bool? _autoSummary;
        public bool AutoSummary {
            get {
                if (_autoSummary == null) {
                    _autoSummary = Properties.Settings.Default.AutoSummary;
                }
                return _autoSummary.Value;
            }
            set {
                _autoSummary = value;
                Properties.Settings.Default.AutoSummary = value;
            }
        }

        // AutoDescriptionWithOpenAI
        private bool? _autoDescriptionWithOpenAI;
        public bool AutoDescriptionWithOpenAI {
            get {
                if (_autoDescriptionWithOpenAI == null) {
                    _autoDescriptionWithOpenAI = Properties.Settings.Default.AutoDescriptionWithOpenAI;
                }
                return _autoDescriptionWithOpenAI.Value;
            }
            set {
                _autoDescriptionWithOpenAI = value;
                Properties.Settings.Default.AutoDescriptionWithOpenAI = value;
            }
        }
        // AutoExtractImageWithOpenAI
        private bool? _autoExtractImageWithOpenAI;
        public bool AutoExtractImageWithOpenAI {
            get {
                if (_autoExtractImageWithOpenAI == null) {
                    _autoExtractImageWithOpenAI = Properties.Settings.Default.AutoExtractImageWithOpenAI;
                }
                return _autoExtractImageWithOpenAI.Value;
            }
            set {
                _autoExtractImageWithOpenAI = value;
                Properties.Settings.Default.AutoExtractImageWithOpenAI = value;
            }
        }
        // UserMaskedDataInOpenAI
        private bool? _userMaskedDataInOpenAI;
        public bool UserMaskedDataInOpenAI {
            get {
                if (_userMaskedDataInOpenAI == null) {
                    _userMaskedDataInOpenAI = Properties.Settings.Default.UserMaskedDataInOpenAI;
                }
                return _userMaskedDataInOpenAI.Value;
            }
            set {
                _userMaskedDataInOpenAI = value;
                Properties.Settings.Default.UserMaskedDataInOpenAI = value;
            }
        }

        // AutoFileExtract
        private bool? _autoFileExtract;
        public bool AutoFileExtract {
            get {
                if (_autoFileExtract == null) {
                    _autoFileExtract = Properties.Settings.Default.AutoFileExtract;
                }
                return _autoFileExtract.Value;
            }
            set {
                _autoFileExtract = value;
                Properties.Settings.Default.AutoFileExtract = value;
            }
        }
        // BackupGeneration
        private int _backupGeneration = -1;
        public int BackupGeneration {
            get {
                if (_backupGeneration == -1) {
                    _backupGeneration = Properties.Settings.Default.BackupGeneration;
                }
                return _backupGeneration;
            }
            set {
                _backupGeneration = value;
                Properties.Settings.Default.BackupGeneration = value;
            }
        }



        // TextWrapping
        private bool _textWrapping = false;
        public System.Windows.TextWrapping TextWrapping {
            get {
                _textWrapping = Properties.Settings.Default.TextWrapping;
                return _textWrapping ? System.Windows.TextWrapping.Wrap : System.Windows.TextWrapping.NoWrap;
            }
            set {
                _textWrapping = value == System.Windows.TextWrapping.Wrap;
                Properties.Settings.Default.TextWrapping = _textWrapping;
            }
        }
        // AutoTextWrapping
        private bool? _autoTextWrapping;
        public bool AutoTextWrapping {
            get {
                if (_autoTextWrapping == null) {
                    _autoTextWrapping = Properties.Settings.Default.AutoTextWrapping;
                }
                return _autoTextWrapping.Value;
            }
            set {
                _autoTextWrapping = value;
                // valueがTrueの場合はTextWrappingをWrapにする
                if (value) {
                    TextWrapping = System.Windows.TextWrapping.Wrap;
                }
                Properties.Settings.Default.AutoTextWrapping = value;
            }
        }

        // IgnoreLineCount
        private int _ignoreLineCount = -1;
        public int IgnoreLineCount {
            get {
                if (_ignoreLineCount == -1) {
                    _ignoreLineCount = Properties.Settings.Default.IgnoreLineCount;
                }
                return _ignoreLineCount;
            }
            set {
                _ignoreLineCount = value;
                Properties.Settings.Default.IgnoreLineCount = value;
            }
        }
        #region 開発中機能関連の設定
        // UseSpacy
        private bool? _useSpacy;
        public bool UseSpacy {
            get {
                if (_useSpacy == null) {
                    _useSpacy = Properties.Settings.Default.UseSpacy;
                }
                return _useSpacy.Value;
            }
            set {
                _useSpacy = value;
                Properties.Settings.Default.UseSpacy = value;
            }
        }
        // SpacyModel
        private string? _spacyModel;
        public string SpacyModel {
            get {
                if (_spacyModel == null) {
                    _spacyModel = Properties.Settings.Default.SpacyModel;
                }
                return _spacyModel;
            }
            set {
                _spacyModel = value;
                Properties.Settings.Default.SpacyModel = value;
            }
        }
        // TesseractExePath
        private string? _tesseractExePath;
        public string TesseractExePath {
            get {
                if (_tesseractExePath == null) {
                    _tesseractExePath = Properties.Settings.Default.TesseractExePath;
                }
                return _tesseractExePath;
            }
            set {
                _tesseractExePath = value;
                Properties.Settings.Default.TesseractExePath = value;
            }
        }

        // AutoTag
        private bool? _autoTag;
        public bool AutoTag {
            get {
                if (_autoTag == null) {
                    _autoTag = Properties.Settings.Default.AutoTag;
                }
                return _autoTag.Value;
            }
            set {
                _autoTag = value;
                Properties.Settings.Default.AutoTag = value;
            }
        }

        // AutoDescription
        private bool? _autoDescription;
        public bool AutoDescription {
            get {
                if (_autoDescription == null) {
                    _autoDescription = Properties.Settings.Default.AutoDescription;
                }
                return _autoDescription.Value;
            }
            set {
                _autoDescription = value;
                Properties.Settings.Default.AutoDescription = value;
            }
        }

        // 自動的にTasksを生成する
        private bool? _autoGenerateTasks;
        public bool AutoGenerateTasks {
            get {
                if (_autoGenerateTasks == null) {
                    _autoGenerateTasks = Properties.Settings.Default.AutoGenerateTasks;
                }
                return _autoGenerateTasks.Value;
            }
            set {
                _autoGenerateTasks = value;
                Properties.Settings.Default.AutoGenerateTasks = value;
            }
        }

        // ProxyURL
        private string? _proxyURL;
        public string ProxyURL {
            get {
                if (_proxyURL == null) {
                    _proxyURL = Properties.Settings.Default.ProxyURL;
                }
                return _proxyURL;
            }
            set {
                _proxyURL = value;
                Properties.Settings.Default.ProxyURL = value;
            }
        }
        // NoProxyList
        private string? _noProxyList;
        public string NoProxyList {
            get {
                if (_noProxyList == null) {
                    _noProxyList = Properties.Settings.Default.NoProxyList;
                }
                return _noProxyList;
            }
            set {
                _noProxyList = value;
                Properties.Settings.Default.NoProxyList = value;
            }
        }
        // AutoDocumentReliabilityCheck
        private bool? _autoDocumentReliabilityCheck;
        public bool AutoDocumentReliabilityCheck {
            get {
                if (_autoDocumentReliabilityCheck == null) {
                    _autoDocumentReliabilityCheck = Properties.Settings.Default.AutoDocumentReliabilityCheck;
                }
                return _autoDocumentReliabilityCheck.Value;
            }
            set {
                _autoDocumentReliabilityCheck = value;
                Properties.Settings.Default.AutoDocumentReliabilityCheck = value;
            }
        }

        // ShowProperties
        private bool? _showProperties;
        public bool ShowProperties {
            get {
                if (_showProperties == null) {
                    _showProperties = Properties.Settings.Default.ShowProperties;
                }
                return _showProperties.Value;
            }
            set {
                _showProperties = value;
                Properties.Settings.Default.ShowProperties = value;
            }
        }
        // APIServerURL
        private string? _apiServerURL;
        public string APIServerURL {
            get {
                if (_apiServerURL == null) {
                    _apiServerURL = Properties.Settings.Default.APIServerURL;
                }
                return _apiServerURL;
            }
            set {
                _apiServerURL = value;
                Properties.Settings.Default.APIServerURL = value;
            }
        }
        // UseInternalAPI
        private bool? _useInternalAPI;
        public bool UseInternalAPI {
            get {
                if (_useInternalAPI == null) {
                    _useInternalAPI = Properties.Settings.Default.UseInternalAPI;
                }
                return _useInternalAPI.Value;
            }
            set {
                _useInternalAPI = value;
                Properties.Settings.Default.UseInternalAPI = value;
            }
        }
        // UseAPI
        private bool? _useExternalAPI;
        public bool UseExternalAPI {
            get {
                if (_useExternalAPI == null) {
                    _useExternalAPI = Properties.Settings.Default.UseExternalAPI;
                }
                return _useExternalAPI.Value;
            }
            set {
                _useExternalAPI = value;
                Properties.Settings.Default.UseExternalAPI = value;
            }
        }

        #endregion


        public void Save() {
            Properties.Settings.Default.Save();

        }
        public void Reload() {
            Properties.Settings.Default.Reload();
        }
        public OpenAIProperties CreateOpenAIProperties() {
            OpenAIProperties openAIProperties = new() {
                OpenAIKey = OpenAIKey,
                OpenAICompletionModel = OpenAICompletionModel,
                OpenAIEmbeddingModel = OpenAIEmbeddingModel,
                AzureOpenAI = AzureOpenAI,
            };
            if (string.IsNullOrEmpty(Properties.Settings.Default.AzureOpenAIAPIVersion) == false && AzureOpenAI) {
                openAIProperties.AzureOpenAIAPIVersion = Properties.Settings.Default.AzureOpenAIAPIVersion;
            }

            if (string.IsNullOrEmpty(Properties.Settings.Default.AzureOpenAIEndpoint) == false && AzureOpenAI) {
                openAIProperties.AzureOpenAIEndpoint = Properties.Settings.Default.AzureOpenAIEndpoint;
            }

            if (Properties.Settings.Default.OpenAIBaseURL != "") {
                openAIProperties.OpenAIBaseURL = Properties.Settings.Default.OpenAIBaseURL;
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
