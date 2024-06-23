using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Windows.Controls;
using System.Windows.Media;
using Python.Runtime;
using PythonLib.Model;

namespace PythonLib.PythonIF {
    public enum SpacyEntityNames {

        PERSON,
        ORG,
        GPE,
        LOC,
        PRODUCT,
        EVENT,
        WORK_OF_ART,
        LAW,
        LANGUAGE,
        DATE,
        TIME,
        PERCENT,
        MONEY,
        QUANTITY,
        ORDINAL,
        CARDINAL
    }

    public enum GitFileStatus {
        Added,
        Modified,
        Deleted
    }
}

namespace PythonLib.PythonIF {
    public class PythonTask(Action action) : Task(action) {

        public CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();

    }
    public class PythonNetFunctions : IPythonFunctions {

        private readonly Dictionary<string, PyModule> PythonModules = [];


        private static StringResources StringResources { get; } = StringResources.Instance;

        public PyModule GetPyModule(string scriptPath) {
            if (PythonModules.TryGetValue(scriptPath, out PyModule? value)) {
                return value;
            }

            PyModule pyModule = Py.CreateScope();
            string script = PythonExecutor.LoadPythonScript(scriptPath);
            pyModule.Exec(script);
            // PythonModulesに、scriptPathが存在しない場合は追加
            PythonModules[scriptPath] = pyModule;

            return pyModule;
        }

        private void InitPythonNet(string pythonDLLPath) {
            // Pythonスクリプトを実行するための準備

            // 既に初期化されている場合は初期化しない
            if (PythonEngine.IsInitialized) {
                return;
            }

            // PythonDLLのパスを設定
            Runtime.PythonDLL = pythonDLLPath;

            // Runtime.PythonDLLのファイルが存在するかチェック
            if (!File.Exists(Runtime.PythonDLL)) {
                string message = StringResources.PythonDLLNotFound;
                Tools.Error(message + Runtime.PythonDLL);
                return;
            }

            try {
                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();

            } catch (TypeInitializationException e) {
                string message = StringResources.PythonInitFailed + e.Message;
                Tools.Error(message);
            }
        }

        public PythonNetFunctions(string pythonDLLPath) {
            InitPythonNet(pythonDLLPath);
        }

        public void ExecPythonScript(string scriptPath, Action<PyModule> action) {
            // Pythonスクリプトを実行する
            using (Py.GIL()) {
                // scriptPathからPyModuleを取得
                PyModule pyModule = GetPyModule(scriptPath);
                action(pyModule);
            }
        }

        public dynamic GetPythonFunction(PyModule ps, string function_name) {
            // Pythonスクリプトの関数を呼び出す
            dynamic? function_object = (ps?.Get(function_name))
                ?? throw new Exception(StringResources.FunctionNotFound(function_name));
            return function_object;
        }


        public static string CreatePythonExceptionMessage(PythonException e) {
            string pythonErrorMessage = e.Message;
            string message = StringResources.PythonExecuteError + "\n";
            if (pythonErrorMessage.Contains("No module named")) {
                message += StringResources.ModuleNotFound + "\n";
            }
            message += StringResources.PythonExecuteErrorDetail(e);
            return message;
        }

