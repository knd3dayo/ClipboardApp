using System.IO;
using ClipboardApp.Model.Folders.Browser;
using ClipboardApp.Model.Folders.Clipboard;
using ClipboardApp.Model.Folders.FileSystem;
using ClipboardApp.Model.Folders.Outlook;
using ClipboardApp.Model.Folders.Search;
using ClipboardApp.Model.Folders.ShortCut;
using LibPythonAI.Model.Content;
using LibUIPythonAI.Resource;
using PythonAILib.Common;
using PythonAILib.Model.Content;
using PythonAILib.Model.Folder;
using PythonAILib.Model.Search;

namespace ClipboardApp.Model.Main {
    public class FolderManager {

        public static readonly string CLIPBOARD_ROOT_FOLDER_NAME = CommonStringResources.Instance.Clipboard;
        public static readonly string SEARCH_ROOT_FOLDER_NAME = CommonStringResources.Instance.SearchFolder;
        public static readonly string CHAT_ROOT_FOLDER_NAME = CommonStringResources.Instance.ChatHistory;
        public static readonly string IMAGECHECK_ROOT_FOLDER_NAME = CommonStringResources.Instance.ImageChat;
        public static readonly string FILESYSTEM_ROOT_FOLDER_NAME = CommonStringResources.Instance.FileSystem;
        public static readonly string SHORTCUT_ROOT_FOLDER_NAME = CommonStringResources.Instance.Shortcut;
        public static readonly string OUTLOOK_ROOT_FOLDER_NAME = CommonStringResources.Instance.Outlook;
        public static readonly string EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME = CommonStringResources.Instance.EdgeBrowseHistory;
        public static readonly string RECENT_FILES_ROOT_FOLDER_NAME = CommonStringResources.Instance.RecentFiles;

        // 英語名
        public static readonly string CLIPBOARD_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.ClipboardEnglish;
        public static readonly string SEARCH_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.SearchFolderEnglish;
        public static readonly string CHAT_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.ChatHistoryEnglish;
        public static readonly string IMAGECHECK_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.ImageChatEnglish;
        public static readonly string FILESYSTEM_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.FileSystemEnglish;
        public static readonly string SHORTCUT_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.ShortcutEnglish;
        public static readonly string OUTLOOK_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.OutlookEnglish;
        public static readonly string EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.EdgeBrowseHistoryEnglish;
        public static readonly string RECENT_FILES_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.RecentFilesEnglish;




        #region static methods

