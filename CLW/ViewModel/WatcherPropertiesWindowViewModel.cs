using CLW.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CLW.Model;
using CLW.ViewModel;
using System.Windows.Media;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows;
using System.Diagnostics;

namespace CLW.ViewModel
{
    class WatcherPropertiesWindowViewModel :  BaseViewModel, IDataErrorInfo, ICloseWindowViewModel
    {
        public WatcherPropertiesWindowViewModel()
        {
            
            CookieOrHeader[] ch = new CookieOrHeader[] {
                new Header() {Key="header1", Value="dummy value1", IsEnabled=true },
                 new Cookie() {Key="cookie2_long_key_name", Value="dummy value2", IsEnabled=false },

                new Header() {Key="header2 long key example", Value="dummy value2", IsEnabled=false },
                 new Cookie() {Key="cookie1", Value="dummy value1", IsEnabled=true },
               new Header() {Key="header3", Value="dummy value3 long value text example", IsEnabled=true },

            };
            CookiesAndHeaders = new ObservableCollection<CookieOrHeader>(ch);
            RefFilePath = "ieoz";
            Uri = "not a uri hezl";
            Interval = TimeSpan.FromSeconds(5);
            //used for design-time data
            Headers = new ObservableCollection<Header>()
            {
                /*new Header() {Key="dummy key1", Value="dummy value1", IsEnabled=true },
                new Header() {Key="dummy key2 long key example", Value="dummy value2", IsEnabled=false },
                new Header() {Key="dummy key3", Value="dummy value3 long value text example", IsEnabled=true },
                new Header() {Key="dummy key4", Value="dummy value4", IsEnabled=true },
                new Header() {Key="dummy key5", Value="dummy value5", IsEnabled=false },
                new Header() {Key="dummy key3", Value="dummy value3 long value text example", IsEnabled=true },
                new Header() {Key="dummy key4", Value="dummy value4", IsEnabled=true },
                new Header() {Key="dummy key5", Value="dummy value5", IsEnabled=false },*/
            };
            Cookies = new ObservableCollection<Cookie>()
            {
                new Cookie() {Key="cookie1", Value="dummy value1", IsEnabled=true },
                /*new Cookie() {Key="cookie2_long_key_name", Value="dummy value2", IsEnabled=false },
                new Cookie() {Key="cookie1", Value="dummy value1", IsEnabled=true },
                new Cookie() {Key="cookie2_long_key_name", Value="dummy value2", IsEnabled=false },
                 new Cookie() {Key="cookie1", Value="dummy value1", IsEnabled=true },
                new Cookie() {Key="cookie2_long_key_name", Value="dummy value2", IsEnabled=false },

                */
            };
        }
        public WatcherPropertiesWindowViewModel(WatcherModel model)
        {
            RefFilePath = model.CoreCustomLW.ReferenceFilePath;
            Title = model.CoreCustomLW.Name;
            Uri = model.CoreCustomLW.Href;
            Interval = TimeSpan.FromMilliseconds( model.CoreCustomLW.Interval);
            CookiesAndHeaders = new ObservableCollection<CookieOrHeader>();
            Name = model.CoreCustomLW.Name;
            Color = model.Color.Value;
        }

        

        #region Properties
        //some props shoud not be confused with the props fond within the preset file 
        //those are (usually) immutable, while the props here are user-specific and are bundeled in a config appdata file
        //and they will overrde the default props, 


        private string _Title; //overiides CLWP name
        public string Title
        {
            set { _Title = value; notif(nameof(Title)); }
            get { return _Title; }
        }

        public string Name { get; protected set; }


        private TimeSpan? _Interval; //overrides CLWP defaultInterval
        public TimeSpan? Interval
        {
            set { _Interval = value; notif(nameof(Interval)); }
            get { return _Interval; }
        }



        private string _Uri; //overrides clwp uri, but should be used as readonly
        public string Uri
        {
            set { _Uri = value; notif(nameof(Uri)); }
            get { return _Uri; }
        }




        [Obsolete]
        private ObservableCollection<Header> _Headers; //
        public ObservableCollection<Header> Headers
        {
            set { _Headers = value; notif(nameof(Headers)); }
            get { return _Headers; }
        }


        [Obsolete]
        private ObservableCollection<Cookie> _Cookies;
        public ObservableCollection<Cookie> Cookies
        {
            set { _Cookies = value; notif(nameof(Cookies)); }
            get { return _Cookies; }
        }




        private Color _Color;  //overrides clwp color
        public Color Color
        {
            set { _Color = value; notif(nameof(Color));
                ((SolidColorBrush)_ColorAsBrush).Color = value;
                notif(nameof(ColorAsBrush));
            }
            get { return _Color; }
        }


        private Brush _ColorAsBrush  = new SolidColorBrush(Colors.WhiteSmoke);
        public Brush ColorAsBrush 
        {
            
            get { return _ColorAsBrush; }
        }



