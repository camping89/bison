using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Logging;
using Nop.Core.Extensions;
using Nop.Plugin.Integration.KiotViet.Integration.KiotViet;
using Nop.Plugin.Integration.KiotViet.Integration.KiotViet.Entities;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Tasks;
using System;
using System.Linq;
using System.Net;

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
            _kiotVietService = new KiotVietService(categoryService, urlRecordService);
        }

        public void Execute()
        {
            _logger.InsertLog(LogLevel.Information, "Start to sync data KiotViet to Bison nopcommerce.");

            var uncategory = _categoryService.GetAllCategories("KiotViet", showHidden: true).FirstOrDefault();
            if (uncategory != null)
            {
                var lstProducts = _apiConsumer.GetAllProducts();
                foreach (var kvProduct in lstProducts)
                {
                    //rst312-l-bla
                    var procBySku = _productService.GetProductBySku(kvProduct.code);
                    if (procBySku == null)
                    {
                        var nopProduct = ConvertKVProcToNopProduct(kvProduct);

                        if (kvProduct.code.Contains("|"))
                        {
                            var parentSku = kvProduct.code.Split('|')[0];
                            var parentProduct = _productService.GetProductBySku(parentSku);
                            if (parentProduct == null)
                            {
                                parentProduct = _productService.GetProductBySkuSingle(nopProduct.Sku);
                                if (parentProduct == null)
                                {
                                    parentProduct = nopProduct;
                                }
                                parentProduct.Sku = parentSku;
                                parentProduct.KiotVietId = string.Empty;
                                parentProduct.Price = lstProducts.Where(_ => _.code.Contains(parentSku)).Min(p => p.basePrice);
                                //parentProduct.ProductTypeId = ProductType.GroupedProduct.ToInt();
                                _productService.InsertProduct(parentProduct);
                                var parentseName = parentProduct.ValidateSeName(string.Empty, nopProduct.Name, true);
                                _urlRecordService.SaveSlug(parentProduct, parentseName, 0);
                                SaveCategoryMappings(parentProduct.Id, uncategory.Id);

                                InsertAttributeToProduct(kvProduct.code.Split('|')[1], parentProduct, parentProduct);

                                if (kvProduct.images == null) continue;
                                foreach (var img in kvProduct.images)
                                {
                                    InsertProductPictureFromUrl(img, parentseName, nopProduct.Id);
                                }
                                //if (nopProduct.Id != parentProduct.Id)
                                //{
                                //    nopProduct.ParentGroupedProductId = parentProduct.Id;
                                //}

                            }
                            else
                            {
                                //if (nopProduct.Id != parentProduct.Id)
                                //{
                                //    nopProduct.ParentGroupedProductId = parentProduct.Id;
                                //}
                                parentProduct.ProductType = ProductType.SimpleProduct;
                                parentProduct.ManageInventoryMethodId = ManageInventoryMethod.ManageStockByAttributes.ToInt();
                                parentProduct.VisibleIndividually = true;
                                _productService.UpdateProduct(parentProduct);
                                InsertAttributeToProduct(kvProduct.code.Split('|')[1], nopProduct, parentProduct);
                            }

                        }
                        else
                        {
                            _productService.InsertProduct(nopProduct);
                            if (nopProduct.Id <= 0) continue;
                            var seName = nopProduct.ValidateSeName(string.Empty, nopProduct.Name, true);
                            _urlRecordService.SaveSlug(nopProduct, seName, 0);
                            SaveCategoryMappings(nopProduct.Id, uncategory.Id);
                            if (kvProduct.images == null) continue;
                            foreach (var img in kvProduct.images)
                            {
                                InsertProductPictureFromUrl(img, seName, nopProduct.Id);
                            }
                        }

                    }
                    else
                    {
                        //group product
                        if (procBySku.Sku.Contains("|"))
                        {
                            var parentSku = procBySku.Sku.Split('|')[0];
                            var parentProduct = _productService.GetProductBySku(parentSku);
                            if (parentProduct == null)
                            {
                                parentProduct = _productService.GetProductBySkuSingle(procBySku.Sku);
                                parentProduct.Sku = parentSku;
                                parentProduct.KiotVietId = string.Empty;
                                parentProduct.VisibleIndividually = true;
                                parentProduct.ManageInventoryMethodId = ManageInventoryMethod.ManageStockByAttributes.ToInt();
                                parentProduct.Price = lstProducts.Where(_ => _.code.Contains(parentSku)).Min(p => p.basePrice);
                                //parentProduct.ProductTypeId = ProductType.GroupedProduct.ToInt();
                                _productService.InsertProduct(parentProduct);
                                var parentseName = parentProduct.ValidateSeName(string.Empty, procBySku.Name, true);
                                _urlRecordService.SaveSlug(parentProduct, parentseName, 0);
                                SaveCategoryMappings(parentProduct.Id, uncategory.Id);


                                if (procBySku.Id != parentProduct.Id)
                                {
                                    //procBySku.ParentGroupedProductId = parentProduct.Id;
                                    //procBySku.VisibleIndividually = false;
                                    foreach (var productPicture in procBySku.ProductPictures)
                                    {
                                        var picture = productPicture.Picture;
                                        var pictureCopy = _pictureService.InsertPicture(
                                            _pictureService.LoadPictureBinary(picture),
                                            picture.MimeType,
                                            _pictureService.GetPictureSeName(productPicture.Picture.SeoFilename),
                                            picture.AltAttribute,
                                            picture.TitleAttribute);
                                        _productService.InsertProductPicture(new ProductPicture
                                        {
                                            ProductId = parentProduct.Id,
                                            PictureId = pictureCopy.Id,
                                            DisplayOrder = productPicture.DisplayOrder
                                        });
                                    }
                                }
                                InsertAttributeToProduct(kvProduct.code.Split('|')[1], parentProduct, parentProduct);
                            }
                            else
                            {
                                //if (procBySku.Id != parentProduct.Id)
                                //{
                                //    procBySku.ParentGroupedProductId = parentProduct.Id;
                                //    procBySku.VisibleIndividually = false;
                                //}
                                parentProduct.ProductType = ProductType.SimpleProduct;
                                parentProduct.ManageInventoryMethodId = ManageInventoryMethod.ManageStockByAttributes.ToInt();
                                parentProduct.VisibleIndividually = true;
                                _productService.UpdateProduct(parentProduct);
                                InsertAttributeToProduct(kvProduct.code.Split('|')[1], procBySku, parentProduct);
                            }
                            _productService.DeleteProduct(procBySku);
                        }
                        else
                        {
                            var inventory = kvProduct.inventories.FirstOrDefault();
                            var stock = 0;
                            if (inventory != null)
                            {
                                stock = Convert.ToInt32(inventory.onHand);
                            }
                            //procBySku.Name = kvProduct.fullName;
                            //procBySku.ShortDescription = kvProduct.description;
                            //procBySku.FullDescription = kvProduct.description;
                            procBySku.Price = kvProduct.basePrice;
                            procBySku.StockQuantity = stock;
                            procBySku.ManageInventoryMethodId = ManageInventoryMethod.ManageStock.ToInt(); //1 track inventory product
                            procBySku.KiotVietId = kvProduct.id.ToString();
                            _productService.UpdateProduct(procBySku);
                        }
                    }
                }
            }
            _logger.InsertLog(LogLevel.Information, "End sync data KiotViet to Bison nopcommerce.");
        }

        private void InsertProductPictureFromUrl(string urlImage, string seName, int productId)
        {
            var webClient = new WebClient();
            var imageBytes = webClient.DownloadData(urlImage);
            var picture = _pictureService.InsertPicture(imageBytes, "image/jpeg", seName, validateBinary: false);
            _productService.InsertProductPicture(new ProductPicture
            {
                PictureId = picture.Id,
                ProductId = productId,
                DisplayOrder = 0,
            });
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

        private void InsertAttributeToProduct(string attributeValue, Product newProduct, Product existProduct)
        {
            //var kvProductMinPrice = kvProducts.Where(_ => _.code.Contains(srouceProduct.Sku)).Min(p => p.basePrice);
            var attributeSize = _productAttributeService.GetAllProductAttributes().FirstOrDefault(_ => _.Name.Equals("Size", StringComparison.InvariantCultureIgnoreCase));
            if (attributeSize != null)
            {
                var productAttributeMapping = _productAttributeService
                    .GetProductAttributeMappingsByProductId(existProduct.Id).FirstOrDefault(x => x.ProductAttributeId == attributeSize.Id);
                if (productAttributeMapping != null)
                {
                    if (productAttributeMapping.ProductAttributeValues.Count(a => a.Name.Equals(attributeValue, StringComparison.InvariantCultureIgnoreCase)) == 0)
                    {
                        _productAttributeService.InsertProductAttributeValue(new ProductAttributeValue
                        {
                            ProductAttributeMappingId = productAttributeMapping.Id,
                            Name = attributeValue,
                            PriceAdjustment = newProduct.Price - existProduct.Price,
                            Quantity = newProduct.StockQuantity
                        });
                    }
                }
                else
                {
                    //insert mapping
                    productAttributeMapping = new ProductAttributeMapping
                    {
                        ProductId = existProduct.Id,
                        ProductAttributeId = attributeSize.Id,
                        TextPrompt = "Size",
                        IsRequired = true,
                        AttributeControlTypeId = AttributeControlType.DropdownList.ToInt(),
                        DisplayOrder = 0,
                        //ValidationMinLength = model.ValidationMinLength,
                        //ValidationMaxLength = model.ValidationMaxLength,
                        //ValidationFileAllowedExtensions = model.ValidationFileAllowedExtensions,
                        //ValidationFileMaximumSize = model.ValidationFileMaximumSize,
                        //DefaultValue = model.DefaultValue
                    };
                    _productAttributeService.InsertProductAttributeMapping(productAttributeMapping);
                    if (productAttributeMapping.Id > 0)
                    {
                        _productAttributeService.InsertProductAttributeValue(new ProductAttributeValue
                        {
                            ProductAttributeMappingId = productAttributeMapping.Id,
                            Name = attributeValue,
                            PriceAdjustment = newProduct.Price - existProduct.Price,
                            Quantity = newProduct.StockQuantity
                        });
                    }
                }
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
            var inventory = kvProduct.inventories.FirstOrDefault();
            var stock = 0;
            if (inventory != null)
            {
                stock = Convert.ToInt32(inventory.onHand);
            }
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
                Published = true,
                StockQuantity = stock,
                ManageInventoryMethodId = 1, //1 track inventory product
                KiotVietId = kvProduct.id.ToString()
            };
        }
    }
}
