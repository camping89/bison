using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class CategorySpecificationAttributeMap : NopEntityTypeConfiguration<CategorySpecificationAttribute>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public CategorySpecificationAttributeMap()
        {
            this.ToTable("Category_SpecificationAttribute_Mapping");
            this.HasKey(psa => psa.Id);

            this.Property(psa => psa.CustomValue).HasMaxLength(4000);

            this.Ignore(psa => psa.AttributeType);

            this.HasRequired(psa => psa.SpecificationAttributeOption)
                .WithMany()
                .HasForeignKey(psa => psa.SpecificationAttributeOptionId);


            this.HasRequired(psa => psa.Category)
                .WithMany(p => p.CategorySpecificationAttributes)
                .HasForeignKey(psa => psa.CategoryId);
        }
    }
}