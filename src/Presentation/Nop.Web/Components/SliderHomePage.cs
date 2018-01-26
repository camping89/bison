using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Slider;

namespace Nop.Web.Components
{
    public class SliderHomePageViewComponent : NopViewComponent
    {
       
        public SliderHomePageViewComponent()
        {
        }

        public IViewComponentResult Invoke()
        {
            List<SliderModel> sliderModels = new List<SliderModel>();
            return View(sliderModels);
        }
    }
}
