using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class HomepageNewProductsViewComponent : NopViewComponent
    {
        private readonly IAclService _aclService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly CatalogSettings _catalogSettings; 
        public HomepageNewProductsViewComponent(IAclService aclService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IStoreMappingService storeMappingService, CatalogSettings catalogSettings)
        {
            this._aclService = aclService;
            this._productModelFactory = productModelFactory;
            this._productService = productService;
            this._storeMappingService = storeMappingService;
            _catalogSettings = catalogSettings;
        }

        public IViewComponentResult Invoke(int? productThumbPictureSize)
        {
            var products = _productService.GetAllNewProductsDisplayedOnHomePage(_catalogSettings.NewProductsNumber);
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            if (!products.Any())
                return Content("");

            var model = _productModelFactory.PrepareProductOverviewModels(products, true, true, productThumbPictureSize).ToList();

            return View(model);
        }
    }
}
