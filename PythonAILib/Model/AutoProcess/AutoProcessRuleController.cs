using System.Collections.ObjectModel;
using PythonAILib.Common;
using PythonAILib.Model.Content;
using PythonAILib.Model.Prompt;
using PythonAILib.PythonIF;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Utils.Common;
using PythonAILib.Resource;

namespace PythonAILib.Model.AutoProcess {
    public class AutoProcessRuleController {

        // DBから自動処理ルールのコレクションを取得する
        public static ObservableCollection<AutoProcessRule> GetAutoProcessRules(ContentFolder targetFolder) {
            ObservableCollection<AutoProcessRule> rules = [];
            PythonAILibManager libManager = PythonAILibManager.Instance;

            var collection = libManager.DataFactory.GetAutoProcessRuleCollection();
            var items = collection.Find(x => x.TargetFolderId == targetFolder.Id);
            foreach (var item in items) {
                if (item != null) {
                    rules.Add(item);
                }
            }
            return rules;

        }
        // TypeがCopyTo または MoveToのルールをLiteDBから取得する。
        public static IEnumerable<AutoProcessRule> GetCopyToMoveToRules() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetAutoProcessRuleCollection();

            var items = collection.Find(
                x => x.RuleAction != null
                && (x.RuleAction.Name == SystemAutoProcessItem.TypeEnum.CopyToFolder.ToString()
                    || x.RuleAction.Name == SystemAutoProcessItem.TypeEnum.MoveToFolder.ToString()));
            return items;
        }

        /// <summary>
        /// Apply automatic processing
        /// </summary>
        /// <param name="item"></param>
        /// <param name="image"></param>
        public static async Task<ContentItem?> ApplyAutoAction(ContentItem item) {

            IPythonAILibConfigParams configParams = PythonAILibManager.Instance.ConfigParams;

            // ★TODO Implement processing based on automatic processing rules.
            // 指定した行数以下のテキストアイテムは無視
            int lineCount = item.Content.Split('\n').Length;
            if (item.ContentType == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text && lineCount <= configParams.IgnoreLineCount()) {
                return null;
            }

            // ★TODO Implement processing based on automatic processing rules.
            // If AutoMergeItemsBySourceApplicationTitle is set, automatically merge items
            /**
            if (configParams.AutoMergeItemsBySourceApplicationTitle()) {
                LogWrapper.Info(CommonStringResources.Instance.AutoMerge);
                FolderManager.RootFolder.MergeItemsBySourceApplicationTitleCommandExecute(item);
            }
            **/
            // If AutoFileExtract is set, extract files
            if (configParams.AutoFileExtract() && item.ContentType == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Files) {
                string text = PythonExecutor.PythonAIFunctions.ExtractFileToText(item.FilePath);
                item.Content += "\n" + text;
            }
            if (item.IsImage() && item.Image != null) {
                // ★TODO Implement processing based on automatic processing rules.
                // If AutoExtractImageWithPyOCR is set, perform OCR
                if (configParams.AutoExtractImageWithPyOCR()) {
                    string extractImageText = PythonExecutor.PythonMiscFunctions.ExtractTextFromImage(item.Image, configParams.TesseractExePath());
                    item.Content += "\n" + extractImageText;
                    LogWrapper.Info(PythonAILibStringResources.Instance.OCR);

                } else if (configParams.AutoExtractImageWithOpenAI()) {

                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoExtractImageText);
                    ContentItemCommands.ExtractImageWithOpenAI(item);
                }
            }

            // ★TODO Implement processing based on automatic processing rules.
            var task1 = Task.Run(() => {
                // If AUTO_TAG is set, automatically set the tags
                if (configParams.AutoTag()) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoSetTag);
                    // ClipboardItem.CreateAutoTags(item);
                }
            });
            var task2 = Task.Run(() => {
                // If AUTO_DESCRIPTION is set, automatically set the DisplayText
                if (configParams.AutoTitle()) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoSetTitle);
                    ContentItemCommands.CreateAutoTitle(item);

                } else if (configParams.AutoTitleWithOpenAI()) {

                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoSetTitle);
                    ContentItemCommands.CreateAutoTitleWithOpenAI(item);
                }
            });
            var task3 = Task.Run(() => {
                // 背景情報
                if (configParams.AutoBackgroundInfo()) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoSetBackgroundInfo);
                    ContentItemCommands.CreateAutoBackgroundInfo(item);
                }
            });
            var task4 = Task.Run(() => {
                // サマリー
                if (configParams.AutoSummary()) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoCreateSummary);
                    ContentItemCommands.CreateChatResult(item, SystemDefinedPromptNames.SummaryGeneration.ToString());
                }
            });
            var task5 = Task.Run(() => {
                // Tasks
                if (configParams.AutoGenerateTasks()) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoCreateTaskList);
                    ContentItemCommands.CreateChatResult(item, SystemDefinedPromptNames.TasksGeneration.ToString());
                }
            });
            var task6 = Task.Run(() => {
                // Tasks
                if (configParams.AutoDocumentReliabilityCheck()) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoCheckDocumentReliability);
                    ContentItemCommands.CheckDocumentReliability(item);
                }
            });

            await Task.WhenAll(task1, task2, task3, task4, task5, task6);

            return item;
        }


    }
}
