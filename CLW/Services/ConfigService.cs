using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CLW.Services
{
    [Serializable]
    /// <summary>
    /// the MVVM alternative for the Config class, most code buit on it
    /// singletone, gets created once it's first required, and says around untile the app shuts down
    /// </summary>
    public class ConfigService : INotifyPropertyChanged
    {
        [NonSerialized]
        private static ConfigService _instance;
        public static ConfigService Instance
        {
            get
            { if (_instance != null) return _instance;
              else { _instance = ConfigService.Load();
                    Trace.WriteLine("ConfigService Load ended");

                    return _instance; }
            }
        }

        private ConfigService()
        {
            CCInstances++;

        }

        
        

        


        private ObservableCollection<CLWPresetDeclaration> cLWPresetsDeclarations  = new ObservableCollection<CLWPresetDeclaration>();
        private string globalOutputDirectory = MI.DEFAULT_GLOBAL_OUTPUT_DIR;
        private string rememberedDownloadOutputDirectory  = MI.DEFAULT_GLOBAL_OUTPUT_DIR;
        private OverrideBehaviour defaultOverrideBehaviour  = OverrideBehaviour.Override;
        private bool rememberDownloadsOutputDirectory  = true;
        private DownloadClients defaultDownloadClient = DownloadClients.native;
        private bool openMainWindowOnStartup = false;
        private int maxCheckingConcurrency = 3;
        private int itemsPerPage = 7;

        public ObservableCollection<CLWPresetDeclaration> CLWPresetsDeclarations
        {
            get { return cLWPresetsDeclarations;  }
            set { cLWPresetsDeclarations = value; notif(nameof(CLWPresetsDeclarations)); }
        } 
        public string GlobalOutputDirectory
        {
            get { return globalOutputDirectory; }
            set { globalOutputDirectory = value; notif(nameof(GlobalOutputDirectory)); }
        }
        public string RememberedDownloadOutputDirectory
        {
            get { return rememberedDownloadOutputDirectory; }
            set { rememberedDownloadOutputDirectory = value; notif(nameof(RememberedDownloadOutputDirectory)); }
        }
        public OverrideBehaviour DefaultOverrideBehaviour
        {
            get { return defaultOverrideBehaviour; }
            set { defaultOverrideBehaviour = value; notif(nameof(DefaultOverrideBehaviour)); }
        }
        public bool RememberDownloadsOutputDirectory
        {
            get { return rememberDownloadsOutputDirectory; }
            set { rememberDownloadsOutputDirectory = value; notif(nameof(RememberDownloadsOutputDirectory)); }
        }
        public DownloadClients DefaultDownloadClient
        {
            get { return defaultDownloadClient; }
            set { defaultDownloadClient = value; notif(nameof(DefaultDownloadClient)); }
        }
        public bool OpenMainWindowOnStartup
        {
            get { return openMainWindowOnStartup; }
            set { openMainWindowOnStartup = value; notif(nameof(OpenMainWindowOnStartup)); }
        }

        public int MaxCheckingConcurrency
        {
            get { return maxCheckingConcurrency; }
            set { maxCheckingConcurrency = value; notif(nameof(MaxCheckingConcurrency)); }
        }

        public int ItemsPerPage
        {
            get { return itemsPerPage; }
            set { itemsPerPage = value; notif(nameof(ItemsPerPage)); }
        }

        public bool CloseNotifWhenUserActive { get; set; }
        public bool AutoRunCLW { get; set; }
        public bool AllowMultipleNotifications { get;set;}


    public ConfigService Save(string saveAS = null)
        {
            if (saveAS == null) saveAS = MI.APP_CONFIG_FILE_V2;
            using (var stream = File.Open(saveAS, FileMode.Create))
            {
                sr.Serialize(stream, this);
            }
            Trace.WriteLine("configservice saved");
            MainWindow.ShowMessage("Settings Saved");
            return this;
        }



        /// <summary>
        /// attemps to load the xml config file, if file is missing the factory config is automatically saved and returned
        /// throws unhandled exceptions if file deserialization fails
        /// </summary>
        /// <param name="ConfigFile">if not specified, the MI.APP_CONFIG_FILE_V2 is used  </param>
        /// <returns></returns>
        private static ConfigService Load(string ConfigFile = null)
        {
            Trace.WriteLine("ConfigService Load was called");
            if (ConfigFile == null) ConfigFile = MI.APP_CONFIG_FILE_V2;
            if (!File.Exists(ConfigFile))
            {
                return ConfigService.FactoryConfig().Save();
            }
            using (var stream = File.OpenRead(ConfigFile))
            {
                return sr.Deserialize(stream) as ConfigService;
            }
           
        }


        /// <summary>
        /// makes up a new config object, used to reset factory setting when requested by the user or when the xml conf is missing or is corrupt
        /// NOTE: this doesnt change the Instance, but rathr creates and returns a new object
        /// </summary>
        private static ConfigService FactoryConfig()
        {
            var facto = new ConfigService();
            facto.CLWPresetsDeclarations = new ObservableCollection<CLWPresetDeclaration>();
            facto.CLWPresetsDeclarations.Add(new CLWPresetDeclaration(){ Path = @"C:\TOOLS\fbhd-gui\xml\moodleUploadsDummy.clw", AutoLoad = true, AutoStart = false});
            facto.CLWPresetsDeclarations.Add(new CLWPresetDeclaration() { Path = @"C:\TOOLS\fbhd-gui\xml\FsdmUploads.clw", AutoLoad = true, AutoStart = false });
            facto.CLWPresetsDeclarations.Add(new CLWPresetDeclaration() { Path = @"C:\TOOLS\fbhd-gui\xml\moodleUploadsDummy.clw", AutoLoad = true, AutoStart = false });
            return facto;
        }


   




        static XmlSerializer sr = new XmlSerializer(typeof(ConfigService));
        public static int CCInstances;
        internal bool Dev_IsMockedWebClient = true;
        internal bool Dev_IsCrapifyProxyEnabled = true;

        public event PropertyChangedEventHandler PropertyChanged;
        private void notif(string propertyName)
        {
           

                
            
            Trace.WriteLine("ConfigServce Instance is dirty");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



    }

    

}
