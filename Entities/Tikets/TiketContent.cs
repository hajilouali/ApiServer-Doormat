using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Tikets
{
   public class TiketContent:BaseEntity
    {
        public TiketContent()
        {
            DataCreate = DateTime.Now;
            DataModified = DateTime.Now;
        }
        public int TiketId { get; set; }
        public bool IsAdminSide { get; set; }
        public string Text { get; set; }
        public string FileURL { get; set; }
        public DateTime DataCreate { get; set; }
        public DateTime DataModified { get; set; }
        public int UserID { get; set; }
        public Tiket Tiket { get; set; }
        public User User { get; set; }
        public class ManufactureConfiguration : IEntityTypeConfiguration<TiketContent>
        {
            public void Configure(EntityTypeBuilder<TiketContent> builder)
            {
                builder.HasOne(p => p.Tiket).WithMany(p => p.tiketContents).HasForeignKey(p => p.TiketId);
                builder.HasOne(p => p.User).WithMany(p => p.TiketContents).HasForeignKey(p => p.UserID);
                builder.Property(p => p.Text).HasMaxLength(1000);
                builder.Property(p => p.FileURL).HasMaxLength(500);
            }
        }
    }
}
