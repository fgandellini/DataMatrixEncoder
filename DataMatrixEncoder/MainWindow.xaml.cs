using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Globalization;
using System.Threading;
using System.Configuration;
using System.IO;
using MahApps.Metro.Controls.Dialogs;
using System.Drawing;
using System.Drawing.Imaging;

namespace DataMatrixEncoder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        DataMatrixEncoderLib.Encoder encoder = new DataMatrixEncoderLib.Encoder();
        DataMatrixEncoderLib.ContentManager contentManager = new DataMatrixEncoderLib.ContentManager();

        string logFilePath = "";

        public MainWindow()
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "MM/yyyy";
            Thread.CurrentThread.CurrentCulture = ci;

            logFilePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "log.txt");

            InitializeComponent();
        }

        private void GenerateDataMatrix_Handler(object sender, DataMatrixArgs dmArgs)
        {
            try
            {
                Bitmap bmp = encoder
                    .SetSize(DataMatrixEncoder.Properties.Settings.Default.Size)
                    .SetContent(contentManager.Parse(dmArgs.DataMatrixDescriptor).ToString())
                    .Encode();

                string filename = string.Format("{0}_{1}_{2}_{3}.png",
                    DateTime.Now.ToString("yyyyMMddHHmmss"),
                    dmArgs.DataMatrixDescriptor.Fields.First(f => f.Name == "CIP").Value,
                    dmArgs.DataMatrixDescriptor.Fields.First(f => f.Name == "Scadenza").Value,
                    dmArgs.DataMatrixDescriptor.Fields.First(f => f.Name == "Lotto").Value);

                string path = System.IO.Path.Combine(
                    DataMatrixEncoder.Properties.Settings.Default.DestinationDirectory, filename);
                
                using (var fs = new System.IO.FileStream(path, System.IO.FileMode.CreateNew))
                {
                    bmp.Save(fs, ImageFormat.Png);
                    bmp.Dispose();
                }

                ShowPreview(path);
            }
            catch (Exception ex)
            {
                string errorMessage = 
                    ex.Message == "A generic error occurred in GDI+." ? 
                    "Errore durante il salvataggio del file." : 
                    ex.Message;

                File.AppendAllText(logFilePath, 
                    string.Format("{1}{0}{2}{0}{0}", 
                    Environment.NewLine,
                    DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
                    ex.ToString()));

                ShowMessage("ERROR", errorMessage);
            }
        }

        private void PreviewClose_Handler(object sender, EventArgs e)
        {
            mainTabControl.SelectedIndex = 0;
        }

        private void ShowPreview(string path)
        {
            // update image path
            dataMatrixPreview.ImageAbsolutePath = path;

            // move to next tab
            mainTabControl.SelectedIndex = 1;
        }

        private void ToggleSettingsFlyout(object sender, RoutedEventArgs e)
        {
            DataMatrixEncoder.Properties.Settings.Default.Reload();
            settingsFlyout.IsOpen = !settingsFlyout.IsOpen;
        }

        private async void SaveSettings(object sender, RoutedEventArgs e)
        {
            bool canSave = !Validation.GetHasError(sizeTextBox);
            if (!Directory.Exists(destinationDirectoryTextBox.Text))
            {
                if (await UserWantsToCreateDirectory())
                {
                    try
                    {
                        Directory.CreateDirectory(destinationDirectoryTextBox.Text);
                    }
                    catch (Exception ex)
                    {
                        canSave = false;

                        File.AppendAllText(logFilePath,
                            string.Format("{1}{0}{2}{0}{0}",
                            Environment.NewLine,
                            DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
                            ex.ToString()));

                        ShowMessage("ERROR", ex.Message);
                    }
                }
                else
                {
                    canSave = false;
                }
            }

            // save settings
            if (canSave)
            {
                DataMatrixEncoder.Properties.Settings.Default.Save();
            }
            ToggleSettingsFlyout(sender, e);
        }

        private async Task<bool> UserWantsToCreateDirectory()
        {
            MessageDialogResult response = await this.ShowMessageAsync(
                "Directory non trovata", 
                "La directory specificata non esiste, vuoi crearla?", 
                MessageDialogStyle.AffirmativeAndNegative);
            return response == MessageDialogResult.Affirmative;
        }

        private void ShowMessage(string severity, string message)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "INFO", "Informazione" },
                { "WARNING", "Attenzione" },
                { "ERROR", "Errore" }
            };
            Dictionary<string, System.Windows.Media.Color> colors = new Dictionary<string, System.Windows.Media.Color>()
            {
                { "INFO", System.Windows.Media.Color.FromRgb(65, 177, 225) },
                { "WARNING", System.Windows.Media.Color.FromRgb(251, 134, 51) },
                { "ERROR", System.Windows.Media.Color.FromRgb(234, 67, 51) }
            };

            statusFlyout.Header = headers[severity];
            statusFlyout.Background = new SolidColorBrush(colors[severity]);
            statusMessageTextBlock.Text = message;
            statusFlyout.IsOpen = !statusFlyout.IsOpen;
        }
    }

    public class DataMatrixSizeRule : ValidationRule
    {
        public DataMatrixSizeRule()
        {
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int size = 0;

            try
            {
                if (((string)value).Length > 0)
                    size = Int32.Parse((String)value);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Immettere un numero." + Environment.NewLine + "Questo valore non verrà salvato.");
            }

            if (size < 20 || (size % 20 != 0))
            {
                return new ValidationResult(false, "Immettere una dimensione maggiore di 0 e multipla di 20." + Environment.NewLine + "Questo valore non verrà salvato.");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
