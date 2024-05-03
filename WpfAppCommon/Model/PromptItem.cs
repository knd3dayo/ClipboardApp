﻿
using LiteDB;

namespace QAChat.Model {
    public class PromptItem {

        public ObjectId? Id { get; set; }
        // 名前
        public string Name { get; set; } = "";
        // 説明
        public string Description { get; set; } = "";

        // プロンプト
        public string Prompt { get; set; } = "";
    }
}