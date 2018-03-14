using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product attribute service
    /// </summary>
    public partial class CategoryAttributeService : ICategoryAttributeService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : page index
        /// {1} : page size
        /// </remarks>
        private const string CATEGORYATTRIBUTES_ALL_KEY = "Nop.categoryattribute.all-{0}-{1}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : category attribute ID
        /// </remarks>
        private const string CATEGORYATTRIBUTES_BY_ID_KEY = "Nop.categoryattribute.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        private const string CATEGORYATTRIBUTEMAPPINGS_ALL_KEY = "Nop.categoryattributemapping.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : category attribute mapping ID
        /// </remarks>
        private const string CATEGORYATTRIBUTEMAPPINGS_BY_ID_KEY = "Nop.categoryattributemapping.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : category attribute mapping ID
        /// </remarks>
        private const string CATEGORYATTRIBUTEVALUES_ALL_KEY = "Nop.categoryattributevalue.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : category attribute value ID
        /// </remarks>
        private const string CATEGORYATTRIBUTEVALUES_BY_ID_KEY = "Nop.categoryattributevalue.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        private const string CATEGORYATTRIBUTECOMBINATIONS_ALL_KEY = "Nop.categoryattributecombination.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CATEGORYATTRIBUTES_PATTERN_KEY = "Nop.categoryattribute.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CATEGORYATTRIBUTEMAPPINGS_PATTERN_KEY = "Nop.categoryattributemapping.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CATEGORYATTRIBUTEVALUES_PATTERN_KEY = "Nop.categoryattributevalue.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CATEGORYATTRIBUTECOMBINATIONS_PATTERN_KEY = "Nop.categoryattributecombination.";

        #endregion

        #region Fields

        private readonly IRepository<CategoryAttribute> _categoryAttributeRepository;
        private readonly IRepository<CategoryAttributeMapping> _categoryAttributeMappingRepository;
        private readonly IRepository<ProductAttributeValue> _productAttributeValueRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        public CategoryAttributeService(IRepository<CategoryAttribute> categoryAttributeRepository, IRepository<CategoryAttributeMapping> categoryAttributeMappingRepository, IRepository<ProductAttributeValue> productAttributeValueRepository, IEventPublisher eventPublisher, ICacheManager cacheManager)
        {
            _categoryAttributeRepository = categoryAttributeRepository;
            _categoryAttributeMappingRepository = categoryAttributeMappingRepository;
            _productAttributeValueRepository = productAttributeValueRepository;
            _eventPublisher = eventPublisher;
            _cacheManager = cacheManager;
        }

        #endregion


        #region Methods

        #region Category attributes

        /// <summary>
        /// Deletes a category attribute
        /// </summary>
        /// <param name="categoryAttribute">Category attribute</param>
        public virtual void DeleteCategoryAttribute(CategoryAttribute categoryAttribute)
        {
            if (categoryAttribute == null)
                throw new ArgumentNullException(nameof(categoryAttribute));

            _categoryAttributeRepository.Delete(categoryAttribute);

            //cache
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(categoryAttribute);
        }

        /// <summary>
        /// Gets all Category attributes
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Category attributes</returns>
        public virtual IPagedList<CategoryAttribute> GetAllCategoryAttributes(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(CATEGORYATTRIBUTES_ALL_KEY, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = from pa in _categoryAttributeRepository.Table
                            orderby pa.Name
                            select pa;
                var categoryAttributes = new PagedList<CategoryAttribute>(query, pageIndex, pageSize);
                return categoryAttributes;
            });
        }

        /// <summary>
        /// Gets a Category attribute 
        /// </summary>
        /// <param name="categoryAttributeId">Category attribute identifier</param>
        /// <returns>Category attribute </returns>
        public virtual CategoryAttribute GetCategoryAttributeById(int categoryAttributeId)
        {
            if (categoryAttributeId == 0)
                return null;

            var key = string.Format(CATEGORYATTRIBUTES_BY_ID_KEY, categoryAttributeId);
            return _cacheManager.Get(key, () => _categoryAttributeRepository.GetById(categoryAttributeId));
        }

        /// <summary>
        /// Inserts a Category attribute
        /// </summary>
        /// <param name="categoryAttribute">Category attribute</param>
        public virtual void InsertCategoryAttribute(CategoryAttribute categoryAttribute)
        {
            if (categoryAttribute == null)
                throw new ArgumentNullException(nameof(categoryAttribute));

            _categoryAttributeRepository.Insert(categoryAttribute);

            //cache
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(categoryAttribute);
        }

        /// <summary>
        /// Updates the Category attribute
        /// </summary>
        /// <param name="categoryAttribute">Category attribute</param>
        public virtual void UpdateCategoryAttribute(CategoryAttribute categoryAttribute)
        {
            if (categoryAttribute == null)
                throw new ArgumentNullException(nameof(categoryAttribute));

            _categoryAttributeRepository.Update(categoryAttribute);

            //cache
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(categoryAttribute);
        }

        /// <summary>
        /// Returns a list of IDs of not existing attributes
        /// </summary>
        /// <param name="attributeId">The IDs of the attributes to check</param>
        /// <returns>List of IDs not existing attributes</returns>
        public virtual int[] GetNotExistingAttributes(int[] attributeId)
        {
            if (attributeId == null)
                throw new ArgumentNullException(nameof(attributeId));

            var query = _categoryAttributeRepository.Table;
            var queryFilter = attributeId.Distinct().ToArray();
            var filter = query.Select(a => a.Id).Where(m => queryFilter.Contains(m)).ToList();
            return queryFilter.Except(filter).ToArray();
        }

        #endregion

        #region Product attributes mappings

        /// <summary>
        /// Deletes a product attribute mapping
        /// </summary>
        /// <param name="categoryAttributeMapping">Product attribute mapping</param>
        public virtual void DeleteCategoryAttributeMapping(CategoryAttributeMapping categoryAttributeMapping)
        {
            if (categoryAttributeMapping == null)
                throw new ArgumentNullException(nameof(categoryAttributeMapping));

            _categoryAttributeMappingRepository.Delete(categoryAttributeMapping);

            //cache
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(categoryAttributeMapping);
        }

        /// <summary>
        /// Gets Category attribute mappings by Category identifier
        /// </summary>
        /// <param name="categoryId">The Category identifier</param>
        /// <returns>Category attribute mapping collection</returns>
        public virtual IList<CategoryAttributeMapping> GetCategoryAttributeMappingsByCategoryId(int categoryId)
        {
            var key = string.Format(CATEGORYATTRIBUTEMAPPINGS_ALL_KEY, categoryId);

            return _cacheManager.Get(key, () =>
            {
                var query = from pam in _categoryAttributeMappingRepository.Table
                            orderby pam.DisplayOrder, pam.Id
                            where pam.CategoryId == categoryId
                            select pam;
                var productAttributeMappings = query.ToList();
                return productAttributeMappings;
            });
        }

        /// <summary>
        /// Gets a Category attribute mapping
        /// </summary>
        /// <param name="categoryAttributeMappingId">Category attribute mapping identifier</param>
        /// <returns>Category attribute mapping</returns>
        public virtual CategoryAttributeMapping GetCategoryAttributeMappingById(int categoryAttributeMappingId)
        {
            if (categoryAttributeMappingId == 0)
                return null;

            var key = string.Format(CATEGORYATTRIBUTEMAPPINGS_BY_ID_KEY, categoryAttributeMappingId);
            return _cacheManager.Get(key, () => _categoryAttributeMappingRepository.GetById(categoryAttributeMappingId));
        }

        /// <summary>
        /// Inserts a Category attribute mapping
        /// </summary>
        /// <param name="categoryAttributeMapping">The Category attribute mapping</param>
        public virtual void InsertCategoryAttributeMapping(CategoryAttributeMapping categoryAttributeMapping)
        {
            if (categoryAttributeMapping == null)
                throw new ArgumentNullException(nameof(categoryAttributeMapping));

            _categoryAttributeMappingRepository.Insert(categoryAttributeMapping);

            //cache
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(categoryAttributeMapping);
        }

        /// <summary>
        /// Updates the Category attribute mapping
        /// </summary>
        /// <param name="categoryAttributeMapping">The Category attribute mapping</param>
        public virtual void UpdateCategoryAttributeMapping(CategoryAttributeMapping categoryAttributeMapping)
        {
            if (categoryAttributeMapping == null)
                throw new ArgumentNullException(nameof(categoryAttributeMapping));

            _categoryAttributeMappingRepository.Update(categoryAttributeMapping);

            //cache
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(categoryAttributeMapping);
        }

        #endregion

        #region Product attribute values

        /// <summary>
        /// Deletes a product attribute value
        /// </summary>
        /// <param name="productAttributeValue">Product attribute value</param>
        public virtual void DeleteProductAttributeValue(ProductAttributeValue productAttributeValue)
        {
            if (productAttributeValue == null)
                throw new ArgumentNullException(nameof(productAttributeValue));

            _productAttributeValueRepository.Delete(productAttributeValue);

            //cache
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productAttributeValue);
        }

        /// <summary>
        /// Gets product attribute values by product attribute mapping identifier
        /// </summary>
        /// <param name="productAttributeMappingId">The product attribute mapping identifier</param>
        /// <returns>Product attribute mapping collection</returns>
        public virtual IList<ProductAttributeValue> GetProductAttributeValues(int productAttributeMappingId)
        {
            var key = string.Format(CATEGORYATTRIBUTEVALUES_ALL_KEY, productAttributeMappingId);
            return _cacheManager.Get(key, () =>
            {
                var query = from pav in _productAttributeValueRepository.Table
                            orderby pav.DisplayOrder, pav.Id
                            where pav.ProductAttributeMappingId == productAttributeMappingId
                            select pav;
                var productAttributeValues = query.ToList();
                return productAttributeValues;
            });
        }

        /// <summary>
        /// Gets a product attribute value
        /// </summary>
        /// <param name="productAttributeValueId">Product attribute value identifier</param>
        /// <returns>Product attribute value</returns>
        public virtual ProductAttributeValue GetProductAttributeValueById(int productAttributeValueId)
        {
            if (productAttributeValueId == 0)
                return null;
            
           var key = string.Format(CATEGORYATTRIBUTEVALUES_BY_ID_KEY, productAttributeValueId);
           return _cacheManager.Get(key, () => _productAttributeValueRepository.GetById(productAttributeValueId));
        }

        /// <summary>
        /// Inserts a product attribute value
        /// </summary>
        /// <param name="productAttributeValue">The product attribute value</param>
        public virtual void InsertProductAttributeValue(ProductAttributeValue productAttributeValue)
        {
            if (productAttributeValue == null)
                throw new ArgumentNullException(nameof(productAttributeValue));

            _productAttributeValueRepository.Insert(productAttributeValue);

            //cache
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productAttributeValue);
        }

        /// <summary>
        /// Updates the product attribute value
        /// </summary>
        /// <param name="productAttributeValue">The product attribute value</param>
        public virtual void UpdateProductAttributeValue(ProductAttributeValue productAttributeValue)
        {
            if (productAttributeValue == null)
                throw new ArgumentNullException(nameof(productAttributeValue));

            _productAttributeValueRepository.Update(productAttributeValue);

            //cache
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTEMAPPINGS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productAttributeValue);
        }

        #endregion

        #endregion
    }
}
