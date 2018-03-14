using System.Collections.Generic;
using FluentValidation.Attributes;
using Nop.Web.Areas.Admin.Validators.Catalog;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    [Validator(typeof(CategoryAttributeValidator))]
    public partial class CategoryAttributeModel : BaseNopEntityModel, ILocalizedModel<CategoryAttributeLocalizedModel>
    {
        public CategoryAttributeModel()
        {
            Locales = new List<CategoryAttributeLocalizedModel>();
        }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CategoryAttributes.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CategoryAttributes.Fields.Description")]
        public string Description {get;set;}

        public string ValueForProduct { get; set; }
        public IList<CategoryAttributeLocalizedModel> Locales { get; set; }

        //#region Nested classes

        //public partial class UsedByProductModel : BaseNopEntityModel
        //{
        //    [NopResourceDisplayName("Admin.Catalog.Attributes.CategoryAttributes.UsedByProducts.Product")]
        //    public string ProductName { get; set; }
        //    [NopResourceDisplayName("Admin.Catalog.Attributes.CategoryAttributes.UsedByProducts.Published")]
        //    public bool Published { get; set; }
        //}

        //#endregion
    }

    public partial class CategoryAttributeLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CategoryAttributes.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.CategoryAttributes.Fields.Description")]
        public string Description {get;set;}
    }

    //[Validator(typeof(PredefinedCategoryAttributeValueModelValidator))]
    //public partial class PredefinedCategoryAttributeValueModel : BaseNopEntityModel, ILocalizedModel<PredefinedCategoryAttributeValueLocalizedModel>
    //{
    //    public PredefinedCategoryAttributeValueModel()
    //    {
    //        Locales = new List<PredefinedCategoryAttributeValueLocalizedModel>();
    //    }

    //    public int CategoryAttributeId { get; set; }

    //    [NopResourceDisplayName("Admin.Catalog.Attributes.CategoryAttributes.PredefinedValues.Fields.Name")]
    //    public string Name { get; set; }

    //    [NopResourceDisplayName("Admin.Catalog.Attributes.CategoryAttributes.PredefinedValues.Fields.PriceAdjustment")]
    //    public decimal PriceAdjustment { get; set; }
    //    [NopResourceDisplayName("Admin.Catalog.Attributes.CategoryAttributes.PredefinedValues.Fields.PriceAdjustment")]
    //    //used only on the values list page
    //    public string PriceAdjustmentStr { get; set; }

    //    [NopResourceDisplayName("Admin.Catalog.Attributes.CategoryAttributes.PredefinedValues.Fields.WeightAdjustment")]
    //    public decimal WeightAdjustment { get; set; }
    //    [NopResourceDisplayName("Admin.Catalog.Attributes.CategoryAttributes.PredefinedValues.Fields.WeightAdjustment")]
    //    //used only on the values list page
    //    public string WeightAdjustmentStr { get; set; }

    //    [NopResourceDisplayName("Admin.Catalog.Attributes.CategoryAttributes.PredefinedValues.Fields.Cost")]
    //    public decimal Cost { get; set; }

    //    [NopResourceDisplayName("Admin.Catalog.Attributes.CategoryAttributes.PredefinedValues.Fields.IsPreSelected")]
    //    public bool IsPreSelected { get; set; }

    //    [NopResourceDisplayName("Admin.Catalog.Attributes.CategoryAttributes.PredefinedValues.Fields.DisplayOrder")]
    //    public int DisplayOrder { get; set; }

    //    public IList<PredefinedCategoryAttributeValueLocalizedModel> Locales { get; set; }
    //}

    //public partial class PredefinedCategoryAttributeValueLocalizedModel : ILocalizedModelLocal
    //{
    //    public int LanguageId { get; set; }

    //    [NopResourceDisplayName("Admin.Catalog.Attributes.CategoryAttributes.PredefinedValues.Fields.Name")]
    //    public string Name { get; set; }
    //}
}