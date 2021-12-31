using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLW.Model
{
    public class NewModel
    {
        //specially treated properties: Title SubTitle, Date, Link, Download, TextContent

        public NewModel()
        {
            Props = new Dictionary<string, object>();
        }


        //other props are not visible unless user chose to
        public bool IsRead
        {
            get { return true; }
            set { /*importnt to do business logic here*/}
        }
        public bool HasDownload { get { return HasProperty("DownloadLink"); } }
        public bool HasLink { get { return HasProperty("Link"); } }
        public bool HasTextContent { get { return HasProperty("TextContent"); } }
        public bool HasDate { get; set; }


        public Dictionary<string, object> Props { get; set; }


        public string ID
        {
            get { return Props["ID"] as string; }
            set { Props.Add("ID", value); }

        }

        public string TextContent
        {
            get { return Props["TextContent"] as string; }
            set { Props.Add("TextContent", value); }

        }

        public string Title
        {
            get { return HasProperty("Title") ? Props["Title"] as string : "unknown"; }
            set { Props.Add("Title", value); }
        }

        bool HasProperty(string propname)
        {    
            return Props.Keys.Contains(propname);

        }

        public string SubTitle
        {
            get { return HasProperty("SubTitle") ? Props["SubTitle"] as string : "unknown"; }
            set { Props.Add("SubTitle", value); }
        }

        public string Link
        {
            get { return HasProperty("Link") ? Props["Link"] as string : "unknown"; }
            set { Props.Add("Link", value); }
        }

        public string DownloadLink
        {
            get { return HasProperty("DownloadLink") ? Props["DownloadLink"] as string : "unknown"; }
            set { Props.Add("DownloadLink", value); }
        }

        public DateTime? Date
        {
            get { return HasProperty("Date") ? Props["Date"] as DateTime? : new DateTime(); }
        }
    }
}
