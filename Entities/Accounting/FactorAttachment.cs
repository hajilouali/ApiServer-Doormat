using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities
{
    public class FactorAttachment:BaseEntity
    {
        public int Facor_ID { get; set; }
        public string FileName { get; set; }
        public string Discription { get; set; }

        public Factor Factor { get; set; }
    }
    public class FactorAttachmentConfiguration : IEntityTypeConfiguration<FactorAttachment>
    {
        public void Configure(EntityTypeBuilder<FactorAttachment> builder)
        {
            builder.Property(p => p.FileName).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Discription).HasMaxLength(600);
            builder.HasOne(p => p.Factor).WithMany(p => p.FactorAttachment).HasForeignKey(p => p.Facor_ID);
        }
    }
}