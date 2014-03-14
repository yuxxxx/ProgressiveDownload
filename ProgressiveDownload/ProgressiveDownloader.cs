using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using System.Threading.Tasks;
using System.Net.Http;

namespace ProgressiveDownload
{

    // 参考: http://stackoverflow.com/questions/20661652/progress-bar-with-httpclient

    /// <summary>
    /// 作業中に進捗を更新するダウンローダーです。
    /// <remarks>
    /// 送れるリクエストは現状GETだけです。
    /// キャンセルはできません。
    /// </remarks>
    /// </summary>
    public class ProgressiveDownloader : NotificationObject
    {
        /// <summary>
        /// 進捗を更新しつつ非同期にダウンロードします。
        /// </summary>
        /// <param name="source">ダウンロードするファイルのURL</param>
        /// <param name="destination">保存先</param>
        /// <param name="ticks">部分ダウンロードする単位</param>
        public async Task DownloadProgressive(string source, string destination, int ticks = 0x10000)
        {
            using (var client = new HttpClient())
            {
                State = RequestState.Sending;
                client.DefaultRequestHeaders.ExpectContinue = false;
                // HttpCompletionOption.ResponseHeadersRead を設定することで、ヘッダーが送られてきたらすぐにレスポンスを処理できるようにします。
                var response = await client.GetAsync(source, HttpCompletionOption.ResponseHeadersRead);

                using (var fs = new System.IO.FileStream(destination, System.IO.FileMode.Create))
                {
                    if (response.Content.Headers.ContentLength != null)
                    {
                        State = RequestState.Progress;
                        Size = (long)(response.Content.Headers.ContentLength);
                    }

                    await SaveAsync(response, fs, ticks);
                }

                State = RequestState.Finished;
            }
        }

        /// <summary>
        /// 泥臭く保存処理をします。
        /// </summary>
        /// <param name="response">レスポンスメッセージ</param>
        /// <param name="fs">ファイルストリーム</param>
        /// <param name="ticks">部分ダウンロードする単位</param>
        /// <remarks>進捗率の更新はここでやっています。</remarks>>
        private async Task SaveAsync(HttpResponseMessage response, System.IO.FileStream fs, int ticks)
        {
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                while (true)
                {
                    var buffer = new byte[ticks];
                    var t = await stream.ReadAsync(buffer, 0, ticks);
                    // 0バイト読みこんだら終わり
                    if (t == 0) { break; }

                    Progress += t;

                    await fs.WriteAsync(buffer, 0, t);
                }
            }
        }

        #region Sending変更通知プロパティ
        private bool _Sending;

        /// <summary>
        /// リクエストを送信中で、ファイルサイズがわからない状態であるかを表します。
        /// </summary>
        public bool Sending
        {
            get
            { return _Sending; }
            set
            {
                if (_Sending == value)
                    return;
                _Sending = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Progress変更通知プロパティ
        private long _Progress;

        public long Progress
        {
            get
            { return _Progress; }
            set
            {
                if (_Progress == value)
                    return;
                _Progress = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Size変更通知プロパティ
        private long _Size;

        public long Size
        {
            get
            { return _Size; }
            set
            {
                if (_Size == value)
                    return;
                _Size = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region State変更通知プロパティ
        private RequestState _State;

        public RequestState State
        {
            get
            { return _State; }
            set
            { 
                if (_State == value)
                    return;
                _State = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public void Reset()
        {
            State = RequestState.None;
            Size = 0;
            Progress = 0;
        }
    }
}
