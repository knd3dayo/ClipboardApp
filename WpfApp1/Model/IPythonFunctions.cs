﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model {

    public class MaskedData {
        public HashSet<MaskedEntity> Entities { get; set; } = new HashSet<MaskedEntity>();
        public List<string> BeforeTextList { get; set; } = new List<string>();
        public List<string> AfterTextList { get; set; } = new List<string>();

        public MaskedData(List<string> beforeList) {

            foreach (var before in BeforeTextList) {
                BeforeTextList.Add(before);
            }
        }
    }
    public class MaskedEntity {
        public string Before { get; set; } = "";
        public string After { get; set; } = "";
        public string Label { get; set; } = "";

    }
    public interface IPythonFunctions {

        public string ExtractText(string path);
        public string GetMaskedString(string text);

        public MaskedData GetMaskedData(List<string> textList);
       
        public string OpenAIChat(List<JSONChatItem> jSONChatItems);

        public void RunScript(ScriptItem scriptItem, ClipboardItem clipboardItem);

        // 引数として渡した文字列をSpacyで処理してEntityを抽出する
        public HashSet<string> ExtractEntity(string text);




    }
}
