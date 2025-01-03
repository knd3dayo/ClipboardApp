using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.VectorDB;

namespace PythonAILib.PythonIF {
    public partial interface IPythonAIFunctions {

        public string ExtractFileToText(string path);

        public string ExtractBase64ToText(string base64, string extension);

        public ChatResult OpenAIChat(ChatRequestContext chatRequestContext, ChatRequest chatController);

        public ChatResult LangChainChat(ChatRequestContext chatRequestContext, ChatRequest chatRequest);

        public ChatResult AutoGenGroupChat(ChatRequestContext chatRequestContext, ChatRequest chatRequest, Action<string> iteration);

        public ChatResult AutoGenNormalChat(ChatRequestContext chatRequestContext, ChatRequest chatRequest, Action<string> iteration);

        public ChatResult AutoGenNestedChat(ChatRequestContext chatRequestContext, ChatRequest chatRequest, Action<string> iteration);



        // AutoGenのデフォルトの設定を取得する
        public Dictionary<string, dynamic?> GetAutoGenDefaultSettings(ChatRequestContext chatRequestContext);


        public List<VectorDBEntry> VectorSearch(ChatRequestContext chatRequestContext, VectorSearchRequest request);

        public void DeleteVectorDBCollection(ChatRequestContext chatRequestContext);

        public void DeleteVectorDBIndex(ChatRequestContext chatRequestContext, VectorDBEntry vectorDBEntry);

        public void UpdateVectorDBIndex(ChatRequestContext chatRequestContext, VectorDBEntry vectorDBEntry);

        // 引数として渡されたList<List<string>>の文字列をExcelファイルに出力する
        public void ExportToExcel(string filePath, CommonDataTable data);

        // 引数として渡されたExcelファイルを読み込んでList<List<string>>に変換して返す
        public CommonDataTable ImportFromExcel(string filePath);

        // GetMimeType
        public string GetMimeType(string filePath);

        //テスト用
        public string HelloWorld();

    }
}
