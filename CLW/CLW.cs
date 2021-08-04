using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows;

using System.Windows.Data;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net.WebSockets;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Xml;

using System.Security.Policy;
using System.Dynamic;
using System.Xml.Serialization;
using System.Windows.Documents;

namespace CLW
{

    //cff : denotes that this part is copied from the fbhd project








    public class ApplicationInfo
    {

        public ApplicationInfo()
        {
            // Environment.CurrentDirectory
        }

        public static bool IsDev { get; set; } = true;
        public static string CLW_APP_TITLE { get; set; } = "CL Watcher 1.0";
        public static string CLW_APP_SUB_TITLE { get; set; } = "© Mi 2021 ";
        public static string CLW_VERSION { get; set; } = "0.1.0-fb0.6.0 (?)" + (IsDev ? " [dev]" : "");
        public static string CLW_DEVELOPER { get; set; } = "Yass.Mi";
        public static string CLW_GUI_DESIGNER { get; set; } = "Yass.Mi";
        public static string CLW_GITHUB_URL { get; set; } = "https://github.com/UndefinedYass/clw";


        public static string CLW_DEVELOPER_EMAIL { get; set; } = "DIR16CAT17@gmail.com";
    }
    public class MI
    {
        public static string MAIN_PATH = Path.GetDirectoryName(
           System.Reflection.Assembly.GetExecutingAssembly().Location);

        public static string CURL_PATH = MAIN_PATH + "\\curl\\curl.exe";
        public static string APP_DATA = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Mi\\CLW";
        public static MainWindow mw = (MainWindow)Application.Current.MainWindow;

        public static string DEFAULT_GLOBAL_OUTPUT_DIR = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CLW Output";

        public static string CLW_PRESETS_DIR = MAIN_PATH + "\\CLW Presets";
        public static string APP_CONFIG_FILE = APP_DATA + "\\config.mi.xml";
        internal static string TEMP_HTML_FILES = APP_DATA + "\\Temp HTML";
        internal static string SCRIPTS_DIR = MAIN_PATH + "\\scripts";
        internal static string regexTestPattern = "\\\\x3CPeriod duration=\\\\\\\"PT(.*?)H(.*?)M(.*?)S\\\">";
        internal static object SFX_DIRECTORY = MAIN_PATH + "\\SFX";
        internal static string ERRORS_LOG_FILE = APP_DATA + @"\Errors.log";

       

       

       


        public static async void DumpError(Exception exception, string source)
        {
            File.AppendAllText(MI.ERRORS_LOG_FILE, $"Error, {DateTime.Now.ToString("d/MM/yyyy hh:mm:ss")}, In [{source}] : '{exception.Message}'{Environment.NewLine}");
        }



        public static async void Verbose(string VerboseStatus, int DurationInSeconds = -1)
        {
            await mw.Dispatcher.Invoke(async () =>
            {
                if (VerboseStatus == null)
                {
                    //mw.verboseStatusStack.Clear(); //not implemented yet
                    //mw.verboseStatus = null;  //not implemented yet
                    return;
                }
                //mw.verboseStatusStack.Add(VerboseStatus); //not implemented yet
                //mw.verboseStatus = mw.verboseStatusStack.Last(); //not implemented yet
                if (DurationInSeconds != -1)
                {
                    await Task.Delay(1000 * DurationInSeconds);
                    //mw.verboseStatusStack.Remove(VerboseStatus); //all comments in this section are not implemented yet

                    //if (mw.verboseStatusStack.Count > 0)
                    //{
                        //mw.verboseStatus = mw.verboseStatusStack.Last();
                    //}
                    //else
                    //{
                        //mw.verboseStatus = null;
                    //}
                }
            });
        }

        public static void updateLogger(string VerboseStatus)
        {

            /*mw.Dispatcher.Invoke(() =>
            {
                mw.comma.Text = VerboseStatus;
            });*/

        }

        internal static string TestingFunc(string text)
        {
            return TimeSpan.Parse(text).ToString();
        }
    }














    public enum DownloadClients { curl, powershell, ffmpeg }

    [Serializable]
    public class CLWPresetDeclaration
    {
        public string Path { get; set; }
        public bool AutoStart { get; set; }
        public bool AutoLoad { get; set; }
    }



    [Serializable]
    public class Config
    {


        public List<CLWPresetDeclaration> CLWPresetsDeclarations { get; set; } = new List<CLWPresetDeclaration>();


        public string GlobalOutputDirectory { get; set; } = MI.DEFAULT_GLOBAL_OUTPUT_DIR;

        public List<string> RecentGlobalDirectories { get; set; } = new List<string>();
        public OverrideBehaviour DefaultOverrideBehaviour { get; set; } = OverrideBehaviour.Override;
        public bool DownloadRawStreams { get; set; } = false;
        public bool AutoStartServer { get; set; } = false;
        public bool UseChunckedDownloading { get; set; } = false;
        public DownloadClients DefaultDownloadClient { get; set; } = DownloadClients.curl;

        static XmlSerializer sr = new XmlSerializer(typeof(Config));

        public void Save(string saveAS = null)
        {
            if (saveAS == null) saveAS = MI.APP_CONFIG_FILE;
            using (var stream = File.Open(saveAS, FileMode.Create))
            {
                sr.Serialize(stream, this);
            }
        }

        public static Config Load(string ConfigFile = null)
        {
            if (ConfigFile == null) ConfigFile = MI.APP_CONFIG_FILE;
            if (!File.Exists(ConfigFile))
            {
                Config.FactoryConfig().Save();
                return FactoryConfig();
            }
            using (var stream = File.OpenRead(ConfigFile))
            {
                return sr.Deserialize(stream) as Config;
            }
        }

        public static Config FactoryConfig()
        {
            return new Config();

        }


        /// <summary>
        /// updates the RecentGlobalDirectories list adding the new entry at the top and removing excess entries if any
        /// </summary>
        /// <param name="pickedDir"></param>
        internal void StackRecentDirectory(string pickedDir)
        {
            RecentGlobalDirectories.Remove(pickedDir);
            RecentGlobalDirectories.Insert(0, pickedDir);
            int excess = RecentGlobalDirectories.Count - 5;
            if (excess > 0)
            {
                RecentGlobalDirectories.RemoveRange(5, excess);

            }
        }


    }













    public class Session : INotifyPropertyChanged
    {

        private string printSpectioalFolder(Environment.SpecialFolder sf)
        {
            return sf.ToString() + ": " + Environment.GetFolderPath(sf);
        }

        public ApplicationInfo AppInfo { get; set; } = new ApplicationInfo();


