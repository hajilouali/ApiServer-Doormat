using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
namespace Entities
{
    
    public class ExpertHistory:BaseEntity
    {
        public ExpertHistory()
        {
            DateTime = DateTime.Now;
        }
        public ExpertCordition ExpertCordition { get; set; }
        public DateTime DateTime { get; set; }
        public int User_ID { get; set; }
        public int Expert_ID { get; set; }
        public int Facor_ID { get; set; }
        public User User { get; set; }
        public Expert Expert { get; set; }
        public Factor Factor { get; set; }

    }
    public class ExpertHistoryConfiguration : IEntityTypeConfiguration<ExpertHistory>
    {
        public void Configure(EntityTypeBuilder<ExpertHistory> builder)
        {
            builder.HasOne(p => p.User).WithMany(p => p.ExpertHistories).HasForeignKey(p => p.User_ID);
            builder.HasOne(p => p.Expert).WithMany(p => p.ExpertHistories).HasForeignKey(p => p.Expert_ID);
            builder.HasOne(p => p.Factor).WithMany(p => p.ExpertHistories).HasForeignKey(p => p.Facor_ID);
        }
    }

}