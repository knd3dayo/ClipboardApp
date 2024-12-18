namespace PythonAILib.Resource {
    public class PromptStringResource {

        // Instance
        public static PromptStringResource Instance { get; set; } = new();


        // 定義が不明な文章については、以下の説明を参考にしてください
        public virtual string UnknownContentDescription { get; } = "定義が不明な文章については、以下の説明を参考にしてください";


        // サマリー生成
        public virtual string SummaryGeneration { get; } = "サマリー";

        // "以下の文章から100～200文字程度のサマリーを生成してください。\n"
        public virtual string SummaryGenerationPrompt { get; } = "以下の文章から100～200文字程度のサマリーを生成してください。\n";


        // BackgroundInformationGeneration
        public virtual string BackgroundInformationGeneration { get; } = "背景情報";

        // "以下の文章の背景情報(経緯、目的、原因、構成要素、誰が？いつ？どこで？など)を生成してください。\n"
        public virtual string BackgroundInformationGenerationPrompt { get; } = "以下の文章の背景情報(経緯、目的、原因、構成要素、誰が？いつ？どこで？など)を生成してください。\n";

        // 日本語文章解析
        public virtual string AnalyzeJapaneseSentenceRequest { get; } = "* 命題とは(すべて|ある)主語(または主題)は〇〇である（またはではない)を表す文のことです。\r\n" +
            "* 一般的に日本語は次の構造をとります。\r\n " +
            "日本語構造=[主題]は[〇〇+格助詞] + 述語 + [時制、モダリティ、テンス]\r\n" +
            "* 日本語は、話し手と受け手の間ですでに共通認識がある部分については省略することがあり、述語さえあれば会話することが出来ます。\r\n" +
            "* 「モダリティ」とは、その文の内容に対する話し手の判断・聞き手に対する伝え方といった文の述べ方を担うもののことです。\r\n" +
            "* モダリティは、大きく\r\n" +
            "   -  文の伝達的な表し分けを表すモダリティ：表現類型のモダリティ\r\n" +
            "   -  事態に対する捉え方を表すモダリティ：評価のモダリティ、認識のモダリティ\r\n" +
            "   -  文と先行文脈との関連付けを表すモダリティ：説明のモダリティ\r\n" +
            "   -  聞き手に対する伝え方を表すモダリティ：丁寧さのモダリティ、伝達態度のモダリティ\r\n" +
            "の4つのタイプに分類することができます。\r\n" +
            "* 「表現類型のモダリティ」は、【叙述】【意志】【命令】【疑問】といった文の基本的な性質を表しています。\r\n" +
            "  - 【叙述】教科書を読む。\r\n  - 【意志】教科書を読もう。\r\n  - 【命令】教科書を読め。\r\n  - 【疑問】教科書を読みますか？\r\n" +
            "* 「評価のモダリティ」は、必要・不必要など、その事態に対する話し手の評価的な捉え方を表しています。\r\n" +
            "  - 図書館では、静かにしなくてはならない。\r\n" +
            "* 「認識のモダリティ」は、命題の内容である事態を話し手がどのような認識的な態度で捉えたかを表しています。\r\n" +
            "  - 明日は晴れるだろう。\r\n" +
            "* 「説明のモダリティ」は、そのモダリティが含まれる文と先行している文とが関連付いていることを表しています。\r\n" +
            "  - 雪が降っているのか。道理で寒いわけだ。\r\n" +
            "* 「丁寧さのモダリティ」とは、聞き手に対してその文を【普通体】【丁寧体】のどちらで伝えるかといったスタイルの選択に関わるもののことです。\r\n" +
            "  - 【普通体】今日は、この本を読んだ。\r\n  - 【丁寧体】今日は、この本を読みました。\r\n" +
            "* 「伝達態度のモダリティ」とは、聞き手に伝えるにあたっての微調整を行ったり、話し手の認識状態を表したりするもののことです。\r\n" +
            "  - ここを見てね\r\n  - きれいな景色だなあ\r\n\r\n" +
            "次の文章について以下の処理を行ってください。\r\n" +
            "- 省略部分を補完して上記の「日本語構造」の形式の命題のリストにしてください。  また、各命題がどのようなモダリティであるかも説明してください。\r\n   " +
            "想定される結果が複数の場合はそれらのうち可能性が高いものを10個まで挙げてください。";

        // 質問生成
        public virtual string GenerateQuestionRequest { get; } = "文章を分析して質問を挙げてください。\r\n例：\r\n# 定義(類と種差)に関する質問\r\n 文章. ぽんちょろりん汁はおいしいです。\r\n 質問. ぽんちょろりん汁は汁物料理のカテゴリに属するものですか？またはカテゴリ内の他のものとの異なる点はなんですか？\r\n# 目的・理由に関する質問\r\n 文章. 〇〇という仕事は今日中に終えなければならない\r\n 質問.  〇〇という仕事を今日中に終えなければいけない理由は何ですか？ また、〇〇という仕事を行う目的はなんですか？\r\n# 原因・経緯・歴史に関する質問\r\n 文章. 徳川家康は将軍である\r\n  質問. 徳川家康が将軍になった原因はなんですか？\r\n# 構成要素、機能に関する質問\r\n 文章. ぽんちょろりん汁は健康に良い\r\n 質問. ぽんちょろりん汁はどのような材料で作成されていますか？また、どの様な効果があるのですか？";

        // 回答依頼
        public virtual string AnswerRequest { get; } = "以下の質問に回答してください。\n";

        // タイトル生成
        public virtual string TitleGeneration { get; } = "タイトル生成";
        // "以下の文章からタイトルを生成してください。\n"
        public virtual string TitleGenerationPrompt { get; } = "以下の文章からタイトルを生成してください。\n";

        // "この画像のテキストを抽出してください。\n"
        public virtual string ExtractTextRequest { get; } = "この画像のテキストを抽出してください。\n";

        // 上記の文章の不明点については、以下の関連情報を参考にしてください
        public virtual string RelatedInformation { get; } = "------\n 以下は参考情報です。不正確な情報が含まれる可能性がありますが、本文の背景や文脈を理解するための参考にしてください\n";

        // 以下の文章を解析して、定義が不明な言葉を含む文を洗い出してください。" +
        // "定義が不明な言葉とはその言葉の類と種差、原因、目的、機能、構成要素が不明確な言葉です。" +
        // "出力は以下のJSON形式のリストで返してください。解析対象の文章がない場合や解析不能な場合は空のリストを返してください\n" +
        // "{'result':[{'sentence':'定義が不明な言葉を含む文','reason':'定義が不明な言葉を含むと判断した理由'}]}"

        // TODOリスト生成
        public virtual string TasksGeneration { get; } = "TODOリスト";

        // "以下の文章から課題リストを生成してください。\n"
        public virtual string TasksGenerationPrompt { get; } = "以下の文章からTODOとアクションプランのリストを生成してください。" +
            "なお参考情報がある場合には参考情報から得た背景や文脈を踏まえてTODOとアクションプランを具体的なものにしてください\n" +
            "TODOには対応すべき優先順位をつけてください。本文とは関連度が低いTODOは除外してください。\n" +
            "出力はJSON形式で{result:['todo': 'TODOの内容','plan': 'プランの内容']}でお願いします。\n";

        // Json形式で文字列のリストを生成するプロンプト
        public virtual string JsonStringListGenerationPrompt { get; } = "出力は文字列のリストとして、JSON形式で{result:[リストの項目]}でお願いします。\n";

    }
}
