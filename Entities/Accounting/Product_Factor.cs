using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities
{
    public class Product_Factor : BaseEntity
    {
        public Product_Factor()
        {
            Width = 1;
            length = 1;
            Unit = 1;
        }
        public int ProductAndService_ID { get; set; }
        public int Factor_ID { get; set; }
        public decimal Width { get; set; }
        public decimal length { get; set; }
        public int Unit { get; set; }
        public double UnitPrice { get; set; }
        public double Price { get; set; }
        public string RowDiscription { get; set; }
        public  Factor Factor { get; set; }
        public  ProductAndService ProductAndService { get; set; }
    }
    public class Product_FactorConfiguration : IEntityTypeConfiguration<Product_Factor>
    {
        public void Configure(EntityTypeBuilder<Product_Factor> builder)
        {
            builder.HasOne(p => p.ProductAndService).WithMany(c => c.Product_Factor).HasForeignKey(p => p.ProductAndService_ID);
            builder.HasOne(p => p.Factor).WithMany(c => c.Product_Factor).HasForeignKey(p => p.Factor_ID);
        }
    }
}