        // 言語変更時にルートフォルダ名を変更する
        public static void ChangeRootFolderNames(CommonStringResources toRes) {

            // ClipboardRootFolder
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<ContentFolder>();
            ContentFolder? clipboardRootFolder = collection.Find(x => x.ParentId == null && x.FolderType == FolderTypeEnum.Normal).FirstOrDefault();

            if (clipboardRootFolder != null) {
                clipboardRootFolder.FolderName = toRes.Clipboard;
                clipboardRootFolder.Save();
            }
            var searchCollection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<ContentFolder>();
            // SearchRootFolder
            ContentFolder? searchRootFolder = searchCollection.Find(x => x.ParentId == null && x.FolderType == FolderTypeEnum.Search).FirstOrDefault();
            if (searchRootFolder != null) {
                searchRootFolder.FolderName = toRes.SearchFolder;
                searchRootFolder.Save();
            }
            // ChatRootFolder
            ContentFolder? chatRootFolder = collection.Find(x => x.ParentId == null && x.FolderType == FolderTypeEnum.Chat).FirstOrDefault();
            if (chatRootFolder != null) {
                chatRootFolder.FolderName = toRes.ChatHistory;
                chatRootFolder.Save();
            }
            // ImageCheckRootFolder
            ContentFolder? imageCheckRootFolder = collection.Find(x => x.ParentId == null && x.FolderType == FolderTypeEnum.ImageCheck).FirstOrDefault();
            if (imageCheckRootFolder != null) {
                imageCheckRootFolder.FolderName = toRes.ImageChat;
                imageCheckRootFolder.Save();
            }
            // FileSystemRootFolder
            var fileSystemCollection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<ContentFolder>();
            ContentFolder? fileSystemRootFolder = fileSystemCollection.Find(x => x.ParentId == null && x.FolderType == FolderTypeEnum.FileSystem).FirstOrDefault();
            if (fileSystemRootFolder != null) {
                fileSystemRootFolder.FolderName = toRes.FileSystem;
                fileSystemRootFolder.Save();
            }
            // ShortcutRootFolder
            var shortCutCollection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<ContentFolder>();
            ContentFolder? shortcutRootFolder = shortCutCollection.Find(x => x.ParentId == null && x.FolderType == FolderTypeEnum.ShortCut).FirstOrDefault();
            if (shortcutRootFolder != null) {
                shortcutRootFolder.FolderName = toRes.Shortcut;
                shortcutRootFolder.Save();
            }
            // EdgeBrowseHistoryRootFolder
            var edgeBrowseHistoryCollection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<ContentFolder>();
            ContentFolder? edgeBrowseHistoryRootFolder = edgeBrowseHistoryCollection.Find(x => x.ParentId == null && x.FolderTypeString == EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN).FirstOrDefault();
            if (edgeBrowseHistoryRootFolder != null) {
                edgeBrowseHistoryRootFolder.FolderName = toRes.EdgeBrowseHistory;
                edgeBrowseHistoryRootFolder.Save();
            }
            // RecentFilesRootFolder
            var recentFilesCollection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<ContentFolder>();
            ContentFolder? recentFilesRootFolder = recentFilesCollection.Find(x => x.ParentId == null && x.FolderTypeString == RECENT_FILES_ROOT_FOLDER_NAME_EN).FirstOrDefault();
            if (recentFilesRootFolder != null) {
                recentFilesRootFolder.FolderName = toRes.RecentFiles;
                recentFilesRootFolder.Save();
            }
        }

        //--------------------------------------------------------------------------------
        private static ClipboardFolder? clipboardRootFolder;
        public static ClipboardFolder RootFolder {
            get {
                if (clipboardRootFolder == null) {
                    var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<ContentFolder>();
                    ContentFolder? folder = collection.Find(x => x.ParentId == LiteDB.ObjectId.Empty && x.FolderType == FolderTypeEnum.Normal).FirstOrDefault();
                    if (folder == null) {
                        folder = new() {
                            FolderName = CLIPBOARD_ROOT_FOLDER_NAME,
                            IsRootFolder = true,
                            IsAutoProcessEnabled = true,
                            FolderType = FolderTypeEnum.Normal,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), CLIPBOARD_ROOT_FOLDER_NAME_EN)
                        };
                        folder.Save();
                    }
                    // 既にRootFolder作成済みの環境のための措置
                    folder.IsRootFolder = true;
                    folder.ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), CLIPBOARD_ROOT_FOLDER_NAME_EN);
                    folder.FolderTypeString = CLIPBOARD_ROOT_FOLDER_NAME_EN;
                    folder.Save();

