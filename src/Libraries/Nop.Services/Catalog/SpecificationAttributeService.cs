using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Specification attribute service
    /// </summary>
    public partial class SpecificationAttributeService : ISpecificationAttributeService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// {1} : specification attribute option ID
        /// {2} : allow filtering
        /// {3} : show on product page
        /// </remarks>
        private const string PRODUCTSPECIFICATIONATTRIBUTE_ALLBYPRODUCTID_KEY = "Nop.productspecificationattribute.allbyproductid-{0}-{1}-{2}-{3}";

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY = "Nop.productspecificationattribute.";
        private const string CATEGORYSPECIFICATIONATTRIBUTE_ALLBYPRODUCTID_KEY = "Nop.categoryspecificationattribute.allbycategoryid-{0}";

        private const string CATEGORYSPECIFICATIONATTRIBUTE_PATTERN_KEY = "Nop.categoryspecificationattribute.";

        #endregion

        #region Fields

        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        private readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRepository;
        private readonly IRepository<ProductSpecificationAttribute> _productSpecificationAttributeRepository;
        private readonly IRepository<CategorySpecificationAttribute> _categorySpecificationAttributeRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICategoryService _categoryService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="specificationAttributeRepository">Specification attribute repository</param>
        /// <param name="specificationAttributeOptionRepository">Specification attribute option repository</param>
        /// <param name="productSpecificationAttributeRepository">Product specification attribute repository</param>
        /// <param name="eventPublisher">Event published</param>
        public SpecificationAttributeService(ICacheManager cacheManager,
            IRepository<SpecificationAttribute> specificationAttributeRepository,
            IRepository<SpecificationAttributeOption> specificationAttributeOptionRepository,
            IRepository<ProductSpecificationAttribute> productSpecificationAttributeRepository,
            IEventPublisher eventPublisher, IRepository<CategorySpecificationAttribute> categorySpecificationAttributeRepository, ICategoryService categoryService)
        {
            _cacheManager = cacheManager;
            _specificationAttributeRepository = specificationAttributeRepository;
            _specificationAttributeOptionRepository = specificationAttributeOptionRepository;
            _productSpecificationAttributeRepository = productSpecificationAttributeRepository;
            _eventPublisher = eventPublisher;
            _categorySpecificationAttributeRepository = categorySpecificationAttributeRepository;
            _categoryService = categoryService;
        }

        #endregion

        #region Methods

        #region Specification attribute

        /// <summary>
        /// Gets a specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute</returns>
        public virtual SpecificationAttribute GetSpecificationAttributeById(int specificationAttributeId)
        {
            if (specificationAttributeId == 0)
                return null;

            return _specificationAttributeRepository.GetById(specificationAttributeId);
        }

        /// <summary>
        /// Gets specification attributes
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Specification attributes</returns>
        public virtual IPagedList<SpecificationAttribute> GetSpecificationAttributes(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from sa in _specificationAttributeRepository.Table
                        orderby sa.DisplayOrder, sa.Id
                        select sa;
            var specificationAttributes = new PagedList<SpecificationAttribute>(query, pageIndex, pageSize);
            return specificationAttributes;
        }

        /// <summary>
        /// Deletes a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual void DeleteSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException(nameof(specificationAttribute));

            _specificationAttributeRepository.Delete(specificationAttribute);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(specificationAttribute);
        }

        /// <summary>
        /// Inserts a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual void InsertSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException(nameof(specificationAttribute));

            _specificationAttributeRepository.Insert(specificationAttribute);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(specificationAttribute);
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual void UpdateSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException(nameof(specificationAttribute));

            _specificationAttributeRepository.Update(specificationAttribute);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(specificationAttribute);
        }

        #endregion

        #region Specification attribute option

        /// <summary>
        /// Gets a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier</param>
        /// <returns>Specification attribute option</returns>
        public virtual SpecificationAttributeOption GetSpecificationAttributeOptionById(int specificationAttributeOptionId)
        {
            if (specificationAttributeOptionId == 0)
                return null;

            return _specificationAttributeOptionRepository.GetById(specificationAttributeOptionId);
        }

        /// <summary>
        /// Get specification attribute options by identifiers
        /// </summary>
        /// <param name="specificationAttributeOptionIds">Identifiers</param>
        /// <returns>Specification attribute options</returns>
        public virtual IList<SpecificationAttributeOption> GetSpecificationAttributeOptionsByIds(int[] specificationAttributeOptionIds)
        {
            if (specificationAttributeOptionIds == null || specificationAttributeOptionIds.Length == 0)
                return new List<SpecificationAttributeOption>();

            var query = from sao in _specificationAttributeOptionRepository.Table
                        where specificationAttributeOptionIds.Contains(sao.Id)
                        select sao;
            var specificationAttributeOptions = query.ToList();
            //sort by passed identifiers
            var sortedSpecificationAttributeOptions = new List<SpecificationAttributeOption>();
            foreach (var id in specificationAttributeOptionIds)
            {
                var sao = specificationAttributeOptions.Find(x => x.Id == id);
                if (sao != null)
                    sortedSpecificationAttributeOptions.Add(sao);
            }
            return sortedSpecificationAttributeOptions;
        }
        public IList<int> GetSpecificationAttributeOptionsIdsByTerm(string term)
        {

            var query = from sao in _specificationAttributeOptionRepository.Table
                        where sao.Name.Contains(term)
                        select sao.Id;
            return query.ToList();
        }

        public virtual List<SpecificationAttributeOption> GetSpecificationAttributeOptionsByParentIds(int[] specificationAttributeOptionIds)
        {
            List<SpecificationAttributeOption> specOptionChilds = new List<SpecificationAttributeOption>();
            GetAllSpecOptionChilds(specificationAttributeOptionIds, ref specOptionChilds);
            return specOptionChilds;
        }

        private void GetAllSpecOptionChilds(int[] specificationAttributeOptionIds, ref List<SpecificationAttributeOption> specOptionChilds)
        {
            if (specificationAttributeOptionIds == null || specificationAttributeOptionIds.Length == 0)
                return;

            var query = from sao in _specificationAttributeOptionRepository.Table
                        where specificationAttributeOptionIds.Contains(sao.ParentSpecificationAttributeId)
                        select sao;
            var specificationAttributeOptions = query.ToList();
            if (specificationAttributeOptions.Count > 0)
            {
                specOptionChilds.AddRange(specificationAttributeOptions);
                GetAllSpecOptionChilds(specificationAttributeOptions.Select(_ => _.Id).ToArray(), ref specOptionChilds);
            }
        }
        /// <summary>
        /// Gets a specification attribute option by specification attribute id
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute option</returns>
        public virtual IList<SpecificationAttributeOption> GetSpecificationAttributeOptionsBySpecificationAttribute(int specificationAttributeId)
        {
            var query = from sao in _specificationAttributeOptionRepository.Table
                        orderby sao.DisplayOrder, sao.Id
                        where sao.SpecificationAttributeId == specificationAttributeId
                        select sao;
            var specificationAttributeOptions = query.OrderBy(c => c.ParentSpecificationAttributeId).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Id).ToList();
            var sortedpecificationAttributeOptions = specificationAttributeOptions.SortSaoForTree();
            return sortedpecificationAttributeOptions;
        }

        /// <summary>
        /// Deletes a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual void DeleteSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException(nameof(specificationAttributeOption));

            _specificationAttributeOptionRepository.Delete(specificationAttributeOption);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(specificationAttributeOption);
        }

        /// <summary>
        /// Inserts a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual void InsertSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException(nameof(specificationAttributeOption));

            _specificationAttributeOptionRepository.Insert(specificationAttributeOption);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(specificationAttributeOption);
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual void UpdateSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException(nameof(specificationAttributeOption));

            _specificationAttributeOptionRepository.Update(specificationAttributeOption);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(specificationAttributeOption);
        }

        /// <summary>
        /// Returns a list of IDs of not existing specification attribute options
        /// </summary>
        /// <param name="attributeOptionIds">The IDs of the attribute options to check</param>
        /// <returns>List of IDs not existing specification attribute options</returns>
        public virtual int[] GetNotExistingSpecificationAttributeOptions(int[] attributeOptionIds)
        {
            if (attributeOptionIds == null)
                throw new ArgumentNullException(nameof(attributeOptionIds));

            var query = _specificationAttributeOptionRepository.Table;
            var queryFilter = attributeOptionIds.Distinct().ToArray();
            var filter = query.Select(a => a.Id).Where(m => queryFilter.Contains(m)).ToList();
            return queryFilter.Except(filter).ToArray();
        }

        #endregion

        #region Product specification attribute

        /// <summary>
        /// Deletes a product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute</param>
        public virtual void DeleteProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException(nameof(productSpecificationAttribute));

            _productSpecificationAttributeRepository.Delete(productSpecificationAttribute);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productSpecificationAttribute);
        }

        /// <summary>
        /// Gets a product specification attribute mapping collection
        /// </summary>
        /// <param name="productId">Product identifier; 0 to load all records</param>
        /// <param name="specificationAttributeOptionId">Specification attribute option identifier; 0 to load all records</param>
        /// <param name="allowFiltering">0 to load attributes with AllowFiltering set to false, 1 to load attributes with AllowFiltering set to true, null to load all attributes</param>
        /// <param name="showOnProductPage">0 to load attributes with ShowOnProductPage set to false, 1 to load attributes with ShowOnProductPage set to true, null to load all attributes</param>
        /// <returns>Product specification attribute mapping collection</returns>
        public virtual IList<ProductSpecificationAttribute> GetProductSpecificationAttributes(int productId = 0,
            int specificationAttributeOptionId = 0, bool? allowFiltering = null, bool? showOnProductPage = null)
        {
            var allowFilteringCacheStr = allowFiltering.HasValue ? allowFiltering.ToString() : "null";
            var showOnProductPageCacheStr = showOnProductPage.HasValue ? showOnProductPage.ToString() : "null";
            var key = string.Format(PRODUCTSPECIFICATIONATTRIBUTE_ALLBYPRODUCTID_KEY,
                productId, specificationAttributeOptionId, allowFilteringCacheStr, showOnProductPageCacheStr);

            return _cacheManager.Get(key, () =>
            {
                var query = _productSpecificationAttributeRepository.Table;
                if (productId > 0)
                    query = query.Where(psa => psa.ProductId == productId);
                if (specificationAttributeOptionId > 0)
                    query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);
                if (allowFiltering.HasValue)
                    query = query.Where(psa => psa.AllowFiltering == allowFiltering.Value);
                if (showOnProductPage.HasValue)
                    query = query.Where(psa => psa.ShowOnProductPage == showOnProductPage.Value);
                query = query.OrderBy(psa => psa.DisplayOrder).ThenBy(psa => psa.Id);

                var productSpecificationAttributes = query.ToList();
                return productSpecificationAttributes;
            });
        }

        /// <summary>
        /// Gets a product specification attribute mapping 
        /// </summary>
        /// <param name="productSpecificationAttributeId">Product specification attribute mapping identifier</param>
        /// <returns>Product specification attribute mapping</returns>
        public virtual ProductSpecificationAttribute GetProductSpecificationAttributeById(int productSpecificationAttributeId)
        {
            if (productSpecificationAttributeId == 0)
                return null;

            return _productSpecificationAttributeRepository.GetById(productSpecificationAttributeId);
        }

        /// <summary>
        /// Inserts a product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
        public virtual void InsertProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException(nameof(productSpecificationAttribute));

            _productSpecificationAttributeRepository.Insert(productSpecificationAttribute);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productSpecificationAttribute);
        }

        /// <summary>
        /// Updates the product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
        public virtual void UpdateProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException(nameof(productSpecificationAttribute));

            _productSpecificationAttributeRepository.Update(productSpecificationAttribute);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productSpecificationAttribute);
        }

        /// <summary>
        /// Gets a count of product specification attribute mapping records
        /// </summary>
        /// <param name="productId">Product identifier; 0 to load all records</param>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier; 0 to load all records</param>
        /// <returns>Count</returns>
        public virtual int GetProductSpecificationAttributeCount(int productId = 0, int specificationAttributeOptionId = 0)
        {
            var query = _productSpecificationAttributeRepository.Table;
            if (productId > 0)
                query = query.Where(psa => psa.ProductId == productId);
            if (specificationAttributeOptionId > 0)
                query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);

            return query.Count();
        }

        #endregion
        #region Product specification attribute


        public virtual void DeleteCategorySpecificationAttribute(CategorySpecificationAttribute categorySpecificationAttribute)
        {
            if (categorySpecificationAttribute == null)
                throw new ArgumentNullException(nameof(categorySpecificationAttribute));

            _categorySpecificationAttributeRepository.Delete(categorySpecificationAttribute);

            _cacheManager.RemoveByPattern(CATEGORYSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(categorySpecificationAttribute);
        }


        public virtual IList<CategorySpecificationAttribute> GetCategorySpecificationAttributes(int categoryId = 0,
            int specificationAttributeOptionId = 0, bool? allowFiltering = null, bool? showOnProductPage = null)
        {
            var key = string.Format(CATEGORYSPECIFICATIONATTRIBUTE_ALLBYPRODUCTID_KEY, categoryId);

            return _cacheManager.Get(key, () =>
            {
                var query = _categorySpecificationAttributeRepository.Table;
                if (categoryId > 0)
                    query = query.Where(psa => psa.CategoryId == categoryId);
                if (specificationAttributeOptionId > 0)
                    query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);
                if (allowFiltering.HasValue)
                    query = query.Where(psa => psa.AllowFiltering == allowFiltering.Value);
                if (showOnProductPage.HasValue)
                    query = query.Where(psa => psa.ShowOnProductPage == showOnProductPage.Value);
                query = query.OrderBy(psa => psa.DisplayOrder).ThenBy(psa => psa.Id);

                var productSpecificationAttributes = query.ToList();
                return productSpecificationAttributes;
            });
        }

        public virtual CategorySpecificationAttribute GetCategorySpecificationAttributeById(int categorySpecificationAttributeId)
        {
            if (categorySpecificationAttributeId == 0)
                return null;

            return _categorySpecificationAttributeRepository.GetById(categorySpecificationAttributeId);
        }

        public IList<CategorySpecificationAttribute> GetByCatId(int categoryId)
        {
            var query = from pa in _categorySpecificationAttributeRepository.Table
                        where pa.CategoryId == categoryId
                        orderby pa.DisplayOrder
                        select pa;
            return query.ToList();
        }
        public virtual void InsertCategorySpecificationAttribute(CategorySpecificationAttribute categorySpecificationAttribute)
        {
            if (categorySpecificationAttribute == null)
                throw new ArgumentNullException(nameof(categorySpecificationAttribute));

            _categorySpecificationAttributeRepository.Insert(categorySpecificationAttribute);

            _cacheManager.RemoveByPattern(CATEGORYSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(categorySpecificationAttribute);
        }
        public virtual List<int> Insert(CategorySpecificationAttribute mapping, bool cascadeToChildren)
        {
            if (mapping == null)
                throw new ArgumentNullException(nameof(mapping));
            var listCategories = new List<int>();
            if (GetByCatId(mapping.CategoryId).All(x => x.SpecificationAttributeOptionId != mapping.SpecificationAttributeOptionId))
            {
                listCategories.Add(mapping.CategoryId);
                InsertCategorySpecificationAttribute(mapping);
                foreach (var category in _categoryService.GetAllCategoriesByParentCategoryId(mapping.CategoryId, true, true))
                {
                    mapping.CategoryId = category.Id;
                    listCategories.Add(category.Id);
                    InsertCategorySpecificationAttribute(mapping);
                }
            }
            return listCategories;
        }

        public virtual void UpdateCategorySpecificationAttribute(CategorySpecificationAttribute categorySpecificationAttribute)
        {
            if (categorySpecificationAttribute == null)
                throw new ArgumentNullException(nameof(categorySpecificationAttribute));

            _categorySpecificationAttributeRepository.Update(categorySpecificationAttribute);

            _cacheManager.RemoveByPattern(CATEGORYSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(categorySpecificationAttribute);
        }

        public virtual int GetCategorySpecificationAttributeCount(int categoryId = 0, int specificationAttributeOptionId = 0)
        {
            var query = _categorySpecificationAttributeRepository.Table;
            if (categoryId > 0)
                query = query.Where(psa => psa.CategoryId == categoryId);
            if (specificationAttributeOptionId > 0)
                query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);

            return query.Count();
        }

        #endregion

        #endregion
    }
}
