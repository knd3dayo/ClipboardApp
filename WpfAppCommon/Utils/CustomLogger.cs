using System.Windows;
using NLog;
using WpfAppCommon.Model;

namespace WpfAppCommon.Utils {
    public class CustomLogger : Logger {

        public static Window ActiveWindow { get; set; } = Application.Current.MainWindow;

        private static StatusText StatusText {
            get {
                return Tools.StatusText;
            }
        }

        public new void Info(string message) {
            // 親クラスのメソッドを呼び出す
            base.Info(message);
            MainUITask.Run(() => {
                StatusText.Text = message;
            });
        }
        public new void Warn(string message) {
            MainUITask.Run(() => {
                base.Warn(message);
                StatusText.Text = message;
                // 開発中はメッセージボックスを表示する
                System.Windows.MessageBox.Show(ActiveWindow, message);
            });
        }

        public new void Error(string message) {
            MainUITask.Run(() => {
                base.Error(message);
                StatusText.Text = message;
                System.Windows.MessageBox.Show(ActiveWindow, message);
            });
        }

    }
}
