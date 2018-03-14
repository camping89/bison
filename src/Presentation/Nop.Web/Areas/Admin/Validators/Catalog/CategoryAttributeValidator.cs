using FluentValidation;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Catalog
{
    public partial class CategoryAttributeValidator : BaseNopValidator<CategoryAttributeModel>
    {
        public CategoryAttributeValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Attributes.CategoryAttribute.Fields.Name.Required"));
            SetDatabaseValidationRules<CategoryAttribute>(dbContext);
        }
    }
}