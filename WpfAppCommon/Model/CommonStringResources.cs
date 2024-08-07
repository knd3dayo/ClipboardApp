namespace WpfAppCommon.Model {
    public class CommonStringResources {

        public static CommonStringResources Instance { get; } = new CommonStringResources();
        public string AppName { get; } = "コピペアプリ";
        // ファイル
        public string File { get; } = "ファイル";
        // 作成
        public string Create { get; } = "作成";
        // アイテム作成
        public string CreateItem { get; } = "アイテム作成";
        // 終了
        public string Exit { get; } = "終了";
        // 編集
        public string Edit { get; } = "編集";

        // 背景情報を生成
        public string GenerateBackgroundInfo { get; } = "背景情報を生成";

        // サマリーを生成
        public string GenerateSummary { get; } = "サマリーを生成";

        //ベクトル検索
        public string VectorSearch { get; } = "ベクトル検索";

        // 開始
        public string Start { get; } = "開始";
        // 停止
        public string Stop { get; } = "停止";
        // 選択
        public string Select { get; } = "選択";
        // ヘルプ
        public string Help { get; } = "ヘルプ";
        // バージョン情報
        public string VersionInfo { get; } = "バージョン情報";

        // 表示
        public string View { get; } = "表示";

        // クリップボード監視開始
        public string StartClipboardWatch { get; } = "クリップボード監視開始";
        // クリップボード監視停止
        public string StopClipboardWatch { get; } = "クリップボード監視停止";
        // Windows通知監視開始
        public string StartNotificationWatch { get; } = "Windows通知監視開始";
        // Windows通知監視停止
        public string StopNotificationWatch { get; } = "Windows通知監視停止";

        // クリップボード監視を開始しました
        public string StartClipboardWatchMessage { get; } = "クリップボード監視を開始しました";
        // クリップボード監視を停止しました
        public string StopClipboardWatchMessage { get; } = "クリップボード監視を停止しました";
        // Windows通知監視を開始しました
        public string StartNotificationWatchMessage { get; } = "Windows通知監視を開始しました";
        // Windows通知監視を停止しました
        public string StopNotificationWatchMessage { get; } = "Windows通知監視を停止しました";

        // タグ編集
        public string EditTag { get; } = "タグ編集";
        // 自動処理ルール編集
        public string EditAutoProcessRule { get; } = "自動処理ルール編集";
        // Pythonスクリプト編集
        public string EditPythonScript { get; } = "Pythonスクリプト編集";
        // プロンプトテンプレート編集
        public string EditPromptTemplate { get; } = "プロンプトテンプレート編集";
        // RAGソース編集
        public string EditGitRagSource { get; } = "RAGソース(git)編集";

        // -- 表示メニュー
        // テキストを右端で折り返す
        public string TextWrapping { get; } = "テキストを右端で折り返す";
        // プレビューモード
        public string PreviewMode { get; } = "プレビューを有効にする";
        // コンパクト表示モード
        public string CompactMode { get; } = "コンパクト表示にする";

        // ツール
        public string Tool { get; } = "ツール";
        // OpenAIチャット
        public string OpenAIChat { get; } = "OpenAIチャット";
        // 画像エビデンスチェッカー
        public string ImageChat { get; } = "イメージチャット";

        // 検索
        public string Search { get; } = "検索";
        // 設定
        public string Setting { get; } = "設定";
        // 削除
        public string Delete { get; } = "削除";
        // 追加
        public string Add { get; } = "追加";
        // OK
        public string OK { get; } = "OK";
        // キャンセル
        public string Cancel { get; } = "キャンセル";
        // 閉じる
        public string Close { get; } = "閉じる";
        // エクスポート
        public string Export { get; } = "エクスポート";
        // インポート
        public string Import { get; } = "インポート";
        // 自動処理ルール一覧
        public string ListAutoProcessRule { get; } = "自動処理ルール一覧";
        // Pythonスクリプト一覧
        public string ListPythonScript { get; } = "Pythonスクリプト一覧";

        // タグ一覧
        public string ListTag { get; } = "タグ一覧";

        // 新規タグ
        public string NewTag { get; } = "新規タグ";
        // タグ
        public string Tag { get; } = "タグ";

        // ベクトルDB一覧
        public string ListVectorDB { get; } = "ベクトルDB一覧";
        // ベクトルDB編集
        public string EditVectorDB { get; } = "ベクトルDB編集";

        // --- ToolTip ---
        // 開始：クリップボード監視を開始します。停止：クリップボード監視を停止します。
        public string ToggleClipboardWatchToolTop { get; } = "開始：クリップボード監視を開始します。停止：クリップボード監視を停止します。";

        // 開始：Windows通知監視を開始します。停止：Windows通知監視を停止します.
        public string ToggleNotificationWatchToolTop { get; } = "開始：Windows通知監視を開始します。停止：Windows通知監視を停止します.";

        // 選択中のフォルダにアイテムを作成します。
        public string CreateItemToolTip { get; } = "選択中のフォルダにアイテムを作成します。";

        // アプリケーションを終了します。
        public string ExitToolTip { get; } = "アプリケーションを終了します。";
        // タグを編集します。
        public string EditTagToolTip { get; } = "タグを編集します。";

        // 選択したタグを削除します。
        public string DeleteSelectedTag { get; } = "選択したタグを削除";
        // すべて選択します。
        public string SelectAll { get; } = "すべて選択";
        // すべて選択解除します。
        public string UnselectAll { get; } = "すべて選択解除";

        // --- 画面タイトル ---

        // 自動処理ルール一覧
        public string ListAutoProcessRuleWindowTitle {
            get {
                return $"{AppName} - {ListAutoProcessRule}";
            }
        }
        // 自動処理ルール編集
        public string EditAutoProcessRuleWindowTitle {
            get {
                return $"{AppName} - {EditAutoProcessRule}";
            }
        }
        // Pythonスクリプト一覧
        public string ListPythonScriptWindowTitle {
            get {
                return $"{AppName} - {ListPythonScript}";
            }
        }
        // Pythonスクリプト編集
        public string EditPythonScriptWindowTitle {
            get {
                return $"{AppName} - {EditPythonScript}";
            }
        }
        // 設定
        public string SettingWindowTitle {
            get {
                return $"{AppName} - {Setting}";
            }
        }
        // 設定チェック結果
        public string SettingCheckResultWindowTitle {
            get {
                return $"{AppName} - 設定チェック結果";
            }
        }

        // RAGソース(git)編集
        public string EditGitRagSourceWindowTitle {
            get {
                return $"{AppName} - {EditGitRagSource}";
            }
        }
        // RAGソース一覧
        public string ListGitRagSourceWindowTitle {
            get {
                return $"{AppName} - RAGソース(git)一覧";
            }
        }
        // ベクトルDB一覧
        public string ListVectorDBWindowTitle {
            get {
                return $"{AppName} - {ListVectorDB}";
            }
        }
        // ベクトルDB編集
        public string EditVectorDBWindowTitle {
            get {
                return $"{AppName} - {EditVectorDB}";
            }
        }
        // コミット選択
        public string SelectCommitWindowTitle {
            get {
                return $"{AppName} - コミット選択";
            }
        }
        // QAチャット
        public string QAChatWindowTitle {
            get {
                return $"{AppName} - {OpenAIChat}";
            }
        }

        // タグ一覧
        public string ListTagWindowTitle {
            get {
                return $"{AppName} - {ListTag}";
            }
        }

        // ログ表示
        public string LogWindowTitle {
            get {
                return $"{AppName} - ログ表示";
            }
        }
        // スクリーンショットチェック用プロンプト生成
        public string ScreenShotCheckPromptWindowTitle {
            get {
                return $"{AppName} - スクリーンショットチェック用プロンプト生成";
            }
        }

        // --- namespace WpfAppCommon.PythonIF ---

        // --- DefaultClipboardController.cs ---
        // クリップボードの内容が変更されました
        public string ClipboardChangedMessage { get; } = "クリップボードの内容が変更されました";
        // クリップボードアイテムを処理
        public string ProcessClipboardItem { get; } = "クリップボードアイテムを処理";
        // 自動処理を実行中
        public string AutoProcessing { get; } = "自動処理を実行中";
        // クリップボードアイテムの追加処理が失敗しました。
        public string AddItemFailed { get; } = "クリップボードアイテムの追加処理が失敗しました。";

        // 自動タイトル設定処理を実行します
        public string AutoSetTitle { get; } = "自動タイトル設定処理を実行します";
        // タイトル設定処理が失敗しました
        public string SetTitleFailed { get; } = "タイトル設定処理が失敗しました";

        // 自動背景情報追加処理を実行します
        public string AutoSetBackgroundInfo { get; } = "自動背景情報追加処理を実行します";
        // 背景情報追加処理が失敗しました
        public string AddBackgroundInfoFailed { get; } = "背景情報追加処理が失敗しました";

        // 自動サマリー作成処理を実行します
        public string AutoCreateSummary { get; } = "自動サマリー作成処理を実行します";
        // サマリー作成処理が失敗しました
        public string CreateSummaryFailed { get; } = "サマリー作成処理が失敗しました";


        // 自動イメージテキスト抽出処理を実行します
        public string AutoExtractImageText { get; } = "自動イメージテキスト抽出処理を実行します";
        // イメージテキスト抽出処理が失敗しました
        public string ExtractImageTextFailed { get; } = "イメージテキスト抽出処理が失敗しました";

        // 自動タグ設定処理を実行します
        public string AutoSetTag { get; } = "自動タグ設定処理を実行します";
        // タグ設定処理が失敗しました
        public string SetTagFailed { get; } = "タグ設定処理が失敗しました";
        // 自動マージ処理を実行します
        public string AutoMerge { get; } = "自動マージ処理を実行します";
        // マージ処理が失敗しました
        public string MergeFailed { get; } = "マージ処理が失敗しました";
        // OCR処理を実行します
        public string OCR { get; } = "OCR処理を実行します";
        // OCR処理が失敗しました
        public string OCRFailed { get; } = "OCR処理が失敗しました";

        // 自動ファイル抽出処理を実行します
        public string ExecuteAutoFileExtract { get; } = "自動ファイル抽出処理を実行します";
        // 自動ファイル抽出処理が失敗しました
        public string AutoFileExtractFailed { get; } = "自動ファイル抽出処理が失敗しました";

        // --- EmptyPythonFunctions.cs ---
        // Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。
        public string PythonNotEnabledMessage { get; } = "Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。";

        // --- PythonExecutor.cs ---
        // カスタムPythonスクリプトの、templateファイル
        public string TemplateScript { get; } = "python/script_template.py";

        // クリップボードアプリ用のPythonスクリプト
        public string WpfAppCommonUtilsScript { get; } = "python/ai_app.py";

        // テンプレートファイルが見つかりません
        public string TemplateScriptNotFound { get; } = "テンプレートファイルが見つかりません";

        // --- PythonNetFunctions.cs ---
        // "PythonDLLが見つかりません。PythonDLLのパスを確認してください:"
        public string PythonDLLNotFound { get; } = "PythonDLLが見つかりません。PythonDLLのパスを確認してください:";
        //  "Pythonの初期化に失敗しました。"
        public string PythonInitFailed { get; } = "Pythonの初期化に失敗しました。";

        // "Pythonスクリプトファイルに、{function_name}関数が見つかりません"
        public string FunctionNotFound(string function_name) {
            return $"Pythonスクリプトファイルに、{function_name}関数が見つかりません";
        }
        // "Pythonスクリプトの実行中にエラーが発生しました
        public string PythonExecuteError { get; } = "Pythonスクリプトの実行中にエラーが発生しました";

        // "Pythonのモジュールが見つかりません。pip install <モジュール名>>でモジュールをインストールしてください。
        public string ModuleNotFound { get; } = "Pythonのモジュールが見つかりません。pip install <モジュール名>>でモジュールをインストールしてください。";

        // $"メッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}";
        public string PythonExecuteErrorDetail(Exception e) {
            return $"メッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}";
        }
        // "Spacyモデル名が設定されていません。設定画面からSPACY_MODEL_NAMEを設定してください"
        public string SpacyModelNameNotSet { get; } = "Spacyモデル名が設定されていません。設定画面からSPACY_MODEL_NAMEを設定してください";

        // "マスキング結果がありません"
        public string MaskingResultNotFound { get; } = "マスキング結果がありません";

        // "マスキングした文字列取得に失敗しました"
        public string MaskingResultFailed { get; } = "マスキングした文字列取得に失敗しました";

        // "マスキング解除結果がありません"
        public string UnmaskingResultNotFound { get; } = "マスキング解除結果がありません";
        // "マスキング解除した文字列取得に失敗しました"
        public string UnmaskingResultFailed { get; } = "マスキング解除した文字列取得に失敗しました";

        // "画像のバイト列に変換できません"
        public string ImageByteFailed { get; } = "画像のバイト列に変換できません";

        // "VectorDBItemsが空です"
        public string VectorDBItemsEmpty { get; } = "VectorDBItemsが空です";

        // "OpenAIの応答がありません"
        public string OpenAIResponseEmpty { get; } = "OpenAIの応答がありません";

        // ファイルが存在しません
        public string FileNotFound { get; } = "ファイルが存在しません";



    }
}
    