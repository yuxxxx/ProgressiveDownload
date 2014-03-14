ProgressiveDownload
===================

HttpClientで進捗付きダウンロードするサンプル

参考: [c# - Progress bar with HttpClient - Stack Overflow](http://stackoverflow.com/questions/20661652/progress-bar-with-httpclient)

## 用意するもの

- [HttpClient](http://msdn.microsoft.com/ja-JP/library/system.net.http.httpclient(VS.110).aspx) : 今回の主役
- [Livet](https://github.com/ugaya40/Livet) : もうこれなしでWPFアプリ作れなくなりそう
- [Rx(Reactive Extensions)](https://rx.codeplex.com/) : 更新通知を横流ししたりフィルタリングしたり地味に大活躍

## ざっくりとした使い方

1. ダウンローダーをPrivateで持っておく
2. ProgressとかをVMに横流しする
3. 適当なコマンドでDownloadProgressiveを呼ぶ

## 内容とか

- MainWindow.xaml : 特に見るべきものはない何の変哲もないXAML
- <del>MainWindow.xaml.cs : InitializeComponent()をコンストラクターで呼ぶだけのほぼ空気</del>
- MainWoindowViewModel.cs : 横流しするだけ、やたら行数が多いのは変更通知を送るための仕方ない事情
- ProgressiveDownloader.cs : 今回の主役
- RequestState.cs : リクエスト状態の列挙型
- RequestStateToBooleanConverter.cs : はーT4勉強したい

## ポイント

いちいちストリームをバイト配列にしてかじっていくという、泥臭いことをしないといけないのがすごくあれです。こういうのはもうこの1回書いただけでおしまいにしたい。
