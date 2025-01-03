using ClipboardApp.Factory;
using PythonAILib.Model.Folder;
using PythonAILib.Model.Search;
using QAChat.Resource;

namespace ClipboardApp.Model.Folder {
    public class FolderManager {

        public static readonly string CLIPBOARD_ROOT_FOLDER_NAME = CommonStringResources.Instance.Clipboard;
        public static readonly string SEARCH_ROOT_FOLDER_NAME = CommonStringResources.Instance.SearchFolder;
        public static readonly string CHAT_ROOT_FOLDER_NAME = CommonStringResources.Instance.ChatHistory;
        public static readonly string IMAGECHECK_ROOT_FOLDER_NAME = CommonStringResources.Instance.ImageChat;
        public static readonly string FILESYSTEM_ROOT_FOLDER_NAME = CommonStringResources.Instance.FileSystem;
        public static readonly string SHORTCUT_ROOT_FOLDER_NAME = CommonStringResources.Instance.Shortcut;
        public static readonly string OUTLOOK_ROOT_FOLDER_NAME = CommonStringResources.Instance.Outlook;


        #region static methods

        // 言語変更時にルートフォルダ名を変更する
        public static void ChangeRootFolderNames(CommonStringResources toRes) {

            // ClipboardRootFolder
            var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<ClipboardFolder>();
            ClipboardFolder? clipboardRootFolder = collection.Find(x => x.ParentId == null && x.FolderType == FolderTypeEnum.Normal).FirstOrDefault();
            if (clipboardRootFolder != null) {
                clipboardRootFolder.FolderName = toRes.Clipboard;
                clipboardRootFolder.Save();
            }
            var searchCollection = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<SearchFolder>();
            // SearchRootFolder
            SearchFolder? searchRootFolder = searchCollection.Find(x => x.ParentId == null && x.FolderType == FolderTypeEnum.Search).FirstOrDefault();
            if (searchRootFolder != null) {
                searchRootFolder.FolderName = toRes.SearchFolder;
                searchRootFolder.Save();
            }
            // ChatRootFolder
            ClipboardFolder? chatRootFolder = collection.Find(x => x.ParentId == null && x.FolderType == FolderTypeEnum.Chat).FirstOrDefault();
            if (chatRootFolder != null) {
                chatRootFolder.FolderName = toRes.ChatHistory;
                chatRootFolder.Save();
            }
            // ImageCheckRootFolder
            ClipboardFolder? imageCheckRootFolder = collection.Find(x => x.ParentId == null && x.FolderType == FolderTypeEnum.ImageCheck).FirstOrDefault();
            if (imageCheckRootFolder != null) {
                imageCheckRootFolder.FolderName = toRes.ImageChat;
                imageCheckRootFolder.Save();
            }
            // FileSystemRootFolder
            var fileSystemCollection = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<FileSystemFolder>();
            FileSystemFolder? fileSystemRootFolder = fileSystemCollection.Find(x => x.ParentId == null && x.FolderType == FolderTypeEnum.FileSystem).FirstOrDefault();
            if (fileSystemRootFolder != null) {
                fileSystemRootFolder.FolderName = toRes.FileSystem;
                fileSystemRootFolder.Save();
            }
            // ShortcutRootFolder
            var shortCutCollection = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<ShortCutFolder>();
            ShortCutFolder? shortcutRootFolder = shortCutCollection.Find(x => x.ParentId == null && x.FolderType == FolderTypeEnum.ShortCut).FirstOrDefault();
            if (shortcutRootFolder != null) {
                shortcutRootFolder.FolderName = toRes.Shortcut;
                shortcutRootFolder.Save();
            }
        }

        // アプリ共通の検索条件
        public static SearchRule GlobalSearchCondition { get; set; } = new();

        //--------------------------------------------------------------------------------
        private static ClipboardFolder? clipboardRootFolder;
        public static ClipboardFolder RootFolder {
            get {
                if (clipboardRootFolder == null) {
                    var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<ClipboardFolder>();
                    ClipboardFolder? folder = collection.Find(x => x.ParentId == LiteDB.ObjectId.Empty && x.FolderType == FolderTypeEnum.Normal).FirstOrDefault();
                    if (folder == null) {
                        folder = new() {
                            FolderName = CLIPBOARD_ROOT_FOLDER_NAME,
                            IsRootFolder = true,
                            IsAutoProcessEnabled = true,
                            FolderType = FolderTypeEnum.Normal
                        };
                        folder.Save();
                    }
                    // 既にRootFolder作成済みの環境のための措置
                    folder.IsRootFolder = true;
                    clipboardRootFolder = folder;
                }
                return clipboardRootFolder;
            }
        }
        private static SearchFolder? searchRootFolder;
        public static SearchFolder SearchRootFolder {
            get {
                if (searchRootFolder == null) {
                    var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<SearchFolder>();
                    SearchFolder? folder = collection.Find(x => x.ParentId == LiteDB.ObjectId.Empty && x.FolderType == FolderTypeEnum.Search).FirstOrDefault();
                    if (folder == null) {
                        folder = new() {
                            FolderName = SEARCH_ROOT_FOLDER_NAME,
                            IsRootFolder = true,
                            IsAutoProcessEnabled = true,
                            FolderType = FolderTypeEnum.Search
                        };
                        folder.Save();
                    }
                    // 既にRootFolder作成済みの環境のための措置
                    folder.IsRootFolder = true;
                    searchRootFolder = folder;
                }
                return searchRootFolder;
            }
        }
        private static ClipboardFolder? chatRootFolder;

