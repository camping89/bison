using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product attribute service interface
    /// </summary>
    public partial interface ICategoryAttributeService
    {
        #region Product attributes

        /// <summary>
        /// Deletes a product attribute
        /// </summary>
        /// <param name="categoryAttribute">Category attribute</param>
        void DeleteCategoryAttribute(CategoryAttribute categoryAttribute);

        /// <summary>
        /// Gets all Category attributes
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Category attributes</returns>
        IPagedList<CategoryAttribute> GetAllCategoryAttributes(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets a Category attribute 
        /// </summary>
        /// <param name="categoryAttributeId">Product attribute identifier</param>
        /// <returns>Category attribute </returns>
        CategoryAttribute GetCategoryAttributeById(int categoryAttributeId);

        /// <summary>
        /// Inserts a Category attribute
        /// </summary>
        /// <param name="categoryAttribute">Category attribute</param>
        void InsertCategoryAttribute(CategoryAttribute categoryAttribute);

        /// <summary>
        /// Updates the Category attribute
        /// </summary>
        /// <param name="categoryAttribute">Category attribute</param>
        void UpdateCategoryAttribute(CategoryAttribute categoryAttribute);

        /// <summary>
        /// Returns a list of IDs of not existing attributes
        /// </summary>
        /// <param name="attributeId">The IDs of the attributes to check</param>
        /// <returns>List of IDs not existing attributes</returns>
        int[] GetNotExistingAttributes(int[] attributeId);

        #endregion

        #region Product attributes mappings

        /// <summary>
        /// Deletes a Category attribute mapping
        /// </summary>
        /// <param name="categoryAttributeMapping">Category attribute mapping</param>
        void DeleteCategoryAttributeMapping(CategoryAttributeMapping categoryAttributeMapping);

        /// <summary>
        /// Gets Category attribute mappings by Category identifier
        /// </summary>
        /// <param name="categoryId">The Category identifier</param>
        /// <returns>Category attribute mapping collection</returns>
        IList<CategoryAttributeMapping> GetCategoryAttributeMappingsByCategoryId(int categoryId);

        /// <summary>
        /// Gets a Category attribute mapping
        /// </summary>
        /// <param name="categoryAttributeMappingId">Category attribute mapping identifier</param>
        /// <returns>Category attribute mapping</returns>
        CategoryAttributeMapping GetCategoryAttributeMappingById(int categoryAttributeMappingId);

        /// <summary>
        /// Inserts a Category attribute mapping
        /// </summary>
        /// <param name="categoryAttributeMapping">The product attribute mapping</param>
        void InsertCategoryAttributeMapping(CategoryAttributeMapping categoryAttributeMapping);

        /// <summary>
        /// Updates the Category attribute mapping
        /// </summary>
        /// <param name="categoryAttributeMapping">The Category attribute mapping</param>
        void UpdateCategoryAttributeMapping(CategoryAttributeMapping categoryAttributeMapping);

        #endregion

        #region Product attribute values

        /// <summary>
        /// Deletes a product attribute value
        /// </summary>
        /// <param name="productAttributeValue">Product attribute value</param>
        void DeleteProductAttributeValue(ProductAttributeValue productAttributeValue);

        /// <summary>
        /// Gets Category attribute values by Category attribute mapping identifier
        /// </summary>
        /// <param name="categoryAttributeMappingId">The product attribute mapping identifier</param>
        /// <returns>Product attribute values</returns>
        IList<ProductAttributeValue> GetProductAttributeValues(int categoryAttributeMappingId);

        /// <summary>
        /// Gets a product attribute value
        /// </summary>
        /// <param name="productAttributeValueId">Product attribute value identifier</param>
        /// <returns>Product attribute value</returns>
        ProductAttributeValue GetProductAttributeValueById(int productAttributeValueId);

        /// <summary>
        /// Inserts a product attribute value
        /// </summary>
        /// <param name="productAttributeValue">The product attribute value</param>
        void InsertProductAttributeValue(ProductAttributeValue productAttributeValue);

        /// <summary>
        /// Updates the product attribute value
        /// </summary>
        /// <param name="productAttributeValue">The product attribute value</param>
        void UpdateProductAttributeValue(ProductAttributeValue productAttributeValue);

        #endregion

    }
}
