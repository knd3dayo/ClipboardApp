using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using PythonAILib.Common;
using PythonAILib.Model.Chat;

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
            // debug用のScriptのディレクトリ
            string debugScriptDir = Path.Combine(pythonAILibDir, "debug_tool");
            // Scriptのフルパス
            string pythonScriptPath = Path.Combine(debugScriptDir, pythonScriptName);
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
            cmdLines.Add($"set PYTHONPATH={pythonAILibDir}");
            cmdLines.Add($"python {pythonScriptPath} {pythonScriptArgs}");
            // 事後処理
            cmdLines.Add(afterExecScriptCommands);
            cmdLines.Add("\n");

            return cmdLines;
        }

        // ChatRequestの内容とList<VectorDBItem>からパラメーターファイルを作成する。
        public static string CreateParameterJsonFile(ChatRequestContext chatRequestContext, ChatRequest? chatRequest) {
            // JSONファイルに保存
            string parametersJson = CreateParameterJson(chatRequestContext, chatRequest);
            string parametersJsonFile = Path.GetTempFileName();
            File.WriteAllText(parametersJsonFile, parametersJson);

            return parametersJsonFile;
        }

        // ChatRequestの内容とList<VectorDBItem>からパラメーターJsonを作成する。
        public static string CreateParameterJson(ChatRequestContext chatRequestContext, ChatRequest? chatRequest) {
            Dictionary<string, object> parametersDict = [];
            parametersDict["context"] = chatRequestContext.ToDict();
            // ChatRequestをDictionaryに保存
            if (chatRequest != null) {
                parametersDict["request"] = chatRequest.ToDict();
            }

            string parametersJson = JsonSerializer.Serialize(parametersDict, options);
            return parametersJson;

        }

        // AutoGenNormalChatのテスト1を実行するコマンド文字列を生成する。
        public static List<string> CreateAutoGenNormalChatTest1CommandLine(string parametersJsonFile, string? outputFile) {
            // 事前コマンド デバッグ用に、notepadでパラメーターファイルを開く
            string beforeExecScriptCommands = "notepad " + parametersJsonFile + "\n" + "pause";
            // 事後コマンド pauseで一時停止
            string afterExecScriptCommands = "pause";
            string options = $"-p {parametersJsonFile}";
            List<string> cmdLines = DebugUtil.GetPythonScriptCommand("test_ai_app_autogen_normal_chat_01.py", $"{options}",
                               beforeExecScriptCommands, afterExecScriptCommands);

            return cmdLines;
        }

        // AutoGenGroupChatのテスト1を実行するコマンド文字列を生成する。
        public static List<string> CreateAutoGenGroupChatTest1CommandLine(string parametersJsonFile, string? outputFile) {

            // 事前コマンド デバッグ用に、notepadでパラメーターファイルを開く
            string beforeExecScriptCommands = "notepad " + parametersJsonFile + "\n" + "pause";
            // 事後コマンド pauseで一時停止
            string afterExecScriptCommands = "pause";
            string options = $"-p {parametersJsonFile}";
            List<string> cmdLines = DebugUtil.GetPythonScriptCommand("test_ai_app_autogen_group_chat_01.py", $"{options}",
                beforeExecScriptCommands, afterExecScriptCommands);

            return cmdLines;
        }
        // AutoGenNestedChatのテスト1を実行するコマンド文字列を生成する。
        public static List<string> CreateAutoGenNestedChatTest1CommandLine(string parametersJsonFile, string? outputFile) {
            // 事前コマンド デバッグ用に、notepadでパラメーターファイルを開く
            string beforeExecScriptCommands = "notepad " + parametersJsonFile + "\n" + "pause";
            // 事後コマンド pauseで一時停止
            string afterExecScriptCommands = "pause";
            string options = $"-p {parametersJsonFile}";
            List<string> cmdLines = DebugUtil.GetPythonScriptCommand("test_ai_app_autogen_nested_chat_01.py", $"{options}",
                               beforeExecScriptCommands, afterExecScriptCommands);

            return cmdLines;
        }


        // Chatを実行するコマンド文字列を生成する。
        public static string CreateChatCommandLine(ChatRequestContext chatRequestContext, ChatRequest chatRequest) {
            // ModeがNormalまたはOpenAIRAGの場合は、OpenAIChatを実行するコマンドを返す
            if (chatRequest.ChatMode == OpenAIExecutionModeEnum.Normal || chatRequest.ChatMode == OpenAIExecutionModeEnum.OpenAIRAG) {
                // パラメーターファイルを作成
                string parametersJson = DebugUtil.CreateParameterJson(chatRequestContext, chatRequest);
                File.WriteAllText(DebugUtil.DebugRequestParametersFile, parametersJson);
                return string.Join("\n\n", DebugUtil.CreateOpenAIChatCommandLine(DebugUtil.DebugRequestParametersFile));
            }
            // ModeがLangChainの場合は、LangChainChatを実行するコマンドを返す
            if (chatRequest.ChatMode == OpenAIExecutionModeEnum.LangChain) {
                // パラメーターファイルを作成
                string parametersJson = DebugUtil.CreateParameterJson(chatRequestContext, chatRequest);
                File.WriteAllText(DebugUtil.DebugRequestParametersFile, parametersJson);
                return string.Join("\n\n", DebugUtil.CreateLangChainChatCommandLine(DebugUtil.DebugRequestParametersFile));
            }
            // ModeがAutoGenの場合は、AutoGenのNormalChatを実行するコマンドを返す
            if (chatRequest.ChatMode == OpenAIExecutionModeEnum.AutoGenNormalChat) {
                // パラメーターファイルを作成
                string parametersJson = DebugUtil.CreateParameterJson(chatRequestContext, chatRequest);
                File.WriteAllText(DebugUtil.DebugRequestParametersFile, parametersJson);
                return string.Join("\n\n", DebugUtil.CreateAutoGenNormalChatTest1CommandLine(DebugUtil.DebugRequestParametersFile, null));
            }
            // ModeがAutoGenの場合は、AutoGenのGroupChatを実行するコマンドを返す
            if (chatRequest.ChatMode == OpenAIExecutionModeEnum.AutoGenGroupChat) {
                // パラメーターファイルを作成
                string parametersJson = DebugUtil.CreateParameterJson(chatRequestContext, chatRequest);
                File.WriteAllText(DebugUtil.DebugRequestParametersFile, parametersJson);

                return string.Join("\n\n", DebugUtil.CreateAutoGenGroupChatTest1CommandLine(DebugUtil.DebugRequestParametersFile, null));
            }
            // ModeがAutoGenの場合は、AutoGenのNestedChatを実行するコマンドを返す
            if (chatRequest.ChatMode == OpenAIExecutionModeEnum.AutoGenNestedChat) {
                // パラメーターファイルを作成
                string parametersJson = DebugUtil.CreateParameterJson(chatRequestContext, chatRequest);
                File.WriteAllText(DebugUtil.DebugRequestParametersFile, parametersJson);

                return string.Join("\n\n", DebugUtil.CreateAutoGenNestedChatTest1CommandLine(DebugUtil.DebugRequestParametersFile, null));
            }
            return "";
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
