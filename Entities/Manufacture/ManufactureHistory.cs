using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
namespace Entities
{
    public class ManufactureHistory:BaseEntity
    {
        public ManufactureHistory()
        {
            DateTime = DateTime.Now;
        }
        public int User_ID { get; set; }
        public int Manufacture_ID { get; set; }
        public DateTime DateTime { get; set; }
        public ConditionManufacture ConditionManufacture { get; set; }
        public string Discription { get; set; }


        public User User { get; set; }
        public Manufacture Manufacture { get; set; }
    }
    public class ManufactureHistoryConfiguration : IEntityTypeConfiguration<ManufactureHistory>
    {
        public void Configure(EntityTypeBuilder<ManufactureHistory> builder)
        {
            builder.Property(p => p.Discription).HasMaxLength(600);
            builder.HasOne(p => p.User).WithMany(p => p.ManufactureHistories).HasForeignKey(p => p.User_ID);
            builder.HasOne(p => p.Manufacture).WithMany(p => p.ManufactureHistories).HasForeignKey(p => p.Manufacture_ID);
        }
    }
}