        private bool _IsMute; //
        public bool IsMute
        {
            set { _IsMute = value; notif(nameof(IsMute)); }
            get { return _IsMute; }
        }




        private string _RefFilePath; // overrides clwp referenceFile
        public string RefFilePath
        {
            set { _RefFilePath = value; notif(nameof(RefFilePath)); }
            get { return _RefFilePath; }
        }



        private ObservableCollection<CookieOrHeader> _CookiesAndHeadrs;
        public ObservableCollection<CookieOrHeader> CookiesAndHeaders
        {
            set { _CookiesAndHeadrs = value; notif(nameof(CookiesAndHeaders)); }
            get { return _CookiesAndHeadrs; }
        }





        public Window TargetWindow { get; set; }








        #endregion


        #region Methods&Commands
        public ICommand AddCookieCommand {get{return new MICommand(()=> { CookiesAndHeaders.Add(new Cookie()); });}}
        public ICommand AddHeaderCommand {get{return new MICommand(() => { CookiesAndHeaders.Add(new Header()); });}}

      

      
        public ICommand DeleteCookieOrHeaderCommand  {get {return new MICommand<object>((param) => {
                    CookieOrHeader ch = param as CookieOrHeader;
                    if (ch != null) CookiesAndHeaders.Remove(ch);});}
        }


        public ICommand SaveCommand
        {
            get
            {
                return new MICommand(() => {
                    MainWindow.ShowMessage("Watcher properties updated");
                    CloseRequest?.Invoke(this, new EventArgs());
                }, ()=> {
                    return string.IsNullOrEmpty(Error);
                });
            }
        }
        public ICommand CancelCommand
        {
            get
            {
                return new MICommand(() => {
                    CloseRequest?.Invoke(this, new EventArgs());
                });
            }
        }

        public string Error
        {
            get
            {
                return error;
            }
        }

        public event EventHandler CloseRequest;

       

        private string error = string.Empty;
        public string this[string columnName]
        {
            get
            {
                error = string.Empty;
                switch (columnName)
                {
                    case nameof(Interval):
                        if (TimeSpan.Zero == Interval)
                            error = "Invalid interval";
                        else if (Interval < TimeSpan.FromSeconds(10))
                            error = "5 seconds min required";
                        else if (Interval > TimeSpan.FromDays(10))
                            error = "Interval too big";

                        break;
                    case nameof(RefFilePath):
                        System.Uri pot_uri;
                        if (!System.Uri.TryCreate(RefFilePath, UriKind.Absolute, out pot_uri))
                            error = "Invalid File Path";
                        else if (!System.IO.File.Exists(RefFilePath))
                            error = "Specified file doesn't exist";
                        break;
                    case nameof(Uri):
                        System.Uri pot_uri2;
                        if (!System.Uri.TryCreate(RefFilePath, UriKind.Absolute, out pot_uri))
                            error = "Invalid URL";
                       
                        break;
                    default:
                        break;
                }

                notif(nameof(Error));
                return error;
            }
        }

        #endregion
    }

    public class Header: CookieOrHeader
    {
        

        public override CookieHeaderEnum Type{get{return CookieHeaderEnum.Header;}}
    }
    public class Cookie : CookieOrHeader
    {
       
        public override CookieHeaderEnum Type { get { return CookieHeaderEnum.Cookie; } }

    }
    public enum CookieHeaderEnum { Cookie, Header}
    public abstract class CookieOrHeader
    {
        abstract public CookieHeaderEnum Type { get;  }
         public string Key { get; set; }
         public string Value { get; set; }
         public bool IsEnabled { get; set; }
    }

    public class CookieOrHeaderDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
           if(item!= null && container!=null && item is CookieOrHeader)
            {
                if (((CookieOrHeader)item).Type == CookieHeaderEnum.Cookie)
                {
                    Trace.WriteLine("cookie");
                    return (DataTemplate)(container as FrameworkElement).FindResource("CookieTemplate");
                }
                else if (((CookieOrHeader)item).Type == CookieHeaderEnum.Header)
                {
                    Trace.WriteLine("header");

                    return (DataTemplate)(container as FrameworkElement).FindResource("ConnectionHeaderTemplate");

                }
                else return null;
            }
            else return null;
        }
    }

    



    public interface ICloseWindowViewModel
    {

        /// <summary>
        /// invoking this from the view model leads to closing the window
        /// usage: invoke the event e.g at the end of a cancel or save command action
        /// </summary>
        event EventHandler CloseRequest;

        /// <summary>
        /// the attached property ViewModel takes care of assigning this with the window reference, in orther to use it when handeling the CloseRequest event
        /// USAGE: do not use directely as the WindowPropeties class uses it automatically through the attached property "ViewModel" that is supposed to be bound correctely to the ViewModel (data context) in the xaml 
        /// </summary>
        Window TargetWindow { get; set; }
    }

}
