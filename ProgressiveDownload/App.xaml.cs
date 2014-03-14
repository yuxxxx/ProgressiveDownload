using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ProgressiveDownload
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public App () : base()
        {
            Livet.DispatcherHelper.UIDispatcher = Current.Dispatcher;
        }
    }
}
