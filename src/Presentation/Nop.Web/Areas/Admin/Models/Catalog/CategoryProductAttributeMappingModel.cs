using System.Collections.Generic;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    //[Validator(typeof(CategoryAttributeValidator))]
    public partial class CategoryProductAttributeMappingModel : ProductModel.ProductAttributeMappingModel
    {
        public int CategoryId { get; set; }
        public CategoryModel CategoryModel { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Categories.CategoryProductAttributes.Attributes.Fields.IsUpdateProduct")]
        public bool IsUpdateProduct { get; set; }
    }
   
}