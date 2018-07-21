using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class RecentNewsViewComponent : NopViewComponent
    {
        private readonly INewsModelFactory _newsModelFactory;

        public RecentNewsViewComponent(INewsModelFactory newsModelFactory)
        {
            this._newsModelFactory = newsModelFactory;
        }

        public IViewComponentResult Invoke()
        {
            var model = _newsModelFactory.PrepareRecentNewsItemsModel();
            return View(model);
        }
    }
}
