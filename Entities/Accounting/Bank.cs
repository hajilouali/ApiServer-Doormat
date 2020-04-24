using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities
{
    public class Bank:BaseEntity
    {
        public string BankTitle { get; set; }
        public int AccountingHeading_ID { get; set; }

        public AccountingHeading AccountingHeading { get; set; }
    }
    public class BankConfiguration : IEntityTypeConfiguration<Bank>
    {
        public void Configure(EntityTypeBuilder<Bank> builder)
        {
            builder.Property(p => p.BankTitle).IsRequired().HasMaxLength(200);
            builder.HasOne(p => p.AccountingHeading).WithMany(p => p.Banks).HasForeignKey(p => p.AccountingHeading_ID);
        }
    }
}