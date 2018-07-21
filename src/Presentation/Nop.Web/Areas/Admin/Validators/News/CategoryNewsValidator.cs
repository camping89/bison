using FluentValidation;
using Nop.Core.Domain.News;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.News;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.News
{
    public partial class CategoryNewsValidator : BaseNopValidator<CategoryNewsModel>
    {
        public CategoryNewsValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Categories.Fields.Name.Required"));
            SetDatabaseValidationRules<CategoryNews>(dbContext);
        }
    }
}