        public static ClipboardFolder ChatRootFolder {
            get {
                if (chatRootFolder == null) {
                    var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<ClipboardFolder>();
                    ClipboardFolder? folder = collection.Find(x => x.ParentId == LiteDB.ObjectId.Empty && x.FolderType == FolderTypeEnum.Chat).FirstOrDefault();
                    if (folder == null) {
                        folder = new() {
                            FolderName = CHAT_ROOT_FOLDER_NAME,
                            IsRootFolder = true,
                            IsAutoProcessEnabled = true,
                            FolderType = FolderTypeEnum.Chat
                        };
                        folder.Save();
                    }
                    // 既にRootFolder作成済みの環境のための措置
                    folder.IsRootFolder = true;
                    chatRootFolder = folder;
                }
                return chatRootFolder;
            }
        }
        // Local File System Root Folder
        private static FileSystemFolder? fileSystemRootFolder;
        public static FileSystemFolder FileSystemRootFolder {
            get {
                if (fileSystemRootFolder == null) {
                    var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<FileSystemFolder>();
                    FileSystemFolder? folder = collection.Find(x => x.ParentId == LiteDB.ObjectId.Empty && x.FolderType == FolderTypeEnum.FileSystem).FirstOrDefault();
                    if (folder == null) {
                        folder = new() {
                            FolderName = FILESYSTEM_ROOT_FOLDER_NAME,
                            IsRootFolder = true,
                            IsAutoProcessEnabled = true,
                            FolderType = FolderTypeEnum.FileSystem
                        };
                        folder.Save();
                    }
                    // 既にRootFolder作成済みの環境のための措置
                    folder.IsRootFolder = true;
                    fileSystemRootFolder = folder;
                }
                return fileSystemRootFolder;
            }
        }
        // Shortcut Root Folder
        private static ShortCutFolder? shortcutRootFolder;
        public static ShortCutFolder ShortcutRootFolder {
            get {
                if (shortcutRootFolder == null) {
                    var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<ShortCutFolder>();
                    ShortCutFolder? folder = collection.Find(x => x.ParentId == LiteDB.ObjectId.Empty && x.FolderType == FolderTypeEnum.ShortCut).FirstOrDefault();
                    if (folder == null) {
                        folder = new ShortCutFolder {
                            FolderName = SHORTCUT_ROOT_FOLDER_NAME,
                            FolderType = FolderTypeEnum.ShortCut,
                            IsRootFolder = true,
                            // 自動処理を無効にする
                            IsAutoProcessEnabled = false
                        };
                        folder.Save();
                    }
                    // 既にSearchRootFolder作成済みの環境のための措置
                    folder.IsRootFolder = true;
                    shortcutRootFolder = folder;
                }
                return shortcutRootFolder;
            }
        }
        // Outlook Root Folder
        private static OutlookFolder? outlookRootFolder;
        public static OutlookFolder OutlookRootFolder {
            get {
                if (outlookRootFolder == null) {
                    var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<OutlookFolder>();
                    OutlookFolder? folder = collection.Find(x => x.ParentId == LiteDB.ObjectId.Empty && x.FolderType == FolderTypeEnum.Outlook).FirstOrDefault();
                    if (folder == null) {
                        folder = new OutlookFolder {
                            FolderName = OUTLOOK_ROOT_FOLDER_NAME,
                            FolderType = FolderTypeEnum.Outlook,
                            IsRootFolder = true,
                            // 自動処理を無効にする
                            IsAutoProcessEnabled = false
                        };
                        folder.Save();
                    }
                    // 既にOutlookRootFolder作成済みの環境のための措置
                    folder.IsRootFolder = true;
                    outlookRootFolder = folder;
                }
                return outlookRootFolder;
            }
        }
        #endregion

    }
}
