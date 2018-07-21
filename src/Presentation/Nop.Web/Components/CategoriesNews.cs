using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class CategoriesNewsViewComponent : NopViewComponent
    {
        private readonly INewsModelFactory _newsModelFactory;

        public CategoriesNewsViewComponent(INewsModelFactory newsModelFactory)
        {
            this._newsModelFactory = newsModelFactory;
        }

        public IViewComponentResult Invoke()
        {
            var model = _newsModelFactory.PrepareCategoriesNewsModel();
            return View(model);
        }
    }
}
