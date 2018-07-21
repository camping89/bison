using System.Collections.Generic;

namespace Nop.Web.Models.News
{
    public class CategoriesNewsModel
    {
        public CategoriesNewsModel()
        {
            CategoriesNewsItems = new List<CategoryItemNewsModel>();
        }
        public List<CategoryItemNewsModel> CategoriesNewsItems { get; set; }
    }
}
