using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities
{
    public enum ConditionManufacture
    {
        [Display(Name ="در انتظار ساخت")]
        PendingForConstruction,
        [Display(Name = "برش خورده")]
        Cut,
        [Display(Name = "ساخته شده")]
        Built,
        [Display(Name = "تحویل به مشتری")]
        DeliverToClient,
        [Display(Name = "تحویل به همکار")]
        DeliverToPartner,
        [Display(Name = "نصب شده")]
        Install

    }
    public class Manufacture:BaseEntity
    {
        public Manufacture()
        {
            InDateTime = DateTime.Now;
            ConditionManufacture = ConditionManufacture.PendingForConstruction;
        }
        public int Factor_ID { get; set; }
        public DateTime InDateTime { get; set; }
        public ConditionManufacture ConditionManufacture { get; set; }


        public Factor Factor { get; set; }
        public ICollection<ManufactureHistory> ManufactureHistories  { get; set; }
    }
    public class ManufactureConfiguration : IEntityTypeConfiguration<Manufacture>
    {
        public void Configure(EntityTypeBuilder<Manufacture> builder)
        {
            builder.HasOne(p => p.Factor).WithMany(p => p.Manufacture).HasForeignKey(p => p.Factor_ID);
        }
    }
}