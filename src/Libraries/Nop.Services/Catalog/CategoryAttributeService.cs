using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Catalog
{
    public interface ICategoryAttributeService
    {
        IList<CategoryProductAttributeMapping> GetByCatId(int categoryId);
        CategoryProductAttributeMapping Get(int id);
        void Delete(CategoryProductAttributeMapping entity);
        void Insert(CategoryProductAttributeMapping categoryProductAttributeMapping);
        void Update(CategoryProductAttributeMapping categoryProductAttributeMapping);
    }

    public class CategoryAttributeService
    {
        private const string CATEGORY_PRODUCTATTRIBUTE_ALL_KEY = "Nop.categoryproductattribute.all-{0}";
        private const string CATEGORY_PRODUCTATTRIBUTE_BY_ID_KEY = "Nop.categoryproductattribute.id-{0}";
        private const string CATEGORY_PRODUCTATTRIBUTE_PATTERN_KEY = "Nop.categoryproductattribute.";

        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<CategoryProductAttributeMapping> _categoryProductAttributeMappingRepository;

        #region Category Mapping Product Attribute

        public CategoryAttributeService(ICacheManager cacheManager, IRepository<CategoryProductAttributeMapping> categoryProductAttributeMappingRepository, IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _categoryProductAttributeMappingRepository = categoryProductAttributeMappingRepository;
            _eventPublisher = eventPublisher;
        }


        public IList<CategoryProductAttributeMapping> GetByCatId(int categoryId)
        {
            return _cacheManager.Get(CATEGORY_PRODUCTATTRIBUTE_ALL_KEY, () =>
            {
                var query = from pa in _categoryProductAttributeMappingRepository.Table
                            where pa.CategoryId == categoryId
                            orderby pa.DisplayOrder
                            select pa;
                return query.ToList();
            });
        }

        public CategoryProductAttributeMapping Get(int id)
        {
            if (id == 0)
                return null;

            var key = string.Format(CATEGORY_PRODUCTATTRIBUTE_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _categoryProductAttributeMappingRepository.GetById(id));
        }

        public void Delete(CategoryProductAttributeMapping entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            _categoryProductAttributeMappingRepository.Delete(entity);
            //cache
            _cacheManager.RemoveByPattern(CATEGORY_PRODUCTATTRIBUTE_PATTERN_KEY);
            //event notification
            _eventPublisher.EntityDeleted(entity);
        }

        public virtual void Insert(CategoryProductAttributeMapping categoryProductAttributeMapping)
        {
            if (categoryProductAttributeMapping == null)
                throw new ArgumentNullException(nameof(categoryProductAttributeMapping));

            _categoryProductAttributeMappingRepository.Insert(categoryProductAttributeMapping);

            //cache
            _cacheManager.RemoveByPattern(CATEGORY_PRODUCTATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(categoryProductAttributeMapping);
        }

        public virtual void Update(CategoryProductAttributeMapping categoryProductAttributeMapping)
        {
            if (categoryProductAttributeMapping == null)
                throw new ArgumentNullException(nameof(categoryProductAttributeMapping));

            _categoryProductAttributeMappingRepository.Update(categoryProductAttributeMapping);

            //cache
            _cacheManager.RemoveByPattern(CATEGORY_PRODUCTATTRIBUTE_PATTERN_KEY);
            //event notification
            _eventPublisher.EntityUpdated(categoryProductAttributeMapping);
        }

        #endregion
    }
}