        public Session()
        {
            AppInfo = new ApplicationInfo();
            OnPropertyChanged(nameof(AppInfo));

            if (!Directory.Exists(MI.TEMP_HTML_FILES)) Directory.CreateDirectory(MI.TEMP_HTML_FILES);
            bool curlExists = File.Exists(MI.CURL_PATH);

            Directory.CreateDirectory(MI.APP_DATA);

            Trace.Assert(curlExists, $"couldnt find the curl executable at {MI.CURL_PATH} the app will not be able to work correctely unless another location is specidied in the settings or it is defined as an envirenment variable.");

            MainConfig = Config.Load();
            if (string.IsNullOrWhiteSpace(MainConfig.GlobalOutputDirectory) == false)
                GlobalOutputFolder = MainConfig.GlobalOutputDirectory;

            if (MainConfig == null) MainConfig = Config.FactoryConfig();

            mp.Open(new Uri($"{MI.SFX_DIRECTORY}\\mi-notif-5th.wav"));




            CustomListWatchers = new BindingList<CustomLW>();
            foreach (var presetDeclaration in MainConfig.CLWPresetsDeclarations)
            {
                string presetPath = presetDeclaration.Path;
                if (presetDeclaration.AutoLoad)
                {
                    CustomLW clwObject = null;
                    Task.Run(async () =>
                    {

                        clwObject = await LoadXMLLW(presetPath);


                    }).GetAwaiter().GetResult();
                    if (clwObject != null)
                    {
                          MessageBox.Show(clwObject.Name);
                    }
                    if (presetDeclaration.AutoStart)
                        clwObject.StartWatching();

                }

            }
            /// starting the clw file passed by the commandline if any

            if (Environment.GetCommandLineArgs().Length > 1)
            {
                string maybeFile = Environment.GetCommandLineArgs()[1];
                if (File.Exists(maybeFile) && (Path.GetExtension(maybeFile).ToLower() == ".clw"))
                {
                    CustomLW CommandlineCLW = null;
                    Task.Run(async () =>
                    {
                        CommandlineCLW = await LoadXMLLW(Path.GetFullPath(maybeFile));
                    }).GetAwaiter().GetResult();
                    if (CommandlineCLW != null)
                    {
                    }

                    CommandlineCLW.StartWatching();
                }
            }

            OnPropertyChanged(nameof(ExistLoadedCLWatchers));
            // Tasks = new BindingList<FBHDTask>();
            //SelectedTask = null;
        }



        MainWindow mw = (MainWindow)Application.Current.MainWindow;

        private string globalOutputFolder = MI.DEFAULT_GLOBAL_OUTPUT_DIR;
        private int runningPy_fetcherCount = 0;





        private CustomLW selectedCustomListWatcher;
        public CustomLW SelectedCustomListWatcher
        {
            set
            {
                selectedCustomListWatcher = value;
                OnPropertyChanged(nameof(SelectedCustomListWatcher));

            }
            get { return selectedCustomListWatcher; }
        }



        private BindingList<CustomLW> customListWatchers;
        public BindingList<CustomLW> CustomListWatchers
        {
            set
            {
                customListWatchers = value;
                OnPropertyChanged(nameof(CustomListWatchers));

            }
            get { return customListWatchers; }
        }


        public bool ExistLoadedCLWatchers
        {
            get { return CustomListWatchers.Count > 0; }
        }



        public string GlobalOutputFolder
        {
            get
            {
                return globalOutputFolder;
            }
            set
            {
                value = value.TrimEnd(new char[] { '\\' }) + "\\";
                globalOutputFolder = value;
                OnPropertyChanged(nameof(GlobalOutputFolder));
            }
        }
      
        public int RunningPy_fetcherCount
        {
            get
            {
                return runningPy_fetcherCount;
            }
            set
            {
                runningPy_fetcherCount = value;
                OnPropertyChanged(nameof(RunningPy_fetcherCount));
            }
        }

        public Config MainConfig { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == nameof(CustomListWatchers))
            {
                OnPropertyChanged(nameof(ExistLoadedCLWatchers));
            }
        }


        private MediaPlayer mp = new MediaPlayer();
        internal void PlayNotification()
        {


            // mp.Volume = 1;
            mp.Position = TimeSpan.FromMilliseconds(0);
            mp.Play();
        }



        /// <summary>
        /// loads the file content, parses it, adds a listwatcher objct to the CustomListWatchers 
        /// returnng the new CustomLW instance on succes and null on failure
        ///  </summary>
        internal async Task<CustomLW> LoadXMLLW(string fileName)
        {
            CustomLW newCustomLW = XMLLW.LoadXMLPreset(File.ReadAllText(fileName));


            if (File.Exists(newCustomLW.ReferenceFilePath))
            {
                newCustomLW.InitialReferenceContent = File.ReadAllText(newCustomLW.ReferenceFilePath);

                if (string.IsNullOrEmpty(newCustomLW.InitialReferenceContent))
                {
                    MessageBox.Show($"Empty ref file! FBHD will attemp to fetch content from {newCustomLW.Href} and override the file:\n {newCustomLW.ReferenceFilePath}  ");
                    var userConfirmation = MessageBox.Show($"Empty ref file! FBHD will attemp to fetch content from {newCustomLW.Href} and override the file:\n {newCustomLW.ReferenceFilePath}  "
                        , "Empty ref file", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (userConfirmation == MessageBoxResult.Cancel)
                    {
                        return null;
                    }
                    var data = await WebClient.cURL.GetTextStatic(newCustomLW.Href, Headers.FakeUserAgentHeaders, true);
                    if (data.Success)
                    {

                        File.WriteAllText(newCustomLW.ReferenceFilePath, data.Text);
                        newCustomLW.InitialReferenceContent = data.Text;
                    }
                    else
                    {
                        MessageBox.Show($"couldnt fetch the data, try later\n errorcoe:{data.ClientExitCode}");
                        return newCustomLW;// //done//todo: shold inform the caller that things went wrong
                    }
                }
            }

            else
            {
                var userConfirmation = MessageBox.Show($@"Reference file does not exist at {newCustomLW.ReferenceFilePath}{Environment.NewLine}
                FBHD will attempt to create one with initial data from {newCustomLW.Href}", "Missing ref", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (userConfirmation == MessageBoxResult.Cancel)
                {
                    return null;
                }
                var data = await WebClient.cURL.GetTextStatic(newCustomLW.Href, Headers.FakeUserAgentHeaders, true);

                if (data.Success)
                {
                    // create the directory first if it does not exist
                    if (Directory.Exists(Path.GetDirectoryName(newCustomLW.ReferenceFilePath)) == false)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(newCustomLW.ReferenceFilePath));
                    }
                    File.WriteAllText(newCustomLW.ReferenceFilePath, data.Text);
                    newCustomLW.InitialReferenceContent = data.Text;

                }
                else
                {
                    MessageBox.Show($"couldnt fetch the data, try later\n errorcoe:{data.ClientExitCode}");
                    return null;
                }

            }


            newCustomLW.NewItems += (s, news) =>
            {
                PlayNotification();
                mw.ShowNotificationNews(news, newCustomLW); //niy
            };
            CustomListWatchers.Add(newCustomLW);
            OnPropertyChanged(nameof(ExistLoadedCLWatchers));
            return newCustomLW;

        }






        /// <summary>
        /// changes the current output directory and,
        /// and updates the recentOutputDirecories list calling StackRecentDirectory
        /// and saves changes 
        /// 
        /// </summary>
        /// <param name="pickedDir">string specifying the path</param>
        internal void SetOutputDirectory(string pickedDir)
        {
            if (string.IsNullOrWhiteSpace(pickedDir)) return;
            if (!Directory.Exists(pickedDir))
            {
                //this is nreached code because pickes doesn't allow the user to select a non exstent directory path
                var yesNo = MessageBox.Show("Picked directory does not exist, do you want to create it?", "Directory does not exist", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (yesNo == MessageBoxResult.Yes)
                {
                    // todo: creat output folder
                    Directory.CreateDirectory(pickedDir);

                }
                else
                {
                    return;
                }
            }
            GlobalOutputFolder = pickedDir;
            MainConfig.GlobalOutputDirectory = pickedDir;
            MainConfig.StackRecentDirectory(pickedDir);
            MainConfig.Save();

        }
    }



















