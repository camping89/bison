using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Core.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Nop.Web.Models.Catalog;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Web.Controllers
{
    public partial class CatalogController : BasePublicController
    {
        #region Fields

        private readonly ICatalogModelFactory _catalogModelFactory;
        private readonly IProductModelFactory _productModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IProductTagService _productTagService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public CatalogController(ICatalogModelFactory catalogModelFactory,
            IProductModelFactory productModelFactory,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IProductService productService,
            IVendorService vendorService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            IProductTagService productTagService,
            IGenericAttributeService genericAttributeService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            ICustomerActivityService customerActivityService,
            MediaSettings mediaSettings,
            CatalogSettings catalogSettings,
            VendorSettings vendorSettings, ISpecificationAttributeService specificationAttributeService)
        {
            this._catalogModelFactory = catalogModelFactory;
            this._productModelFactory = productModelFactory;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._productService = productService;
            this._vendorService = vendorService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            this._webHelper = webHelper;
            this._productTagService = productTagService;
            this._genericAttributeService = genericAttributeService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._permissionService = permissionService;
            this._customerActivityService = customerActivityService;
            this._mediaSettings = mediaSettings;
            this._catalogSettings = catalogSettings;
            this._vendorSettings = vendorSettings;
            _specificationAttributeService = specificationAttributeService;
        }

        #endregion


        #region Categories

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult Category(int categoryId, CatalogPagingFilteringModel command)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null || category.Deleted)
                return InvokeHttp404();

            var notAvailable =
                //published?
                !category.Published ||
                //ACL (access control list) 
                !_aclService.Authorize(category) ||
                //Store mapping
                !_storeMappingService.Authorize(category);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            if (notAvailable && !_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return InvokeHttp404();

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.LastContinueShoppingPage,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);

            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                DisplayEditLink(Url.Action("Edit", "Category", new { id = category.Id, area = AreaNames.Admin }));

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewCategory", _localizationService.GetResource("ActivityLog.PublicStore.ViewCategory"), category.Name);

            //model
            var model = _catalogModelFactory.PrepareCategoryModel(category, command);

            //template
            var templateViewPath = _catalogModelFactory.PrepareCategoryTemplateViewPath(category.CategoryTemplateId);
            return View(templateViewPath, model);
        }


        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult ProductFilter(CatalogPagingFilteringModel command)
        {

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.LastContinueShoppingPage,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ProductFilter", _localizationService.GetResource("ActivityLog.PublicStore.ProductFilter"));

            //model
            var model = _catalogModelFactory.PrepareProductFilterModel(command);

            return View("ProductTemplateFilter.ProductsInGridOrLines", model);
        }


        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult ProductFilterAjax(CatalogPagingFilteringModel command)
        {

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.LastContinueShoppingPage,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);

            //activity log
            //_customerActivityService.InsertActivity("PublicStore.ProductFilter", _localizationService.GetResource("ActivityLog.PublicStore.ProductFilter"));
            var model = _catalogModelFactory.PrepareProductFilterModelAjax(command);
            return Json(new
            {
                ViewData = RenderPartialViewToString("_ProductFilterAjax", model),
                FilterableSpecAttr = model.PagingFilteringContext.SpecificationFilter.NotFilteredItems.Select(_ => _.SpecificationAttributeOptionId).ToList(),
                ManufacturersIds = model.AllManufacturers.Select(_ => _.Id).ToList(),
                CategoryIds = model.CategoriesFilteredIds,
                ViewDataFilterSpec = RenderPartialViewToString("_FilterSpecsBox", model.PagingFilteringContext.SpecificationFilter)
            });
        }

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult GetSubCategories(int categoryId)
        {
            //var categories = _catalogModelFactory.GetSubCategoryModels(categoryId);
            var categories = _catalogModelFactory.PrepareCategorySimpleModels(categoryId);
            return Json(new { Categories = categories, DisplayType = FilterControlType.SingleCheckbox });
        }

        #endregion

        #region Manufacturers

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult Manufacturer(int manufacturerId, CatalogPagingFilteringModel command)
        {
            var manufacturer = _manufacturerService.GetManufacturerById(manufacturerId);
            if (manufacturer == null || manufacturer.Deleted)
                return InvokeHttp404();

            var notAvailable =
                //published?
                !manufacturer.Published ||
                //ACL (access control list) 
                !_aclService.Authorize(manufacturer) ||
                //Store mapping
                !_storeMappingService.Authorize(manufacturer);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            if (notAvailable && !_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return InvokeHttp404();

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.LastContinueShoppingPage,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);

            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                DisplayEditLink(Url.Action("Edit", "Manufacturer", new { id = manufacturer.Id, area = AreaNames.Admin }));

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewManufacturer", _localizationService.GetResource("ActivityLog.PublicStore.ViewManufacturer"), manufacturer.Name);

            //model
            var model = _catalogModelFactory.PrepareManufacturerModel(manufacturer, command);

            //template
            var templateViewPath = _catalogModelFactory.PrepareManufacturerTemplateViewPath(manufacturer.ManufacturerTemplateId);
            return View(templateViewPath, model);
        }

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult ManufacturerAll()
        {
            var model = _catalogModelFactory.PrepareManufacturerAllModels();
            return View(model);
        }

        #endregion

        #region Vendors

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult Vendor(int vendorId, CatalogPagingFilteringModel command)
        {
            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null || vendor.Deleted || !vendor.Active)
                return InvokeHttp404();

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.LastContinueShoppingPage,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);

            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                DisplayEditLink(Url.Action("Edit", "Vendor", new { id = vendor.Id, area = AreaNames.Admin }));

            //model
            var model = _catalogModelFactory.PrepareVendorModel(vendor, command);

            return View(model);
        }

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult VendorAll()
        {
            //we don't allow viewing of vendors if "vendors" block is hidden
            if (_vendorSettings.VendorsBlockItemsToDisplay == 0)
                return RedirectToRoute("HomePage");

            var model = _catalogModelFactory.PrepareVendorAllModels();
            return View(model);
        }

        #endregion

        #region Product tags

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult ProductsByTag(int productTagId, CatalogPagingFilteringModel command)
        {
            var productTag = _productTagService.GetProductTagById(productTagId);
            if (productTag == null)
                return InvokeHttp404();

            var model = _catalogModelFactory.PrepareProductsByTagModel(productTag, command);
            return View(model);
        }

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult ProductTagsAll()
        {
            var model = _catalogModelFactory.PrepareProductTagsAllModel();
            return View(model);
        }

        #endregion

        #region Searching

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult Search(SearchModel model, CatalogPagingFilteringModel command)
        {
            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.LastContinueShoppingPage,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);

            if (model == null)
                model = new SearchModel();

            model = _catalogModelFactory.PrepareSearchModel(model, command);
            return View("ProductTemplateSearch", model);
        }

        public virtual IActionResult AjaxSearch(SearchModel model, CatalogPagingFilteringModel command)
        {
            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.LastContinueShoppingPage,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);

            if (model == null)
                model = new SearchModel();

            model = _catalogModelFactory.PrepareSearchModel(model, command);
            return Json(new
            {
                ViewData = RenderPartialViewToString("_ProductSearchAjax", model),
                FilterableSpecAttr = model.PagingFilteringContext.SpecificationFilter.AllFilterableItems.Select(_ => _.SpecificationAttributeOptionId).ToList(),
                ManufacturersIds = model.Manufacturers.Select(_ => _.Id).ToList()
            });
        }

        public virtual IActionResult SearchTermAutoComplete(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < _catalogSettings.ProductSearchTermMinimumLength) return Content("");

            var specsOptionIds = _specificationAttributeService.GetSpecificationAttributeOptionsIdsByTerm(term).ToList();
            var childSpecsOptions = _specificationAttributeService.GetSpecificationAttributeOptionsByParentIds(specsOptionIds.ToArray()).Select(s => s.Id).ToList();
            specsOptionIds.AddRange(childSpecsOptions);

            var result = GetSearchTermAutoCompleteByKeywords(term, specsOptionIds);

            return Json(result.DistinctBy(_ => _.producturl));
        }

        private List<CatalogSearchModel> GetSearchTermAutoCompleteByKeywords(string term, List<int> specsOptionIds)
        {
            var productNumber = _catalogSettings.ProductSearchAutoCompleteNumberOfProducts > 0 ? _catalogSettings.ProductSearchAutoCompleteNumberOfProducts : 10;

            var products = _productService.SearchProductsAjax(out IList<int> categoriesFilteredIds,
                out IList<int> filterableSpecificationAttributeOptionIds,
                out IList<int> manufactureFilteredIds,
                storeId: _storeContext.CurrentStore.Id,
                keywords: term,
                filteredSpecs: specsOptionIds,
                languageId: _workContext.WorkingLanguage.Id,
                visibleIndividuallyOnly: true,
                pageSize: productNumber);

            var models = _productModelFactory.PrepareProductOverviewModels(products, false, _catalogSettings.ShowProductImagesInSearchAutoComplete, _mediaSettings.AutoCompleteSearchThumbPictureSize).ToList();
            var result = (from p in models
                          select new CatalogSearchModel
                          {
                              label = p.Name,
                              producturl = Url.RouteUrl("Product", new { SeName = p.SeName }),
                              productpictureurl = p.DefaultPictureModel.ImageUrl
                          })
                .ToList();
            return result;
        }

        #endregion

        public IActionResult CategoryHasProducts(int categoryId)
        {
            var checkHasProduct = _productService.GetHasProductByCategoryId(categoryId);
            return Json(checkHasProduct);
        }

        public IActionResult GetManufactures()
        {
            var model = _manufacturerService.GetAllManufacturers();

            return Json(new { Manufacturers = model.ToList(), DisplayType = FilterControlType.SingleCheckbox });
        }

        public IActionResult GetPrice(int categoryId)
        {
            var dicsRangePrices = new Dictionary<string, int>();
            dicsRangePrices.Add("MinPrice", _catalogSettings.RangeMinPrice);
            dicsRangePrices.Add("MaxPrice", _catalogSettings.RangeMaxPrice);
            return Json(new[] { dicsRangePrices });
        }

    }
}