using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public enum AccountingType
    {
        [Display(Name ="بدهکار")]
        debt,
        [Display(Name = "بستانکار")]
        Crediting
    }
    public class AccountingHeading:BaseEntity
    {
        public string Title { get; set; }        
        public AccountingType AccountingType { get; set; }
        public string Discription { get; set; }

        public ICollection<Sanad> Sanads { get; set; }
        public ICollection<Bank>  Banks { get; set; }
        public ICollection<Client> Clients { get; set; }
    }
    public class AccountingHeadingConfiguration : IEntityTypeConfiguration<AccountingHeading>
    {
        public void Configure(EntityTypeBuilder<AccountingHeading> builder)
        {
            builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Discription).HasMaxLength(700);
        }
    }
}