    //cff

    /// <summary>
    /// base class for watching a static web page that has a list-like patern, by Checking it's content regularely with the pre configured Interval
    /// notifies changes through firing the events NewItems, RevokedItems ;
    /// the OnError event is fired in when the client fails to connect or when the list parser method fails 
    /// NOTE/ it's important for the ID property of an item to be unique and stay unchanged over time in order for this to work
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ListWatcher<T> : INotifyPropertyChanged, IWatch where T : ListWatcherItem
    {



        public event EventHandler<List<T>> NewItems;
        public event EventHandler<List<T>> RevokedItems;

        public event EventHandler<string> OnError;
        public event PropertyChangedEventHandler PropertyChanged;





        private int interval = 60000;
        public int Interval
        {
            set
            {
                if (value < 30000) value = 30000;
                interval = value;
                notif(nameof(Interval));
                notif(nameof(IntervalAsString));

            }
            get { return interval; }
        }




        public int ChecksCount { get; set; } = 0; // countes both successfull and unsuccessful checkings
        public TimeSpan WatchingFor { get; set; }



        internal void notif(string propertyName)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private bool isWatching = false;
        public bool IsWatching
        {
            get { return isWatching; }
            private set
            {
                isWatching = value;
                notif(nameof(IsWatching));
                notif(nameof(StatusMessage));


            }
        }


        public bool IsFailing
        {

            get { return FailingCount > 0; }
        }

        public bool IsRunningSuccessfully
        {
            get { return !IsFailing; }
        }



        /// <summary>
        /// seccessful checking counter, starts when first initalized
        /// </summary>
        private int successfulCheckCount;
        public int SuccessfulCheckCount
        {
            set
            {
                successfulCheckCount = value;
                notif(nameof(SuccessfulCheckCount)); notif(nameof(StatusMessage));
            }
            get { return successfulCheckCount; }
        }





        public void MarkAllAsRead()
        {
            UnreadNews.Clear();
            UnreadNews = new BindingList<ExpandoLWItemObject>();
            notif(nameof(HasUnreadNews));
            notif(nameof(UnreadCount));
            UpdateReferenceFile();

        }

        public void MarkAsRead(IEnumerable<ExpandoLWItemObject> what)
        {
            foreach (var item in what)
            {
                UnreadNews.Remove(item);
            }
            notif(nameof(HasUnreadNews));
            notif(nameof(UnreadCount));
        }


        private BindingList<ExpandoLWItemObject> unreadNews;
        public BindingList<ExpandoLWItemObject> UnreadNews
        {
            set
            {
                unreadNews = value;
                notif(nameof(UnreadNews));
                notif(nameof(HasUnreadNews));
                notif(nameof(UnreadCount));
            }
            get { return unreadNews; }
        }


        public bool HasUnreadNews
        {
            set { notif(nameof(HasUnreadNews)); }
            get { return UnreadNews.Count > 0; }
        }


        public int UnreadCount
        {
            set { notif(nameof(UnreadCount)); }
            get { return UnreadNews.Count; }
        }






        public string StatusMessage
        {
            set { notif(nameof(StatusMessage)); }
            get { return !IsWatching ? "Disabled" : IsFailing ? $"Failed [{FailingCount}]" : $"Running [{SuccessfulCheckCount}]"; }
        }



        /// <summary>
        /// unsuccessful checkings cc, gets reset to 0 when a successfull check occurs
        /// </summary>
        private int failingCount;
        public int FailingCount
        {
            set
            {
                failingCount = value;
                notif(nameof(FailingCount));
                notif(nameof(IsFailing));
                notif(nameof(IsRunningSuccessfully));
                notif(nameof(StatusMessage));


            }
            get { return failingCount; }
        }




        /// <summary>
        /// short description: only assign something that Fucs.TimeSpanFromString can 
        /// parse, and expect the getter to return "03:30"-like strings
        /// this property works as a parser/converter for the actual int Interval property 
        /// simplifing the two way binding for the UI IncreaseTextBox control
        /// NOTE: when passing a string tha cannot be resolved the value 00:30 is supplied 
        /// </summary>
        public string IntervalAsString
        {
            set
            {
                var ts = Fucs.TimeSpanFromString(value);
                if (!ts.HasValue)
                {
                    Interval = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
                }
                else
                {
                    Interval = (int)ts.Value.TotalMilliseconds;
                }

            }
            get { return TimeSpan.FromMilliseconds(Interval).ToString().Substring(3); }
        }





        public abstract string Href { get; }
        public string InitialReferenceContent { get; set; }
        public List<T> CurrentReferenceList { get; set; }
        public string ReferenceFilePath { get; set; }

        //internal abstract string GetPageContent(string href);

        /// <summary>
        /// returns false on any parsing failure, 
        /// on successful parsing it assigns a new list oject to the out parsedList argument
        /// </summary>
        /// <returns></returns>
        internal abstract bool ParseList(string pageContent, out List<T> parsedList);


        /// <summary>
        /// iterates through the newList items and searchs for new ones that does not exist in the oldList, 
        /// returns false if not new items were found, true otherwise
        /// the passed newItems object should be pre-assigned cuw the new items will be added ther using its .add() method
        /// </summary>
        private bool findNewItems(List<T> oldList, List<T> newList, List<T> newItemsOutput)
        {
            bool newsExist = false;
            foreach (var item in newList)
            {
                int ix = oldList.FindIndex((One) => One.ID == item.ID);
                if (ix == -1)
                {
                    newItemsOutput.Add(item);
                    newsExist = true;
                }
            }

            return newsExist;
        }

        public string currentContent;


        /// <summary>
        /// iterates through the oldList items and searchs for ones that are missing in the newList, 
        /// returning true when revoked items exist, false otherwise
        /// the passed revokedItems object should be pre-assigned cuz the revoked items will be added there using its .add() method
        /// Note: the results relies only on the items's ID
        ///  </summary>
        private bool findRevokedItems(List<T> oldList, List<T> newList, List<T> revokedItemsOutput)
        {
            bool Exist = false;
            foreach (var item in oldList)
            {
                int ix = newList.FindIndex((One) => One.ID == item.ID);
                if (ix == -1)
                {
                    revokedItemsOutput.Add(item);
                    Exist = true;
                }
            }

            return Exist;
        }


        struct CheckingResult
        {
            public bool ExistChanges;
            public bool Success;
            public Exception Error;
        }

        private async Task<CheckingResult> Check()
        {



            var r = await WebClient.cURL.GetTextStatic(Href, Headers.FakeUserAgentHeaders, true);

            if (!r.Success)
            {
                return new CheckingResult() { Success = false, Error = new Exception("Couldnt fetch: curl exited wih code: " + r.ClientExitCode.ToString()), ExistChanges = false };

            }
            currentContent = r.Text;


            if (currentContent == null)
            {
                return new CheckingResult() { Success = false, Error = new Exception("couldent fetch"), ExistChanges = false };

            }

            // File.WriteAllText(MI.MAIN_PATH + "\\aaaareeeef.html", currentContent);

            // MessageBox.Show("saved html");

            List<T> currentList;
            bool successfulParsing = ParseList(currentContent, out currentList);
            if (!successfulParsing)
            {
                return new CheckingResult() { Success = false, Error = new Exception("mi: parsing method failed") };
            }
            List<T> maybeNewItems = new List<T>();
            List<T> maybeRevokedItems = new List<T>();
            bool existNews = findNewItems(CurrentReferenceList, currentList, maybeNewItems);
            bool existRevoked = findRevokedItems(CurrentReferenceList, currentList, maybeRevokedItems);
            bool existChanges = existNews || existRevoked;
            if (existChanges)
            {
                CurrentReferenceList = currentList;
                if (existNews)
                {
                    if (NewItems != null) NewItems(this, maybeNewItems);
                }
                if (existRevoked)
                {
                    if (RevokedItems != null) RevokedItems(this, maybeRevokedItems);
                }
            }

            return new CheckingResult() { Success = true, ExistChanges = existChanges };



        }

        public async void StartWatching()
        {
            Debug.WriteLine($"StartWatching() was called", "ListWatcher<>");

            if (CurrentReferenceList == null)
            {
                Debug.WriteLine($"CurrentReferenceList is null", "ListWatcher<>");

                if (string.IsNullOrWhiteSpace(InitialReferenceContent))
                {
                    throw new Exception("mi: ListWatcher: cannot start watching before assigning the initialContentReference");
                }
                List<T> initialList;
                bool succesfullInitialization = ParseList(InitialReferenceContent, out initialList);
                if (succesfullInitialization)
                {
                    CurrentReferenceList = initialList;
                    Debug.WriteLine($"initialized CurrentReferenceList ({CurrentReferenceList.Count} Items)", "ListWatcher<>");

                }
                else
                {
                    throw new Exception("mi: canot init the watcher because parsing the initialContentReference failed");
                }

            }
            IsWatching = true;
            while (IsWatching)
            {
                CheckingResult res = await Check();
                if (res.Success == false)
                {
                    FailingCount++;
                    Debug.WriteLine($"Check() method returned unsuccessfull CheckingResult with Error: {res.Error} ", "ListWatcher<>");
                    Debug.WriteLine($"Firing the OnError event", "ListWatcher<>");

                    if (OnError != null) OnError(this, "could'nt fetch");


                }
                else
                {
                    if (FailingCount > 0) FailingCount = 0;
                    SuccessfulCheckCount++;
                    Debug.WriteLine($"Check() method succeed, CurrentReferenceList has {CurrentReferenceList.Count} items ", "ListWatcher<>");

                }
                await Task.Delay(Interval);
            }

        }



        public void StopWatching()
        {
            IsWatching = false;
        }


        internal void UpdateReferenceFile()
        {
            File.WriteAllText(this.ReferenceFilePath, currentContent);
        }


    }








