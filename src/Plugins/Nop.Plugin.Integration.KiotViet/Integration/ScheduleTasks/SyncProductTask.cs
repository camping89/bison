using Nop.Core.Domain.Catalog;
using Nop.Core.Extensions;
using Nop.Plugin.Integration.KiotViet.Integration.KiotViet;
using Nop.Plugin.Integration.KiotViet.Integration.KiotViet.Entities;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.Seo;
using Nop.Services.Tasks;
using System;
using System.Linq;

namespace Nop.Plugin.Integration.KiotViet.Integration.ScheduleTasks
{
    public class SyncProductTask : IScheduleTask
    {
        private readonly KiotVietApiConsumer _apiConsumer;
        private KiotVietService _kiotVietService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ILogger _logger;

        public SyncProductTask(ILogger logger, ICategoryService categoryService,
            IProductService productService, IUrlRecordService urlRecordService, IProductAttributeService productAttributeService)
        {
            _logger = logger;
            _categoryService = categoryService;
            _productService = productService;
            _urlRecordService = urlRecordService;
            _productAttributeService = productAttributeService;
            _apiConsumer = new KiotVietApiConsumer();
            _kiotVietService = new KiotVietService(categoryService, urlRecordService);
        }

        public void Execute()
        {
            //_logger.InsertLog(LogLevel.Information, "Start to sync data KiotViet to Bison nopcommerce.");
            var kiotVietCategory = _categoryService.GetAllCategories("KiotViet", showHidden: true).FirstOrDefault();
            if (kiotVietCategory == null) return;

            var sourceProductGroups = _apiConsumer.GetProducts().GroupBy(_ => _.sku);
            foreach (var group in sourceProductGroups)
            {
                // RST429|BLACK-L
                var sku = group.Key;
                var sourceProducts = group.ToList();
                var sourceProduct = sourceProducts.First();
                var basePrice = sourceProducts.Min(_ => _.basePrice);

                var product = _productService.GetProductBySku(sku);
                if (product == null) // create new product
                {
                    product = KiotVietHelper.MapNewProduct(sourceProduct);
                    product.Price = basePrice;
                    product.StockQuantity = (int)sourceProducts.SelectMany(_ => _.inventories).Sum(_ => _.onHand);

                    _productService.InsertProduct(product);
                    if (product.Id <= 0) continue;

                    var parentSeachEngineName = product.ValidateSeName(string.Empty, product.Name, true);
                    _urlRecordService.SaveSlug(product, parentSeachEngineName, 0);
                    SaveCategoryMappings(product.Id, kiotVietCategory.Id);

                    foreach (var variant in sourceProducts)
                    {
                        MapProductAttributes("Size", variant, "Size", product, true, basePrice);
                        MapProductAttributes("Colour", variant, "Color", product);
                    }
                }
                else // update to existing product
                {

                    KiotVietHelper.MergeProduct(sourceProduct, product);
                    product.StockQuantity = (int)sourceProducts.SelectMany(_ => _.inventories).Sum(_ => _.onHand);

                    var attMappings = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
                    var sizeMappings = attMappings.Where(_ => _.ProductAttribute.Name.Equals("Size", StringComparison.InvariantCultureIgnoreCase)).ToList();
                    var colorMappings = attMappings.Where(_ => _.ProductAttribute.Name.Equals("Color", StringComparison.InvariantCultureIgnoreCase)).ToList();
                    var madeInMappings = attMappings.Where(_ => _.ProductAttribute.Name.Equals("Made in", StringComparison.InvariantCultureIgnoreCase)).ToList();
                    foreach (var mapping in sizeMappings.Union(colorMappings).Union(madeInMappings))
                    {
                        _productAttributeService.DeleteProductAttributeMapping(mapping);
                    }

                    foreach (var variant in sourceProducts)
                    {
                        MapProductAttributes("Size", variant, "Size", product, true, basePrice);
                        MapProductAttributes("Colour", variant, "Color", product);
                    }


                    _productService.UpdateProduct(product);
                }

            }
        }

        private void SaveCategoryMappings(int productId, int categoryId)
        {
            if (categoryId > 0)
            {
                _categoryService.InsertProductCategory(new ProductCategory
                {
                    ProductId = productId,
                    CategoryId = categoryId,
                    DisplayOrder = 0
                });
            }
        }