                    clipboardRootFolder = new ClipboardFolder(folder);
                }
                return clipboardRootFolder;
            }
        }
        private static SearchFolder? searchRootFolder;
        public static SearchFolder SearchRootFolder {
            get {
                if (searchRootFolder == null) {
                    var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<ContentFolder>();
                    ContentFolder? folder = collection.Find(x => x.ParentId == LiteDB.ObjectId.Empty && x.FolderType == FolderTypeEnum.Search).FirstOrDefault();
                    if (folder == null) {
                        folder = new() {
                            FolderName = SEARCH_ROOT_FOLDER_NAME,
                            IsRootFolder = true,
                            IsAutoProcessEnabled = true,
                            FolderType = FolderTypeEnum.Search,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), SEARCH_ROOT_FOLDER_NAME_EN)
                        };
                        folder.Save();
                    }
                    // 既にRootFolder作成済みの環境のための措置
                    folder.IsRootFolder = true;
                    folder.ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), SEARCH_ROOT_FOLDER_NAME_EN);
                    folder.FolderTypeString = SEARCH_ROOT_FOLDER_NAME_EN;
                    folder.Save();
                    searchRootFolder = new SearchFolder(folder);
                }
                return searchRootFolder;
            }
        }
        private static ClipboardFolder? chatRootFolder;

        public static ClipboardFolder ChatRootFolder {
            get {
                if (chatRootFolder == null) {
                    var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<ContentFolder>();
                    ContentFolder? folder = collection.Find(x => x.ParentId == LiteDB.ObjectId.Empty && x.FolderType == FolderTypeEnum.Chat).FirstOrDefault();
                    if (folder == null) {
                        folder = new() {
                            FolderName = CHAT_ROOT_FOLDER_NAME,
                            IsRootFolder = true,
                            IsAutoProcessEnabled = true,
                            FolderType = FolderTypeEnum.Chat,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), CHAT_ROOT_FOLDER_NAME_EN)
                        };
                        folder.Save();
                    }
                    // 既にRootFolder作成済みの環境のための措置
                    folder.IsRootFolder = true;
                    folder.ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), CHAT_ROOT_FOLDER_NAME_EN);
                    folder.FolderTypeString = CHAT_ROOT_FOLDER_NAME_EN;
                    folder.Save();
                    chatRootFolder = new ClipboardFolder(folder);
                }
                return chatRootFolder;
            }
        }
        // Local File System Root Folder
        private static FileSystemFolder? fileSystemRootFolder;
        public static FileSystemFolder FileSystemRootFolder {
            get {
                if (fileSystemRootFolder == null) {
                    var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<ContentFolder>();
                    ContentFolder? folder = collection.Find(x => x.ParentId == LiteDB.ObjectId.Empty && x.FolderType == FolderTypeEnum.FileSystem).FirstOrDefault();
                    if (folder == null) {
                        folder = new() {
                            FolderName = FILESYSTEM_ROOT_FOLDER_NAME,
                            IsRootFolder = true,
                            IsAutoProcessEnabled = true,
                            FolderType = FolderTypeEnum.FileSystem,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), FILESYSTEM_ROOT_FOLDER_NAME_EN)
                        };
                        folder.Save();
                    }
                    // 既にRootFolder作成済みの環境のための措置
                    folder.IsRootFolder = true;
                    folder.ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), FILESYSTEM_ROOT_FOLDER_NAME_EN);
                    folder.FolderTypeString = FILESYSTEM_ROOT_FOLDER_NAME_EN;
                    folder.Save();
                    fileSystemRootFolder = new FileSystemFolder(folder);
                }
                return fileSystemRootFolder;
            }
        }
        // Shortcut Root Folder
        private static ShortCutFolder? shortcutRootFolder;
        public static ShortCutFolder ShortcutRootFolder {
            get {
                if (shortcutRootFolder == null) {
                    var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<ContentFolder>();
                    ContentFolder? folder = collection.Find(x => x.ParentId == LiteDB.ObjectId.Empty && x.FolderType == FolderTypeEnum.ShortCut).FirstOrDefault();
                    if (folder == null) {
                        folder = new() {
                            FolderName = SHORTCUT_ROOT_FOLDER_NAME,
                            FolderType = FolderTypeEnum.ShortCut,
                            IsRootFolder = true,
                            // 自動処理を無効にする
                            IsAutoProcessEnabled = false,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), SHORTCUT_ROOT_FOLDER_NAME_EN)
                        };
                        folder.Save();
                    }
                    // 既にSearchRootFolder作成済みの環境のための措置
                    folder.IsRootFolder = true;
                    folder.ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), SEARCH_ROOT_FOLDER_NAME_EN);
                    folder.FolderTypeString = SEARCH_ROOT_FOLDER_NAME_EN;
                    folder.Save();
                    shortcutRootFolder = new ShortCutFolder(folder);
                }
                return shortcutRootFolder;
            }
        }
        // Outlook Root Folder
        private static OutlookFolder? outlookRootFolder;
        public static OutlookFolder OutlookRootFolder {
            get {
                if (outlookRootFolder == null) {
                    var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<ContentFolder>();
                    ContentFolder? folder = collection.Find(x => x.ParentId == LiteDB.ObjectId.Empty && x.FolderType == FolderTypeEnum.Outlook).FirstOrDefault();
                    if (folder == null) {
                        folder = new() {
                            FolderName = OUTLOOK_ROOT_FOLDER_NAME,
                            FolderType = FolderTypeEnum.Outlook,
                            IsRootFolder = true,
                            // 自動処理を無効にする
                            IsAutoProcessEnabled = false,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), OUTLOOK_ROOT_FOLDER_NAME_EN)
                        };
                        folder.Save();
                    }
                    // 既にOutlookRootFolder作成済みの環境のための措置
                    folder.IsRootFolder = true;
                    folder.ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), OUTLOOK_ROOT_FOLDER_NAME_EN);
                    folder.FolderTypeString = OUTLOOK_ROOT_FOLDER_NAME_EN;
                    folder.Save();
                    outlookRootFolder = new OutlookFolder(folder);
                }
                return outlookRootFolder;
                #endregion
            }

        }
        // EdgeBrowseHistory Root Folder
        private static EdgeBrowseHistoryFolder? edgeBrowseHistoryRootFolder;

        public static EdgeBrowseHistoryFolder EdgeBrowseHistoryRootFolder {
            get {
                if (edgeBrowseHistoryRootFolder == null) {
                    var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<ContentFolder>();
                    ContentFolder? folder = collection.Find(x => x.ParentId == LiteDB.ObjectId.Empty && x.FolderTypeString == EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN).FirstOrDefault();
                    if (folder == null) {
                        folder = new() {
                            FolderName = EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME,
                            IsRootFolder = true,
                            // 自動処理を無効にする
                            IsAutoProcessEnabled = false,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN)
                        };
                        folder.Save();
                    }
                    // 既にOutlookRootFolder作成済みの環境のための措置
                    folder.IsRootFolder = true;
                    folder.ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN);
                    folder.FolderTypeString = EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN;
                    folder.Save();
                    edgeBrowseHistoryRootFolder = new EdgeBrowseHistoryFolder(folder);
                }
                return edgeBrowseHistoryRootFolder;
            }
        }

        // RecentFiles Root Folder
        private static RecentFilesFolder? recentFilesRootFolder;
        public static RecentFilesFolder RecentFilesRootFolder {
            get {
                if (recentFilesRootFolder == null) {
                    var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<ContentFolder>();
                    ContentFolder? folder = collection.Find(x => x.ParentId == LiteDB.ObjectId.Empty && x.FolderTypeString == RECENT_FILES_ROOT_FOLDER_NAME_EN).FirstOrDefault();
                    if (folder == null) {
                        folder = new() {
                            FolderName = RECENT_FILES_ROOT_FOLDER_NAME,
                            IsRootFolder = true,
                            // 自動処理を無効にする
                            IsAutoProcessEnabled = false,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), RECENT_FILES_ROOT_FOLDER_NAME_EN)
                        };
                        folder.Save();
                    }
                    // 既にOutlookRootFolder作成済みの環境のための措置
                    folder.IsRootFolder = true;
                    folder.ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), RECENT_FILES_ROOT_FOLDER_NAME_EN);
                    folder.FolderTypeString = RECENT_FILES_ROOT_FOLDER_NAME_EN;
                    folder.Save();
                    recentFilesRootFolder = new RecentFilesFolder(folder);
                }
                return recentFilesRootFolder;
            }
        }

    }
}