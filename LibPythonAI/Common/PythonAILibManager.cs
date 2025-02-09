using PythonAILib.Model.Prompt;
using PythonAILib.PythonIF;
using PythonAILib.Resources;
using PythonAILib.Utils.Common;

namespace PythonAILib.Common
{
    public class PythonAILibManager {

        private static PythonAILibManager? _instance;
        public static PythonAILibManager Instance {
            get {
                return _instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            }
            private set {
                _instance = value;
            }
        }

        /// <summary>
        /// PythonAILibManagerの初期化
        /// IPythonAILibConfigPramsを実装したクラスを引数に渡す
        /// </summary>
        /// <param name="parmas"></param>
        public static void Init(IPythonAILibConfigParams parmas) {

            Instance = new PythonAILibManager(parmas);
            // PromptItemの初期化
            PromptItem.InitSystemPromptItems();
        }


        public IPythonAILibConfigParams ConfigParams { get; private set; }

        private PythonAILibManager(IPythonAILibConfigParams parameters) {

            ConfigParams = parameters;
            Action<string> debugAction = parameters.GetDebugAction();
            Action<string> infoAction = parameters.GetInfoAction();
            Action<string> warnAction = parameters.GetWarnAction();
            Action<string> errorAction = parameters.GetErrorAction();

            // 言語設定
            PythonAILibStringResources.Lang = parameters.GetLang();
            // Python処理機能の初期化
            PythonExecutor.Init(parameters);

            // DBControllerの設定
            DataFactory = parameters.GetDataFactory();
            // LogWrapperのログ出力設定
            LogWrapper.SetActions(debugAction, infoAction, warnAction, errorAction);

        }

        public IDataFactory DataFactory { get; set; }



    }
}
