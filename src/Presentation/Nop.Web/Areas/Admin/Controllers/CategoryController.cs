using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Helpers;
using Nop.Web.Areas.Admin.Infrastructure.Cache;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class CategoryController : BaseAdminController
    {
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IDiscountService _discountService;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IExportManager _exportManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IVendorService _vendorService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IWorkContext _workContext;
        private readonly IImportManager _importManager;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ICategoryAttributeService _categoryAttributeService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        #endregion

        #region Ctor

        public CategoryController(ICategoryService categoryService, ICategoryTemplateService categoryTemplateService,
            IManufacturerService manufacturerService, IProductService productService,
            ICustomerService customerService,
            IUrlRecordService urlRecordService,
            IPictureService pictureService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IDiscountService discountService,
            IPermissionService permissionService,
            IAclService aclService,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IExportManager exportManager,
            IVendorService vendorService,
            ICustomerActivityService customerActivityService,
            CatalogSettings catalogSettings,
            IWorkContext workContext,
            IImportManager importManager,
            IStaticCacheManager cacheManager, IProductAttributeService productAttributeService, ISpecificationAttributeService specificationAttributeService, ICategoryAttributeService categoryAttributeService)
        {
            this._categoryService = categoryService;
            this._categoryTemplateService = categoryTemplateService;
            this._manufacturerService = manufacturerService;
            this._productService = productService;
            this._customerService = customerService;
            this._urlRecordService = urlRecordService;
            this._pictureService = pictureService;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._discountService = discountService;
            this._permissionService = permissionService;
            this._vendorService = vendorService;
            this._aclService = aclService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._exportManager = exportManager;
            this._customerActivityService = customerActivityService;
            this._catalogSettings = catalogSettings;
            this._workContext = workContext;
            this._importManager = importManager;
            this._cacheManager = cacheManager;
            _productAttributeService = productAttributeService;
            _specificationAttributeService = specificationAttributeService;
            _categoryAttributeService = categoryAttributeService;
        }

        #endregion

        #region Utilities

        protected virtual void UpdateLocales(Category category, CategoryModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(category,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = category.ValidateSeName(localized.SeName, localized.Name, false);
                _urlRecordService.SaveSlug(category, seName, localized.LanguageId);
            }
        }
        protected virtual void UpdateLocales(ProductAttributeMapping pam, CategoryProductAttributeMappingModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(pam,
                    x => x.TextPrompt,
                    localized.TextPrompt,
                    localized.LanguageId);
            }
        }


        protected virtual void UpdatePictureSeoNames(Category category)
        {
            var picture = _pictureService.GetPictureById(category.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(category.Name));
        }

        protected virtual void PrepareCategoryModel(CategoryModel model)
        {
            if (model != null)
            {
                //specification attributes
                model.AddSpecificationAttributeModel.AvailableAttributes = _cacheManager
                    .Get(ModelCacheEventConsumer.SPEC_ATTRIBUTES_MODEL_KEY, () =>
                    {
                        var availableSpecificationAttributes = new List<SelectListItem>();
                        foreach (var sa in _specificationAttributeService.GetSpecificationAttributes())
                        {
                            availableSpecificationAttributes.Add(new SelectListItem
                            {
                                Text = sa.Name,
                                Value = sa.Id.ToString()
                            });
                        }
                        return availableSpecificationAttributes;
                    });

                //options of preselected specification attribute
                if (model.AddSpecificationAttributeModel.AvailableAttributes.Any())
                {
                    var selectedAttributeId = int.Parse(model.AddSpecificationAttributeModel.AvailableAttributes.First().Value);
                    foreach (var sao in _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(selectedAttributeId))
                        model.AddSpecificationAttributeModel.AvailableOptions.Add(new SelectListItem
                        {
                            Text = sao.Name,
                            Value = sao.Id.ToString()
                        });
                }
                //default specs values
                model.AddSpecificationAttributeModel.ShowOnProductPage = true;
            }
        }
        protected virtual void PrepareAllCategoriesModel(CategoryModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.AvailableCategories.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Catalog.Categories.Fields.Parent.None"),
                Value = "0"
            });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);
        }

        protected virtual void PrepareTemplatesModel(CategoryModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var templates = _categoryTemplateService.GetAllCategoryTemplates();
            foreach (var template in templates)
            {
                model.AvailableCategoryTemplates.Add(new SelectListItem
                {
                    Text = template.Name,
                    Value = template.Id.ToString()
                });
            }
        }

        protected virtual void PrepareDiscountModel(CategoryModel model, Category category, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties && category != null)
                model.SelectedDiscountIds = category.AppliedDiscounts.Select(d => d.Id).ToList();

            foreach (var discount in _discountService.GetAllDiscounts(DiscountType.AssignedToCategories, showHidden: true))
            {
                model.AvailableDiscounts.Add(new SelectListItem
                {
                    Text = discount.Name,
                    Value = discount.Id.ToString(),
                    Selected = model.SelectedDiscountIds.Contains(discount.Id)
                });
            }
        }

        protected virtual void PrepareAclModel(CategoryModel model, Category category, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties && category != null)
                model.SelectedCustomerRoleIds = _aclService.GetCustomerRoleIdsWithAccess(category).ToList();

            var allRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var role in allRoles)
            {
                model.AvailableCustomerRoles.Add(new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Id.ToString(),
                    Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
                });
            }
        }

        protected virtual void SaveCategoryAcl(Category category, CategoryModel model)
        {
            category.SubjectToAcl = model.SelectedCustomerRoleIds.Any();

            var existingAclRecords = _aclService.GetAclRecords(category);
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        _aclService.InsertAclRecord(category, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }

        protected virtual void PrepareStoresMappingModel(CategoryModel model, Category category, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties && category != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(category).ToList();

            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                model.AvailableStores.Add(new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString(),
                    Selected = model.SelectedStoreIds.Contains(store.Id)
                });
            }
        }

        protected virtual void SaveStoreMappings(Category category, CategoryModel model)
        {
            category.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(category);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(category, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var model = new CategoryListModel();
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command, CategoryListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedKendoGridJson();

            var categories = _categoryService.GetAllCategories(model.SearchCategoryName,
                model.SearchStoreId, command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = categories.Select(x =>
                {
                    var categoryModel = x.ToModel();
                    categoryModel.Breadcrumb = x.GetFormattedBreadCrumb(_categoryService);
                    return categoryModel;
                }),
                Total = categories.TotalCount
            };
            return Json(gridModel);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var model = new CategoryModel();
            //locales
            AddLocales(_languageService, model.Locales);
            //templates
            PrepareTemplatesModel(model);
            //categories
            PrepareAllCategoriesModel(model);
            //discounts
            PrepareDiscountModel(model, null, true);
            //ACL
            PrepareAclModel(model, null, false);
            //Stores
            PrepareStoresMappingModel(model, null, false);
            //default values
            model.PageSize = _catalogSettings.DefaultCategoryPageSize;
            model.PageSizeOptions = _catalogSettings.DefaultCategoryPageSizeOptions;
            model.Published = true;
            model.IncludeInTopMenu = true;
            model.AllowCustomersToSelectPageSize = true;

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(CategoryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var category = model.ToEntity();
                category.CreatedOnUtc = DateTime.UtcNow;
                category.UpdatedOnUtc = DateTime.UtcNow;
                _categoryService.InsertCategory(category);
                //search engine name
                model.SeName = category.ValidateSeName(model.SeName, category.Name, true);
                _urlRecordService.SaveSlug(category, model.SeName, 0);
                //locales
                UpdateLocales(category, model);
                //discounts
                var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToCategories, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                        category.AppliedDiscounts.Add(discount);
                }
                _categoryService.UpdateCategory(category);
                //update picture seo file name
                UpdatePictureSeoNames(category);
                //ACL (customer roles)
                SaveCategoryAcl(category, model);
                //Stores
                SaveStoreMappings(category, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewCategory", _localizationService.GetResource("ActivityLog.AddNewCategory"), category.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = category.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            //templates
            PrepareTemplatesModel(model);
            //categories
            PrepareAllCategoriesModel(model);
            //discounts
            PrepareDiscountModel(model, null, true);
            //ACL
            PrepareAclModel(model, null, true);
            //Stores
            PrepareStoresMappingModel(model, null, true);
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var category = _categoryService.GetCategoryById(id);
            if (category == null || category.Deleted)
                //No category found with the specified id
                return RedirectToAction("List");

            var model = category.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = category.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = category.GetLocalized(x => x.Description, languageId, false, false);
                locale.MetaKeywords = category.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = category.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = category.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = category.GetSeName(languageId, false, false);
            });
            PrepareCategoryModel(model);
            //templates
            PrepareTemplatesModel(model);
            //categories
            PrepareAllCategoriesModel(model);
            //discounts
            PrepareDiscountModel(model, category, false);
            //ACL
            PrepareAclModel(model, category, false);
            //Stores
            PrepareStoresMappingModel(model, category, false);
            model.CategoryProductAttributesExist = _productAttributeService.GetAllProductAttributes().Any();
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(CategoryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var category = _categoryService.GetCategoryById(model.Id);
            if (category == null || category.Deleted)
                //No category found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevPictureId = category.PictureId;
                category = model.ToEntity(category);
                category.UpdatedOnUtc = DateTime.UtcNow;
                _categoryService.UpdateCategory(category);
                //search engine name
                model.SeName = category.ValidateSeName(model.SeName, category.Name, true);
                _urlRecordService.SaveSlug(category, model.SeName, 0);
                //locales
                UpdateLocales(category, model);
                //discounts
                var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToCategories, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    {
                        //new discount
                        if (category.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                            category.AppliedDiscounts.Add(discount);
                    }
                    else
                    {
                        //remove discount
                        if (category.AppliedDiscounts.Count(d => d.Id == discount.Id) > 0)
                            category.AppliedDiscounts.Remove(discount);
                    }
                }
                _categoryService.UpdateCategory(category);
                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != category.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }
                //update picture seo file name
                UpdatePictureSeoNames(category);
                //ACL
                SaveCategoryAcl(category, model);
                //Stores
                SaveStoreMappings(category, model);

                //activity log
                _customerActivityService.InsertActivity("EditCategory", _localizationService.GetResource("ActivityLog.EditCategory"), category.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = category.Id });
                }
                return RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            //templates
            PrepareTemplatesModel(model);
            //categories
            PrepareAllCategoriesModel(model);
            //discounts
            PrepareDiscountModel(model, category, true);
            //ACL
            PrepareAclModel(model, category, true);
            //Stores
            PrepareStoresMappingModel(model, category, true);
            model.CategoryProductAttributesExist = _productAttributeService.GetAllProductAttributes().Any();
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var category = _categoryService.GetCategoryById(id);
            if (category == null)
                //No category found with the specified id
                return RedirectToAction("List");

            _categoryService.DeleteCategory(category);

            //activity log
            _customerActivityService.InsertActivity("DeleteCategory", _localizationService.GetResource("ActivityLog.DeleteCategory"), category.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Deleted"));
            return RedirectToAction("List");
        }


        #endregion

        #region Export / Import

        public virtual IActionResult ExportXml()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            try
            {
                var xml = _exportManager.ExportCategoriesToXml();
                return File(Encoding.UTF8.GetBytes(xml), "application/xml", "categories.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        public virtual IActionResult ExportXlsx()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            try
            {
                var bytes = _exportManager.ExportCategoriesToXlsx(_categoryService.GetAllCategories(showHidden: true).Where(p => !p.Deleted).ToList());
                return File(bytes, MimeTypes.TextXlsx, "categories.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual IActionResult ImportFromXlsx(IFormFile importexcelfile)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //a vendor cannot import categories
            if (_workContext.CurrentVendor != null)
                return AccessDeniedView();

            try
            {
                if (importexcelfile != null && importexcelfile.Length > 0)
                {
                    _importManager.ImportCategoriesFromXlsx(importexcelfile.OpenReadStream());
                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Imported"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion

        #region Products

        [HttpPost]
        public virtual IActionResult ProductList(DataSourceRequest command, int categoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedKendoGridJson();

            var productCategories = _categoryService.GetProductCategoriesByCategoryId(categoryId,
                command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = productCategories.Select(x => new CategoryModel.CategoryProductModel
                {
                    Id = x.Id,
                    CategoryId = x.CategoryId,
                    ProductId = x.ProductId,
                    ProductName = _productService.GetProductById(x.ProductId).Name,
                    IsFeaturedProduct = x.IsFeaturedProduct,
                    DisplayOrder = x.DisplayOrder
                }),
                Total = productCategories.TotalCount
            };

            return Json(gridModel);
        }

        public virtual IActionResult ProductUpdate(CategoryModel.CategoryProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var productCategory = _categoryService.GetProductCategoryById(model.Id);
            if (productCategory == null)
                throw new ArgumentException("No product category mapping found with the specified id");

            productCategory.IsFeaturedProduct = model.IsFeaturedProduct;
            productCategory.DisplayOrder = model.DisplayOrder;
            _categoryService.UpdateProductCategory(productCategory);

            return new NullJsonResult();
        }

        public virtual IActionResult ProductDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var productCategory = _categoryService.GetProductCategoryById(id);
            if (productCategory == null)
                throw new ArgumentException("No product category mapping found with the specified id");

            //var categoryId = productCategory.CategoryId;
            _categoryService.DeleteProductCategory(productCategory);

            return new NullJsonResult();
        }

        public virtual IActionResult ProductAddPopup(int categoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var model = new CategoryModel.AddCategoryProductModel();
            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var manufacturers = SelectListHelper.GetManufacturerList(_manufacturerService, _cacheManager, true);
            foreach (var m in manufacturers)
                model.AvailableManufacturers.Add(m);

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
            foreach (var v in vendors)
                model.AvailableVendors.Add(v);

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ProductAddPopupList(DataSourceRequest command, CategoryModel.AddCategoryProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedKendoGridJson();

            var gridModel = new DataSourceResult();
            var products = _productService.SearchProducts(
                categoryIds: new List<int> { model.SearchCategoryId },
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true
                );
            gridModel.Data = products.Select(x => x.ToModel());
            gridModel.Total = products.TotalCount;

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult ProductAddPopup(CategoryModel.AddCategoryProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            if (model.SelectedProductIds != null)
            {
                foreach (var id in model.SelectedProductIds)
                {
                    var product = _productService.GetProductById(id);
                    if (product != null)
                    {
                        var existingProductCategories = _categoryService.GetProductCategoriesByCategoryId(model.CategoryId, showHidden: true);
                        if (existingProductCategories.FindProductCategory(id, model.CategoryId) == null)
                        {
                            _categoryService.InsertProductCategory(
                                new ProductCategory
                                {
                                    CategoryId = model.CategoryId,
                                    ProductId = id,
                                    IsFeaturedProduct = false,
                                    DisplayOrder = 1
                                });
                        }
                    }
                }
            }

            ViewBag.RefreshPage = true;
            return View(model);
        }

        #endregion

        #region Category Attribute Mapping
        [HttpPost]
        public virtual IActionResult CategoryProductAttributeMappingList(DataSourceRequest command, int categoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null)
                throw new ArgumentException("No category found with the specified id");
            var attributes = _categoryAttributeService.GetByCatId(categoryId);
            var attributesModel = attributes
                .Select(x =>
                {
                    var attributeModel = new CategoryProductAttributeMappingModel();
                    PrepareCategoryProductAttributeMappingModel(attributeModel, x, x.Category, false);
                    return attributeModel;
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = attributesModel,
                Total = attributesModel.Count
            };

            return Json(gridModel);
        }
        protected virtual void PrepareCategoryProductAttributeMappingModel(CategoryProductAttributeMappingModel model,
            CategoryProductAttributeMapping pam, Category category, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (category == null)
                throw new ArgumentNullException(nameof(category));

            model.CategoryId = category.Id;

            foreach (var productAttribute in _productAttributeService.GetAllProductAttributes())
            {
                model.AvailableProductAttributes.Add(new SelectListItem
                {
                    Text = productAttribute.Name,
                    Value = productAttribute.Id.ToString()
                });
            }

            if (pam == null)
                return;

            model.Id = pam.Id;
            model.ProductAttribute = _productAttributeService.GetProductAttributeById(pam.ProductAttributeId).Name;
            model.AttributeControlType = pam.AttributeControlType.GetLocalizedEnum(_localizationService, _workContext);
            model.IsUpdateProduct = true;
            if (!excludeProperties)
            {
                model.ProductAttributeId = pam.ProductAttributeId;
                model.TextPrompt = pam.TextPrompt;
                model.IsRequired = pam.IsRequired;
                model.AttributeControlTypeId = pam.AttributeControlTypeId;
                model.DisplayOrder = pam.DisplayOrder;
                model.ValidationMinLength = pam.ValidationMinLength;
                model.ValidationMaxLength = pam.ValidationMaxLength;
                model.ValidationFileAllowedExtensions = pam.ValidationFileAllowedExtensions;
                model.ValidationFileMaximumSize = pam.ValidationFileMaximumSize;
                model.DefaultValue = pam.DefaultValue;
            }

            if (pam.ValidationRulesAllowed())
            {
                var validationRules = new StringBuilder(string.Empty);
                if (pam.ValidationMinLength != null)
                    validationRules.AppendFormat("{0}: {1}<br />",
                        _localizationService.GetResource("Admin.Catalog.Categories.CategoryProductAttributes.Attributes.ValidationRules.MinLength"),
                        pam.ValidationMinLength);
                if (pam.ValidationMaxLength != null)
                    validationRules.AppendFormat("{0}: {1}<br />",
                        _localizationService.GetResource("Admin.Catalog.Categories.CategoryProductAttributes.Attributes.ValidationRules.MaxLength"),
                        pam.ValidationMaxLength);
                if (!string.IsNullOrEmpty(pam.ValidationFileAllowedExtensions))
                    validationRules.AppendFormat("{0}: {1}<br />",
                        _localizationService.GetResource("Admin.Catalog.Categories.CategoryProductAttributes.Attributes.ValidationRules.FileAllowedExtensions"),
                        WebUtility.HtmlEncode(pam.ValidationFileAllowedExtensions));
                if (pam.ValidationFileMaximumSize != null)
                    validationRules.AppendFormat("{0}: {1}<br />",
                        _localizationService.GetResource("Admin.Catalog.Categories.CategoryProductAttributes.Attributes.ValidationRules.FileMaximumSize"),
                        pam.ValidationFileMaximumSize);
                if (!string.IsNullOrEmpty(pam.DefaultValue))
                    validationRules.AppendFormat("{0}: {1}<br />",
                        _localizationService.GetResource("Admin.Catalog.Categories.CategoryProductAttributes.Attributes.ValidationRules.DefaultValue"),
                        WebUtility.HtmlEncode(pam.DefaultValue));
                model.ValidationRulesString = validationRules.ToString();
            }

        }

        public virtual IActionResult CategoryProductAttributeMappingCreate(int categoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null)
                throw new ArgumentException("No product found with the specified id");


            var model = new CategoryProductAttributeMappingModel();
            PrepareCategoryProductAttributeMappingModel(model, null, category, false);
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult CategoryProductAttributeMappingCreate(CategoryProductAttributeMappingModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var category = _categoryService.GetCategoryById(model.CategoryId);
            if (category == null)
                throw new ArgumentException("No product found with the specified id");

            //ensure this attribute is not mapped yet
            if (_categoryAttributeService.GetByCatId(category.Id).Any(x => x.ProductAttributeId == model.ProductAttributeId))
            {
                //redisplay form
                ErrorNotification(_localizationService.GetResource("Admin.Catalog.Categories.CategoryProductAttributes.Attributes.AlreadyExists"));
                //model
                PrepareCategoryProductAttributeMappingModel(model, null, category, true);
                return View(model);
            }

            //insert mapping
            var categoryProductAttributeMapping = new CategoryProductAttributeMapping
            {
                CategoryId = model.CategoryId,
                ProductAttributeId = model.ProductAttributeId,
                TextPrompt = model.TextPrompt,
                IsRequired = model.IsRequired,
                AttributeControlTypeId = model.AttributeControlTypeId,
                DisplayOrder = model.DisplayOrder,
                ValidationMinLength = model.ValidationMinLength,
                ValidationMaxLength = model.ValidationMaxLength,
                ValidationFileAllowedExtensions = model.ValidationFileAllowedExtensions,
                ValidationFileMaximumSize = model.ValidationFileMaximumSize,
                DefaultValue = model.DefaultValue
            };
            var listCateIds = _categoryAttributeService.Insert(categoryProductAttributeMapping, true);
            UpdateLocales(categoryProductAttributeMapping, model);


            // insert mapping for all children category

            //if (model.IsUpdateProduct)
            //{
            //    //predefined values
            //    var predefinedValues = _productAttributeService.GetPredefinedProductAttributeValues(model.ProductAttributeId);

            //    foreach (var cateId in listCateIds)
            //    {
            //        //Get list product
            //        var productCategories = _categoryService.GetProductCategoriesByCategoryId(cateId, showHidden: true);

            //        foreach (var product in productCategories)
            //        {
            //            if (_productAttributeService.GetProductAttributeMappingsByProductId(product.Id)
            //                .Any(x => x.ProductAttributeId == model.ProductAttributeId))
            //            {
            //                continue;
            //            }

            //            //insert mapping
            //            var productAttributeMapping = new ProductAttributeMapping
            //            {
            //                ProductId = product.ProductId,
            //                ProductAttributeId = model.ProductAttributeId,
            //                TextPrompt = model.TextPrompt,
            //                IsRequired = model.IsRequired,
            //                AttributeControlTypeId = model.AttributeControlTypeId,
            //                DisplayOrder = model.DisplayOrder,
            //                ValidationMinLength = model.ValidationMinLength,
            //                ValidationMaxLength = model.ValidationMaxLength,
            //                ValidationFileAllowedExtensions = model.ValidationFileAllowedExtensions,
            //                ValidationFileMaximumSize = model.ValidationFileMaximumSize,
            //                DefaultValue = model.DefaultValue
            //            };
            //            _productAttributeService.InsertProductAttributeMapping(productAttributeMapping);
            //            UpdateLocales(productAttributeMapping, model);

            //            if (productAttributeMapping.Id > 0 && !_productAttributeService.GetProductAttributeValues(productAttributeMapping.Id).Any())
            //            {
            //                foreach (var predefinedValue in predefinedValues)
            //                {
            //                    var pav = new ProductAttributeValue
            //                    {
            //                        ProductAttributeMappingId = productAttributeMapping.Id,
            //                        AttributeValueType = AttributeValueType.Simple,
            //                        Name = predefinedValue.Name,
            //                        PriceAdjustment = predefinedValue.PriceAdjustment,
            //                        WeightAdjustment = predefinedValue.WeightAdjustment,
            //                        Cost = predefinedValue.Cost,
            //                        IsPreSelected = predefinedValue.IsPreSelected,
            //                        DisplayOrder = predefinedValue.DisplayOrder
            //                    };
            //                    _productAttributeService.InsertProductAttributeValue(pav);
            //                    //locales
            //                    var languages = _languageService.GetAllLanguages(true);
            //                    //localization
            //                    foreach (var lang in languages)
            //                    {
            //                        var name = predefinedValue.GetLocalized(x => x.Name, lang.Id, false, false);
            //                        if (!string.IsNullOrEmpty(name))
            //                            _localizedEntityService.SaveLocalizedValue(pav, x => x.Name, name, lang.Id);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.CategoryProductAttributes.Attributes.Added"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("CategoryProductAttributeMappingEdit", new { id = categoryProductAttributeMapping.Id });
            }

            SaveSelectedTabName("tab-product-attributes");
            return RedirectToAction("Edit", new { id = category.Id });
        }

        public virtual IActionResult CategoryProductAttributeMappingEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var pam = _categoryAttributeService.Get(id);
            if (pam == null)
                throw new ArgumentException("No product attribute mapping found with the specified id");

            var category = _categoryService.GetCategoryById(pam.CategoryId);
            if (category == null)
                throw new ArgumentException("No product found with the specified id");

            var model = new CategoryProductAttributeMappingModel();
            PrepareCategoryProductAttributeMappingModel(model, pam, category, false);

            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.TextPrompt = pam.GetLocalized(x => x.TextPrompt, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult CategoryProductAttributeMappingEdit(CategoryProductAttributeMappingModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var categoryProductAttributeMapping = _categoryAttributeService.Get(model.Id);
            if (categoryProductAttributeMapping == null)
                throw new ArgumentException("No category product attribute mapping found with the specified id");

            var category = _categoryService.GetCategoryById(model.CategoryId);
            if (category == null)
                throw new ArgumentException("No product found with the specified id");

            //ensure this attribute is not mapped yet
            if (_categoryAttributeService.GetByCatId(category.Id)
                .Any(x => x.ProductAttributeId == model.ProductAttributeId && x.Id != categoryProductAttributeMapping.Id))
            {
                //redisplay form
                ErrorNotification(_localizationService.GetResource("Admin.Catalog.Categories.CategoryProductAttributes.Attributes.AlreadyExists"));
                //model
                PrepareCategoryProductAttributeMappingModel(model, categoryProductAttributeMapping, category, true);
                return View(model);
            }

            categoryProductAttributeMapping.ProductAttributeId = model.ProductAttributeId;
            categoryProductAttributeMapping.TextPrompt = model.TextPrompt;
            categoryProductAttributeMapping.IsRequired = model.IsRequired;
            categoryProductAttributeMapping.AttributeControlTypeId = model.AttributeControlTypeId;
            categoryProductAttributeMapping.DisplayOrder = model.DisplayOrder;
            categoryProductAttributeMapping.ValidationMinLength = model.ValidationMinLength;
            categoryProductAttributeMapping.ValidationMaxLength = model.ValidationMaxLength;
            categoryProductAttributeMapping.ValidationFileAllowedExtensions = model.ValidationFileAllowedExtensions;
            categoryProductAttributeMapping.ValidationFileMaximumSize = model.ValidationFileMaximumSize;
            categoryProductAttributeMapping.DefaultValue = model.DefaultValue;
            _categoryAttributeService.Update(categoryProductAttributeMapping);

            UpdateLocales(categoryProductAttributeMapping, model);
            //if (model.IsUpdateProduct)
            //{
            //    //Get list product
            //    var productCategories = _categoryService.GetProductCategoriesByCategoryId(model.CategoryId, showHidden: true);
            //    //predefined values
            //    var predefinedValues = _productAttributeService.GetPredefinedProductAttributeValues(model.ProductAttributeId);
            //    foreach (var product in productCategories)
            //    {
            //        var productAttributeMapping = _productAttributeService.GetProductAttributeMappingsByProductId(product.ProductId)
            //            .FirstOrDefault(x => x.ProductAttributeId == model.ProductAttributeId);
            //        if (productAttributeMapping == null)
            //        {
            //            //insert mapping
            //            productAttributeMapping = new ProductAttributeMapping
            //            {
            //                ProductId = product.ProductId,
            //                ProductAttributeId = model.ProductAttributeId,
            //                TextPrompt = model.TextPrompt,
            //                IsRequired = model.IsRequired,
            //                AttributeControlTypeId = model.AttributeControlTypeId,
            //                DisplayOrder = model.DisplayOrder,
            //                ValidationMinLength = model.ValidationMinLength,
            //                ValidationMaxLength = model.ValidationMaxLength,
            //                ValidationFileAllowedExtensions = model.ValidationFileAllowedExtensions,
            //                ValidationFileMaximumSize = model.ValidationFileMaximumSize,
            //                DefaultValue = model.DefaultValue
            //            };
            //            _productAttributeService.InsertProductAttributeMapping(productAttributeMapping);
            //        }
            //        else
            //        {
            //            productAttributeMapping.ProductAttributeId = model.ProductAttributeId;
            //            productAttributeMapping.TextPrompt = model.TextPrompt;
            //            productAttributeMapping.IsRequired = model.IsRequired;
            //            productAttributeMapping.AttributeControlTypeId = model.AttributeControlTypeId;
            //            productAttributeMapping.DisplayOrder = model.DisplayOrder;
            //            productAttributeMapping.ValidationMinLength = model.ValidationMinLength;
            //            productAttributeMapping.ValidationMaxLength = model.ValidationMaxLength;
            //            productAttributeMapping.ValidationFileAllowedExtensions = model.ValidationFileAllowedExtensions;
            //            productAttributeMapping.ValidationFileMaximumSize = model.ValidationFileMaximumSize;
            //            productAttributeMapping.DefaultValue = model.DefaultValue;
            //            _productAttributeService.UpdateProductAttributeMapping(productAttributeMapping);

            //        }
            //        if (productAttributeMapping.Id > 0 && !_productAttributeService.GetProductAttributeValues(productAttributeMapping.Id).Any())
            //        {
            //            foreach (var predefinedValue in predefinedValues)
            //            {
            //                var pav = new ProductAttributeValue
            //                {
            //                    ProductAttributeMappingId = productAttributeMapping.Id,
            //                    AttributeValueType = AttributeValueType.Simple,
            //                    Name = predefinedValue.Name,
            //                    PriceAdjustment = predefinedValue.PriceAdjustment,
            //                    WeightAdjustment = predefinedValue.WeightAdjustment,
            //                    Cost = predefinedValue.Cost,
            //                    IsPreSelected = predefinedValue.IsPreSelected,
            //                    DisplayOrder = predefinedValue.DisplayOrder
            //                };
            //                _productAttributeService.InsertProductAttributeValue(pav);
            //                //locales
            //                var languages = _languageService.GetAllLanguages(true);
            //                //localization
            //                foreach (var lang in languages)
            //                {
            //                    var name = predefinedValue.GetLocalized(x => x.Name, lang.Id, false, false);
            //                    if (!string.IsNullOrEmpty(name))
            //                        _localizedEntityService.SaveLocalizedValue(pav, x => x.Name, name, lang.Id);
            //                }
            //            }
            //        }
            //        UpdateLocales(productAttributeMapping, model);
            //    }


            //}
            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.CategoryProductAttributes.Attributes.Updated"));
            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("CategoryProductAttributeMappingEdit", new { id = categoryProductAttributeMapping.Id });
            }

            SaveSelectedTabName("tab-product-attributes");
            return RedirectToAction("Edit", new { id = category.Id });
        }

        [HttpPost]
        public virtual IActionResult CategoryProductAttributeMappingDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var categoryProductAttributeMapping = _categoryAttributeService.Get(id);
            if (categoryProductAttributeMapping == null)
                throw new ArgumentException("No category attribute mapping found with the specified id");

            var categoryId = categoryProductAttributeMapping.CategoryId;
            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null)
                throw new ArgumentException("No category found with the specified id");
            _categoryAttributeService.Delete(categoryProductAttributeMapping);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.CategoryProductAttributes.Attributes.Deleted"));
            SaveSelectedTabName("tab-product-attributes");
            return RedirectToAction("Edit", new { id = categoryId });
        }
        protected virtual void UpdateLocales(CategoryProductAttributeMapping pam, CategoryProductAttributeMappingModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(pam,
                    x => x.TextPrompt,
                    localized.TextPrompt,
                    localized.LanguageId);
            }
        }

        [HttpPost]
        public virtual IActionResult CategorySpecificationAttributeAdd(int attributeTypeId, int specificationAttributeOptionId,
            string customValue, bool allowFiltering, bool showOnProductPage,
            int displayOrder, int categoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();


            //we allow filtering only for "Option" attribute type
            if (attributeTypeId != (int)SpecificationAttributeType.Option)
            {
                allowFiltering = false;
            }
            //we don't allow CustomValue for "Option" attribute type
            if (attributeTypeId == (int)SpecificationAttributeType.Option)
            {
                customValue = null;
            }

            var psa = new CategorySpecificationAttribute
            {
                AttributeTypeId = attributeTypeId,
                SpecificationAttributeOptionId = specificationAttributeOptionId,
                CategoryId = categoryId,
                CustomValue = customValue,
                AllowFiltering = allowFiltering,
                ShowOnProductPage = showOnProductPage,
                DisplayOrder = displayOrder,
            };
            var listCateIds = _specificationAttributeService.Insert(psa,true);

            //if (psa.Id > 0)
            //{
            //    foreach (var cateId in listCateIds)
            //    {
            //        //Get list product
            //        var productCategories = _categoryService.GetProductCategoriesByCategoryId(cateId, showHidden: true);
            //        foreach (var product in productCategories)
            //        {
            //            if (_specificationAttributeService.GetProductSpecificationAttributes(product.Id)
            //                .Any(x => x.ProductId == product.ProductId && x.SpecificationAttributeOptionId == specificationAttributeOptionId))
            //            {
            //                continue;
            //            }
            //            //insert mapping
            //            var psaProduct = new ProductSpecificationAttribute
            //            {
            //                AttributeTypeId = attributeTypeId,
            //                SpecificationAttributeOptionId = specificationAttributeOptionId,
            //                ProductId = product.ProductId,
            //                CustomValue = customValue,
            //                AllowFiltering = allowFiltering,
            //                ShowOnProductPage = showOnProductPage,
            //                DisplayOrder = displayOrder,
            //            };
            //            _specificationAttributeService.InsertProductSpecificationAttribute(psaProduct);
            //        }
            //    }
                
            //}
            return Json(new { Result = true });
        }


        [HttpPost]
        public virtual IActionResult CategorySpecAttrList(DataSourceRequest command, int categoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();



            var productrSpecs = _specificationAttributeService.GetCategorySpecificationAttributes(categoryId);

            var productrSpecsModel = productrSpecs
                .Select(x =>
                {
                    var psaModel = new CategorySpecificationAttributeModel
                    {
                        Id = x.Id,
                        AttributeTypeId = x.AttributeTypeId,
                        AttributeTypeName = x.AttributeType.GetLocalizedEnum(_localizationService, _workContext),
                        AttributeId = x.SpecificationAttributeOption.SpecificationAttribute.Id,
                        AttributeName = x.SpecificationAttributeOption.SpecificationAttribute.Name,
                        AllowFiltering = x.AllowFiltering,
                        ShowOnProductPage = x.ShowOnProductPage,
                        DisplayOrder = x.DisplayOrder
                    };
                    switch (x.AttributeType)
                    {
                        case SpecificationAttributeType.Option:
                            psaModel.ValueRaw = WebUtility.HtmlEncode(x.SpecificationAttributeOption.Name);
                            psaModel.SpecificationAttributeOptionId = x.SpecificationAttributeOptionId;
                            break;
                        case SpecificationAttributeType.CustomText:
                            psaModel.ValueRaw = WebUtility.HtmlEncode(x.CustomValue);
                            break;
                        case SpecificationAttributeType.CustomHtmlText:
                            //do not encode?
                            //psaModel.ValueRaw = x.CustomValue;
                            psaModel.ValueRaw = WebUtility.HtmlEncode(x.CustomValue);
                            break;
                        case SpecificationAttributeType.Hyperlink:
                            psaModel.ValueRaw = x.CustomValue;
                            break;
                        default:
                            break;
                    }
                    return psaModel;
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = productrSpecsModel,
                Total = productrSpecsModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult CategorySpecAttrUpdate(CategorySpecificationAttributeModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var psa = _specificationAttributeService.GetCategorySpecificationAttributeById(model.Id);
            if (psa == null)
                return Content("No category specification attribute found with the specified id");

            //we allow filtering and change option only for "Option" attribute type
            if (model.AttributeTypeId == (int)SpecificationAttributeType.Option)
            {
                psa.AllowFiltering = model.AllowFiltering;
                psa.SpecificationAttributeOptionId = model.SpecificationAttributeOptionId;
            }

            psa.ShowOnProductPage = model.ShowOnProductPage;
            psa.DisplayOrder = model.DisplayOrder;
            _specificationAttributeService.UpdateCategorySpecificationAttribute(psa);
            //Get list product
            //var productCategories = _categoryService.GetProductCategoriesByCategoryId(psa.CategoryId, showHidden: true);
            //foreach (var product in productCategories)
            //{
            //    var psaProduct = _specificationAttributeService.GetProductSpecificationAttributeById(product.ProductId);

            //    //we allow filtering and change option only for "Option" attribute type
            //    if (model.AttributeTypeId == (int)SpecificationAttributeType.Option)
            //    {
            //        psa.AllowFiltering = model.AllowFiltering;
            //        psa.SpecificationAttributeOptionId = model.SpecificationAttributeOptionId;
            //    }

            //    psa.ShowOnProductPage = model.ShowOnProductPage;
            //    psa.DisplayOrder = model.DisplayOrder;
            //    _specificationAttributeService.UpdateProductSpecificationAttribute(psaProduct);
            //}
            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult CategorySpecAttrDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var psa = _specificationAttributeService.GetCategorySpecificationAttributeById(id);
            if (psa == null)
                throw new ArgumentException("No specification attribute found with the specified id");
            ////Get list product
            //var productCategories = _categoryService.GetProductCategoriesByCategoryId(psa.CategoryId, showHidden: true);
            //foreach (var product in productCategories)
            //{
            //    var specAttributeProducts = _specificationAttributeService.GetProductSpecificationAttributes(product.ProductId);
            //    foreach (var spec in specAttributeProducts)
            //    {
            //        if (spec.SpecificationAttributeOptionId == psa.SpecificationAttributeOptionId)
            //        {
            //            _specificationAttributeService.DeleteProductSpecificationAttribute(spec);
            //        }
            //    }
            //}
            _specificationAttributeService.DeleteCategorySpecificationAttribute(psa);

            return new NullJsonResult();
        }

        #endregion
    }
}