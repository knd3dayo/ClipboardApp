using System.Reflection;
using ClipboardApp.Model.Folder;
using ClipboardApp.Settings;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Model.Content;
using QAChat.Resource;
using WK.Libraries.SharpClipboardNS;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace ClipboardApp.Common
{
    /// <summary>
    /// Class for clipboard monitoring feature
    /// </summary>
    public class ClipboardController {
        //--------------------------------------------------------------------------------
        // 最後に処理したクリップボードのEventArgs
        public static ClipboardChangedEventArgs? LastClipboardChangedEventArgs { get; set; } = null;

        private static ClipboardController? _instance;
        public static ClipboardController Instance {
            get {
                _instance ??= new ClipboardController();
                return _instance;
            }
        }

        // コンストラクタ
        private ClipboardController() {
            // コンストラクタ
            _clipboard = new SharpClipboard();
            _clipboard.ClipboardChanged += ClipboardChanged;

        }
        // ClipboardChangedが呼ばれたときの処理
        public Action<ClipboardChangedEventArgs> OnClipboardChanged { get; set; } = (e) => { };

        // Clipboard monitoring enable/disable flag
        public bool IsClipboardMonitorEnabled { get; set; } = false;
        private SharpClipboard _clipboard;
        private Action<ContentItem> _afterClipboardChanged = (parameter) => { };

        /// <summary>
        /// Start clipboard monitoring
        /// </summary>
        /// <param name="afterClipboardChanged"></param>
        public void Start(Action<ContentItem> afterClipboardChanged) {
            _afterClipboardChanged = afterClipboardChanged;
            // Enable clipboard monitoring
            IsClipboardMonitorEnabled = true;
        }

        /// <summary>
        /// Stop clipboard monitoring
        /// </summary>
        public void Stop() {
            IsClipboardMonitorEnabled = false;
        }
        /// <summary>
        /// Copy the content of ContentItem to the clipboard
        /// </summary>
        /// <param name="item"></param>
        public void SetDataObject(ContentItem item) {
            // System.Windows.MessageBox.Show(item.SourcePath);

            IsClipboardMonitorEnabled = false;
            // If ContentType is Text, copy to clipboard
            if (item.ContentType == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text) {
                if (item.Content == null) {
                    return;
                }
                System.Windows.Clipboard.SetDataObject(item.Content);
            }
            // If ContentType is Files, copy files to clipboard
            else if (item.ContentType == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Files) {
                // FilePathの取得
                System.Collections.Specialized.StringCollection strings = [item.FilePath];
                // Stringsが空の場合は何もしない
                if (strings.Count == 0) {
                    return;
                }
                System.Windows.Clipboard.SetFileDropList(strings);
            }
            IsClipboardMonitorEnabled = true;
        }

        /// <summary>
        /// Processing when the content of the clipboard is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClipboardChanged(object? sender, ClipboardChangedEventArgs e) {
            LastClipboardChangedEventArgs = e;

            // このアプリケーションのクリップボード操作は無視
            var assembly = Assembly.GetExecutingAssembly().GetName();
            if (e.SourceApplication.Name == assembly.Name + ".exe") {
                return;
            }
            OnClipboardChanged(e);


            if (IsClipboardMonitorEnabled == false) {
                // System.Windows.MessageBox.Show("Clipboard monitor disabled");
                return;
            }
            // Determine if it is a target application for monitoring
            if (!IsMonitorTargetApp(e)) {
                return;
            }

            if (_clipboard == null) {
                return;
            }
            FolderManager.RootFolder.ProcessClipboardItem(e, _afterClipboardChanged);

        }

        /// <summary>
        /// Determine if the clipboard item is a monitoring target
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static bool IsMonitorTargetApp(ClipboardChangedEventArgs e) {
            // If MonitorTargetAppNames is not an empty string and does not contain in MonitorTargetAppNames, do not process
            if (ClipboardAppConfig.Instance.MonitorTargetAppNames != "") {
                // Compare uppercase letters
                string upperSourceApplication = e.SourceApplication.Name.ToUpper();
                string upperMonitorTargetAppNames = ClipboardAppConfig.Instance.MonitorTargetAppNames.ToUpper();
                if (!upperMonitorTargetAppNames.Contains(upperSourceApplication)) {
                    return false;
                }
            }
            return true;
        }

        /// Process clipboard item 
        /// <summary>
        /// Process clipboard item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="_afterClipboardChanged"></param>
        public static void ProcessClipboardItem(ContentItem item, Action<ContentItem> _afterClipboardChanged) {

            // Execute in a separate thread
            Task.Run(() => {
                StatusText statusText = Tools.StatusText;
                MainUITask.Run(() => {
                    statusText.InProgressText = CommonStringResources.Instance.AutoProcessing;
                    statusText.IsInProgress = true;
                });
                try {
                    // Apply automatic processing
                    Task<ContentItem?> updatedItemTask = AutoProcessRuleController.ApplyAutoAction(item);
                    if (updatedItemTask.Result == null) {
                        // If the item is ignored, return
                        return;
                    }
                    // Notify the completion of processing
                    _afterClipboardChanged(updatedItemTask.Result);

                } catch (Exception ex) {
                    LogWrapper.Error($"{CommonStringResources.Instance.AddItemFailed}\n{ex.Message}\n{ex.StackTrace}");

                } finally {
                    MainUITask.Run(() => {
                        statusText.IsInProgress = false;
                    });
                }
            });
        }

    }
}
