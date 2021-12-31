using CLW.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CLW.ViewModel
{
    public class SettingsWindowViewModel : BaseViewModel
    {

        public SettingsWindowViewModel()
        {
            OpenMainWindowOnStartup = true;
            
            //this ctor is used for design-time data
            //if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(App.Current.MainWindow))
            //{
                this.PressetsDeclarations = new ObservableCollection<CLWPresetDeclaration>()
                {
                    new CLWPresetDeclaration() {Path="Dummy path 1", AutoLoad=true, AutoStart = false },
                    new CLWPresetDeclaration() {Path="Dummy path 2", AutoLoad=true, AutoStart = false },
                    new CLWPresetDeclaration() {Path="Dummy path 3", AutoLoad=true, AutoStart = false },
                    new CLWPresetDeclaration() {Path="Dummy path 4", AutoLoad=true, AutoStart = false },
                     new CLWPresetDeclaration() {Path="Dummy path 5", AutoLoad=true, AutoStart = false },
                    new CLWPresetDeclaration() {Path="Dummy path 6", AutoLoad=true, AutoStart = false },
                  new CLWPresetDeclaration() {Path="Dummy path 7", AutoLoad=true, AutoStart = false },
                    new CLWPresetDeclaration() {Path="Dummy path 8", AutoLoad=true, AutoStart = false },

                };
            //}
            //else
            //{
            //    throw new Exception("0 Param ctor for the SettingsWindowViewModel is only supported in design-time ");
            //}
        }
        public SettingsWindowViewModel(ConfigService _ConfigModelInstance)
        {
            ConfigModelInstance = _ConfigModelInstance;

            //cloning properties values into the viewModel
            PressetsDeclarations = new ObservableCollection<CLWPresetDeclaration>(
                ConfigModelInstance.CLWPresetsDeclarations.Select((cpd)=> { return new CLWPresetDeclaration() { Path = cpd.Path,AutoLoad=cpd.AutoLoad,AutoStart=cpd.AutoStart }; }));
            RememberDownloadOutputDirectory = ConfigModelInstance.RememberDownloadsOutputDirectory;
            RememberedDownloadOutputDirectory = ConfigModelInstance.RememberedDownloadOutputDirectory;

            OpenMainWindowOnStartup = ConfigModelInstance.OpenMainWindowOnStartup;
            MaxCheckingConcurrency= ConfigModelInstance.MaxCheckingConcurrency;
            CloseNotifWhenUserActive = ConfigModelInstance.CloseNotifWhenUserActive;
            AutoRunCLW = ConfigModelInstance.AutoRunCLW;
            AllowMultipleNotifications = ConfigModelInstance.AllowMultipleNotifications;
            Dev_IsCrapifyProxyEnabled = ConfigModelInstance.Dev_IsCrapifyProxyEnabled;
            Dev_IsMockedWebClient = ConfigModelInstance.Dev_IsMockedWebClient;
            ItemsPerPage = ConfigModelInstance.ItemsPerPage;
        }


        private string rememberedDownloadOutputDirectory;
        public string RememberedDownloadOutputDirectory
        {
            set { rememberedDownloadOutputDirectory = value; notif(nameof(RememberedDownloadOutputDirectory)); }
            get { return rememberedDownloadOutputDirectory; }
        }

        private bool rememberDownloadOutputDirectory;
        public bool RememberDownloadOutputDirectory
        {
            set { rememberDownloadOutputDirectory = value; notif(nameof(RememberDownloadOutputDirectory)); }
            get { return rememberDownloadOutputDirectory; }
        }


        private bool allowMultipleNotifications;
        public bool AllowMultipleNotifications
        {
            set { allowMultipleNotifications = value; notif(nameof(AllowMultipleNotifications)); }
            get { return allowMultipleNotifications; }
        }



        private bool autoRunCLW;
        public bool AutoRunCLW
        {
            set { autoRunCLW = value; notif(nameof(AutoRunCLW)); }
            get { return autoRunCLW; }
        }



        private bool closeNotifWhenUserActive;
        public bool CloseNotifWhenUserActive
        {
            set { closeNotifWhenUserActive = value; notif(nameof(CloseNotifWhenUserActive)); }
            get { return closeNotifWhenUserActive; }
        }



        private int maxCheckingConcurrency;
        public int MaxCheckingConcurrency
        {
            set { maxCheckingConcurrency = value; notif(nameof(MaxCheckingConcurrency)); }
            get { return maxCheckingConcurrency; }
        }



        private bool openMainWindowOnStartup;
        public bool OpenMainWindowOnStartup
        {
            set { openMainWindowOnStartup = value; notif(nameof(OpenMainWindowOnStartup)); }
            get { return openMainWindowOnStartup; }
        }


        private bool dev_IsMockedWebClient;
        public bool Dev_IsMockedWebClient
        {
            set { dev_IsMockedWebClient = value; notif(nameof(Dev_IsMockedWebClient)); }
            get { return dev_IsMockedWebClient; }
        }


        private bool dev_IsCrapifyProxyEnabled;
        public bool Dev_IsCrapifyProxyEnabled
        {
            set { dev_IsCrapifyProxyEnabled = value; notif(nameof(Dev_IsCrapifyProxyEnabled)); }
            get { return dev_IsCrapifyProxyEnabled; }
        }


        private int itemsPerPage;
        public int ItemsPerPage
        {
            set { itemsPerPage = value; notif(nameof(ItemsPerPage)); }
            get { return itemsPerPage; }
        }



        private ObservableCollection<CLWPresetDeclaration> pressetsDeclarations;
        public ObservableCollection<CLWPresetDeclaration> PressetsDeclarations
        {
            set { pressetsDeclarations = value; notif(nameof(PressetsDeclarations)); }
            get { return pressetsDeclarations;
                
            }
            
        }

        public ConfigService ConfigModelInstance { get; private set; }



        public MICommand<object> DeletePresetDeclarationCommand {
            get { return new MICommand<object>(HandleDeletePresetDeclarationCommand); }

        }
        public MICommand AddPresetDeclarationCommand
        {
            get { return new MICommand(HandleAddPresetDeclarationCommand); }

        }
        private void HandleDeletePresetDeclarationCommand(object obj)
        {
            try
            {
                CLWPresetDeclaration pd = obj as CLWPresetDeclaration;
                if (pd != null)
                    PressetsDeclarations.Remove(pd);
            }
            catch (Exception arr)
            {
                MainWindow.ShowMessage(arr?.Message);
            }

        }
        private void HandleAddPresetDeclarationCommand()
        {
            try
            {
               
                    PressetsDeclarations.Add(new CLWPresetDeclaration());
            }
            catch (Exception arr)
            {
                MainWindow.ShowMessage(arr?.Message);
            }

        }

        public MICommand CancelCommand { get { return new MICommand(onClose); } }

        //irrelevent because it's ok to hanfle ths in code behind
        public MICommand SaveCommand { get { return new MICommand(onSave); } }

        public MICommand DevStartMockingServerCommand { get { return new MICommand(()=> {
            Services.Utils.TryOpenFileAsync(@"C:\TOOLS\simulating-fb\run.bat");
        }); } }


        private void onSave()
        {
            //assgning the model instance's properties with the new values and calling save method
            ConfigModelInstance.RememberDownloadsOutputDirectory = this.RememberDownloadOutputDirectory;
            ConfigModelInstance.RememberedDownloadOutputDirectory = this.RememberedDownloadOutputDirectory;
            ConfigModelInstance.CLWPresetsDeclarations = this.PressetsDeclarations;

            ConfigModelInstance.OpenMainWindowOnStartup = this.OpenMainWindowOnStartup;
            ConfigModelInstance.MaxCheckingConcurrency = this.MaxCheckingConcurrency;
            ConfigModelInstance.CloseNotifWhenUserActive = this.CloseNotifWhenUserActive;
            ConfigModelInstance.AutoRunCLW = this.AutoRunCLW;
            ConfigModelInstance.AllowMultipleNotifications = this.AllowMultipleNotifications;
            ConfigModelInstance.Dev_IsCrapifyProxyEnabled = this.Dev_IsCrapifyProxyEnabled;
            ConfigModelInstance.Dev_IsMockedWebClient = this.Dev_IsMockedWebClient;

            ConfigModelInstance.ItemsPerPage = this.ItemsPerPage;
   
            ConfigModelInstance.Save();

            //closiing the window
            onClose();
        }

        private void onClose()
        {
            if (OnRequestClose != null) OnRequestClose(this, new EventArgs());
        }

        public event EventHandler OnRequestClose;


    }
}
