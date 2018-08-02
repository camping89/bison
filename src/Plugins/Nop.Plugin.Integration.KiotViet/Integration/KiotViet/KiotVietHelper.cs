using Nop.Core.Domain.Catalog;
using Nop.Core.Extensions;
using Nop.Plugin.Integration.KiotViet.Integration.KiotViet.Entities;
using System;
using System.Linq;

namespace Nop.Plugin.Integration.KiotViet.Integration.KiotViet
{
    public static class KiotVietHelper
    {
        public static string GetSku(string value)
        {
            return value.Contains("|") ? value.Split('|').First() : value;
        }

        public static string GetName(string value)
        {
            return value.Contains("|") ? value.Split('|').First() : value;
        }

        public static void MergeProduct(KVProduct kvProduct, Product product)
        {
            product.Name = kvProduct.fullName.Contains("|") ? kvProduct.fullName.Split('|').FirstOrDefault() : kvProduct.fullName;
            product.Sku = kvProduct.code.Contains("|") ? kvProduct.code.Split('|').First() : kvProduct.code;
            product.Price = kvProduct.basePrice;
            product.UpdatedOnUtc = DateTime.UtcNow;
            product.KiotVietId = kvProduct.id.ToString();
        }

        public static Product MapNewProduct(KVProduct kvProduct)
        {
            var inventory = kvProduct.inventories.FirstOrDefault();
            var stock = inventory != null ? Convert.ToInt32(inventory.onHand) : 0;

            return new Product
            {
                Name = GetName(kvProduct.fullName),
                Sku = GetSku(kvProduct.code),
                Price = kvProduct.basePrice,
                StockQuantity = stock,
                KiotVietId = kvProduct.id.ToString(),

                ProductTypeId = ProductType.SimpleProduct.ToInt(),
                ProductTemplateId = 1, //1: Simple product  | 2: Grouped product(with variants)
                ManageInventoryMethodId = 1, //1 track inventory product
                LowStockActivityId = 1,
                NotifyAdminForQuantityBelow = 1,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,

                Published = true,
                VisibleIndividually = true,
                AllowCustomerReviews = true,
                DisplayStockAvailability = true,
                DisplayStockQuantity = false,

                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };
        }
    }
}
