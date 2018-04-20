using Nop.Web.Framework.Mvc.Models;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Web.Models.Catalog
{
    public partial class TopMenuModel : BaseNopModel
    {
        public TopMenuModel()
        {
            Categories = new List<CategorySimpleModel>();
            Topics = new List<TopicModel>();
            CateSpecificationAttributes = new List<CateSpecificationAttributeSimpleModel>();
        }

        public IList<CategorySimpleModel> Categories { get; set; }
        public IList<TopicModel> Topics { get; set; }
        
        public List<CateSpecificationAttributeSimpleModel> CateSpecificationAttributes { get; set; }
        public bool BlogEnabled { get; set; }
        public bool NewProductsEnabled { get; set; }
        public bool ForumEnabled { get; set; }

        public bool DisplayHomePageMenuItem { get; set; }
        public bool DisplayNewProductsMenuItem { get; set; }
        public bool DisplayProductSearchMenuItem { get; set; }
        public bool DisplayCustomerInfoMenuItem { get; set; }
        public bool DisplayBlogMenuItem { get; set; }
        public bool DisplayForumsMenuItem { get; set; }
        public bool DisplayContactUsMenuItem { get; set; }

        public bool HasOnlyCategories
        {
            get
            {
                return Categories.Any()
                       && !Topics.Any()
                       && !DisplayHomePageMenuItem
                       && !(DisplayNewProductsMenuItem && NewProductsEnabled)
                       && !DisplayProductSearchMenuItem
                       && !DisplayCustomerInfoMenuItem
                       && !(DisplayBlogMenuItem && BlogEnabled)
                       && !(DisplayForumsMenuItem && ForumEnabled)
                       && !DisplayContactUsMenuItem;
            }
        }

        #region Nested classes
        
        public class TopicModel : BaseNopEntityModel
        {
            public string Name { get; set; }
            public string SeName { get; set; }
        }

        public class CategoryLineModel : BaseNopModel
        {
            public int Level { get; set; }
            public bool ResponsiveMobileMenu { get; set; }
            public CategorySimpleModel Category { get; set; }
        }

        public class SpecAttrLineModel : BaseNopModel
        {
            public int Level { get; set; }
            public bool ResponsiveMobileMenu { get; set; }
            public CategorySimpleModel Category { get; set; }
            public CateSpecificationAttributeSimpleModel CateSpecAttr { get; set; }
        }

        public class SpecAttrOptionLineModel : BaseNopModel
        {
            public int Level { get; set; }
            public bool ResponsiveMobileMenu { get; set; }
            public CategorySimpleModel Category { get; set; }
            public CateSaoSimpleModel CateSaoModel { get; set; }
        }

        #endregion
    }
}