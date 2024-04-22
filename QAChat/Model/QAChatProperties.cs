﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAChat.Model {
    public class QAChatProperties {

        public static  Dictionary<string, string> CreateOpenAIProperties() {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("OpenAIKey", Properties.Settings.Default.OpenAIKey);
            dict.Add("OpenAICompletionModel", Properties.Settings.Default.OpenAICompletionModel);
            dict.Add("OpenAIEmbeddingModel", Properties.Settings.Default.OpenAIEmbeddingModel);
            dict.Add("AzureOpenAI", Properties.Settings.Default.AzureOpenAI.ToString());

            if (Properties.Settings.Default.OpenAICompletionBaseURL != "") {
                dict.Add("OpenAICompletionBaseURL", Properties.Settings.Default.OpenAICompletionBaseURL);
            }
            if (Properties.Settings.Default.OpenAIEmbeddingBaseURL != "") {
                dict.Add("OpenAIEmbeddingBaseURL", Properties.Settings.Default.OpenAIEmbeddingBaseURL);
            }
            dict.Add("VectorDBURL", Properties.Settings.Default.VectorDBURL);
            return dict;
        }

        public static void SaveSettings(Dictionary<string, string> settings) {
            Properties.Settings.Default.AzureOpenAI = bool.Parse(settings["AzureOpenAI"]);
            Properties.Settings.Default.OpenAIKey = settings["OpenAIKey"];
            Properties.Settings.Default.OpenAICompletionModel = settings["OpenAICompletionModel"];
            Properties.Settings.Default.OpenAIEmbeddingModel = settings["OpenAIEmbeddingModel"];
            Properties.Settings.Default.OpenAICompletionBaseURL = settings["OpenAICompletionBaseURL"];
            Properties.Settings.Default.OpenAIEmbeddingBaseURL = settings["OpenAIEmbeddingBaseURL"];
            Properties.Settings.Default.VectorDBURL = settings["VectorDBURL"];
            Properties.Settings.Default.SourceDocumentURL = settings["SourceDocumentURL"];
            Properties.Settings.Default.PythonDllPath = settings["PythonDllPath"];

            Properties.Settings.Default.Save();
        }

    }
}