    public class XMLLW
    {

        public static class MESSAGES
        {

            public static string missingProp(string elementName, string propName)
            {
                return $"Mi: {elementName} element missing the '{propName}' property";
            }
            public static string missingElem(string elementName)
            {
                return $"Mi: {elementName} element missing";
            }

            public const string listWatcher_missing_name_prop = "Mi: ListWatcher is missing the name property";
            public const string zez = "Mi: ListWatcher";
            public const string zezr = "Mi: ListWatcher";
            public const string zerz = "Mi: ListWatcher";
            public const string err = "Mi: ListWatcher";
            public const string erre = "Mi: ListWatcher";
            public const string rte = "Mi: ListWatcher";

        }



        public static CustomLW LoadXMLPreset(string PrestXmldata)
        {

            XDocument d = XDocument.Parse(PrestXmldata);

            XElement listWatcher = null;
            foreach (var item in d.Nodes())
            {
                if (item.NodeType == System.Xml.XmlNodeType.Element)
                {
                    //MessageBox.Show(((XElement)item).Name.LocalName);
                    if (((XElement)item).Name == "ListWatcher")
                    {
                        listWatcher = (XElement)item;
                    }
                }
            }
            Trace.Assert(listWatcher != null, XMLLW.MESSAGES.missingElem("ListWatcher"));

            XElement ItemClass = listWatcher.Element("ItemClass");
            Debug.Assert(ItemClass != null, XMLLW.MESSAGES.missingElem("ItemClass"));
            XElement ListParser = null;

            foreach (var item in listWatcher.Descendants())
            {
                if (CLW.Tag.Type(item) == CLW.Tag.TagType.ListParser)
                    ListParser = (XElement)item;
            }
            Debug.Assert(ListParser != null, XMLLW.MESSAGES.missingElem("ListParser"));

            XAttribute nam = listWatcher.Attribute("name");
            string presetName = nam == null ? null : nam.Value;
            XAttribute RefFileAttr = listWatcher.Attribute("referenceFile");
            string referenceFile = RefFileAttr == null ? null : RefFileAttr.Value;
            Debug.Assert(!string.IsNullOrWhiteSpace(presetName), XMLLW.MESSAGES.missingProp("ListWatcher", "name"));
            Debug.Assert(!string.IsNullOrWhiteSpace(referenceFile), XMLLW.MESSAGES.missingProp("ListWatcher", "referenceFile"));

            Debug.WriteLine("loading preset with name: " + presetName);

            XElement ItemParser = ListParser.Element("Item");
            Debug.Assert(ItemParser != null, XMLLW.MESSAGES.missingElem("ItemParser"));

            //string input = File.ReadAllText(MI.FSDM_News_Ref_PATH);

            ListWatcherTag main = new ListWatcherTag(listWatcher, "dummy data");

            Debug.WriteLine("main ListWatcher Tag constructed, the preset model is constructed succesfully");

            ListParser listParserTag = (ListParser)main.GetAllDescendants().Find((elem) => elem.GetType() == typeof(ListParser));

            Debug.Assert(listParserTag != null, "mi: listParserTag did not get cosntructed, please contact the developer iwth error key: 43323");

            //var collection = listParserTag.GetParsedList();


            // Debug.WriteLine($"{collection.Count} items were parsed");
            return new CustomLW(main, referenceFile);

        }


    }






    public interface IHasID
    {
        string ID { get; }

    }



    public interface ListWatcherItem : IHasID
    {
        string PopupMessageString { get; }


    }


    public class ExpandoLWItemObject : DynamicObject, ListWatcherItem, INotifableItem
    {
        public string ID
        {
            get { return ExpandoObj.ID; }
        }

        public string PopupMessageString
        {
            get { return ExpandoObj.PopupMessageString; }
        }


        public dynamic ExpandoObj { get; set; }

        public string Title
        {
            get { return HasProperty("Title") ? ExpandoObj.Title : "unknown"; }

        }

        bool HasProperty(string propname)
        {
            IDictionary<String, object> asDict = (IDictionary<string, object>)this.ExpandoObj;
            return asDict.Keys.Contains(propname);
        }