        // IPythonFunctionsのメソッドを実装
        public string ExtractText(string path) {
            // ResultContainerを作成
            string result = "";

            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "extract_text";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // extract_text関数を呼び出す
                try {
                    result = function_object(path);
                } catch (PythonException e) {
                    // エラーメッセージを表示 Unsupported file typeが含まれる場合は例外をスロー
                    if (e.Message.Contains("Unsupported file type")) {
                        throw new UnsupportedFileTypeException(e.Message);
                    }
                    throw;
                }
            });
            return result;
        }

        // IPythonFunctionsのメソッドを実装
        // データをマスキングする
        public string GetMaskedString(string SpacyModel, string text) {
            List<string> beforeTextList = [text];
            MaskedData maskedData = GetMaskedData(SpacyModel, beforeTextList);
            string result = maskedData.AfterTextList[0];
            return result;
        }

        // IPythonFunctionsのメソッドを実装
        // マスキングされたデータを元に戻す
        public string GetUnmaskedString(string SpacyModel, string maskedText) {
            List<string> beforeTextList = [maskedText];
            MaskedData maskedData = GetMaskedData(SpacyModel, beforeTextList);
            string result = maskedData.AfterTextList[0];
            return result;
        }

        public MaskedData GetMaskedData(string SpacyModel, List<string> beforeTextList) {

            // SPACY_MODEL_NAMEが空の場合は例外をスロー
            if (string.IsNullOrEmpty(SpacyModel)) {
                throw new Exception(StringResources.SpacyModelNameNotSet);
            }
            // mask_data関数を呼び出す. 引数としてTextとSPACY_MODEL_NAMEを渡す
            Dictionary<string, string> dict = new() {
                            { "SpacyModel", SpacyModel }
                        };

            MaskedData actionResult = new(beforeTextList);
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {

                // Pythonスクリプトの関数を呼び出す
                string function_name = "mask_data";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // 結果用のDictionaryを作成
                PyDict resultDict = new();
                resultDict = function_object(beforeTextList, dict);
                // resultDictが空の場合は例外をスロー
                if (resultDict == null || resultDict.Any() == false) {
                    throw new Exception(StringResources.MaskingResultNotFound);
                }
                PyObject? textDictObject = resultDict.GetItem("TEXT") ?? throw new Exception(StringResources.MaskingResultFailed);

                PyDict textDict = textDictObject.As<PyDict>();
                PyList? afterList = textDict.GetItem("AFTER").As<PyList>();
                foreach (PyObject item in afterList) {
                    string? text = item.ToString();
                    if (text == null) {
                        continue;
                    }
                    actionResult.AfterTextList.Add(text);
                }
                // SpacyEntitiesNames毎に処理
                foreach (SpacyEntityNames entityName in Enum.GetValues(typeof(SpacyEntityNames))) {
                    string entityNameString = entityName.ToString();
                    PyObject? entities;
                    try {
                        entities = resultDict.GetItem(entityNameString);
                    } catch (PythonException) {
                        entities = null;
                        return;
                    }
                    PyDict entityDict = entities.As<PyDict>();
                    List<MaskedEntity> maskedEntities = GetMaskedEntities(entityNameString, entityDict);
                    actionResult.Entities.UnionWith(maskedEntities);
                }

            });
            return actionResult;
        }

        // GetUnMaskedDataの実装
        public MaskedData GetUnMaskedData(string SpacyModel, List<string> maskedTextList) {

            // mask_data関数を呼び出す. 引数としてTextとSPACY_MODEL_NAMEを渡す
            Dictionary<string, string> dict = new() {
                            { "SpacyModel", SpacyModel }
                        };
            MaskedData actionResult = new(maskedTextList);
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "unmask_data";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // 結果用のDictionaryを作成
                PyDict resultDict = new();
                resultDict = function_object(actionResult, dict);
                // resultDictが空の場合は例外をスロー
                if (resultDict == null || resultDict.Any() == false) {
                    throw new Exception(StringResources.UnmaskingResultNotFound);
                }

                PyObject? textListObject = resultDict.GetItem("TEXT") ?? throw new Exception(StringResources.UnmaskingResultFailed);
                PyList textList = textListObject.As<PyList>();
                foreach (PyObject item in textList) {
                    PyObject afterTextObject = item.GetItem("AFTER");
                    string? text = afterTextObject.ToString();
                    if (text == null) {
                        continue;
                    }
                    actionResult.AfterTextList.Add(text);
                }
                // SpacyEntitiesNames毎に処理
                foreach (SpacyEntityNames entityName in Enum.GetValues(typeof(SpacyEntityNames))) {
                    string entityNameString = entityName.ToString();
                    PyObject? entities = resultDict.GetItem(entityNameString);
                    if (entities == null) {
                        continue;
                    }
                    PyDict entityDict = entities.As<PyDict>();
                    List<MaskedEntity> maskedEntities = GetMaskedEntities(entityNameString, entityDict);
                    actionResult.Entities.UnionWith(maskedEntities);
                }

            });
            return actionResult;
        }
        public string ExtractTextFromImage(Image image, string tesseractExePath) {
            // Pythonスクリプトを実行する
            string result = "";
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "extract_text_from_image";
                dynamic function_object = GetPythonFunction(ps, function_name);
                ImageSourceConverter imageConverter = new();
                object? bytesObject = imageConverter.ConvertTo(image, typeof(byte[]))
                ?? throw new Exception(StringResources.ImageByteFailed);
                byte[] bytes = (byte[])bytesObject;
                // extract_text_from_image関数を呼び出す。戻り値は{ "text": "抽出したテキスト" , "log": "ログ" }の形式
                Dictionary<string, string> dict = new();
                PyDict? pyDict = function_object(bytes, tesseractExePath);
                if (pyDict == null) {
                    throw new Exception("pyDict is null");
                }
                // textを取得
                PyObject? textObject = pyDict.GetItem("text");
                if (textObject == null) {
                    throw new Exception("textObject is null");
                }
                result = textObject.ToString() ?? "";
                // logを取得
                PyObject? logObject = pyDict.GetItem("log");
                if (logObject != null) {
                    string log = logObject.ToString() ?? "";
                    Tools.Info($"log:{log}");
                }

            });
            return result;
        }

        private List<MaskedEntity> GetMaskedEntities(string label, PyDict pyDict) {

            List<MaskedEntity> result = [];
            foreach (var key in pyDict.Keys()) {
                PyObject? entity = pyDict.GetItem(key);
                if (entity == null) {
                    continue;
                }
                string? keyString = key.ToString();
                if (keyString == null) {
                    continue;
                }
                string? entityString = entity.ToString();
                if (entityString == null) {
                    continue;
                }
                MaskedEntity maskedEntity = new() {
                    Label = label,
                    Before = keyString,
                    After = entityString
                };
                result.Add(maskedEntity);
            }
            return result;
        }


        // IPythonFunctionsのメソッドを実装
        // スクリプトの内容とJSON文字列を引数に取り、結果となるJSON文字列を返す
        public string RunScript(string script, string input) {
            string resultString = "";
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {

                // Pythonスクリプトの関数を呼び出す
                string function_name = "run_script";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // run_script関数を呼び出す
                resultString = function_object(script, input);
            });
            return resultString;

        }

        // IPythonFunctionsのメソッドを実装
        public HashSet<string> ExtractEntity(string SpacyModel, string text) {

            HashSet<string> actionResult = [];
            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {

                // SPACY_MODEL_NAMEが空の場合は例外をスロー
                if (string.IsNullOrEmpty(SpacyModel)) {
                    throw new Exception(StringResources.SpacyModelNameNotSet);
                }

                Dictionary<string, string> dict = new() {
                            { "SpacyModel", SpacyModel }
                        };
                // 結果用のDictionaryを作成
                // Pythonスクリプトの関数を呼び出す
                string function_name = "extract_entity";
                dynamic function_object = GetPythonFunction(ps, function_name);
                PyIterable pyIterable = function_object(text, dict);
                // PythonのリストをC#のHashSetに変換
                foreach (PyObject item in pyIterable) {
                    string? entity = item.ToString();
                    if (entity != null) {
                        actionResult.Add(entity);
                    }
                }
            });
            return actionResult;
        }

        public void OpenAIEmbedding(string text) {

            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "openai_embedding";
                dynamic function_object = GetPythonFunction(ps, function_name);
                string propsJson = JsonSerializer.Serialize(ClipboardAppConfig.CreateOpenAIProperties());
                // open_ai_chat関数を呼び出す
                function_object(text, propsJson);
                // System.Windows.MessageBox.Show(result);
            });
        }

        private ChatResult OpenAIChatExecute(string function_name, Func<dynamic, string> pythonFunction) {
            // ChatResultを作成
            ChatResult chatResult = new();
            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic function_object = GetPythonFunction(ps, function_name);

                // run_openai_chat関数を呼び出す。戻り値は{ "content": "レスポンス" , "log": "ログ" }の形式のJSON文字列
                string resultString = pythonFunction(function_object);

                // resultStringをログに出力
                Tools.Info($"レスポンス:{resultString}");

                // JSON文字列からDictionaryに変換する。
                var op = new JsonSerializerOptions {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true
                };
                Dictionary<string, object>? resultDict = JsonSerializer.Deserialize<Dictionary<string, object>>(resultString, op);
                if (resultDict == null) {
                    throw new Exception(StringResources.OpenAIResponseEmpty);
                }
                // contentを取得
                string? content = resultDict["content"]?.ToString();
                if (content == null) {
                    throw new Exception(StringResources.OpenAIResponseEmpty);
                }
                // ChatResultに設定
                chatResult.Response = content;

            });
            return chatResult;

        }

        public ChatResult OpenAIChat(Dictionary<string, string> props, string requestJson) {
            string propsJson = JsonSerializer.Serialize(props);

            Tools.Info("OpenAI実行");
            Tools.Info($"プロパティ情報 {propsJson}");
            Tools.Info($"リクエスト:{requestJson}");

            // OpenAIChatExecuteを呼び出す
            return OpenAIChatExecute("run_openai_chat", (function_object) => {
                return function_object(propsJson, requestJson);
            });

        }

        public ChatResult OpenAIChat(string prompt, IEnumerable<ChatItem> chatHistory) {
            return OpenAIChat(prompt, chatHistory, ClipboardAppConfig.CreateOpenAIProperties());
        }


        // 通常のOpenAIChatを実行する
        public ChatResult OpenAIChat(string prompt, IEnumerable<ChatItem> chatHistory, Dictionary<string, string> props) {

            // promptからChatItemを作成
            ChatItem chatItem = new(ChatItem.UserRole, prompt);
            // chatHistoryをコピーしてChatItemを追加
            List<ChatItem> chatHistoryList = new(chatHistory) {
                chatItem
            };
            string chat_history_json = ChatItem.ToJson(chatHistoryList);
            string propsJson = JsonSerializer.Serialize(props);

            Tools.Info("OpenAI実行");
            Tools.Info($"プロンプト:{prompt}");
            Tools.Info($"チャット履歴:{chat_history_json}");
            Tools.Info($"プロパティ情報 {propsJson}");

            //OpenAIChatExecuteを呼び出す
            return OpenAIChatExecute("openai_chat", (function_object) => {
                return function_object(propsJson, chat_history_json);
            });
        }

        // OpenAIChatWithVisionを実行する
        public ChatResult OpenAIChatWithVision(string prompt, IEnumerable<string> imageFileNames) {
            return OpenAIChatWithVision(prompt, imageFileNames, ClipboardAppConfig.CreateOpenAIProperties());
        }

        public ChatResult OpenAIChatWithVision(string prompt, IEnumerable<string> imageFileNames, Dictionary<string, string> props) {
            // propsをJSON文字列に変換
            string propsJson = JsonSerializer.Serialize(props);
            // ChatResultを作成
            ChatResult chatResult = new();
            // OpenAIChatExecuteを呼び出す
            return OpenAIChatExecute("openai_chat_with_vision", (function_object) => {
                return function_object(propsJson, prompt, imageFileNames);
            });
        }

        // テスト用
        public string HelloWorld() {
            string result = "";
            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "hello_world";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // hello_world関数を呼び出す
                result = function_object();

            });
            return result;
        }
        private void UpdateVectorDBIndexExecute(string function_name, Func<dynamic, string> pythonFunction) {
            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {

                // Pythonスクリプトの関数を呼び出す
                dynamic function_object = GetPythonFunction(ps, function_name);
                // update_vector_db_index関数を呼び出す
                try {
                    string resultString = pythonFunction(function_object);
                    Tools.Info(resultString);

                } catch (PythonException e) {
                    // エラーメッセージを表示 Unsupported file typeが含まれる場合は例外をスロー
                    if (e.Message.Contains("Unsupported file type")) {
                        throw new UnsupportedFileTypeException(e.Message);
                    }
                    Tools.Info(e.Message);
                    Tools.Info(e.StackTrace);
                    throw;
                }
            });
        }

        public void UpdateVectorDBIndex(IPythonFunctions.VectorDBUpdateMode mode, ClipboardItem item, VectorDBItem vectorDBItem) {

            // modeがUpdateでItem.Contentが空の場合は何もしない
            if (mode == IPythonFunctions.VectorDBUpdateMode.update && string.IsNullOrEmpty(item.Content)) {
                return;
            }
            // modeがDeleteで、Item.Id == ObjectId.Emptyの場合は何もしない
            if (mode == IPythonFunctions.VectorDBUpdateMode.delete && item.Id == LiteDB.ObjectId.Empty) {
                return;
            }
            // propsにVectorDBURLを追加
            var props = ClipboardAppConfig.CreateOpenAIProperties();
            props["VectorDBTypeString"] = vectorDBItem.VectorDBTypeString;
            props["VectorDBURL"] = vectorDBItem.VectorDBURL;
            props["CollectionName"] = item.FolderObjectId.ToString();
            props["DocStoreURL"] = vectorDBItem.DocStoreURL;
            props["IsUseMultiVectorRetriever"] = vectorDBItem.IsUseMultiVectorRetriever.ToString();

            Tools.Info("UpdateVectorDBIndex実行");
            Tools.Info($"VectorDBTypeString:{vectorDBItem.VectorDBTypeString}");
            Tools.Info($"VectorDBURL:{vectorDBItem.VectorDBURL}");
            Tools.Info($"DocStoreURL:{vectorDBItem.DocStoreURL}");
            Tools.Info($"IsUseMultiVectorRetriever:{vectorDBItem.IsUseMultiVectorRetriever}");

            // UpdateVectorDBIndexExecuteを呼び出す
            UpdateVectorDBIndexExecute("update_index_with_clipboard_item", (function_object) => {
                return function_object(props, mode.ToString(), item.Content, item.Id.ToString());
            });
        }
        public void UpdateVectorDBIndex(GitFileStatus fileStatus, string workingDirPath, string repositoryURL, VectorDBItem vectorDBItem) {

            // FileStatus.StatusがAdded、Modifiedのの場合は第1引数に"update"、Deletedの場合は"delete"を渡す
            // それ以外はなにもしない。
            string mode = "";
            if (fileStatus == GitFileStatus.Added || fileStatus == GitFileStatus.Modified) {
                mode = "update";
            } else if (fileStatus == GitFileStatus.Deleted) {
                mode = "delete";
            } else {
                return;
            }
            // workingDirPathとFileStatusのPathを結合する。ファイルが存在しない場合は例外をスロー
            string filePath = Path.Combine(workingDirPath, fileStatus.Path);
            if (!File.Exists(filePath)) {
                Tools.Info($"{StringResources.FileNotFound} : {filePath}");
            }
            // propsにVectorDBURLを追加
            var props = ClipboardAppConfig.CreateOpenAIProperties();
            props["VectorDBTypeString"] = vectorDBItem.VectorDBTypeString;
            props["VectorDBURL"] = vectorDBItem.VectorDBURL;
            props["DocStoreURL"] = vectorDBItem.DocStoreURL;
            props["IsUseMultiVectorRetriever"] = vectorDBItem.IsUseMultiVectorRetriever.ToString();

            // UpdateVectorDBIndexExecuteを呼び出す
            UpdateVectorDBIndexExecute("update_index", (function_object) => {
                return function_object(props, mode, workingDirPath, fileStatus.Path, repositoryURL);
            });
        }

        private ChatResult LangChainChatExecute(string functionName, Func<dynamic, string> pythonFunction) {
            ChatResult chatResult = new();

            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {
                string function_name = functionName;
                dynamic function_object = GetPythonFunction(ps, function_name);

                string resultString = pythonFunction(function_object);

                // resultStringをログに出力
                Tools.Info($"レスポンス:{resultString}");

                // JSON文字列からDictionaryに変換する。
                var op = new JsonSerializerOptions {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true
                };
                Dictionary<string, object>? resultDict = JsonSerializer.Deserialize<Dictionary<string, object>>(resultString, op);
                if (resultDict == null) {
                    throw new Exception(StringResources.OpenAIResponseEmpty);
                }
                // outputを取得
                string? output = resultDict["output"]?.ToString();
                if (output == null) {
                    throw new Exception(StringResources.OpenAIResponseEmpty);
                }
                // ChatResultに設定
                chatResult.Response = output;

                // page_content_listを取得
                List<Dictionary<string, string>> page_content_list = resultDict["page_content_list"] as List<Dictionary<string, string>> ?? new();
                chatResult.ReferencedContents = page_content_list;

                // page_source_listを取得
                List<string> page_source_list = resultDict["page_source_list"] as List<string> ?? new();

                chatResult.ReferencedFilePath = page_source_list;

            });
            return chatResult;

        }
        public ChatResult LangChainChat(Dictionary<string, string> props, IEnumerable<VectorDBItem> vectorDBItems, string request_json) {
            // Pythonスクリプトの関数を呼び出す
            ChatResult chatResult = new();

            // VectorDBItemsのサイズが0の場合は例外をスロー
            if (!vectorDBItems.Any()) {
                throw new Exception(StringResources.VectorDBItemsEmpty);
            }
            // VectorDBItemのリストをJSON文字列に変換
            string vectorDBItemsJson = VectorDBItem.ToJson(vectorDBItems);
            // propsをJSON文字列に変換
            var op = new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            string propsJson = JsonSerializer.Serialize(props, op);

            Tools.Info("LangChain実行");
            Tools.Info($"プロパティ情報 {propsJson}");
            Tools.Info($"ベクトルDB情報 {vectorDBItemsJson}");
            Tools.Info($"リクエスト:{request_json}");

            // LangChainChat関数を呼び出す
            chatResult = LangChainChatExecute("run_langchain_chat", (function_object) => {
                string resultString = function_object(propsJson, vectorDBItemsJson, request_json);
                return resultString;
            });

            return chatResult;
        }

        public ChatResult LangChainChat(IEnumerable<VectorDBItem> vectorDBItems, string prompt, IEnumerable<ChatItem> chatHistory) {
            return LangChainChat(ClipboardAppConfig.CreateOpenAIProperties(), vectorDBItems, prompt, chatHistory);
        }

        public ChatResult LangChainChat(Dictionary<string, string> props, IEnumerable<VectorDBItem> vectorDBItems, string prompt, IEnumerable<ChatItem> chatHistory) {
            // chatHistoryをJSON文字列に変換
            string chatItemsJSon = ChatItem.ToJson(chatHistory);
            // VectorDBItemsのサイズが0の場合は例外をスロー
            if (!vectorDBItems.Any()) {
                throw new Exception(StringResources.VectorDBItemsEmpty);
            }
            // VectorDBItemのリストをJSON文字列に変換
            string vectorDBItemsJson = VectorDBItem.ToJson(vectorDBItems);
            // propsをJSON文字列に変換
            string propsJson = JsonSerializer.Serialize(props);

            Tools.Info("LangChain実行");
            Tools.Info($"ベクトルDB情報 {vectorDBItemsJson}");
            Tools.Info($"プロパティ情報 {propsJson}");
            Tools.Info($"プロンプト:{prompt}");
            Tools.Info($"チャット履歴:{chatItemsJSon}");

            //LangChainChat関数を呼び出す
            ChatResult chatResult = LangChainChatExecute("langchain_chat", (function_object) => {
                string resultString = function_object(propsJson, vectorDBItemsJson, prompt, chatItemsJSon);
                return resultString;
            });
            return chatResult;
        }

    }
}