        private void MapProductAttributes(string kvAttributeName, KVProduct sourceVariant, string attributeName, Product product, bool adjustPrice = false, decimal originPrice = 0)
        {
            var kiotVietAttribute = sourceVariant.attributes?.FirstOrDefault(_ => _.attributeName.Equals(kvAttributeName, StringComparison.InvariantCultureIgnoreCase));
            if (kiotVietAttribute != null)
            {
                var attributeSize = _productAttributeService.GetAllProductAttributes().FirstOrDefault(_ => _.Name.Equals(attributeName, StringComparison.InvariantCultureIgnoreCase));
                if (attributeSize != null)
                {
                    var productAttMappings = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);

                    var sizeMapping = productAttMappings.FirstOrDefault(_ => _.ProductAttributeId == attributeSize.Id);

                    if (sizeMapping != null)
                    {
                        if (!sizeMapping.ProductAttributeValues.Any(a => a.Name.Equals(kiotVietAttribute.attributeValue, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            _productAttributeService.InsertProductAttributeValue(new ProductAttributeValue
                            {
                                ProductAttributeMappingId = sizeMapping.Id,
                                Name = kiotVietAttribute.attributeValue,
                                PriceAdjustment = adjustPrice ? product.Price - originPrice : 0,
                                Quantity = product.StockQuantity
                            });
                        }
                    }
                    else
                    {
                        sizeMapping = new ProductAttributeMapping
                        {
                            ProductId = product.Id,
                            ProductAttributeId = attributeSize.Id,
                            TextPrompt = attributeName,
                            IsRequired = true,
                            AttributeControlTypeId = AttributeControlType.DropdownList.ToInt(),
                            DisplayOrder = 0,
                        };
                        _productAttributeService.InsertProductAttributeMapping(sizeMapping);

                        if (sizeMapping.Id > 0)
                        {
                            _productAttributeService.InsertProductAttributeValue(new ProductAttributeValue
                            {
                                ProductAttributeMappingId = sizeMapping.Id,
                                Name = kiotVietAttribute.attributeValue.Trim(),
                                PriceAdjustment = adjustPrice ? product.Price - originPrice : 0,
                                Quantity = product.StockQuantity
                            });
                        }
                    }
                }
            }
        }

        //private void MapProductAttributes(string attributeValue, Product newProduct, Product existProduct)
        //{
        //    //var kvProductMinPrice = kvProducts.Where(_ => _.code.Contains(srouceProduct.Sku)).Min(p => p.basePrice);
        //    var attributeSize = _productAttributeService.GetAllProductAttributes().FirstOrDefault(_ => _.Name.Equals("Size", StringComparison.InvariantCultureIgnoreCase));
        //    if (attributeSize != null)
        //    {
        //        var productAttributeMapping = _productAttributeService
        //            .GetProductAttributeMappingsByProductId(existProduct.Id).FirstOrDefault(x => x.ProductAttributeId == attributeSize.Id);
        //        if (productAttributeMapping != null)
        //        {
        //            if (productAttributeMapping.ProductAttributeValues.Count(a => a.Name.Equals(attributeValue, StringComparison.InvariantCultureIgnoreCase)) == 0)
        //            {
        //                _productAttributeService.InsertProductAttributeValue(new ProductAttributeValue
        //                {
        //                    ProductAttributeMappingId = productAttributeMapping.Id,
        //                    Name = attributeValue,
        //                    PriceAdjustment = newProduct.Price - existProduct.Price,
        //                    Quantity = newProduct.StockQuantity
        //                });
        //            }
        //        }
        //        else
        //        {
        //            //insert mapping
        //            productAttributeMapping = new ProductAttributeMapping
        //            {
        //                ProductId = existProduct.Id,
        //                ProductAttributeId = attributeSize.Id,
        //                TextPrompt = "Size",
        //                IsRequired = true,
        //                AttributeControlTypeId = AttributeControlType.DropdownList.ToInt(),
        //                DisplayOrder = 0,
        //            };
        //            _productAttributeService.InsertProductAttributeMapping(productAttributeMapping);
        //            if (productAttributeMapping.Id > 0)
        //            {
        //                _productAttributeService.InsertProductAttributeValue(new ProductAttributeValue
        //                {
        //                    ProductAttributeMappingId = productAttributeMapping.Id,
        //                    Name = attributeValue,
        //                    PriceAdjustment = newProduct.Price - existProduct.Price,
        //                    Quantity = newProduct.StockQuantity
        //                });
        //            }
        //        }
        //    }
        //}

        //private void InsertSpecificationAttributeProduct(int attributeTypeId, int specificationAttributeOptionId,
        //    string customValue, bool allowFiltering, bool showOnProductPage,
        //    int displayOrder, int productId)
        //{

        //    //we allow filtering only for "Option" attribute type
        //    if (attributeTypeId != (int)SpecificationAttributeType.Option)
        //    {
        //        allowFiltering = false;
        //    }
        //    //we don't allow CustomValue for "Option" attribute type
        //    if (attributeTypeId == (int)SpecificationAttributeType.Option)
        //    {
        //        customValue = null;
        //    }

        //    var psa = new ProductSpecificationAttribute
        //    {
        //        AttributeTypeId = attributeTypeId,
        //        SpecificationAttributeOptionId = specificationAttributeOptionId,
        //        ProductId = productId,
        //        CustomValue = customValue,
        //        AllowFiltering = allowFiltering,
        //        ShowOnProductPage = showOnProductPage,
        //        DisplayOrder = displayOrder,
        //    };
        //    _specificationAttributeService.InsertProductSpecificationAttribute(psa);
        //}

    }
}
