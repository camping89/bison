using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class CategoryProductAttributeMappingMap : NopEntityTypeConfiguration<CategoryProductAttributeMapping>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public CategoryProductAttributeMappingMap()
        {
            this.ToTable("Category_ProductAttribute_Mapping");
            this.HasKey(pam => pam.Id);
            this.Ignore(pam => pam.AttributeControlType);
            this.HasRequired(pam => pam.Category)
                .WithMany(p => p.CategoryAttributeMappings)
                .HasForeignKey(pam => pam.CategoryId);

            this.HasRequired(pam => pam.ProductAttribute)
                .WithMany()
                .HasForeignKey(pam => pam.ProductAttributeId);
        }
    }
}