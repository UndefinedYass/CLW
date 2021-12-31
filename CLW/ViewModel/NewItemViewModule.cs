using CLW.Model;
using CLW.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CLW.ViewModel
{
    public class NewItemViewModel : BaseViewModel
    {
        public NewItemViewModel(NewModel nw)
        {
            this.Model = nw;
            DownloadCommand = new MICommand(handleDownload, () => Model.HasDownload);
            Extension = "ljlk"; //setter call
            ///asynchronous worker that checks file existance, folowed by the UI updating
            /// todo: after determining the HasDownloded bool and Downloaded File Path, thse shold be stored in the model 
            /// to avoid dong the work again when the user switches between watchers or re-load the window
            resolveDownloadedFilePropertesAsync();
        }



        /// <summary>
        /// does the asynchrounous work needed, and uspdates HasDownloaded & DownloadedFilePath properties, 
        /// </summary>
        private void resolveDownloadedFilePropertesAsync()
        {
            Task<string> fileChecckingTsk = new Task<string>(() => {
                string filepath;
                bool exists = Utils.GetDownloadedFileExists(model, out filepath);
                return exists ? filepath : null;
            });
            fileChecckingTsk.GetAwaiter().OnCompleted(() =>
            {
                this.HasDownloaded = fileChecckingTsk.Result != null;
                this.DownloadedFilePath = fileChecckingTsk.Result;
            });
            fileChecckingTsk.Start();
        }



        #region Properties


        private NewModel model;
        public NewModel Model
        {
            set { model = value; notif(nameof(Model)); }
            get { return model; }
        }


        //will be use in more advanced NewView versions where the non standard props are supported
        private bool isExpanded;
        public bool IsExpanded
        {
            set { isExpanded = value; notif(nameof(IsExpanded)); }
            get { return isExpanded; }
        }


        //will be used to provide file opening feature 
        private bool hasDownloaded=false;
        public bool HasDownloaded
        {
            set { hasDownloaded = value; notif(nameof(HasDownloaded)); }
            get { return hasDownloaded; }
        }


        private string downloadedFilePath=null;
        public string DownloadedFilePath
        {
            set { downloadedFilePath = value; notif(nameof(DownloadedFilePath)); }
            get { return downloadedFilePath; }
        }



        private string extension;
        public string Extension
        {
            set { extension = value; notif(nameof(Extension)); }
            get { return  Path.GetExtension(Model.DownloadLink).Replace(".","").ToUpper(); }
        }

        #endregion

        #region Commands & Handlers

       
        

        private async void handleDownload()
        {
            bool IsCTRLClick = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            var url = Model.DownloadLink;
            //url = "http://fsdmfes.ac.ma/uploads/Docs/Files/2021-05-20-01-54-05_183bddf1b0d6b398ccf9be4dfe32f87bb0364a09.pdf";

            Uri asUri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out asUri))
            {
                MessageBox.Show("bad url"); return;
            }


            var filename = asUri.LocalPath;
            //21-sept-2021 remembering directory feature
            var config = ConfigService.Instance;

            filename = System.IO.Path.GetFileName(filename);
            if (!IsCTRLClick && config.RememberDownloadsOutputDirectory && Directory.Exists(config.RememberedDownloadOutputDirectory))
            {
                //rememering location
                filename = System.IO.Path.Combine(config.RememberedDownloadOutputDirectory,
                    filename);
            }
            else
            {
                //prompting location
                var saveDlg = new Ookii.Dialogs.Wpf.VistaSaveFileDialog();
                saveDlg.FileName = filename;

                var notCanceled = saveDlg.ShowDialog();
                if ((!notCanceled.HasValue) || !notCanceled.Value)
                {
                    // action canceled
                    return;
                }
                filename = saveDlg.FileName;
                config.RememberedDownloadOutputDirectory = System.IO.Path.GetDirectoryName(filename);
                config.Save();
            }


            //downloading




            
            var downloadResult = await Services.WebClientMi.Native.getNewInstance().DownloadBinary(url, filename,null);
            if (downloadResult.Success)
            {
                //Downloaded ∙ 33 Mb ∙ PDF
                //Downloaded • 33Mb • PDF
                //Downloaded | 33Mb | PDF
                //Downloaded ∙ 33Mb|PDF  • 33Mb ·|∙
                string wrapedFileName = System.IO.Path.GetFileName(filename);
                resolveDownloadedFilePropertesAsync();//todo this is CPU consuming while all informations are available 
                string s = "33 MB";
                string x = Extension;
                string msg = $"Downloaded ∙ {s} ∙ {x}";
                ((MainWindow)Application.Current.MainWindow).Snackbar
            .MessageQueue.Enqueue(msg, "Open", (fileToOpen) =>
            {
                Process.Start(fileToOpen);
            }, filename);
                //MessageBox.Show($"Successfully saved {filename}", "downloaded", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"couldn't save file, curl exited with code {downloadResult.agentReturnCode}", "failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public MICommand DownloadCommand { get; set; }
        public MICommand MarkeSeenCommand
        {
            get
            {
                return new MICommand(() => { Model.IsRead = true; }, () => !Model.IsRead );
            }
        }

        public MICommand OpenDownloadedFileCommand
        {
            get
            {
                return new MICommand(handleOpenDownloadedFile);
            }
        }

        private void handleOpenDownloadedFile()
        {
            Utils.TryOpenFileAsync(DownloadedFilePath);
        }

        #endregion

    }
}
