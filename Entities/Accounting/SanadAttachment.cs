using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Accounting
{
  public  class SanadAttachment:BaseEntity
    {
        public int SanadID { get; set; }
        public string FileName { get; set; }
        public string Discription { get; set; }

        public Sanad Sanad { get; set; }
    }
    public class FactorAttachmentConfiguration : IEntityTypeConfiguration<SanadAttachment>
    {
        public void Configure(EntityTypeBuilder<SanadAttachment> builder)
        {
            builder.Property(p => p.FileName).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Discription).HasMaxLength(600);
            builder.HasOne(p => p.Sanad).WithMany(p => p.SanadAttachment).HasForeignKey(p => p.SanadID);
        }
    }
}
