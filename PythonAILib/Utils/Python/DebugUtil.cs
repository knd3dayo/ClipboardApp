using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.VectorDB;

namespace PythonAILib.Utils.Python {
    public class DebugUtil {

        private static readonly JsonSerializerOptions options = new() {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public static string DebugRequestParametersFile {
            get {
                return Path.Combine(PythonAILibManager.Instance.ConfigParams.GetAppDataPath(), "debug_request_parameters.json");
            }
        }

        // ChatRequestの内容からPythonスクリプトを実行するコマンド文字列を生成する。
        public static List<string> GetPythonScriptCommand(string pythonScriptName, string pythonScriptArgs, string beforeExecScriptCommands = "", string afterExecScriptCommands = "") {
            List<string> cmdLines = [];
            // アプリケーションが格納されたディレクトリ
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            // python_ai_libのディレクトリ
            string pythonAILibDir = Path.Combine(appDir, "python_ai_lib");

            // 文字コードをUTF-8に設定
            cmdLines.Add("chcp 65001");
            // venvが有効な場合は、activate.batを実行
            string venvPath = PythonAILibManager.Instance.ConfigParams.GetPathToVirtualEnv();
            if (!string.IsNullOrEmpty(venvPath)) {
                string venvActivateScript = Path.Combine(venvPath, "Scripts", "activate");
                cmdLines.Add($"call {venvActivateScript}");
            }
            // 事前処理
            cmdLines.Add(beforeExecScriptCommands);
            cmdLines.Add($"cd {pythonAILibDir}");
            cmdLines.Add($"python {pythonScriptName} {pythonScriptArgs}");
            // 事後処理
            cmdLines.Add(afterExecScriptCommands);
            cmdLines.Add("\n");

            return cmdLines;
        }

        // ChatRequestの内容とList<VectorDBItem>からパラメーターファイルを作成する。
        public static string CreateParameterJsonFile(OpenAIProperties openAIProperties, List<VectorDBItem> vectorDBItems, ChatRequest? chatRequest) {
            // JSONファイルに保存
            string parametersJson = CreateParameterJson(openAIProperties, vectorDBItems, chatRequest);
            string parametersJsonFile = Path.GetTempFileName();
            File.WriteAllText(parametersJsonFile, parametersJson);

            return parametersJsonFile;
        }

        // ChatRequestの内容とList<VectorDBItem>からパラメーターJsonを作成する。
        public static string CreateParameterJson(OpenAIProperties openAIProperties, List<VectorDBItem> vectorDBItems, ChatRequest? chatRequest) {
            Dictionary<string, object> parametersDict = [];
            // OpenAIPropertiesをDictionaryに保存
            parametersDict["open_ai_props"] = openAIProperties;
            // VectorDBItemsをDictionaryに保存
            parametersDict["vector_db_props"] = vectorDBItems;
            // ChatRequestをDictionaryに保存
            if (chatRequest != null) {
                parametersDict["chat_request"] = chatRequest.ToDict();
            }
            string parametersJson = JsonSerializer.Serialize(parametersDict, options);
            return parametersJson;

        }

        public static List<string> CreateAutoGenGroupChatTest1CommandLine(string message, string parametersJsonFile, string? outputFile) {

            // 事前コマンド デバッグ用に、notepadでパラメーターファイルを開く
            string beforeExecScriptCommands = "notepad " + parametersJsonFile;
            // 事後コマンド pauseで一時停止
            string afterExecScriptCommands = "pause";
            string options;
            if (string.IsNullOrEmpty(outputFile)) {
                options = $"-m {message} -p {parametersJsonFile}";
            } else {
                options = $"-m {message} -p {parametersJsonFile} -o {outputFile}";
            }
            List<string> cmdLines = DebugUtil.GetPythonScriptCommand("test_ai_app_autogen_group_chat_01.py", $"{options}",
                beforeExecScriptCommands, afterExecScriptCommands);

            return cmdLines;
        }
        // OpenAIチャットを実行するコマンド文字列を生成する。
        public static List<string> CreateOpenAIChatCommandLine(string parametersJsonFile) {
            // 事前コマンド デバッグ用に、notepadでパラメーターファイルを開く
            string beforeExecScriptCommands = "notepad " + parametersJsonFile;
            // 事後コマンド pauseで一時停止
            string afterExecScriptCommands = "pause";
            string options = $"-p {parametersJsonFile}";
            List<string> cmdLines = DebugUtil.GetPythonScriptCommand("test_ai_app_open_ai_chat_01.py", $"{options}", beforeExecScriptCommands, afterExecScriptCommands);


            return cmdLines;
        }
        // LangChainチャットを実行するコマンド文字列を生成する。
        public static List<string> CreateLangChainChatCommandLine(string parametersJsonFile) {
            // 事前コマンド デバッグ用に、notepadでパラメーターファイルを開く
            string beforeExecScriptCommands = "notepad " + parametersJsonFile;
            // 事後コマンド pauseで一時停止
            string afterExecScriptCommands = "pause";
            string options = $"-p {parametersJsonFile}";
            List<string> cmdLines = DebugUtil.GetPythonScriptCommand("test_ai_app_langchain_chat_01.py", $"{options}", beforeExecScriptCommands, afterExecScriptCommands);

            return cmdLines;
        }

        public static string ProcessAutoGenGroupChatResult(string responseJson) {
            StringBuilder sb = new();
            // responseJsonはJsonElementのリスト
            List<JsonElement> jsonElements = JsonSerializer.Deserialize<List<JsonElement>>(responseJson) ?? [];
            foreach (var jsonElement in jsonElements) {
                Dictionary<string, dynamic?>? dic = Common.JsonUtil.ParseJson(jsonElement.ToString());
                // role, name , contentを取得
                string role = dic?["role"] ?? "";
                string name = dic?["name"] ?? "";
                string content = dic?["content"] ?? "";
                // roleが[user]または[assistant]の場合は、role, name, contentをStringBuilderに追加
                if (role == "user" || role == "assistant") {
                    sb.Append($"{role} {name}:\n{content}\n");
                }
            }
            return sb.ToString();
        }
    }
}