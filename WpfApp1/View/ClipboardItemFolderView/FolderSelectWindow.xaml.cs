﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1.View.ClipboardItemFolderView
{
    /// <summary>
    /// FolderSelectWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class FolderSelectWindow : Window
    {
        public static FolderSelectWindow? Current;
        public FolderSelectWindow()
        {
            InitializeComponent();
            Current = this;
        }
    }
}