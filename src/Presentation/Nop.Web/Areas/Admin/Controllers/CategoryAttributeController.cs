using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class CategoryAttributeController : BaseAdminController
    {
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly ICategoryAttributeService _categoryAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;

        public CategoryAttributeController(ICategoryService categoryService, ICategoryAttributeService categoryAttributeService, ILanguageService languageService, ILocalizedEntityService localizedEntityService, ILocalizationService localizationService, ICustomerActivityService customerActivityService, IPermissionService permissionService)
        {
            _categoryService = categoryService;
            _categoryAttributeService = categoryAttributeService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
            _permissionService = permissionService;
        }

        #endregion Fields

        #region Ctor

        

        #endregion
        
        #region Utilities

        protected virtual void UpdateLocales(CategoryAttribute categoryAttribute, CategoryAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(categoryAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(categoryAttribute,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);
            }
        }

        //protected virtual void UpdateLocales(PredefinedProductAttributeValue ppav, PredefinedProductAttributeValueModel model)
        //{
        //    foreach (var localized in model.Locales)
        //    {
        //        _localizedEntityService.SaveLocalizedValue(ppav,
        //            x => x.Name,
        //            localized.Name,
        //            localized.LanguageId);
        //    }
        //}

        #endregion
        
        #region Methods

        #region Attribute list / create / edit / delete

        //list
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedKendoGridJson();

            var productAttributes = _categoryAttributeService
                .GetAllCategoryAttributes(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = productAttributes.Select(x => x.ToModel()),
                Total = productAttributes.TotalCount
            };

            return Json(gridModel);
        }
        
        //create
        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = new CategoryAttributeModel();
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(CategoryAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var categoryAttribute = model.ToEntity();
                _categoryAttributeService.InsertCategoryAttribute(categoryAttribute);
                UpdateLocales(categoryAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewCategoryAttribute", _localizationService.GetResource("ActivityLog.AddNewCategoryAttribute"), categoryAttribute.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.CategoryAttributes.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = categoryAttribute.Id });
                }
                return RedirectToAction("List");

            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var categoryAttribute = _categoryAttributeService.GetCategoryAttributeById(id);
            if (categoryAttribute == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            var model = categoryAttribute.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = categoryAttribute.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = categoryAttribute.GetLocalized(x => x.Description, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(CategoryAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var categoryAttribute = _categoryAttributeService.GetCategoryAttributeById(model.Id);
            if (categoryAttribute == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");
            
            if (ModelState.IsValid)
            {
                categoryAttribute = model.ToEntity(categoryAttribute);
                _categoryAttributeService.UpdateCategoryAttribute(categoryAttribute);

                UpdateLocales(categoryAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("EditCategoryAttribute", _localizationService.GetResource("ActivityLog.EditCategoryAttribute"), categoryAttribute.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.CategoryAttributes.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = categoryAttribute.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var categoryAttribute = _categoryAttributeService.GetCategoryAttributeById(id);
            if (categoryAttribute == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            _categoryAttributeService.DeleteCategoryAttribute(categoryAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteCategoryAttribute", _localizationService.GetResource("ActivityLog.DeleteCategoryAttribute"), categoryAttribute.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.CategoryAttributes.Deleted"));
            return RedirectToAction("List");
        }

        #endregion
        
        #endregion
    }
}