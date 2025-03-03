using LibPythonAI.Model.Prompt;
using LiteDB;
using PythonAILib.Model.AutoGen;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Model.Content;
using PythonAILib.Model.Search;
using PythonAILib.Model.Statistics;
using PythonAILib.Model.Tag;
using PythonAILib.Model.VectorDB;

namespace PythonAILib.Common {
    public interface IDataFactory {

        // Database
        public LiteDatabase GetDatabase();

        //-- ContentItemInstance
        public ILiteCollection<T> GetItemCollection<T>() where T : ContentItem;

        // ContentFolder
        public ILiteCollection<T> GetFolderCollection<T>() where T : ContentFolder;

        // Prompt
        public ILiteCollection<T> GetPromptCollection<T>() where T : PromptItem;

        //----  RAGSourceItem
        public ILiteCollection<T> GetRAGSourceCollection<T>() where T : RAGSourceItem;

        //--- -  VectorDBItem
        public ILiteCollection<T> GetVectorDBCollection<T>() where T : VectorDBItem;

        // -- TagItem
        public ILiteCollection<T> GetTagCollection<T>() where T : TagItem;


        // --- Statistics
        public ILiteCollection<T> GetStatisticsCollection<T>() where T : MainStatistics;

        // -- SearchRule
        public ILiteCollection<SearchRule> GetSearchRuleCollection();

        // -- AutoProcessRule
        public ILiteCollection<AutoProcessRule> GetAutoProcessRuleCollection();

    }
}
