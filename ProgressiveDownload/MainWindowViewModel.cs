using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using System.Net.Http;
using System.Reactive.Linq;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using ProgressiveDownload;
using System.Reactive;

namespace ProgressiveDownload
{
    public class MainWindowViewModel : ViewModel
    {
        public MainWindowViewModel() : base()
        {
            Downloader = new ProgressiveDownloader();
        }

        public void Initialize()
        {
            Source = "";
            Destination = "";

            // 進捗メッセージを更新させるためのコードです。イベントが多すぎるので1秒ごとにフィルターをかけています。
            // 本編はSubscribeにあるラムダ式で、それ以外は前座です。
            var progress = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => this.PropertyChanged += h,
                h => this.PropertyChanged -= h)
                .Where(e => e.EventArgs.PropertyName == "Progress")
                .Sample(TimeSpan.FromSeconds(1))
                .SubscribeOn(DispatcherHelper.UIDispatcher)
                .Subscribe(_ => Message += string.Format("{0}: Bytes read: {1}\n", DateTime.Now, Downloader.Progress));
            this.CompositeDisposable.Add(progress);

            // Stateだけはコマンドの状態にかかわるので特別に扱います。
            var state = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => this.PropertyChanged += h,
                h => this.PropertyChanged -= h)
                .Where(e => e.EventArgs.PropertyName == "State")
                .SubscribeOn(DispatcherHelper.UIDispatcher)
                .Subscribe(UpdateCommands);
            this.CompositeDisposable.Add(state);

            // Downloaderのプロパティ更新イベントを横流しするためのコードです。
            var d = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => Downloader.PropertyChanged += h,
                h => Downloader.PropertyChanged -= h)
                .SubscribeOn(DispatcherHelper.UIDispatcher)
                .Subscribe(e => RaisePropertyChanged(e.EventArgs.PropertyName));
            this.CompositeDisposable.Add(d);
        }

        private void UpdateCommands(EventPattern<PropertyChangedEventArgs> obj)
        {
            DownloadCommand.RaiseCanExecuteChanged();
            ResetCommand.RaiseCanExecuteChanged();
        }
        
        private ProgressiveDownloader Downloader { get; set; }
        
        public long Progress
        {
            get { return Downloader.Progress; }
            set { Downloader.Progress = value; }
        }

        public long Size
        {
            get { return Downloader.Size; }
            set { Downloader.Size = value; }
        }

        public RequestState State {
            get { return Downloader.State; }
            set { Downloader.State = value; }
        }

        #region Source変更通知プロパティ
        private string _Source;

        public string Source
        {
            get
            { return _Source; }
            set
            { 
                if (_Source == value)
                    return;
                _Source = value;
                DownloadCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Destination変更通知プロパティ
        private string _Destination;

        public string Destination
        {
            get
            { return _Destination; }
            set
            { 
                if (_Destination == value)
                    return;
                _Destination = value;
                DownloadCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Message変更通知プロパティ
        private string _Message;

        public string Message
        {
            get
            { return _Message; }
            set
            { 
                if (_Message == value)
                    return;
                _Message = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region DownloadCommand
        private ViewModelCommand _DownloadCommand;

        /// <summary>
        /// このプロパティをボタンにバインドすると、そのボタンを押したときにDownloadメソッドが実行されます。
        /// </summary>
        public ViewModelCommand DownloadCommand
        {
            get
            {
                if (_DownloadCommand == null) {
                    _DownloadCommand = new ViewModelCommand(Download, CanDownload);
                }
                return _DownloadCommand;
            }
        }

        /// <summary>
        /// Downloadコマンドが実行できるかどうかを判定するメソッドです。
        /// </summary>
        /// <returns>実行できる場合はtrueを返します。</returns>
        public bool CanDownload()
        {
            return State == RequestState.None &&
                Destination != "" && Source != null && Source.StartsWith("http");
        }

        /// <summary>
        /// ダウンロードします。これが「モデルに丸投げ」の正体
        /// </summary>
        public async void Download()
        {
            await Downloader.DownloadProgressive(Source, Destination);
        }
        #endregion

        #region ResetCommand
        private ViewModelCommand _ResetCommand;

        public ViewModelCommand ResetCommand
        {
            get
            {
                if (_ResetCommand == null)
                {
                    _ResetCommand = new ViewModelCommand(Reset, CanReset);
                }
                return _ResetCommand;
            }
        }

        public bool CanReset()
        {
            return State == RequestState.Finished;
        }

        public void Reset()
        {
            Downloader.Reset();
            Message = "";
            Source = "";
            Destination = "";
        }
        #endregion

    }
}
