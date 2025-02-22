using System.Windows;
using ClipboardApp.ViewModel.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using LibUIPythonAI.Utils;

namespace ClipboardApp.ViewModel.Main {
    public class MainPanelViewModel(AppItemViewModelCommands commands) : ObservableObject {
        public MainPanelTreeViewControlViewModel MainPanelTreeViewControlViewModel { get; set; } = new MainPanelTreeViewControlViewModel(commands);

        public MainPanelDataGridViewControlViewModel MainPanelDataGridViewControlViewModel { get; set; } = new MainPanelDataGridViewControlViewModel(commands);


        // ShowProperties
        public bool ShowProperties {
            get {
                return ClipboardAppConfig.Instance.ShowProperties;
            }
            set {
                ClipboardAppConfig.Instance.ShowProperties = value;
                ClipboardAppConfig.Instance.Save();

                OnPropertyChanged(nameof(ShowProperties));
                OnPropertyChanged(nameof(PropertiesVisibility));
            }
        }
        // PropertiesVisibility
        public Visibility PropertiesVisibility {
            get {
                return Tools.BoolToVisibility(ShowProperties);
            }
        }




    }
}
