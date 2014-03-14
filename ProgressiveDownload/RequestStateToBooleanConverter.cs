using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ProgressiveDownload
{
    class RequestStateToBooleanConverter : DependencyObject, IValueConverter
    {

        public bool NoneIs
        {
            get { return (bool)GetValue(NoneIsProperty); }
            set { SetValue(NoneIsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NoneIs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoneIsProperty =
            DependencyProperty.Register("NoneIs", typeof(bool), typeof(RequestStateToBooleanConverter), new PropertyMetadata(false));

        public bool SendingIs
        {
            get { return (bool)GetValue(SendingIsProperty); }
            set { SetValue(SendingIsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SendingIs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SendingIsProperty =
            DependencyProperty.Register("SendingIs", typeof(bool), typeof(RequestStateToBooleanConverter), new PropertyMetadata(false));

        public bool ProgressIs
        {
            get { return (bool)GetValue(ProgressIsProperty); }
            set { SetValue(ProgressIsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProgressIs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressIsProperty =
            DependencyProperty.Register("ProgressIs", typeof(bool), typeof(RequestStateToBooleanConverter), new PropertyMetadata(false));

        public bool FinishedIs
        {
            get { return (bool)GetValue(FinishedIsProperty); }
            set { SetValue(FinishedIsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FinishedIs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FinishedIsProperty =
            DependencyProperty.Register("FinishedIs", typeof(bool), typeof(RequestStateToBooleanConverter), new PropertyMetadata(false));

        #region IValueConverter メンバー

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is RequestState)) throw new ArgumentException();
            switch(((RequestState)value).ToString())
            {
                case "None":
                    return NoneIs;
                case "Sending":
                    return SendingIs;
                case "Progress":
                    return ProgressIs;
                case "Finished":
                    return FinishedIs;
                default:
                    throw new ArgumentException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
