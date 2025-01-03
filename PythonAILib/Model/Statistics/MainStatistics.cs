using LiteDB;
using PythonAILib.Common;

namespace PythonAILib.Model.Statistics {
    public class MainStatistics {

        public static MainStatistics GetMainStatistics() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetStatisticsCollection<MainStatistics>();
            var item = collection.FindAll().FirstOrDefault();
            if (item == null) {
                item = new MainStatistics();
                collection.Upsert(item);
            }
            return item;
        }

        // SourceId
        public ObjectId Id { get; set; } = ObjectId.NewObjectId();
        // 日毎のStatistics
        public Dictionary<DateTime, DailyStatistics> DailyStatistics { get; set; } = [];


        // 本日のStatisticsを取得するメソッド
        public DailyStatistics GetTodayStatistics() {
            DateTime today = DateTime.Today;
            if (DailyStatistics.ContainsKey(today)) {
                return DailyStatistics[today];
            }
            DailyStatistics todayStatistics = new() {
                Date = today
            };
            DailyStatistics.Add(today, todayStatistics);
            return todayStatistics;
        }
        // 指定した日のStatisticsを取得するメソッド
        public DailyStatistics GetDailyStatistics(DateTime date) {
            // dateの年月日のみを取得
            DateTime dateOnly = date.Date;
            if (DailyStatistics.ContainsKey(dateOnly)) {
                return DailyStatistics[dateOnly];
            }
            // dateOnlyのStatisticsが存在しない場合は新規作成
            DailyStatistics newStatistics = new() {
                Date = dateOnly
            };
            return newStatistics;
        }
        // DailyStatisticsを更新するメソッド
        public void UpdateDailyStatistics(DailyStatistics dailyStatistics) {
            DateTime date = dailyStatistics.Date;
            DailyStatistics[date] = dailyStatistics;
        }
        // 本日のトークン数を追加するメソッド
        public void AddTodayTokens(long tokens, string modelName) {
            DailyStatistics todayStatistics = GetTodayStatistics();
            todayStatistics.TotalTokens += tokens;
            // modelNameのトークン数を追加
            if (todayStatistics.TokenCounts.ContainsKey(modelName)) {
                todayStatistics.TokenCounts[modelName] += tokens;
            } else {
                todayStatistics.TokenCounts[modelName] = tokens;
            }
            // 本日のStatisticsを更新
            UpdateDailyStatistics(todayStatistics);
            // 保存
            Save();
        }

        // トータルトークン数を取得するメソッド
        public long GetTotalTokens() {
            long totalTokens = 0;
            foreach (DailyStatistics dailyStatistics in DailyStatistics.Values) {
                totalTokens += dailyStatistics.TotalTokens;
            }
            return totalTokens;
        }

        // Save
        public void Save() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetStatisticsCollection<MainStatistics>();
            collection.Upsert(this);
        }

        // Get Statistics message
        public static string GetStatisticsMessage() {
            string message;
            // MainStatisticsを取得
            MainStatistics mainStatistics = MainStatistics.GetMainStatistics();
            // 本日のトークン数
            long totalTokens = mainStatistics.GetTotalTokens();
            message = PythonAILib.Resource.PythonAILibStringResources.Instance.TotalTokenFormat(totalTokens) + "\n\n";
            // 日次トークン数情報
            message += PythonAILib.Resource.PythonAILibStringResources.Instance.DailyTokenCount + "\n";
            Dictionary<DateTime, DailyStatistics> keyValuePairs = mainStatistics.DailyStatistics;
            // 日毎のトークン数を表示
            foreach (KeyValuePair<DateTime, DailyStatistics> pair in keyValuePairs) {
                DailyStatistics dailyStatistics = pair.Value;
                string dailyMessage = PythonAILib.Resource.PythonAILibStringResources.Instance.DailyTokenFormat(dailyStatistics.Date.ToShortDateString(), dailyStatistics.TotalTokens);
                message += dailyMessage + "\n";
            }
            return message;
        }
    }
}