        public string SubTitle
        {
            get
            {
                return HasProperty("SubTitle") ? ExpandoObj.SubTitle : null;

            }

        }

        public string Link
        {
            get { return HasProperty("Link") ? ExpandoObj.Link : "unknown"; }

        }
    }





    public interface INotifableItem
    {

        string Title { get; }
        string Link { get; }
        string SubTitle { get; }


    }



    //created to solve some casting problems and abstract away methods between the fsdmlistwatcher and CustomListWatcher
    public interface IWatch
    {

        void MarkAllAsRead();
        void MarkAsRead(IEnumerable<ExpandoLWItemObject> what);

    }





    //cff
    public class CustomLW : ListWatcher<ExpandoLWItemObject>, IWatch
    {
        private string PresetHref;
        public string Name { get { return ListWatcherTag.presetName; } }

        private ListWatcherTag ListWatcherTag { get; set; }
        private ListParser ListParserTag { get; set; }


        // and accessible ctor for the xaml designer , note used in code
        public CustomLW()
        {
            PresetHref = null;
        }

        public CustomLW(ListWatcherTag main, string referenceFile)
        {
            ReferenceFilePath = referenceFile;
            ListWatcherTag = main;
            PresetHref = main.uri;
            Interval = (int)main.DefaultInterval.TotalMilliseconds;

            ListParserTag = main.ListParserRef;
            UnreadNews = new BindingList<ExpandoLWItemObject>();

            base.NewItems += (s, news) => {
                foreach (var item in news)
                {
                    UnreadNews.Add(item);
                }
                notif(nameof(UnreadNews));
                notif(nameof(HasUnreadNews));
                notif(nameof(UnreadCount));


            };

        }
        public override string Href
        {
            get
            {
                return PresetHref;
            }
        }



        public ImageSource IconSource { get; set; }

        public bool HasFavicon { get; set; } = false;

        public string AlternativeAvatarLetter { get {
                Uri tempUri;
                 if(! Uri.TryCreate(Href, UriKind.Absolute, out tempUri) )return "W";
                if (tempUri.HostNameType != UriHostNameType.Dns) return "W";
                return tempUri.Host[0].ToString().ToUpper();

            } }


        internal override bool ParseList(string pageContent, out List<ExpandoLWItemObject> parsedList)
        {

            List<ExpandoLWItemObject> outputList = new List<ExpandoLWItemObject>();
            ListWatcherTag.Input = pageContent;
            ListWatcherTag.Refresh(pageContent);
            outputList = ListParserTag.GetParsedList().ConvertAll<ExpandoLWItemObject>(new Converter<ExpandoObject, ExpandoLWItemObject>((e) => new ExpandoLWItemObject() { ExpandoObj = e }));
            parsedList = outputList;

            if (outputList.Count < 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }



    }











    public abstract class Tag
    {

        public List<Tag> GetAllDescendants()
        {
            List<Tag> outp = new List<Tag>();
            if (Children == null)
            {
                return outp;
            }
            foreach (var item in Children)
            {
                outp.AddRange(item.GetAllDescendants());
                outp.Add(item);
            }
            return outp;
        }


        public void spawnChildren()
        {
            foreach (var item in XElement.Nodes())
            {
                //doesnt include Item tag cz the ListParser class does child spawning by it's owwn, instead of calling this base ethode
                TagType type = Type(item);
                if (type == TagType.Wraper)
                {
                    Children.Add(new wraper((XElement)item, this));
                }
                else if (type == TagType.Replacer)
                {
                    Children.Add(new Replacer((XElement)item, this));
                }
                else if (type == TagType.Match)
                {
                    Children.Add(new Matcher((XElement)item, this));
                }
                else if (type == TagType.TargetProperty)
                {
                    Children.Add(new TargetProperty((XElement)item, this));
                }

                else if (type == TagType.ListParser)
                {
                    Children.Add(new ListParser((XElement)item, this));
                }
                else if (type == TagType.Value)
                {
                    Children.Add(new ValueTag((XElement)item, this));
                }
                else if (type == TagType.Group)
                {
                    Children.Add(new GroupTag((XElement)item, this));
                }
                else if (type == TagType.tracer)
                {
                    Children.Add(new TracerTag((XElement)item, this));
                }
                else if (type == TagType.XParser)
                {
                    Children.Add(new XParserTag((XElement)item, this));
                }
                else if (type == TagType.InnerXML)
                {
                    Children.Add(new InnerXML((XElement)item, this));
                }
                else if (type == TagType.AttributeValue)
                {
                    Children.Add(new AttributeValue((XElement)item, this));
                }

            }
        }



        /// <summary>
        /// returns null if attribute doesnt exist
        /// </summary>
        public string getAttrib(string attribName)
        {
            return XElement.Attribute(attribName)?.Value;
        }

        /// <summary>
        /// returns bool indicating wether the attribute was found and outputed
        /// </summary>
        public bool getAttrib(string attribName, out string maybeValue)
        {
            var xattr = XElement.Attribute(attribName);
            if (xattr != null) { maybeValue = xattr.Value; return true; }
            else { maybeValue = null; return false; }
        }



        /// <summary>
        /// causes all descendants to re apply(), resulting in a new data distrubution over the preset model, 
        /// this allows to re use the preset model to parse another string data, and usually only invoked by the root ListWatcher object 
        /// </summary>
        public virtual void Refresh(string injectInput = "")
        {
            Input = injectInput;
            if (string.IsNullOrEmpty(injectInput))
            {
                Input = Parent.Output;
            }

            apply();
            foreach (var item in Children)
            {
                item.Refresh();
            }
        }

        public enum TagType
        {
            Wraper, Replacer, Match, ListParser, Item, Value, Group, Success, TargetProperty, none,
            tracer,
            XParser,
            InnerXML,
            AttributeValue
        }

        public static TagType Type(XElement xelem)
        {
            if (xelem.Name.Namespace == "xml-parser") return TagType.XParser;
            switch (xelem.Name.LocalName.ToLower())
            {
                case "wraper":
                    return TagType.Wraper;
                case "replacer":
                    return TagType.Replacer;
                case "match":
                    return TagType.Match;
                case "listparser":
                    return TagType.ListParser;
                case "item":
                    return TagType.Item;
                case "value":
                    return TagType.Value;
                case "group":
                    return TagType.Group;
                case "targetproperty":
                    return TagType.TargetProperty;
                case "tracer": return TagType.tracer;
                case "attributevalue": return TagType.AttributeValue;
                case "innerxml": return TagType.InnerXML;
                default:
                    return TagType.none;
            }

        }
        public static TagType Type(XNode xnode)
        {
            if (xnode.NodeType == XmlNodeType.Element)
            {
                return Type((XElement)xnode);
            }
            else
            {
                return TagType.none;
            }


        }

        public List<Tag> Children { get; set; }
        public XElement XElement { get; set; }

        public string Input { set; get; }
        public string Output { set; get; }
        public Tag Parent { set; get; }

        public virtual void apply()
        {
            Output = (Input);
        }


    }







    public class ListWatcherTag : Tag
    {
        public string uri { get; set; }
        public string presetName { get; set; }

        public TimeSpan DefaultInterval { set; get; } = TimeSpan.FromMinutes(3);
        public string DefaultAction { set; get; }
        public string PopupWindowTitleFormatter { get; set; } = "$name : $c New Items";
        public string UnreadButtonCaptionFormatter { get; set; } = "$c News";

