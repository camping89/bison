using Nop.Web.Framework.Mvc.Models;
using System.Collections.Generic;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog
{
    public class CategorySimpleModel : BaseNopEntityModel
    {
        public CategorySimpleModel()
        {
            SubCategories = new List<CategorySimpleModel>();
        }
        public PictureModel PictureModel { get; set; }
        public string Name { get; set; }

        public string SeName { get; set; }

        public int? NumberOfProducts { get; set; }

        public bool IncludeInTopMenu { get; set; }
        public List<CategorySimpleModel> SubCategories { get; set; }
    }
}