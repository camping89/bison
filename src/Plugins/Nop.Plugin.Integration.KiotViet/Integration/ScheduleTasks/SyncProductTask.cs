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
                    else
                    {
                        var inventory = kvProduct.inventories.FirstOrDefault();
                        var stock = 0;
                        if (inventory != null)
                        {
                            stock = Convert.ToInt32(inventory.onHand);
                        }
                        procBySku.Name = kvProduct.fullName;
                        procBySku.ShortDescription = kvProduct.description;
                        procBySku.FullDescription = kvProduct.description;
                        procBySku.Price = kvProduct.basePrice;
                        procBySku.StockQuantity = stock;
                        procBySku.ManageInventoryMethodId = 1; //1 track inventory product
                        _productService.UpdateProduct(procBySku);
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
                ManageInventoryMethodId = 1 //1 track inventory product
            };
        }
    }
}
