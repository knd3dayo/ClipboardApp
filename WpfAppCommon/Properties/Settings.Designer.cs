﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WpfAppCommon.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.9.0.0")]
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AzureOpenAI {
            get {
                return ((bool)(this["AzureOpenAI"]));
            }
            set {
                this["AzureOpenAI"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string OpenAIKey {
            get {
                return ((string)(this["OpenAIKey"]));
            }
            set {
                this["OpenAIKey"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("text-embedding-3-small")]
        public string OpenAIEmbeddingModel {
            get {
                return ((string)(this["OpenAIEmbeddingModel"]));
            }
            set {
                this["OpenAIEmbeddingModel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("gpt-4-1106-preview")]
        public string OpenAICompletionModel {
            get {
                return ((string)(this["OpenAICompletionModel"]));
            }
            set {
                this["OpenAICompletionModel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ja_core_news_sm")]
        public string SpacyModel {
            get {
                return ((string)(this["SpacyModel"]));
            }
            set {
                this["SpacyModel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int PythonExecution {
            get {
                return ((int)(this["PythonExecution"]));
            }
            set {
                this["PythonExecution"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Program Files\\Python311\\python311.dll")]
        public string PythonDllPath {
            get {
                return ((string)(this["PythonDllPath"]));
            }
            set {
                this["PythonDllPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("7")]
        public int BackupGeneration {
            get {
                return ((int)(this["BackupGeneration"]));
            }
            set {
                this["BackupGeneration"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoDescription {
            get {
                return ((bool)(this["AutoDescription"]));
            }
            set {
                this["AutoDescription"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoTag {
            get {
                return ((bool)(this["AutoTag"]));
            }
            set {
                this["AutoTag"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UserMaskedDataInOpenAI {
            get {
                return ((bool)(this["UserMaskedDataInOpenAI"]));
            }
            set {
                this["UserMaskedDataInOpenAI"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoMergeItemsBySourceApplicationTitle {
            get {
                return ((bool)(this["AutoMergeItemsBySourceApplicationTitle"]));
            }
            set {
                this["AutoMergeItemsBySourceApplicationTitle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string MonitorTargetAppNames {
            get {
                return ((string)(this["MonitorTargetAppNames"]));
            }
            set {
                this["MonitorTargetAppNames"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string OpenAICompletionBaseURL {
            get {
                return ((string)(this["OpenAICompletionBaseURL"]));
            }
            set {
                this["OpenAICompletionBaseURL"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UseOCR {
            get {
                return ((bool)(this["UseOCR"]));
            }
            set {
                this["UseOCR"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string OpenAIEmbeddingBaseURL {
            get {
                return ((string)(this["OpenAIEmbeddingBaseURL"]));
            }
            set {
                this["OpenAIEmbeddingBaseURL"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UseOpenAI {
            get {
                return ((bool)(this["UseOpenAI"]));
            }
            set {
                this["UseOpenAI"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string AzureOpenAIEndpoint {
            get {
                return ((string)(this["AzureOpenAIEndpoint"]));
            }
            set {
                this["AzureOpenAIEndpoint"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UseSpacy {
            get {
                return ((bool)(this["UseSpacy"]));
            }
            set {
                this["UseSpacy"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Program Files\\Tesseract-OCR\\tesseract.exe")]
        public string TesseractExePath {
            get {
                return ((string)(this["TesseractExePath"]));
            }
            set {
                this["TesseractExePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool SyncClipboardItemAndOSFolder {
            get {
                return ((bool)(this["SyncClipboardItemAndOSFolder"]));
            }
            set {
                this["SyncClipboardItemAndOSFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SyncFolderName {
            get {
                return ((string)(this["SyncFolderName"]));
            }
            set {
                this["SyncFolderName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoCommit {
            get {
                return ((bool)(this["AutoCommit"]));
            }
            set {
                this["AutoCommit"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool CompactViewMode {
            get {
                return ((bool)(this["CompactViewMode"]));
            }
            set {
                this["CompactViewMode"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool PreviewMode {
            get {
                return ((bool)(this["PreviewMode"]));
            }
            set {
                this["PreviewMode"] = value;
            }
        }
    }
}
