using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.VectorDB;

namespace PythonAILib.PythonIF {
    public partial interface IPythonAIFunctions {

        public string ExtractFileToText(string path);

        public string ExtractBase64ToText(string base64, string extension);

        public ChatResult OpenAIChat(ChatRequestContext chatRequestContext, ChatRequest chatRequest);

        public ChatResult AutoGenGroupChat(ChatRequestContext chatRequestContext, ChatRequest chatRequest, Action<string> iteration);

        public void CancelAutoGenChat(string sessionToken);
        public List<VectorMetadata> VectorSearch(ChatRequestContext chatRequestContext, string query);

        public void DeleteVectorDBCollection(ChatRequestContext chatRequestContext);

        public void DeleteEmbeddings(ChatRequestContext chatRequestContext);

        public void UpdateEmbeddings(ChatRequestContext chatRequestContext);

        // 引数として渡されたList<List<string>>の文字列をExcelファイルに出力する
        public void ExportToExcel(string filePath, CommonDataTable data);

        // 引数として渡されたExcelファイルを読み込んでList<List<string>>に変換して返す
        public CommonDataTable ImportFromExcel(string filePath);

        // GetMimeType
        public string GetMimeType(string filePath);

        // GetTokenCount
        public long GetTokenCount(ChatRequestContext chatRequestContext, string inputText);

        // extract_webpage
        public string ExtractWebPage(string url);

        //テスト用
        public string HelloWorld();

    }
}
