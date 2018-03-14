using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class CategoryAttributeMappingMap : NopEntityTypeConfiguration<CategoryAttributeMapping>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public CategoryAttributeMappingMap()
        {
            this.ToTable("Category_CategoryAttribute_Mapping");
            this.HasKey(pam => pam.Id);
            this.Ignore(pam => pam.AttributeControlType);

            this.HasRequired(pam => pam.Category)
                .WithMany(p => p.CategoryAttributeMappings)
                .HasForeignKey(pam => pam.CategoryId);

            this.HasRequired(pam => pam.CategoryAttribute)
                .WithMany()
                .HasForeignKey(pam => pam.CategoryAttributeId);
        }
    }
}