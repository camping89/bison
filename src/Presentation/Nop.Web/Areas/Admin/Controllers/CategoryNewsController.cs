using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.News;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.News;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Linq;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class CategoryNewsController : BaseAdminController
    {
        #region Fields

        private readonly ICategoryNewsService _cateNewsService;
        private readonly ILanguageService _languageService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor

        public CategoryNewsController(ICategoryNewsService cateNewsService,
            ILanguageService languageService,
            IDateTimeHelper dateTimeHelper,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IUrlRecordService urlRecordService,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            ICustomerActivityService customerActivityService, IWorkContext workContext)
        {
            this._cateNewsService = cateNewsService;
            this._languageService = languageService;
            this._dateTimeHelper = dateTimeHelper;
            this._eventPublisher = eventPublisher;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._urlRecordService = urlRecordService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._customerActivityService = customerActivityService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        protected virtual void PrepareLanguagesModel(CategoryNewsModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var languages = _languageService.GetAllLanguages(true);
            foreach (var language in languages)
            {
                model.AvailableLanguages.Add(new SelectListItem
                {
                    Text = language.Name,
                    Value = language.Id.ToString()
                });
            }
        }

        #endregion 
        #region Category News items

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command, NewsItemListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedKendoGridJson();

            var news = _cateNewsService.GetAllCategoryNews(_workContext.WorkingLanguage.Id, true);
            var gridModel = new DataSourceResult
            {
                Data = news.Select(x =>
                {
                    var m = x.ToModel();

                    return m;
                }),
                Total = news.Count
            };

            return Json(gridModel);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var model = new CategoryNewsModel();
            //languages
            PrepareLanguagesModel(model);

            //default values
            model.Published = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(CategoryNewsModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var categoryNews = model.ToEntity();
                categoryNews.UpdatedOnUtc = DateTime.UtcNow;
                categoryNews.CreatedOnUtc = DateTime.UtcNow;
                _cateNewsService.InsertCategoryNews(categoryNews);

                //activity log
                _customerActivityService.InsertActivity("AddNewCategoryNews", _localizationService.GetResource("ActivityLog.AddNewCategoryNews"), categoryNews.Id);

                //search engine name
                var seName = categoryNews.ValidateSeName(model.SeName, model.Name, true);
                _urlRecordService.SaveSlug(categoryNews, seName, categoryNews.LanguageId);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.CategoryNews.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = categoryNews.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareLanguagesModel(model);
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var categoryNews = _cateNewsService.GetCategoryNewsById(id);
            if (categoryNews == null)
                //No news item found with the specified id
                return RedirectToAction("List");

            var model = categoryNews.ToModel();
            model.SeName = categoryNews.GetSeName(model.LanguageId);
            //languages
            PrepareLanguagesModel(model);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(CategoryNewsModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var categoryNews = _cateNewsService.GetCategoryNewsById(model.Id);
            if (categoryNews == null)
                //No news item found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                categoryNews = model.ToEntity(categoryNews);
                categoryNews.UpdatedOnUtc = DateTime.UtcNow;
                _cateNewsService.UpdateCategoryNews(categoryNews);

                //activity log
                _customerActivityService.InsertActivity("EditCategoryNewsNews", _localizationService.GetResource("ActivityLog.EditCategoryNews"), categoryNews.Id);

                //search engine name
                var seName = categoryNews.ValidateSeName(model.SeName, model.Name, true);
                _urlRecordService.SaveSlug(categoryNews, seName, categoryNews.LanguageId);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.CategoryNews.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = categoryNews.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareLanguagesModel(model);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var newsItem = _cateNewsService.GetCategoryNewsById(id);
            if (newsItem == null)
                //No news item found with the specified id
                return RedirectToAction("List");

            _cateNewsService.DeleteCategoryNews(newsItem);

            //activity log
            _customerActivityService.InsertActivity("DeleteCategoryNews", _localizationService.GetResource("ActivityLog.DeleteCategoryNews"), newsItem.Id);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.CategoryNews.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

    }
}