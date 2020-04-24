using Entities.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Entities
{
    public class Sanad:BaseEntity
    {
        public Sanad()
        {
            Bedehkari = 0;
            Bestankari = 0;
        }
        public int AccountingHeading_ID { get; set; }
        public int SanadHeading_ID { get; set; }
        public int Bedehkari { get; set; }
        public int Bestankari { get; set; }
        public string Comment { get; set; }

        public AccountingHeading AccountingHeading { get; set; }
        public SanadHeading SanadHeading { get; set; }
        public ICollection<SanadAttachment> SanadAttachment { get; set; }
    }
    public class SanadConfiguration : IEntityTypeConfiguration<Sanad>
    {
        public void Configure(EntityTypeBuilder<Sanad> builder)
        {
            builder.Property(p => p.Comment).HasMaxLength(700);
            builder.HasOne(p => p.AccountingHeading).WithMany(p => p.Sanads).HasForeignKey(p => p.AccountingHeading_ID);
            builder.HasOne(p => p.SanadHeading).WithMany(p => p.Sanads).HasForeignKey(p => p.SanadHeading_ID);
        }
    }
}