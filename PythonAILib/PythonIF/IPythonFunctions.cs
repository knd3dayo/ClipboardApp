using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.VectorDB;

namespace PythonAILib.PythonIF {
    public partial interface IPythonAIFunctions {

        public string ExtractFileToText(string path);

        public string ExtractBase64ToText(string base64, string extension);

        public ChatResult OpenAIChat(ChatRequestContext chatRequestContext, ChatRequest chatRequest);

        public ChatResult AutoGenGroupChat(ChatRequestContext chatRequestContext, ChatRequest chatRequest, Action<string> iteration);

        public List<VectorDBEntry> VectorSearch(ChatRequestContext chatRequestContext, string query);

        public void DeleteVectorDBCollection(ChatRequestContext chatRequestContext);

        public void UpdateVectorDBCollection(ChatRequestContext chatRequestContext);

        //　コレクション名とFolderIdにマッチするDescriptionをカタログから取得する。
        public string GetVectorDBDescription(string catalogDBURL, string vectorDBURL, string collectionName, string folder_id);

        // カタログ情報をアップデート
        public string UpdateVectorDBDescription(string catalogDBURL, string vectorDBURL, string collectionName, string folder_id, string Description);


        public void DeleteVectorDBIndex(ChatRequestContext chatRequestContext, VectorDBEntry vectorDBEntry);

        public void UpdateVectorDBIndex(ChatRequestContext chatRequestContext, VectorDBEntry vectorDBEntry);

        // 引数として渡されたList<List<string>>の文字列をExcelファイルに出力する
        public void ExportToExcel(string filePath, CommonDataTable data);

        // 引数として渡されたExcelファイルを読み込んでList<List<string>>に変換して返す
        public CommonDataTable ImportFromExcel(string filePath);

        // GetMimeType
        public string GetMimeType(string filePath);

        // GetTokenCount
        public long GetTokenCount(ChatRequestContext chatRequestContext, ChatRequest chatRequest);

        // extract_webpage
        public string ExtractWebPage(string url);

        //テスト用
        public string HelloWorld();

    }
}
