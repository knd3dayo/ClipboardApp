using System.Collections.ObjectModel;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;

namespace LibUIMergeChat.ViewModel {
    public class MergeChatWindowViewModel : ChatViewModelBase {

        //初期化
        public MergeChatWindowViewModel(ContentFolderViewModel folderViewModel, ObservableCollection<ContentItemViewModel> selectedItems) {
            // PythonAILibのLogWrapperのログ出力設定
            LogWrapper.SetActions(new LogWrapperAction());

            MergeTargetPanelViewModel mergeTargetPanelViewModel = new(folderViewModel, selectedItems, UpdateIndeterminate);

            // ChatControlViewModelを生成
            MergeChatControlViewModel = new(mergeTargetPanelViewModel);

        }

        public MergeChatControlViewModel MergeChatControlViewModel { get; set; }

    }
}
