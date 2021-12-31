using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Media;
using System.Xml.Linq;
using System.Dynamic;
using System.Xml.Serialization;
using System.Windows.Input;
using AngleSharp.Dom;
using System.Collections.ObjectModel;
using CLW.Model;
using CLW.Model.Enums;

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
        public static string CLW_VERSION { get; set; } = "0.4.0 (?)" + (IsDev ? " [dev]" : "");
        public static string CLW_DEVELOPER { get; set; } = "Yass.Mi";
        public static string CLW_GUI_DESIGNER { get; set; } = "Yass.Mi";
        public static string CLW_GITHUB_URL { get; set; } = "https://github.com/UndefinedYass/clw";


        public static string CLW_DEVELOPER_EMAIL { get; set; } = "DIR16CAT17@gmail.com";

        public static int Host_Rendering_Tier { get; set; } =  RenderCapability.Tier >> 16;
    }
    public class MI
    {
        public static string MAIN_PATH = Path.GetDirectoryName(
           System.Reflection.Assembly.GetExecutingAssembly().Location);

        public static string CURL_PATH = MAIN_PATH + "\\curl\\curl.exe";
        public static string APP_DATA = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Mi\\CLW";


        public static string DEFAULT_GLOBAL_OUTPUT_DIR = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CLW Output";

        public static string CLW_PRESETS_DIR = MAIN_PATH + "\\CLW Presets";
        public static string APP_CONFIG_FILE = APP_DATA + "\\config.mi.xml";
        internal static string TEMP_HTML_FILES = APP_DATA + "\\Temp HTML";
        internal static string SCRIPTS_DIR = MAIN_PATH + "\\scripts";
        internal static string regexTestPattern = "\\\\x3CPeriod duration=\\\\\\\"PT(.*?)H(.*?)M(.*?)S\\\">";
        internal static object SFX_DIRECTORY = MAIN_PATH + "\\SFX";
        internal static string ERRORS_LOG_FILE = APP_DATA + @"\Errors.log";

        public static string APP_CONFIG_FILE_V2 = APP_DATA + "\\config.mi.v2.xml";

        [Obsolete("use Logger.Log",true)]
        public static async void DumpError(Exception exception, string source)
        {
            File.AppendAllText(MI.ERRORS_LOG_FILE, $"Error, {DateTime.Now.ToString("d/MM/yyyy hh:mm:ss")}, In [{source}] : '{exception.Message}'{Environment.NewLine}");
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













    public enum DownloadClients { curl, powershell, ffmpeg,
        native
    }

    [Serializable]
    public class CLWPresetDeclaration: INotifyPropertyChanged
    {

      
        private string path;
        private bool autoStart;
        private bool autoLoad;

        public string Path{set { path = value; notif(nameof(Path)); }get { return path; }}
        public bool AutoStart{set { autoStart = value; notif(nameof(AutoStart)); }get { return autoStart; } }
        public bool AutoLoad { set { autoLoad = value; notif(nameof(AutoLoad)); } get { return autoLoad; } }

        private void notif(string propertyName) {PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));}
        public event PropertyChangedEventHandler PropertyChanged;
    }



    [Serializable]
    public class Config
    {

        public List<CLWPresetDeclaration> CLWPresetsDeclarations { get; set; } = new List<CLWPresetDeclaration>();

        public string GlobalOutputDirectory { get; set; } = MI.DEFAULT_GLOBAL_OUTPUT_DIR;
        public string RememberedDownloadOutputDirectory { get; set; } = MI.DEFAULT_GLOBAL_OUTPUT_DIR;

        public List<string> RecentGlobalDirectories { get; set; } = new List<string>();
        public OverrideBehaviour DefaultOverrideBehaviour { get; set; } = OverrideBehaviour.Override;
        public bool DownloadRawStreams { get; set; } = false;
        public bool AutoStartServer { get; set; } = false;
        public bool RememberDownloadsOutputDirectory { get; set; } = true;

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
                
                   
                Config co = null;
                try
                {
                    co = sr.Deserialize(stream) as Config;
                }
                catch (Exception ec)
                {
                    MessageBox.Show($"there is a problem with config file, try deleting it and restart the app {Environment.NewLine}{ec.Message}");
                    App.Current.Shutdown();

                    
                }
                return co;
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











    [Obsolete("switched to MVVM patter on25-sept-2021, no state singlton is resuired", true)]
    public class Session 
    {
        ///code deleted
    }















    //cff

   



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



        public static CustomLW LoadXMLPreset(string PrestXmldata, out IEnumerable <XAttribute> ListWatcherAttribs)
        {

            XDocument d = XDocument.Parse(PrestXmldata);


            //## basic assert ckeckpoints before starting 
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
            //Debug.Assert(ItemClass != null, XMLLW.MESSAGES.missingElem("ItemClass"));
            XElement ListParser = null;

            foreach (var item in listWatcher.Descendants())
            {
                if (CPE.Type(item) == CPEElements.ListParser)
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


            //starting the actual contruction

            ListWatcherCPE main;
            
            main = new ListWatcherCPE(listWatcher, "dummy data");
           
            Debug.WriteLine("main ListWatcher Tag constructed, the preset model is constructed succesfully");

            ListParserCPE listParserTag = (ListParserCPE)main.GetAllDescendants().FirstOrDefault((elem) => elem.GetType() == typeof(ListParserCPE));

            Debug.Assert(listParserTag != null, "mi: listParserTag did not get cosntructed, please contact the developer iwth error key: 43323");

            //var collection = listParserTag.GetParsedList();


            // Debug.WriteLine($"{collection.Count} items were parsed");
            ListWatcherAttribs = listWatcher.Attributes();

            ItemParserCPE the_item_parser = (ItemParserCPE)main.GetAllDescendants().FirstOrDefault((elem) => elem.GetType() == typeof(ItemParserCPE));


            //Trace.WriteLine($"{  main.GetAllDescendants().Count()} CPE's were found");

            
            //Trace.WriteLine($"{ the_item_parser?.TargetPropertyDescendentElements?.Count} TargetProperty CPE's were found");


            return new CustomLW(main, referenceFile);

        }


    }






    public interface IHasID
    {
        string ID { get; }

    }



  

    [Obsolete("use dictionary based alternative, reducing cpu overhead meybe",true)]
    public class ExpandoLWItemObject : DynamicObject, INewItem
    {
        //DictBasedNewItemObject ExpandoLWItemObject
        public string ID
        {
            get { return ExpandoObj.ID; }
        }

        public string DownloadLink
        {
            get { return HasProperty("DownloadLink") ? ExpandoObj.DownloadLink : null; }
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
        public string TextContent
        {
            get { return HasProperty("TextContent") ? ExpandoObj.TextContent : null; }

        }



    }




    public class DictBasedNewItemObject : DynamicObject, INewItem
    {

        public Dictionary<string, object> dict { get; set; }

       
        public string ID
        {
            get { return HasProperty("ID") ? (string)dict["ID"] : null; }
        }
        public string DownloadLink
        {
            get { return HasProperty("DownloadLink") ? (string)dict["DownloadLink"] : null; }
        }
        public string Title
        {
            get { return HasProperty("Title") ? (string)dict["Title"] : null; }
        }
        public string SubTitle
        {
            get { return HasProperty("SubTitle") ? (string)dict["SubTitle"] : null; }
        }
        public string Link
        {
            get { return HasProperty("Link") ? (string)dict["Link"] : null; }
        }
        public string TextContent
        {
            get { return HasProperty("TextContent") ? (string)dict["TextContent"] : null; }
        }


        bool HasProperty(string propname)
        {
            return dict.Keys.Contains(propname);
        }

    }




    public interface INewItem
    {
        string ID { get; }

        string Title { get; }
        string Link { get; }
        string SubTitle { get; }
        string DownloadLink { get; }
        string TextContent { get; }



    }



    //created to solve some casting problems and abstract away methods between the fsdmlistwatcher and CustomListWatcher
    public interface IWatch
    {

        void MarkAllAsRead();
        void MarkAsRead(IEnumerable<DictBasedNewItemObject> what);

    }


    // for matcher and ItemParser (anthing that accepts the success , group, vale sub tags
    public interface IMatcher
    {
        bool Success { get; }
        GroupCollection Groups { get; }
        string Value { get; }

    }





    //cff
    public class CustomLW : ListWatcherBase<DictBasedNewItemObject>, IWatch
    {
        private string PresetHref;
        public string Name { get { return ListWatcherTag.presetName; } }

        private ListWatcherCPE ListWatcherTag { get; set; }
        private ListParserCPE ListParserTag { get; set; }

        // and accessible ctor for the xaml designer , note used in code
       
        public CustomLW(ListWatcherCPE main, string referenceFile)
        {
            ReferenceFilePath = referenceFile;
            ListWatcherTag = main;
            PresetHref = main.uri;
            Interval = (int)main.DefaultInterval.TotalMilliseconds;

            ListParserTag = main.ListParserRef;
            UnreadNews = new Collection<DictBasedNewItemObject>();
            StatusMessage = "Disabled";
            base.NewItems += (s, news) => {
                foreach (var item in news)
                {
                    UnreadNews.Add(item);
                 }
                //21-sept-2021  experimental sorting 
                UnreadNews = new Collection<DictBasedNewItemObject>(UnreadNews.OrderByDescending((expandoobj) => expandoobj.dict.ContainsKey("SortBy") ? expandoobj.dict["SortBy"] :expandoobj.SubTitle).ToList());

                

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

       

        /// <summary>
        /// Runs cpu-extensive operation on a new thred, blockng the calling thread meanwile
        /// </summary>
        /// <param name="pageContent"></param>
        /// <param name="parsedList"></param>
        /// <returns></returns>
        internal override bool TryParseList(string pageContent, out ICollection<DictBasedNewItemObject> parsedList)
        {
            ICollection<DictBasedNewItemObject> outputList;

            //Thread t = new Thread(()=> {
            //var sw = Stopwatch.StartNew();
            ListWatcherTag.Input = pageContent;
            ListWatcherTag.Refresh(pageContent);
            outputList = ListParserTag.GetParsedList();
            //});

            //t.Start();
            //t.Join();
            //Trace.WriteLine(sw.Elapsed);
            parsedList = outputList;

            if (outputList!=null && outputList.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        

        public override void CloseAsync()
        {
            if(ListParserTag?.Children!=null)
            for (int i = 0; i < ListParserTag.Children.Count; i++)
            {
                    ListParserTag.Children[i]?.closExp();
                //ListParserTag.Children[i] = null;
                
            }
            //ListParserTag?.Children?.Clear();
            base.CloseAsync();
            try
            {
                if(ListParserTag!=null)
                foreach (var item in ListParserTag.GetAllDescendants())
                {
                    item?.closExp();
                }
                /*this.ListParserTag = null;
                this.ListParserTag = null;
                this.currentContent = null;
                this.UnreadNews = null;
                this.CurrentReferenceList = null;*/

            }
            catch (Exception)
            {

            }
           
          
        }


    }












































    public class MICommand<T> : ICommand 
    {
        public Action<T> CommandAction { get; set; }
        public Func<T, bool> CanExecuteFunc { get; set; }

        public MICommand(Action<T> _CommandAction)
        {
            CommandAction = _CommandAction;
            CanExecuteFunc = null;

        }

        public MICommand(Action<T> _CommandAction, Func<T,bool> _CanExecuteFunc)
        {
            CommandAction = _CommandAction;
            CanExecuteFunc = _CanExecuteFunc ;
        }

        public void Execute(object parameter)
        {
            CommandAction((T) parameter);
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }


    public class MICommand : ICommand
    {
        public Action CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }

        public MICommand(Action _CommandAction )
        {
            CommandAction = _CommandAction;
            CanExecuteFunc = null;
                
        }

        public MICommand(Action _CommandAction , Func<bool> _CanExecuteFunc)
        {
            CommandAction = _CommandAction;
            CanExecuteFunc = _CanExecuteFunc;
        }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }




}
