using DataMatrixEncoderLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataMatrixEncoder
{
    /// <summary>
    /// Interaction logic for DataMatrixFields.xaml
    /// </summary>
    public partial class DataMatrixFields : UserControl
    {
        public DataMatrixFields()
        {
            InitializeComponent();
        }

        #region Datepicker Configuration

        //private void DataMatrixFields_DisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        //{
        //    if (e.NewMode == CalendarMode.Month)
        //    {
        //        Calendar calendar = (Calendar)sender;
        //        calendar.DisplayModeChanged -= DataMatrixFields_DisplayModeChanged;
        //        calendar.DisplayMode = CalendarMode.Decade;
        //        calendar.DisplayModeChanged += DataMatrixFields_DisplayModeChanged;
        //        Console.WriteLine("DD " + calendar.DisplayDate);
        //        Console.WriteLine("SD " + calendar.SelectedDate);
        //        calendar.SelectedDate = new DateTime(
        //            calendar.DisplayDate.Year, calendar.DisplayDate.Month, 1);
                
        //        // close calendar popup
        //        //expDatePicker.IsDropDownOpen = false;
        //    }
        //}

        private void expDatePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
            var datepicker = sender as DatePicker;
            if (datepicker != null)
            {
                var popup = datepicker.Template.FindName("PART_Popup", datepicker) as Popup;
                if (popup != null && popup.Child is System.Windows.Controls.Calendar)
                {
                    //((Calendar)popup.Child).DisplayModeChanged -= DataMatrixFields_DisplayModeChanged;
                    ((System.Windows.Controls.Calendar)popup.Child).DisplayMode = CalendarMode.Decade;
                    //((Calendar)popup.Child).DisplayModeChanged += DataMatrixFields_DisplayModeChanged;
                }
            }
        }

        //private void expDatePicker_CalendarClosed(object sender, RoutedEventArgs e)
        //{
        //    var datepicker = sender as DatePicker;
        //    if (datepicker != null)
        //    {
        //        var popup = datepicker.Template.FindName("PART_Popup", datepicker) as Popup;
        //        if (popup != null && popup.Child is Calendar)
        //        {
        //            ((Calendar)popup.Child).DisplayModeChanged -= DataMatrixFields_DisplayModeChanged;
        //            ((Calendar)popup.Child).DisplayMode = CalendarMode.Decade;
        //            ((Calendar)popup.Child).DisplayModeChanged += DataMatrixFields_DisplayModeChanged;
        //        }
        //    }
        //}

        #endregion Datepicker Configuration

        private void generateDatamatrixButton_Click(object sender, RoutedEventArgs e)
        {
            DataMatrixArgs dm = new DataMatrixArgs();
            dm.DataMatrixDescriptor = new DataMatrix()
            {
                Fields = new System.Collections.Generic.List<IDataMatrixField>() 
                { 
                    new GtinDataMatrixField("CIP", gtinTextBox.Text),
                    new ExpDataMatrixField("Scadenza", (expDatePicker.SelectedDate != null) ? ((DateTime)expDatePicker.SelectedDate).ToString("yyMM") : ""),
                    new LotDataMatrixField("Lotto", lotTextBox.Text)
                }
            };
            OnGenerateDataMatrix(dm);
        }

        public event EventHandler<DataMatrixArgs> GenerateDataMatrix;
        protected virtual void OnGenerateDataMatrix(DataMatrixArgs dmArgs)
        {
            if (GenerateDataMatrix != null)
            {
                GenerateDataMatrix(this, dmArgs);
            }
        }
    }

    public class DataMatrixArgs
    {
        public IDataMatrix DataMatrixDescriptor;
    }
}
