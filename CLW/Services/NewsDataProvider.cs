using CLW.Model;
using CLW.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLW.Services
{
    public class NewsDataProvider
    {
        public Collection<NewModel> GetUnreadNews(string WatcherName)
        {
            Collection<NewModel> news = new Collection<NewModel>();
            news.Add(new NewModel() { });
            var watcher = WatchingService.Instance.Watchers. First((w) => w.CoreCustomLW. Name == WatcherName);
            for (int i = 0; i < watcher.CoreCustomLW. UnreadNews.Count; i++)
            {
               /* ExpandoLWItemObject NewItem = watcher.CoreCustomLW. UnreadNews[i];
                Dictionary<string, object> new_dict = new Dictionary<string, object>();
                new_dict.Add("ID", NewItem.ID);
                new_dict.Add("Ttle", NewItem.Title);
                new_dict.Add("SubTitle", NewItem.SubTitle);
                new_dict.Add("Download", NewItem.Link);
                new_dict.Add("Link", NewItem.Link);

                news.Add(new NewModel() { Props = new_dict });*/
            }

            return news;

        }
    }
}
