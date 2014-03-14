using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressiveDownload
{
    public enum RequestState
    {
        /// <summary>
        /// 何もしていない状態です。
        /// </summary>
        None,
        /// <summary>
        /// リクエストを送信しましたが、レスポンスが返ってこない状態です。
        /// </summary>
        Sending,
        /// <summary>
        /// ダウンロード中です。
        /// </summary>
        Progress,
        /// <summary>
        /// ダウンロードが完了した状態です。
        /// </summary>
        Finished
    }
}
