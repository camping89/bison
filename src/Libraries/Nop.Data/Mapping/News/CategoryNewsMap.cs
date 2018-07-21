using Nop.Core.Domain.News;

namespace Nop.Data.Mapping.News
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class CategoryNewsMap : NopEntityTypeConfiguration<CategoryNews>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public CategoryNewsMap()
        {
            this.ToTable("CategoryNews");
            this.HasKey(ni => ni.Id);
            this.Property(ni => ni.Name).IsRequired();
            this.Property(ni => ni.MetaKeywords).HasMaxLength(400);
            this.Property(ni => ni.MetaTitle).HasMaxLength(400);

            this.HasRequired(ni => ni.Language)
                .WithMany()
                .HasForeignKey(ni => ni.LanguageId).WillCascadeOnDelete(true);
        }
    }
}