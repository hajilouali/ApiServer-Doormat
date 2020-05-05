using Entities.Tikets;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities
{
    public class User : IdentityUser<int>, IEntity
    {
        public User()
        {
            IsActive = true;
        }
        
        
        public string FullName { get; set; }
        
        public string Address { get; set; }

        public bool IsActive { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }
        public ICollection<Factor> Factors { get; set; }
        public ICollection<ManufactureHistory>  ManufactureHistories { get; set; }
        public ICollection<Client> Clients { get; set; }
        public ICollection<ExpertHistory> ExpertHistories { get; set; }
        public ICollection<Tiket> Tikets { get; set; }
        public ICollection<TiketContent> TiketContents { get; set; }

    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(p => p.Address).HasMaxLength(700);
            builder.Property(p => p.FullName).IsRequired().HasMaxLength(200);
            builder.Property(p => p.UserName).IsRequired().HasMaxLength(200);
        }
    }

    
}
