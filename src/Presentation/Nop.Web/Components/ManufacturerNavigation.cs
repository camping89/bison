using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Web.Components
{
    public class ManufacturerNavigationViewComponent : NopViewComponent
    {
        private readonly ICatalogModelFactory _catalogModelFactory;
        private readonly CatalogSettings _catalogSettings;
        private readonly IManufacturerService _manufacturerService;

        public ManufacturerNavigationViewComponent(ICatalogModelFactory catalogModelFactory,
            CatalogSettings catalogSettings, IManufacturerService manufacturerService)
        {
            this._catalogModelFactory = catalogModelFactory;
            this._catalogSettings = catalogSettings;
            _manufacturerService = manufacturerService;
        }

        public IViewComponentResult Invoke(int currentManufacturerId, List<Manufacturer> manufacturers)
        {
            if (_catalogSettings.ManufacturersBlockItemsToDisplay == 0)
                return Content("");
            if (manufacturers != null)
            {
                var dataResult = new ManufacturerNavigationModel();
                var currentManufacturer = _manufacturerService.GetManufacturerById(currentManufacturerId);
                foreach (var manufacturer in manufacturers)
                {
                    var modelMan = new ManufacturerBriefInfoModel
                    {
                        Id = manufacturer.Id,
                        Name = manufacturer.GetLocalized(x => x.Name),
                        SeName = manufacturer.GetSeName(),
                        IsActive = currentManufacturer != null && currentManufacturer.Id == manufacturer.Id,
                    };
                    dataResult.Manufacturers.Add(modelMan);
                }
                if (!dataResult.Manufacturers.Any())
                    return Content("");
                return View(dataResult);
            }
            var model = _catalogModelFactory.PrepareManufacturerNavigationModel(currentManufacturerId);
            if (!model.Manufacturers.Any())
                return Content("");

            return View(model);
        }
    }
}
