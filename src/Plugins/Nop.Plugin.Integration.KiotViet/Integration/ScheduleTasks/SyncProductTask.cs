using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Nop.Core.Domain.Logging;
using Nop.Plugin.Integration.KiotViet.Integration.KiotViet;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using Newtonsoft.Json;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Extensions;
using Nop.Plugin.Integration.KiotViet.Integration.KiotViet.Entities;
using Nop.Services.Catalog;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Plugin.Integration.KiotViet.Integration.ScheduleTasks
{
    public class SyncProductTask : IScheduleTask
    {
        private KiotVietApiConsumer _apiConsumer;
        private KiotVietService _kiotVietService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IProductAttributeService _productAttributeService;
        private ILogger _logger;
        public SyncProductTask(ILogger logger, ICategoryService categoryService, 
            IProductService productService, IUrlRecordService urlRecordService, 
            IPictureService pictureService, 
            ISpecificationAttributeService specificationAttributeService, IProductAttributeService productAttributeService)
        {
            _logger = logger;
            _categoryService = categoryService;
            _productService = productService;
            _urlRecordService = urlRecordService;
            _pictureService = pictureService;
            _specificationAttributeService = specificationAttributeService;
            _productAttributeService = productAttributeService;
            _apiConsumer = new KiotVietApiConsumer();
            _kiotVietService = new KiotVietService(categoryService,urlRecordService);
        }

        public void Execute()
        {
            _logger.InsertLog(LogLevel.Information, "Sync data KiotViet to Bison nopcommerce.");
            //Sync Catalog 
            _kiotVietService.InsertKiotVietCatalog(_apiConsumer.GetKVCategories());
            var categories = _categoryService.GetAllCategories(showHidden: true);
            foreach (var cate in categories)
            {
                var lstProducts = _apiConsumer.GetProductsByCategoryId(cate.KiotVietCateId);
                foreach (var kvProduct in lstProducts)
                {
                    var checkProc = _productService.GetProductBySku(kvProduct.code);
                    if (checkProc == null)
                    {
                        var nopProduct = ConvertKVProcToNopProduct(kvProduct);
                        _productService.InsertProduct(nopProduct);
                        if (nopProduct.Id > 0)
                        {
                            string seName = nopProduct.ValidateSeName(string.Empty, nopProduct.Name, true);
                            _urlRecordService.SaveSlug(nopProduct, seName, 0);
                            SaveCategoryMappings(nopProduct.Id, kvProduct.categoryId);
                            if (kvProduct.images != null)
                            {
                                foreach (var img in kvProduct.images)
                                {
                                    InsertProductPictureFromUrl(img, seName, nopProduct.Id);
                                }
                            }

                            if (kvProduct.attributes != null)
                            {
                               
                                foreach (var attr in kvProduct.attributes)
                                {
                                    var productAttribute = _productAttributeService.GetAllProductAttributes();
                                    var productAttr = productAttribute.FirstOrDefault(t => t.Name.Equals(attr.attributeName, StringComparison.InvariantCultureIgnoreCase));
                                    if (productAttr == null)
                                    {
                                        productAttr = new ProductAttribute
                                        {
                                            Name = attr.attributeName,
                                            Description = attr.attributeName
                                        };
                                        _productAttributeService.InsertProductAttribute(productAttr);
                                    }

                                    if (productAttr.Id > 0)
                                    {
                                        var productAttrMappingInsert = new ProductAttributeMapping
                                        {
                                            AttributeControlTypeId = AttributeControlType.RadioList.ToInt(),
                                            ConditionAttributeXml = string.Empty,
                                            ProductAttributeId = productAttr.Id,
                                            ProductId = nopProduct.Id
                                        };
                                        _productAttributeService.InsertProductAttributeMapping(productAttrMappingInsert);
                                        if (productAttrMappingInsert.Id > 0)
                                        {
                                            var pav = new ProductAttributeValue
                                            {
                                                ProductAttributeMappingId = productAttrMappingInsert.Id,
                                                AttributeValueType = AttributeValueType.Simple,
                                                Name = attr.attributeValue,
                                                //PriceAdjustment = predefinedValue.PriceAdjustment,
                                                //WeightAdjustment = predefinedValue.WeightAdjustment,
                                                //Cost = predefinedValue.Cost,
                                                //IsPreSelected = predefinedValue.IsPreSelected,
                                                //DisplayOrder = predefinedValue.DisplayOrder
                                            };
                                            _productAttributeService.InsertProductAttributeValue(pav);
                                        }
                                    }
                                }
                            }
                        }
                        
                    }
                }
            }
        }

        private void InsertProductPictureFromUrl(string urlImage,string seName,int productId)
        {
            var webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData(urlImage);
            var picture = _pictureService.InsertPicture(imageBytes, "image/jpeg", seName, validateBinary: false);
            _productService.InsertProductPicture(new ProductPicture
            {
                PictureId = picture.Id,
                ProductId = productId,
                DisplayOrder = 0,
            });
        }
        private void SaveCategoryMappings(int productId,int kvCategoryId)
        {
            var cate = _categoryService.GetCategoryByKVCateId(kvCategoryId);
            if (cate != null)
            {
                var displayOrder = 1;
                _categoryService.InsertProductCategory(new ProductCategory
                {
                    ProductId = productId,
                    CategoryId = cate.Id,
                    DisplayOrder = displayOrder
                });
            }
        }
        private void InsertSpecificationAttributeProduct(int attributeTypeId, int specificationAttributeOptionId,
            string customValue, bool allowFiltering, bool showOnProductPage,
            int displayOrder, int productId)
        {

            //we allow filtering only for "Option" attribute type
            if (attributeTypeId != (int)SpecificationAttributeType.Option)
            {
                allowFiltering = false;
            }
            //we don't allow CustomValue for "Option" attribute type
            if (attributeTypeId == (int)SpecificationAttributeType.Option)
            {
                customValue = null;
            }

            var psa = new ProductSpecificationAttribute
            {
                AttributeTypeId = attributeTypeId,
                SpecificationAttributeOptionId = specificationAttributeOptionId,
                ProductId = productId,
                CustomValue = customValue,
                AllowFiltering = allowFiltering,
                ShowOnProductPage = showOnProductPage,
                DisplayOrder = displayOrder,
            };
            _specificationAttributeService.InsertProductSpecificationAttribute(psa);
        }
        private Product ConvertKVProcToNopProduct(KVProduct kvProduct)
        {
            return new Product
            {
                ProductTypeId = ProductType.SimpleProduct.ToInt(),
                VisibleIndividually = true,
                Name = kvProduct.fullName,
                ShortDescription = kvProduct.description,
                FullDescription = kvProduct.description,
                ProductTemplateId = 1, //1: Simple product  | 2: Grouped product(with variants)
                AllowCustomerReviews = true,
                Sku = kvProduct.code,
                DisplayStockAvailability = true,
                DisplayStockQuantity = false,
                LowStockActivityId = 1,
                NotifyAdminForQuantityBelow = 1,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Price = kvProduct.basePrice,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                Published = true
            };
        }
    }
}