        private ListParser listParserRef = null;
        public ListParser ListParserRef
        {
            get
            {

                if (listParserRef != null)
                {
                    return listParserRef;
                }
                foreach (var item in this.GetAllDescendants())
                {
                    if (item.GetType() == typeof(ListParser))
                    {
                        listParserRef = (ListParser)item;
                        return listParserRef;
                    }
                }

                Debug.Assert(listParserRef != null, "Mi: ListWarcherTag object couldnt find the ListParser descendant ");
                return null;
            }
        }

        public ListWatcherTag(XElement XELEM, string input)
        {

            Debug.WriteLine("spawning ListWatcherTag");
            Children = new List<Tag>();
            XElement = XELEM;
            // ## validating and parsing attributes ## //
            string maybeDefaultAction = getAttrib("defaultAction");
            if (maybeDefaultAction != null) DefaultAction = maybeDefaultAction;
            string maybeDefaultInterval;
            if (getAttrib("defaultInterval", out maybeDefaultInterval))
            {
                var maybeDefaultInterval_ = Fucs.TimeSpanFromString(maybeDefaultInterval);
                Trace.Assert(maybeDefaultInterval_ != null, $"Mi defaultInterval attribute value '{maybeDefaultInterval}' is not a valid timeSpan value");
                if (maybeDefaultInterval_.HasValue)
                    DefaultInterval = maybeDefaultInterval_.Value;
            }
            string maybePPWT;
            if (getAttrib("popupWindowTitle", out maybePPWT))
                PopupWindowTitleFormatter = maybePPWT;

            string maybeUBC;
            if (getAttrib("unreadButtonCaption", out maybeUBC))
                UnreadButtonCaptionFormatter = maybeUBC;

            uri = XElement.Attribute("uri")?.Value;
            Trace.Assert(uri != null, XMLLW.MESSAGES.missingProp("ListWatcher", "uri"));
            presetName = XElement.Attribute("name")?.Value;
            Trace.Assert(uri != null, XMLLW.MESSAGES.missingProp("ListWatcher", "name"));

            Input = input;
            apply();
            base.spawnChildren();

        }




    }











    public class wraper : Tag
    {
        string From { get; set; }
        string To { get; set; }
        int Ignore { get; set; }

        public wraper(XElement XELEM, Tag parent)
        {
            if (XELEM == null && parent == null)
            {
                return;
            }
            Debug.WriteLine("spawning wraper");
            Children = new List<Tag>();
            XElement = XELEM;
            string from = (string)XElement.Attribute("from").Value;
            string to = (string)XElement.Attribute("to").Value;
            int ignor = int.Parse(XElement.Attribute("ignore").Value);

            From = from;
            To = to;
            Ignore = ignor;

            Parent = parent;
            Input = Parent.Output;
            apply();

        }



        public override void apply()
        {
            Output = wraper.Wrap(Input, From, To, Ignore);
        }

        public static string Wrap(string input, string from, string to, int ignore)
        {
            Match m = Regex.Match(input, from + ".*?" + to);
            if (m.Success) return m.Value;
            else return "";
        }

    }






    public class Replacer : Tag
    {
        string Pattern { get; set; }
        string Replacement { get; set; }
        string UseConverter { get; set; }

        public Replacer(XElement XELEM, Tag parent)
        {

            Children = new List<Tag>();
            XElement = XELEM;



            var ReplacementAtt = XElement.Attribute("replacement");
            var PatternAtt = XElement.Attribute("pattern");
            var UseConverterAtt = XElement.Attribute("converter");
            Trace.Assert(ReplacementAtt != null, XMLLW.MESSAGES.missingProp("replacer", "pattern"));
            Trace.Assert(ReplacementAtt != null, XMLLW.MESSAGES.missingProp("replacer", "replacement"));
            Replacement = ReplacementAtt.Value;
            Pattern = PatternAtt.Value;
            UseConverter = UseConverterAtt == null ? "none" : UseConverterAtt.Value;

            Parent = parent;
            Input = Parent.Output;
            apply();
            base.spawnChildren();


        }

        public override void apply()
        {
            if (UseConverter.ToLower() == "xmldecoder")
            {
                Output = Fucs.decodeXml(Input);

            }
            else
            {
                Output = Regex.Replace(Input, Pattern, Replacement);

            }
        }



    }








    public class Matcher : Tag, IMatcher
    {
        string Pattern { get; set; }
        string Options { get; set; }
        public Match MatcherOutput { set; get; }

        public bool Success { get { return MatcherOutput.Success; } }

        public GroupCollection Groups { get { return MatcherOutput.Groups; } }

        public string Value { get { return MatcherOutput.Value; } }

        public Matcher(XElement XELEM, Tag parent)
        {
            Debug.WriteLine("spawning matcher");

            Children = new List<Tag>();
            XElement = XELEM;

            Pattern = XElement.Attribute("pattern")?.Value;
            Options = XElement.Attribute("options")?.Value;
            Trace.Assert(Pattern != null, XMLLW.MESSAGES.missingProp("matcher", "pattern"));

            Parent = parent;
            Input = Parent.Output;
            apply();
            base.spawnChildren();
        }

        public static RegexOptions parseOptions(string asStr)
        {
            switch (asStr)
            {
                case "Singleline": return RegexOptions.Singleline;
                default:
                    return RegexOptions.None;
            }
        }
        public override void apply()
        {
            MatcherOutput = Regex.Match(Input, Pattern, parseOptions(Options));
            Output = MatcherOutput.Success ? MatcherOutput.Value : "";
        }
    }




    public class ListParser : Tag
    {


        private string ItemPattern { get; set; }
        private RegexOptions ItemPatternOptions { get; set; }
        private XElement ItemNode { get; set; }

        public List<ExpandoObject> GetParsedList()
        {
            List<ExpandoObject> outp = new List<ExpandoObject>();
            foreach (var item in Children)
            {
                outp.Add(((ItemParser)item).makeItemObject());

            }

            return outp;
        }


        public ListParser(XElement XELEM, Tag parent)
        {
            Debug.WriteLine("spawning ListParser");

            Children = new List<Tag>();
            XElement = XELEM;



            Parent = parent;
            Input = Parent.Output;
            apply();

            ItemNode = (XElement)XElement.FirstNode;
            Debug.Assert((ItemNode != null) && (Type(ItemNode) == TagType.Item), "ListParser must have an Item element as fist child");

            ItemPatternOptions = Matcher.parseOptions(ItemNode.Attribute("options")?.Value);
            ItemPattern = ItemNode.Attribute("pattern")?.Value;
            Debug.Assert(ItemPattern != null, "mi: Item must have a pattern attribute");



            spawnItems();
        }

        public override void Refresh(string injectedInput = "")
        {
            Input = injectedInput;
            if (string.IsNullOrEmpty(injectedInput))
            {
                Input = Parent.Output;
            }
            apply();
            spawnItems();
            //new code is uneccessary , the new spawned objects will have an up to date data no refreshing needed
            return;
            foreach (var item in Children)
            {
                item.Refresh();
            }
        }

