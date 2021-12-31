using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CLW.Model.Enums
{

    /// <summary>
    /// coincitental grouping of usefull enums
    /// </summary>

    public enum TrayIconOverlayMode { Default, Warning, Dot, }


    public enum SpecialProps
    {
        Title, SubTitle, Link,
        none
    }

    public enum CPEElements
    {
        Wraper, Replacer, Matcher, ListParser, Item, Value, Group, Success,
        TargetProperty, None,
        Tracer,
        HtmlParser,
        InnerHtml,
        AttributeValue, HtmlTextNode
    }

    public static class MockWebClient {
        public static Dictionary<string, string> _transformUrlToMockedVersion = null; 
        public static Dictionary<string, string> TransformUrlToMockedVersion
        {

            get
            {
                if (_transformUrlToMockedVersion == null)
                {
                    _transformUrlToMockedVersion = new Dictionary<string, string>();
                    _transformUrlToMockedVersion
                        .Add("http://fsdmfes.ac.ma/uploads/Docs/Files/",
                        "http://127.0.0.1:5001/uploads-sim.html");
                    _transformUrlToMockedVersion
                       .Add("http://fsdmfes.ac.ma/Publications",
                       "http://127.0.0.1:5001/fsdm-publications.html");
                    _transformUrlToMockedVersion
                       .Add("http://fsdmfes.ac.ma/Events",
                       "http://127.0.0.1:5001/fsdm-events.html");
                    _transformUrlToMockedVersion
                       .Add("http://fsdmfes.ac.ma/Theses",
                       "http://127.0.0.1:5001/fsdm-theses.html");
                    _transformUrlToMockedVersion
                       .Add("http://fsdmfes.ac.ma/Soutenances",
                       "http://127.0.0.1:5001/fsdm-soutenances.html");
                    _transformUrlToMockedVersion
                       .Add("http://fsdmfes.ac.ma/News",
                       "http://127.0.0.1:5001/News.html");




                }
                return _transformUrlToMockedVersion;
            }


        }
        
    }

}
