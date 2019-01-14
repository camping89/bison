using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Mvc.Models;
using Nop.Web.Models.Media;
using System.Collections.Generic;

namespace Nop.Web.Models.Catalog
{
    public partial class ProductFilterModel : CategoryModel
    {

    }
    public partial class CategoryModel : BaseNopEntityModel
    {
        public CategoryModel()
        {
            PictureModel = new PictureModel();
            FeaturedProducts = new List<ProductOverviewModel>();
            Products = new List<ProductOverviewModel>();
            PagingFilteringContext = new CatalogPagingFilteringModel();
            SubCategories = new List<SubCategoryModel>();
            CategoryBreadcrumb = new List<CategoryModel>();
            ShowPriceProduct = true;
            Manufacturers = new List<Manufacturer>();
            AllManufacturers = new List<Manufacturer>();
            CategoriesFilteredIds = new List<int>();
        }

        public List<Manufacturer> Manufacturers { get; set; }
        public IList<Manufacturer> AllManufacturers { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public bool ShowPriceProduct { get; set; }

        public string CurrentSpecId { get; set; }
        public PictureModel PictureModel { get; set; }

        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }

        public bool DisplayCategoryBreadcrumb { get; set; }
        public IList<CategoryModel> CategoryBreadcrumb { get; set; }

        public List<SubCategoryModel> SubCategories { get; set; }
        public List<int> CategoriesFilteredIds { get; set; }

        public IList<ProductOverviewModel> FeaturedProducts { get; set; }
        public IList<ProductOverviewModel> Products { get; set; }

        #region Nested Classes

        public partial class SubCategoryModel : BaseNopEntityModel
        {
            public SubCategoryModel()
            {
                PictureModel = new PictureModel();
            }

            public string Name { get; set; }

            public string SeName { get; set; }

            public string Description { get; set; }

            public PictureModel PictureModel { get; set; }
        }

        #endregion
    }

}