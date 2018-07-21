using System.Collections.Generic;

namespace Nop.Web.Models.News
{
    public class RecentNewsItemModel
    {
        public RecentNewsItemModel()
        {
            NewsItems = new List<NewsItemBasicModel>();
        }
        public IList<NewsItemBasicModel> NewsItems { get; set; }
    }

}
