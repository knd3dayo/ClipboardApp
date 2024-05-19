using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.Views.ClipboardItemView;

namespace ClipboardApp.Control {
    /// <summary>
    /// MainWindowListView1.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindowGridView1 : UserControl {
        public MainWindowGridView1() {
            InitializeComponent();
        }
        // PreviewModeVisibility
        public Visibility PreviewModeVisibility {
            get { return (Visibility)GetValue(PreviewModeVisibilityProperty); }
            set { SetValue(PreviewModeVisibilityProperty, value); }
        }
        public static readonly DependencyProperty PreviewModeVisibilityProperty =
            DependencyProperty.Register(nameof(PreviewModeVisibility), typeof(Visibility), typeof(MainWindowGridView1));

        // SelectedItem
        public ClipboardItemViewModel SelectedItem {
            get { return (ClipboardItemViewModel)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(ClipboardItemViewModel), typeof(MainWindowGridView1));

        public ObservableCollection<ClipboardItemViewModel> ItemsSource {
            get { return (ObservableCollection<ClipboardItemViewModel>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(ObservableCollection<ClipboardItemViewModel>), typeof(MainWindowGridView1));

        public ICommand ClipboardItemSelectionChangedCommand {
            get { return (ICommand)GetValue(ClipboardItemSelectionChangedCommandProperty); }
            set { SetValue(ClipboardItemSelectionChangedCommandProperty, value); }
        }
        public static readonly DependencyProperty ClipboardItemSelectionChangedCommandProperty =
            DependencyProperty.Register(nameof(ClipboardItemSelectionChangedCommand), typeof(ICommand), typeof(MainWindowGridView1));

        public ICommand OpenSelectedItemCommand {
            get { return (ICommand)GetValue(OpenSelectedItemCommandProperty); }
            set { SetValue(OpenSelectedItemCommandProperty, value); }
        }
        public static readonly DependencyProperty OpenSelectedItemCommandProperty =
            DependencyProperty.Register(nameof(OpenSelectedItemCommand), typeof(ICommand), typeof(MainWindowGridView1));
    }


}