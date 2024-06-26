using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonAILib.Utils {
    public class LogWrapper {

        private static Action<string> InfoAction = (message) => { };
        private static Action<string> WarnAction = (message) => { };
        private static Action<string> ErrorAction = (message) => { };

        public static void SetActions(Action<string> infoAction, Action<string> warnAction, Action<string> errorAction) {
            InfoAction = infoAction;
            WarnAction = warnAction;
            ErrorAction = errorAction;
        }

        public static void Info(string message) {
            InfoAction(message);
        }

        public static void Warn(string message) {
            WarnAction(message);
        }

        public static void Error(string message) {
            ErrorAction(message);
        }



    }
}
