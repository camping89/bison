using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Catalog
{
    public static class SpecificationAttributeOptionExtensions
    {
        public static IList<SpecificationAttributeOption> SortSaoForTree(this IList<SpecificationAttributeOption> source, int parentId = 0, bool ignoreCategoriesWithoutExistingParent = false)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var result = new List<SpecificationAttributeOption>();

            foreach (var sao in source.Where(c => c.ParentSpecificationAttributeId == parentId).ToList())
            {
                result.Add(sao);
                result.AddRange(SortSaoForTree(source, sao.Id, true));
            }
            if (!ignoreCategoriesWithoutExistingParent && result.Count != source.Count)
            {
                foreach (var sao in source)
                    if (result.FirstOrDefault(x => x.Id == sao.Id) == null)
                        result.Add(sao);
            }
            return result;
        }

        public static IList<SpecificationAttributeOption> GetSpecificationAttributeBreadCrumb(this SpecificationAttributeOption specificationAttributeOption,
            ISpecificationAttributeService service,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            bool showHidden = false)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException(nameof(specificationAttributeOption));

            var result = new List<SpecificationAttributeOption>();

            //used to prevent circular references
            var alreadyProcessedSpecOptionIds = new List<int>();

            while (specificationAttributeOption != null && !alreadyProcessedSpecOptionIds.Contains(specificationAttributeOption.Id)) //prevent circular references
            {
                result.Add(specificationAttributeOption);

                alreadyProcessedSpecOptionIds.Add(specificationAttributeOption.Id);

                specificationAttributeOption = service.GetSpecificationAttributeOptionById(specificationAttributeOption.ParentSpecificationAttributeId);
            }
            result.Reverse();
            return result;
        }

        public static string GetFormattedSpecBreadCrumb(this SpecificationAttributeOption specificationAttributeOption,
            ISpecificationAttributeService service,
            string separator = ">>", int languageId = 0)
        {
            var result = string.Empty;

            var breadcrumb = GetSpecificationAttributeBreadCrumb(specificationAttributeOption, service, null, null, true);
            for (var i = 0; i <= breadcrumb.Count - 1; i++)
            {
                var categoryName = breadcrumb[i].GetLocalized(x => x.Name, languageId);
                result = string.IsNullOrEmpty(result)
                    ? categoryName
                    : $"{result} {separator} {categoryName}";
            }

            return result;
        }

    }
}
