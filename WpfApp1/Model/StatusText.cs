﻿using CommunityToolkit.Mvvm.ComponentModel;

namespace ClipboardApp.Model
{
    public class StatusText : ObservableObject
    {
        public static string DefaultText { get; } = "Ready";

        private string _text = DefaultText;
        private CancellationTokenSource? _tokenSource;

        public string InitText { get; set; } = DefaultText;

        public List<string> Messages { get; } = new List<string>();

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                // DefaultText以外の場合はメッセージを追加
                if (value != DefaultText) {
                    Messages.Add($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} {value}");
                }
                OnPropertyChanged("Text");
                // _tokenSourceがnullの場合は初期化
                if (_tokenSource != null)
                {
                    //すでに_tokenSourceが存在する場合はキャンセル
                    _tokenSource.Cancel();
                }
                _tokenSource = new CancellationTokenSource();
                //ThreadPoolに新規処理を追加
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClearText), _tokenSource.Token);

            }
        }

        private void ClearText(object? obj)
        {
            if (obj == null)
            {
                return;
            }
            CancellationToken token = (CancellationToken)obj;
            for (int i = 0; i < 20; i++)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                // 100ms待機
                Thread.Sleep(100);
            }
            if (token.IsCancellationRequested)
            {
                return;
            }
            Init();
            OnPropertyChanged("Content");
        }
        public void Init()
        {
            Text = InitText;
        }
        public void Dispose()
        {
            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
            }
        }
    }

}
