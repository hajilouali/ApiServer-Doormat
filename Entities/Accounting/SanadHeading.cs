using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Entities
{
    public class SanadHeading:BaseEntity
    {
        public SanadHeading()
        {
            DateTime = DateTime.Now;
            FactorID = 0;
        }
        public int FactorID { get; set; }
        public DateTime DateTime { get; set; }
        public string Discription { get; set; }


        public ICollection<Sanad> Sanads { get; set; }
    }
    public class SanadHeadingConfiguration : IEntityTypeConfiguration<SanadHeading>
    {
        public void Configure(EntityTypeBuilder<SanadHeading> builder)
        {
            builder.Property(p => p.Discription).HasMaxLength(700);
        }
    }
}