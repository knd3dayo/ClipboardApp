using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPythonAI.Utils.Common {
    public interface ILogWrapperAction {

        public void Debug(string message);
        public void Info(string message);
        public void Warn(string message);
        public void Error(string message);

        public void UpdateInProgress(bool value, string message = "");

    }
}
