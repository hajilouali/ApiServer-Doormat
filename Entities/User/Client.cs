using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Entities
{
    public class Client:BaseEntity
    {
        public Client()
        {
            
            DiscountPercent = 0;
            MaxCreditValue = 0;
            UserConnected = 0;
        }
        public int UserConnected { get; set; }
        public string ClientName { get; set; }
        public string ClientAddress { get; set; }
        public string ClientPhone { get; set; }
        public string CodeMeli { get; set; }
        public string CodeEgtesadi { get; set; }
        public decimal DiscountPercent { get; set; }
        public int AccountingHeading_ID { get; set; }
        public int User_ID { get; set; }
        public double MaxCreditValue { get; set; }
        public string ClientPartnerName { get; set; }


        public User User { get; set; }
        public AccountingHeading AccountingHeading { get; set; }
        public ICollection<Expert> Experts { get; set; }
        public ICollection<Factor> Factors { get; set; }
    }
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.Property(p => p.ClientAddress).HasMaxLength(700);
            builder.Property(p => p.ClientName).IsRequired().HasMaxLength(200);
            builder.Property(p => p.ClientPhone).HasMaxLength(50);
            builder.Property(p => p.ClientPartnerName).HasMaxLength(200);
            builder.HasOne(p => p.User).WithMany(p => p.Clients).HasForeignKey(p => p.User_ID);
            builder.HasOne(p => p.AccountingHeading).WithMany(p => p.Clients).HasForeignKey(p => p.AccountingHeading_ID);
        }
    }
}