using Nop.Web.Framework.Mvc.Models;
using System.Collections.Generic;

namespace Nop.Web.Models.Catalog
{
    public class CateSpecificationAttributeSimpleModel : BaseNopEntityModel 
    {
        public CateSpecificationAttributeSimpleModel()
        {
            CateSaoSimpleModels = new List<CateSaoSimpleModel>();
        }
        public string SpecificationAttrName { get; set; }
        public bool IsShowTopMenu { get; set; }
        public List<CateSaoSimpleModel> CateSaoSimpleModels {get; set; }
    }

    public class CateSaoSimpleModel : BaseNopEntityModel 
    {
        public CateSaoSimpleModel()
        {
            SubCateSaoSimpleModels = new List<CateSaoSimpleModel>();
        }
        public string SpecificationAttrName { get; set; }
        public bool IsShowTopMenu { get; set; }
        public List<CateSaoSimpleModel> SubCateSaoSimpleModels { get; set; }
    }
}
