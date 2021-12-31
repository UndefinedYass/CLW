using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLW.Model;

namespace CLW.Converters
{
    public static class TempGen
    {

        /// <summary>
        /// converts the old xpado type into MVVM replacement NewModel
        /// untill the old stuff is fully deprecated
        /// </summary>
        public static NewModel expandoLWOtoNewModel(DictBasedNewItemObject elwo)
        {
            if (elwo == null) return null;
            var nm = new NewModel();
            nm.ID = elwo.ID;
            nm.Link = elwo.Link;
            nm.DownloadLink = elwo.Link;
            nm.SubTitle = elwo.SubTitle;
            nm.TextContent = elwo.TextContent;
            nm.Title = elwo.Title;
            return nm;
        }
    }
}