        /// <summary>
        /// should be recalled when data changes
        /// </summary>
        private void spawnItems()
        {
            MatchCollection mc = Regex.Matches(Input, ItemPattern, ItemPatternOptions);
            Children.Clear();
            foreach (Match oneMatch in mc)
            {
                Children.Add(new ItemParser(ItemNode, oneMatch, this));


            }
        }


    }




    // for matcher and ItemParser (anthing that accepts the success , group, vale sub tags
    public interface IMatcher
    {
        bool Success { get; }
        GroupCollection Groups { get; }
        string Value { get; }

    }

    public class ItemParser : Tag, IMatcher
    {
        Match RawMatch { get; set; }

        public bool Success { get { return RawMatch.Success; } }
        public GroupCollection Groups { get { return RawMatch.Groups; } }
        public string Value { get { return RawMatch.Value; } }

        public Match MatcherOutput { set; get; }



        public ExpandoObject makeItemObject()
        {
            dynamic obj = new ExpandoObject();
            IDictionary<String, object> asDict = (IDictionary<string, object>)obj;

            foreach (var item in GetAllDescendants())
            {
                if (item.GetType() == typeof(TargetProperty))
                {
                    TargetProperty tp = (TargetProperty)item;
                    asDict.Add(tp.PropertyName, tp.Output);
                    if (tp.UseAs != SpecialProps.none)
                    {
                        asDict.Add(tp.UseAs.ToString(), tp.Output);
                    }
                }
            }
            obj.ldjslk = "lklm";




            return obj;
        }
        public ItemParser(XElement XELEM, Match rawMatch, ListParser parent)
        {
            Debug.WriteLine("spawning ItemParser");

            RawMatch = rawMatch;
            Children = new List<Tag>();
            XElement = XELEM;


            Parent = parent;
            Input = RawMatch.Value;
            apply();
            base.spawnChildren();
        }
        public override void apply()
        {
            Output = Value;
        }
    }


    // most messy shit i've ever written
    public class XParserTag : Tag
    {
        /// <summary>
        /// target, usually html, element name, e.g div, span
        /// </summary>
        public string ElementName { set; get; }

        /// <summary>
        /// contains the raw inner string of the node
        /// </summary>
        public string InnerXmlString { get; set; }


        /// <summary>
        /// gets the first element that passes a set of attribute conditions and/or an index condition
        /// the indedx condition usage is as follow x:index="4", meaning the element must ba the 4th among it's siblings 
        /// </summary>
        static XElement GetElement(XElement InputElem, string targetElemName, IEnumerable<XAttribute> requiredAttributes)
        {
            var attribsAslist = requiredAttributes.ToList();
            var attribsASKeyPairs = attribsAslist.ConvertAll<KeyValuePair<string, string>>(new Converter<XAttribute, KeyValuePair<string, string>>((at) => new KeyValuePair<string, string>((at.Name.NamespaceName == "xml-parser" ? "x:" : "") + at.Name.LocalName, at.Value)));
            return GetElement(InputElem, targetElemName, attribsASKeyPairs);
        }


        /// <summary>
        /// returns the first child element that have all the required attribs with the requiredd values, 
        /// passing a null string as a required attib value will pass any value
        /// returns null if no match was found
        /// </summary>
        static XElement GetElement(XElement InputElem, string targetElemName, IEnumerable<KeyValuePair<string, string>> requiredAttributes)
        {
            List<XElement> lst = new List<XElement>(InputElem.Elements(targetElemName));
            foreach (var item in lst)
            {
                bool passesAllConditions = true;
                foreach (var requiredAttrib in requiredAttributes)
                {
                    //### case of a special x:index=4 condition
                    if (requiredAttrib.Key == "x:index")
                    {
                        int requiredIndexValue = int.Parse(requiredAttrib.Value);
                        if (GetIndexOf(InputElem, item) != requiredIndexValue)
                        {
                            passesAllConditions = false;
                            break;
                        }

                        continue;
                    }


                    //## case of a regular attribute=value condition
                    XAttribute att = (item.Attribute(requiredAttrib.Key));
                    if (att == null)
                    {
                        passesAllConditions = false;
                        break; // no need to keep cheking other requested attribs
                    }
                    if (requiredAttrib.Value != null)
                    {
                        if ((requiredAttrib.Value != "mi:any") && (att.Value != requiredAttrib.Value))
                        {
                            // break if element does not have the requird attrib at all
                            // break ig it have the required attib but with a different vale that the one required, 
                            // if the required value is null then this will not do the value checking
                            passesAllConditions = false;
                            break; // no need to keep cheking other requested attribs
                        }

                    }

                }
                if (passesAllConditions) { return item; }
                // steps here when the item didsnt pass the tests, 
            }
            // steps here when none of the items if any passed the tests, 
            // returning null
            return null;

        }


        /// <summary>
        /// returns the index of an item whithin a parrent's child elements, or -1 if it doesnt exist among them 
        /// </summary>
        private static int GetIndexOf(XElement parentElem, XElement elem, bool sameLocalName = false)
        {
            if (sameLocalName)
                return parentElem.Elements(elem.Name.LocalName).ToList().IndexOf(elem);
            return parentElem.Elements().ToList().IndexOf(elem);
        }

        public XElement ParsedElement { get; set; }
        public bool Success { set; get; }
        public bool isParentAnXParser
        {
            get
            {
                return Parent.GetType() == typeof(XParserTag);
            }
        }
        public XParserTag(XElement XELEM, Tag parent)
        {

            Parent = parent;
            Input = Parent.Output;

            Debug.Assert(XELEM.Name.NamespaceName == "xml-parser", "Mi: the passed xelement does not belong to the 'xml-parser' namespace");
            XElement = XELEM;

            ElementName = XElement.Name.LocalName;


            Debug.WriteLine("spawning XParserTag");

            Children = new List<Tag>();
            XElement = XELEM;



            apply();
            spawnChildren();

        }

        public override void apply()
        {
            if (isParentAnXParser)
            {
                var parentAsXparser = (XParserTag)Parent;
                if (!parentAsXparser.Success)
                {
                    Debug.WriteLine($"XParserTag with elementName name:'{ElementName}' failed because it's XParserTag '{parentAsXparser.ElementName}' parent has failed");
                    Success = false;

                }
                else
                {
                    var parentsParsedElement = ((XParserTag)Parent).ParsedElement;

                    ParsedElement = GetElement(parentsParsedElement, this.ElementName, this.XElement.Attributes());
                    Success = ParsedElement != null;
                    if (Success == false)
                    {
                        Debug.WriteLine($"XParserTag with elementName name:'{ElementName}' failed because GetElement() didnt find any matching element, parent XParserTag is: '{parentAsXparser.ElementName}' ");
                    }
                }

            }
            else
            {
                try
                {
                    ParsedElement = XElement.Parse(Input);
                    Success = true;
                    if (ParsedElement == null)
                    {
                        Success = false;
                        Debug.WriteLine($"Mi: XParserTag with name:'{ElementName}' failed because parsing it's input text returnd a null XElement object");
                    }
                    else if (ParsedElement.Name.LocalName != ElementName)
                    {
                        Success = false;
                        Debug.WriteLine($"Mi: XParserTag with name:'{ElementName}' failed because parsing it's input text returnd an XElement with name '{ParsedElement.Name.LocalName}' when the name '{ElementName}' was excpected");
                    }

                }
                catch (Exception)
                {
                    Success = false;
                    Debug.WriteLine($"Mi: XParserTag with name:'{ElementName}' failed to parse it's Input text");
                }
            }

            Output = Success ? ParsedElement.ToString() : "";
            InnerXmlString = Success ? ParsedElement.Value : "";
        }

    }


