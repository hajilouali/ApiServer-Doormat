using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public enum FactorType
    {
        [Display(Name ="پیش فاکتور")]
        PishFactor,
        [Display(Name = " فاکتور")]
        Factor,
        [Display(Name = "در انتظار تایید")]
        PendingToAccept,
    }
  public  class Factor:BaseEntity
    {
        public Factor()
        {
            FactorType = FactorType.PishFactor;
            DateTime = DateTime.Now;
            Discount = 0;
            TotalPrice = 0;
            Taxes = 0;
            FactorPrice = 0;
            FactorCodeView = Guid.NewGuid().ToString().Replace("-", "").Substring(0,10);
        }
        public DateTime DateTime { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Taxes { get; set; }
        public decimal Discount { get; set; }
        public decimal FactorPrice { get; set; }
        public int User_ID { get; set; }
        public int Client_ID { get; set; }
        public string FactorCodeView { get; set; }
        public string Discription { get; set; }
        public FactorType FactorType { get; set; }



        public  ICollection<Product_Factor> Product_Factor { get; set; }
        public  ICollection<Manufacture> Manufacture { get; set; }
        public ICollection<ExpertHistory> ExpertHistories { get; set; }
        public  ICollection<FactorAttachment> FactorAttachment { get; set; }
        public  User User { get; set; }
        public  Client Client { get; set; }
    }
    public class FactorConfiguration : IEntityTypeConfiguration<Factor>
    {
        public void Configure(EntityTypeBuilder<Factor> builder)
        {
            builder.Property(p => p.FactorCodeView).HasMaxLength(200);
            builder.HasOne(p => p.User).WithMany(c => c.Factors).HasForeignKey(p => p.User_ID);
            builder.HasOne(p => p.Client).WithMany(c => c.Factors).HasForeignKey(p => p.Client_ID);
        }
    }
}
