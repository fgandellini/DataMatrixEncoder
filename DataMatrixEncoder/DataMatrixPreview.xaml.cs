using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace DataMatrixEncoder
{
    /// <summary>
    /// Interaction logic for DataMatrixPreview.xaml
    /// </summary>
    public partial class DataMatrixPreview : UserControl
    {
        private string imageAbsolutePath;
        public string ImageAbsolutePath
        {
            get
            {
                return imageAbsolutePath;
            }
            set 
            {
                imageAbsolutePath = value;
                UpdatePreviewImage(imageAbsolutePath);
            }
        }

        public DataMatrixPreview()
        {
            InitializeComponent();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            previewImage.Source = null;
            OnClose();
        }

        private void openInExplorerButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(System.IO.Path.GetDirectoryName(imageAbsolutePath));
        }
        
        public event EventHandler Close;
        protected virtual void OnClose()
        {
            if (Close != null)
            {
                Close(this, null);
            }
        }

        private void UpdatePreviewImage(string path)
        {
            pathTextBlock.Text = System.IO.Path.GetDirectoryName(path);
            filenameTextBlock.Text = @"\" + System.IO.Path.GetFileName(path);

            using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                MemoryStream ms = new MemoryStream();
                fs.CopyTo(ms);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();

                previewImage.Source = bi.Clone();
            }
        }
    }
}
