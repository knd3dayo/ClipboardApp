using System.Collections.ObjectModel;
using LibUIPythonAI.ViewModel;
using QAChat.ViewModel.Folder;
using QAChat.ViewModel.Item;
using WpfAppCommon.Utils;

namespace MergeChat.ViewModel {
    public class MergeChatWindowViewModel : ChatViewModelBase {

        //初期化
        public MergeChatWindowViewModel(ContentFolderViewModel folderViewModel, ObservableCollection<ContentItemViewModel> selectedItems) {
            // PythonAILibのLogWrapperのログ出力設定
            PythonAILib.Utils.Common.LogWrapper.SetActions(LogWrapper.Info, LogWrapper.Warn, LogWrapper.Error);

            MergeTargetPanelViewModel mergeTargetPanelViewModel = new(folderViewModel, selectedItems, UpdateIndeterminate);

            // ChatControlViewModelを生成
            MergeChatControlViewModel = new(mergeTargetPanelViewModel);

        }
        // QAChatControlのViewModel
        public MergeChatControlViewModel MergeChatControlViewModel { get; set; }

    }
}
