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

namespace CLW
{
    /// <summary>
    /// Interaction logic for NItemUC.xaml
    /// </summary>
    public partial class NItemUC : UserControl
    {
        public NItemUC()
        {
            InitializeComponent();
        }






        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(NItemUC), new PropertyMetadata(null));





        public string SubTitle
        {
            get { return (string)GetValue(SubTitleProperty); }
            set { SetValue(SubTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubTitleProperty =
            DependencyProperty.Register("SubTitle", typeof(string), typeof(NItemUC), new PropertyMetadata(null));






        public string ContentText
        {
            get { return (string)GetValue(ContentTextProperty); }
            set { SetValue(ContentTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentTextProperty =
            DependencyProperty.Register("ContentText", typeof(string), typeof(NItemUC), new PropertyMetadata(null));









        public string FastLink
        {
            get { return (string)GetValue(FastLinkProperty); }
            set { SetValue(FastLinkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FastLink.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FastLinkProperty =
            DependencyProperty.Register("FastLink", typeof(string), typeof(NItemUC), new PropertyMetadata(null));

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
        

            var url = fastLink_lbl.Text;
            //url = "http://fsdmfes.ac.ma/uploads/Docs/Files/2021-05-20-01-54-05_183bddf1b0d6b398ccf9be4dfe32f87bb0364a09.pdf";

            Uri asUri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out asUri))
            {
                MessageBox.Show("bad url"); return;
            }


            var filename = asUri.LocalPath;
            filename = System.IO.Path.GetFileName(filename);

            var saveDlg = new Ookii.Dialogs.Wpf.VistaSaveFileDialog();
            saveDlg.FileName = filename;

            var notCanceled = saveDlg.ShowDialog();
            if ((!notCanceled.HasValue) || !notCanceled.Value)
            {
                // action canceled
                return;
            }


            filename = saveDlg.FileName;

            var crl = new WebClient.cURL();
            var downloadResult = await crl.DownloadBinary(url, filename);
            if (downloadResult.Success)
            {
                MessageBox.Show($"Successfully saved {filename}", "downloaded", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"couldn't save file, curl exited with code {downloadResult.agentReturnCode}", "failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        
    }
    }
}
