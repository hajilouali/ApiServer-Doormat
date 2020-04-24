using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Tikets
{
    public class Tiket : BaseEntity
    {
        public Tiket()
        {
            DataCreate = DateTime.Now;
            DataModified = DateTime.Now;
        }
        public string Title { get; set; }
        public bool Closed { get; set; }
        public short Level { get; set; }
        public short Department { get; set; }
        public bool IsAdminSide { get; set; }
        public int UserID { get; set; }
        public DateTime DataCreate { get; set; }
        public DateTime DataModified { get; set; }
        public User User { get; set; }
        public ICollection<TiketContent> tiketContents { get; set; }
    }
    public class ManufactureConfiguration : IEntityTypeConfiguration<Tiket>
    {
        public void Configure(EntityTypeBuilder<Tiket> builder)
        {
            builder.HasOne(p => p.User).WithMany(p => p.Tikets).HasForeignKey(p => p.UserID);
            builder.Property(p => p.Title).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Department).IsRequired();
            builder.Property(p => p.Level).IsRequired();
        }
    }
}
