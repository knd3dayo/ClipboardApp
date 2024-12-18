using PythonAILib.Model.Content;
using QAChat.Resource;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.ContentItemPanel {
    public abstract class ContentItemPanelViewModel(ContentItem contentItemBase) {

        public static CommonStringResources StringResources { get; } = CommonStringResources.Instance;
        public ContentItem ContentItem { get; set; } = contentItemBase;

        // 選択中のContentAttachedItemBase
        public ContentAttachedItem? SelectedFile { get; set; }

        // 選択中のContentItemBaseを開く
        public abstract void OpenContentItem();

        // 選択中のContentItemBaseを削除
        public abstract void RemoveContentItem();
        // OpenContentItemCommand
        public SimpleDelegateCommand<object> OpenSelectedItemCommand => new((parameter) => {
            OpenContentItem();
        });

        // RemoveSelectedItemCommand
        public SimpleDelegateCommand<object> RemoveSelectedItemCommand => new((parameter) => {
            RemoveContentItem();
        });
    }
}
