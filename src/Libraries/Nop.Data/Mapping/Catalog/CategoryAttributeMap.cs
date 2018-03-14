using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class CategoryAttributeMap : NopEntityTypeConfiguration<CategoryAttribute>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public CategoryAttributeMap()
        {
            this.ToTable("CategoryAttribute");
            this.HasKey(pa => pa.Id);
            this.Property(pa => pa.Name).IsRequired();
        }
    }
}