    // can only appear as a XParserTag child
    // supplies children with the parent's InnertXMLSring 
    // if a 'Trim' attribut is present this performes a string.trim() before outputing text
    public class InnerXML : Tag
    {
        public bool EnableTrimming { get; set; }

        public InnerXML(XElement XELEM, Tag parent)
        {
            Trace.Assert(parent.GetType() == typeof(XParserTag), "Mi: InnerXML tag can only appear as a XParser child ");
            Children = new List<Tag>();
            XElement = XELEM;
            Parent = parent;
            EnableTrimming = XElement.Attribute("Trim") != null;

            var parentAsXParser = (XParserTag)parent;
            Input = parentAsXParser.InnerXmlString; // tofo: make this tag do the inner text exctracting operations instead of the XParser, for better debuging

            Debug.WriteLine($"spawning InnerXML, child of a '{parentAsXParser.ElementName}' XParser");



            apply();
            base.spawnChildren();

        }

        public override void apply()
        {
            if (EnableTrimming)
                Output = Input.Trim();
            else
                Output = Input;
        }

    }



    // can only appear as a XParserTag child
    // supplies children with the parent's attribute value  
    // if a 'Trim' attribut is present this performes a string.trim() before outputing text
    public class AttributeValue : Tag
    {
        public bool EnableTrimming { get; set; }
        public string AttributeName { get; internal set; }

        public AttributeValue(XElement XELEM, Tag parent)
        {
            Children = new List<Tag>();
            XElement = XELEM;
            Parent = parent;
            Trace.Assert(parent.GetType() == typeof(XParserTag), "Mi: AttributeValue tag can only appear as a XParser child ");

            var parentAsXParser = (XParserTag)parent;
            Input = parent.Output;
            Debug.WriteLine($"spawning AttributeValue, child of a '{parentAsXParser.ElementName}' XParser, target attribute is ");

            Trace.Assert((AttributeName = getAttrib("AttributeName")) != null, XMLLW.MESSAGES.missingProp("AttributeValue", "AttributeName"));
            EnableTrimming = XElement.Attribute("Trim") != null;




            apply();
            base.spawnChildren();

        }

        public override void apply()
        {
            var parentAsXParser = (XParserTag)Parent;
            string outString;

            if (parentAsXParser.Success)
            {
                XAttribute targetAtt = parentAsXParser.ParsedElement.Attribute(AttributeName);
                if (targetAtt == null)
                {
                    outString = "";
                    Debug.WriteLine($"Mi: AttributeValue tag couldnt find the requested attribute '{AttributeName}'");

                }
                else
                {
                    outString = targetAtt.Value;

                }
            }
            else /// (!parentAsXParser.Success)
            {
                outString = "";
                Debug.WriteLine($"Mi: AttributeValue tag returned empty string because its XParser '{parentAsXParser.ElementName}' parent has failed ");

            }
            if (EnableTrimming)
                Output = outString.Trim();
            else
                Output = outString;
        }

    }



    public enum SpecialProps
    {
        Title, SubTitle, Link,
        none
    }

    public class TargetProperty : Tag
    {
        public string PropertyName { get; internal set; }
        public string AppendAfter { get; set; }
        public string AppendBefore { get; set; }
        public SpecialProps UseAs { get; set; } = SpecialProps.none;



        public TargetProperty(XElement XELEM, Tag parent)
        {

            Debug.WriteLine("spawning targetProperty");
            Children = null;
            XElement = XELEM;

            PropertyName = XELEM.Attribute("property")?.Value;
            Trace.Assert(PropertyName != null, XMLLW.MESSAGES.missingProp("TargetProperty", "property"));

            AppendBefore = getAttrib("appendBefore");
            AppendAfter = getAttrib("appendAfter");
            string UseAsStr_ = getAttrib("UseAs");
            if (UseAsStr_ != null)
            {
                UseAsStr_ = UseAsStr_.ToLower();
                switch (UseAsStr_)
                {
                    case "title": UseAs = SpecialProps.Title; break;
                    case "link": UseAs = SpecialProps.Link; break;
                    case "subtitle": UseAs = SpecialProps.SubTitle; break;
                    default: UseAs = SpecialProps.none; break;
                }
            }



            Parent = parent;

            Input = Parent.Output;
            apply();
        }


        public override void apply()
        {

            Output = AppendBefore + Input + AppendAfter;
        }

    }



    public class TracerTag : Tag
    {
        public string Message { get; internal set; }

        public override void Refresh(string injectInput = "")
        {
            Input = Parent.Output;
            Debug.WriteLine("tarcer refreshed: " + Message);
            Debug.WriteLine(Input);

        }

        public TracerTag(XElement XELEM, Tag parent)
        {

            Children = null;

            XElement = XELEM;
            Message = XELEM.Attribute("message").Value;
            Debug.WriteLine("tarcer: " + Message);
            Parent = parent;
            Input = Parent.Output;
            Debug.WriteLine(Input);

            apply();
        }

    }



    public class ValueTag : Tag
    {
        public string PropertyName { get; internal set; }

        public ValueTag(XElement XELEM, Tag parent)
        {
            Children = new List<Tag>();

            Debug.WriteLine("spawning valueTag");
            Debug.WriteLine(parent.GetType());

            Debug.Assert((parent.GetType() == typeof(Matcher)) || (parent.GetType() == typeof(ItemParser)), "Mi: a Value tag's parent must be either a Matcher or an Item");

            XElement = XELEM;
            Parent = parent;
            Input = ((IMatcher)Parent).Value;
            apply();
            base.spawnChildren();

        }

    }








    public class GroupTag : Tag
    {

        public int Index { get; internal set; }

        public GroupTag(XElement XELEM, Tag parent)
        {
            Debug.WriteLine("spawning GroupTag");
            Children = new List<Tag>();

            Debug.Assert((parent.GetType() == typeof(ItemParser)) || (parent.GetType() == typeof(Matcher)), "Mi: a Group tag's parent must be either a Matcher or an Item");

            XElement = XELEM;
            Index = int.Parse(XElement.Attribute("index").Value);
            Debug.WriteLine("GroupTag's index is:" + Index.ToString());
            //<td><a href=\"2019-03-26-08-33-33_58f37f00f409b98b8eda58634d6dbeadeec0340f\">2019-03-26-08-33-33_..&gt;</a></td><td align=\"right\">2019-03-26 09:33  </td><td align=\"right\">713K</td><td>&nbsp;</td></tr>\r
            //<a href=\"((.*)(\\..{3,5}))\">
            Parent = parent;
            Debug.Assert(((IMatcher)Parent).Groups.Count > Index, "mi: a Group tag is pointing to an out of range index");
            Input = ((IMatcher)Parent).Groups[Index].Value;
            apply();
            base.spawnChildren();

        }

    }















}
