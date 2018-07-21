using Nop.Web.Framework.Mvc.Models;
using System.Collections.Generic;

namespace Nop.Web.Models.News
{
    public partial class NewsItemListModel : BaseNopModel
    {
        public NewsItemListModel()
        {
            PagingFilteringContext = new NewsPagingFilteringModel();
            NewsItems = new List<NewsItemModel>();
        }
        public string CategoryName { get; set; }
        public int WorkingLanguageId { get; set; }
        public NewsPagingFilteringModel PagingFilteringContext { get; set; }
        public IList<NewsItemModel> NewsItems { get; set; }
    }
}